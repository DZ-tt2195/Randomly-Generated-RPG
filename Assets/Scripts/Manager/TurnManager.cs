using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Linq;
using System.Diagnostics;
using System;

class CharacterPositions
{
    public Vector3 position;
    public Character character;

    public CharacterPositions(Vector3 position)
    {
        this.position = position;
    }
}

[Serializable]
class WaveSetup
{
    public List<int> enemyDifficultySpawn = new();
}

public class TurnManager : MonoBehaviour
{

#region Setup

    public static TurnManager instance;
    [Foldout("Prefabs", true)]
        [SerializeField] GameObject characterPrefab;
        [SerializeField] PointsVisual pointsVisual;
        public TextCollector undoBox;
        [SerializeField] List<WaveSetup> listOfWaveSetup = new();

    [Foldout("UI", true)]
        public List<AbilityBox> listOfBoxes = new();
        public TMP_Text instructions;
        public TMP_Text timerText;
        bool borderDecrease = true;
        [SerializeField] Button quitButton;
        List<CharacterPositions> teammatePositions = new();
        List<CharacterPositions> enemyPositions = new();
        [SerializeField] TMP_Text waveText;
        [SerializeField] TMP_Text roundText;
        [SerializeField] Button resignButton;

    [Foldout("Character lists", true)]
        System.Random dailyRNG;
        [ReadOnly] public List<Character> listOfPlayers = new();
        [ReadOnly] public List<Character> listOfEnemies = new();
        [ReadOnly] public List<Character> listOfDead = new();
        [ReadOnly] public List<Character> speedQueue = new();

    [Foldout("Misc", true)]
        int currentWave;
        int currentRound;
        bool isBattling;
        Character _targetedPlayer;
        [ReadOnly] public Character targetedPlayer { get { return _targetedPlayer; } set { ResetTargetedPlayer(value); } }
        Character _targetedEnemy;
        [ReadOnly] public Character targetedEnemy { get { return _targetedEnemy; } set { ResetTargetedEnemy(value); } }
        public int confirmChoice { get; private set; }
        Stopwatch gameTimer;

    private void Awake()
    {
        instance = this;
        isBattling = true;
        waveText.transform.parent.localPosition = new Vector3(0, 1200, 0);

        resignButton.onClick.AddListener(() => GameFinished(ToTranslate.Quit_Fight, false));

        if (ScreenOverlay.instance.mode == GameMode.Daily)
        {
            DateTime day = DateTime.UtcNow.Date;
            int seed = day.Year * 10000 + day.Month * 100 + day.Day;
            dailyRNG = new System.Random(seed);
        }

        for (int i = 0; i < 5; i++)
        {
            int nextX = -1050 + (350 * i);
            teammatePositions.Add(new CharacterPositions(new Vector3(nextX, -550, 0)));
            enemyPositions.Add(new CharacterPositions(new Vector3(nextX, 300, 0)));
        }
    }

#endregion

#region Gameplay Loop

    public IEnumerator NewWave()
    {
        instructions.text = "";
        instructions.transform.parent.gameObject.SetActive(false);
        DisableCharacterButtons();

        if (currentWave == 0)
        {
            gameTimer = new Stopwatch();
            gameTimer.Start();
        }

        if (currentWave > listOfWaveSetup.Count)
        {
            GameFinished(ToTranslate.Game_Won, true);
        }
        else if (ScreenOverlay.instance.mode == GameMode.Tutorial)
        {
            yield return NewAnimation(true);
            Log.instance.AddText(AutoTranslate.Wave(currentWave.ToString(), "3"));
        }
        else
        {
            yield return NewAnimation(true);
            Log.instance.AddText(AutoTranslate.Wave(currentWave.ToString(), listOfWaveSetup.Count.ToString()));

            if (currentWave >= 2 && ScreenOverlay.instance.ActiveCheat(ToTranslate.New_Abilities))
            {
                Log.instance.AddText(AutoTranslate.DoEnum(ToTranslate.Gain_New_Abilities));
                foreach (Character player in listOfPlayers)
                {
                    for (int i = player.listOfRandomAbilities.Count - 1; i >= 0; i--)
                        player.DropAbility(player.listOfRandomAbilities[i]);

                    List<AbilityData> newAbilties = GameFiles.inst.CompletePlayerAbilities(new(), GameFiles.inst.ConvertToAbilityData(player.data.listOfAbilities, true), dailyRNG);
                    foreach (AbilityData data in newAbilties)
                        player.AddAbility(data, false, false);
                }
                Log.instance.AddText("");
            }

            foreach (int nextTier in listOfWaveSetup[currentWave - 1].enemyDifficultySpawn)
                yield return MakeNewEnemy(GameFiles.inst.listOfEnemies[nextTier]);
            if (currentWave <= 4 && ScreenOverlay.instance.ActiveChallenge(ToTranslate.More_Enemies))
                yield return MakeNewEnemy(GameFiles.inst.listOfEnemies[1]);

            IEnumerator MakeNewEnemy(List<CharacterData> fromList)
            {
                yield return WaitTime();
                if (dailyRNG != null)
                {
                    CharacterData randomEnemy = fromList[dailyRNG.Next(0, fromList.Count)];
                    CreateEnemy(randomEnemy, (Emotion)dailyRNG.Next(1, 5), 0);
                }
                else
                {
                    CharacterData randomEnemy = fromList[UnityEngine.Random.Range(0, fromList.Count)];
                    CreateEnemy(randomEnemy, (Emotion)UnityEngine.Random.Range(1, 5), 0);
                }
            }

            StartCoroutine(NewRound(false));
        }
    }

    public IEnumerator NewRound(bool playAnimation)
    {
        if (playAnimation)
        {
            yield return NewAnimation(false);
        }
        Log.instance.AddText($"");
        Log.instance.AddText(AutoTranslate.Round(currentRound.ToString()));

        speedQueue = AllCharacters();
        List<Character> AllCharacters()
        {
            List<Character> allTargets = new();
            allTargets.AddRange(listOfPlayers);
            allTargets.AddRange(listOfEnemies);
            return allTargets;
        }

        while (speedQueue.Count > 0)
        {
            DisableCharacterButtons();
            listOfBoxes[0].transform.parent.gameObject.SetActive(false);

            Character nextInLine = speedQueue[0];
            speedQueue.RemoveAt(0);

            if (nextInLine != null && nextInLine.currentHealth > 0)
            {
                instructions.text = "";
                Log.instance.AddText($"");
                nextInLine.border.gameObject.SetActive(true);

                yield return nextInLine.MyTurn(0);
                try { nextInLine.border.gameObject.SetActive(false); } catch { /*do nothing*/}
            }

            if (CheckLost())
            {
                GameFinished(ToTranslate.Game_Lost, false);
                yield break;
            }
            else if (listOfEnemies.Count == 0)
            {
                if (ScreenOverlay.instance.mode != GameMode.Tutorial)
                {
                    Log.instance.AddText($"");
                    StartCoroutine(NewWave());
                }
                yield break;
            }
        }

        if (isBattling && ScreenOverlay.instance.mode != GameMode.Tutorial)
            StartCoroutine(NewRound(true));
    }

    IEnumerator NewAnimation(bool increaseWave)
    {
        Vector3 originalPos = new Vector3(0, 1200, 0);
        Vector3 finalPos = new Vector3(0, -1200, 0);

        waveText.transform.parent.localPosition = originalPos;
        waveText.text = AutoTranslate.Wave(currentWave.ToString(), ScreenOverlay.instance.mode == GameMode.Tutorial ? "3" : listOfWaveSetup.Count.ToString());
        roundText.text = AutoTranslate.Round(currentRound.ToString());

        float elapsedTime = 0f;
        float totalTime = PlayerPrefs.GetFloat("Animation Speed");

        while (elapsedTime < totalTime)
        {
            waveText.transform.parent.localPosition = Vector3.Lerp(originalPos, Vector3.zero, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        waveText.transform.parent.localPosition = Vector3.zero;
        yield return WaitTime();

        currentRound++;
        roundText.text = AutoTranslate.Round(currentRound.ToString());
        if (increaseWave)
        {
            currentWave++;
            waveText.text = AutoTranslate.Wave(currentWave.ToString(), ScreenOverlay.instance.mode == GameMode.Tutorial ? "3" : listOfWaveSetup.Count.ToString());
        }

        yield return WaitTime();

        elapsedTime = 0f;
        while (elapsedTime < totalTime)
        {
            waveText.transform.parent.localPosition = Vector3.Lerp(Vector3.zero, finalPos, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        waveText.transform.parent.localPosition = finalPos;
    }

    public void GameFinished(ToTranslate message, bool win)
    {
        try
        {
            StopAllCoroutines();
            TutorialManager.instance.StopAllCoroutines();
        }
        catch
        {
            //do nothing
        }
        isBattling = false;
        DisableCharacterButtons();

        instructions.text = "";
        instructions.transform.parent.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);

        Log.instance.AddText("");
        Log.instance.AddText(AutoTranslate.DoEnum(message));

        int number = (win) ? currentWave : currentWave - 1;
        Log.instance.AddText(AutoTranslate.Waves_Survived(number.ToString()));

        if (gameTimer != null)
        {
            gameTimer.Stop();
            Log.instance.AddText(AutoTranslate.Time_Taken(MyExtensions.StopwatchTime(gameTimer)));
        }
        Log.instance.enabled = false;
    }

    bool CheckLost()
    {
        foreach (Character character in listOfPlayers)
        {
            if (character.currentHealth > 0)
                return false;
        }

        return true;
    }

#endregion

#region UI

    void FixedUpdate()
    {
        Character.borderColor += (borderDecrease) ? -0.05f : 0.05f;
        if (Character.borderColor < 0 || Character.borderColor > 1)
            borderDecrease = !borderDecrease;
    }

    public void DisableCharacterButtons()
    {
        foreach (Character character in listOfPlayers)
        {
            character.myButton.interactable = false;
            character.border.gameObject.SetActive(false);
        }
        foreach (Character character in listOfEnemies)
        {
            character.myButton.interactable = false;
            character.border.gameObject.SetActive(false);
        }
    }

    public TextCollector MakeTextCollector(string header, Vector3 position, List<string> buttons = null)
    {
        TextCollector collector = Instantiate(undoBox);
        collector.StatsSetup(header, position);

        if (buttons != null)
        {
            foreach (string text in buttons)
                collector.AddTextButton(text);
        }
        return collector;
    }

    public void CreateVisual(string text, Vector3 position)
    {
        PointsVisual pv = Instantiate(pointsVisual, ScreenOverlay.instance.sceneCanvas);
        pv.Setup(text, position);
    }


#endregion

#region Misc

    public void CreateEnemy(CharacterData dataFile, Emotion startingEmotion, int logged)
    {
        if (listOfEnemies.Count < 5)
        {
            EnemyCharacter nextEnemy = Instantiate(characterPrefab).AddComponent<EnemyCharacter>();
            nextEnemy.transform.SetParent(ScreenOverlay.instance.sceneCanvas);
            nextEnemy.transform.localScale = Vector3.one;
            nextEnemy.transform.SetAsFirstSibling();
            listOfEnemies.Add(nextEnemy);

            foreach (CharacterPositions position in enemyPositions)
            {
                if (position.character == null)
                {
                    nextEnemy.transform.localPosition = position.position;
                    position.character = nextEnemy;
                    break;
                }
            }

            nextEnemy.name = AutoTranslate.DoEnum(dataFile.characterName);
            Log.instance.AddText(AutoTranslate.Enter_Fight(nextEnemy.name), logged);
            
            nextEnemy.SetupCharacter(dataFile, GameFiles.inst.ConvertToAbilityData(dataFile.listOfAbilities, false), startingEmotion, true);
            if (ScreenOverlay.instance.ActiveCheat(ToTranslate.Weaker_Enemies))
                StartCoroutine(nextEnemy.ChangeMaxHealth(-2, logged + 1));
            if (ScreenOverlay.instance.ActiveChallenge(ToTranslate.Extra_Enemy_Turns))
                StartCoroutine(nextEnemy.ChangeEffect(StatusEffect.Extra, 1, logged+1));
        }
    }

    public void AddPlayer(Character character)
    {
        listOfPlayers.Add(character);
        character.transform.SetParent(ScreenOverlay.instance.sceneCanvas);
        character.transform.localScale = Vector3.one;
        character.transform.SetAsFirstSibling();

        foreach (CharacterPositions position in teammatePositions)
        {
            if (position.character == null)
            {
                character.transform.localPosition = position.position;
                position.character = character;
                break;
            }
        }
    }

    public static string[] SpliceString(string text, char splitUp)
    {
        if (!text.IsNullOrEmpty())
        {
            string divide = text.Replace(" ", "").Trim();
            string[] splitIntoStrings = divide.Split(splitUp);
            return splitIntoStrings;
        }

        return new string[0];
    }

    public IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(PlayerPrefs.GetFloat("Animation Speed"));
    }

    public IEnumerator ConfirmUndo(string header, Vector3 position)
    {
        confirmChoice = -1;
        if (PlayerPrefs.GetInt("Confirm Choices") == 1)
        {
            //instructions.text = "";
            DisableCharacterButtons();

            TextCollector confirmDecision = MakeTextCollector(header, position, new List<string>() { "Confirm", "Rechoose" });
            CoroutineGroup group = new(this);
            group.StartCoroutine(confirmDecision.WaitForChoice());
            while (confirmDecision != null && group.AnyProcessing)
                yield return null;

            if (confirmDecision != null)
            {
                confirmChoice = confirmDecision.chosenButton;
                Destroy(confirmDecision.gameObject);
            }
        }
    }

    public Character CheckForTargeted(List<Character> possibleTargets)
    {
        if (targetedPlayer != null && possibleTargets.Contains(targetedPlayer))
            return targetedPlayer;
        if (targetedEnemy != null && possibleTargets.Contains(targetedEnemy))
            return targetedEnemy;

        return null;
    }

    void ResetTargetedPlayer(Character newTarget)
    {
        Character storePlayer = targetedPlayer;
        _targetedPlayer = newTarget;
        if (newTarget != null)
            newTarget.CharacterUI();
        if (storePlayer != null)
            storePlayer.CharacterUI();
    }

    void ResetTargetedEnemy(Character newTarget)
    {
        Character storePlayer = targetedEnemy;
        _targetedEnemy = newTarget;

        if (newTarget != null)
            newTarget.CharacterUI();
        if (storePlayer != null)
            storePlayer.CharacterUI();
    }

    #endregion

}
