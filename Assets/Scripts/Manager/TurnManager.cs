using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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

    public static TurnManager inst;
    [Foldout("Prefabs", true)]
        [SerializeField] GameObject characterPrefab;
        [SerializeField] PointsVisual pointsVisual;
        [SerializeField] List<WaveSetup> listOfWaveSetup = new();
        [SerializeField] [Scene] string titleScreen;
 
    [Foldout("Other UI", true)]
        Queue<PointsVisual> visualStorage = new();
        bool borderDecrease = true;
        List<CharacterPositions> teammatePositions = new();
        List<CharacterPositions> enemyPositions = new();
        [SerializeField] TMP_Text waveText;
        [SerializeField] TMP_Text roundText;
        [SerializeField] Button quitButton;

    [Foldout("Character lists", true)]
        [ReadOnly] public System.Random dailyRNG;
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
        Stopwatch gameTimer;
    [Foldout("Translate", true)]
        [SerializeField] TMP_Text quit;

    private void Awake()
    {
        inst = this;
        isBattling = true;
        waveText.transform.parent.localPosition = new Vector3(0, 1200, 0);

        quit.text = AutoTranslate.Quit_Game();
        quitButton.onClick.AddListener(() => GameFinished(AutoTranslate.Quit_Fight(), false));

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
    void Start()
    {
        if (ScreenOverlay.instance.mode == GameMode.Daily)
        {
            DateTime day = DateTime.UtcNow.Date;
            Log.instance.AddText(Translator.inst.Translate(AutoTranslate.Daily_Challenge()), 0);
            Log.instance.AddText(AutoTranslate.Current_Date(Translator.inst.Translate($"Month_{day.Month}"), day.Day.ToString(), day.Year.ToString())); 
        }
    }

    #endregion

#region Gameplay Loop
    public IEnumerator NewWave()
    {
        MakeDecision.inst.BlankUI();
        if (currentWave == 0)
        {
            gameTimer = new Stopwatch();
            gameTimer.Start();
        }

        if (currentWave > listOfWaveSetup.Count)
        {
            GameFinished(AutoTranslate.Game_Won(), true);
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

            if (currentWave >= 2 && FightRules.inst.CheckRule(nameof(AutoTranslate.Inspiration), 0))
            {
                foreach (Character player in listOfPlayers)
                {
                    for (int i = player.listOfRandomAbilities.Count - 1; i >= 0; i--)
                        player.DropAbility(player.listOfRandomAbilities[i]);

                    List<AbilityData> newAbilties = GameFiles.inst.CompletePlayerAbilities(new(), GameFiles.inst.ConvertToAbilityData(player.data.listOfAbilities, true), dailyRNG);
                    foreach (AbilityData data in newAbilties)
                        player.AddAbility(data, false, false);
                }
                Log.instance.AddText(AutoTranslate.Blank());
            }
            if (currentWave < 5 && FightRules.inst.CheckRule(nameof(AutoTranslate.Crowds), 0))
                yield return CreateEnemy(GameFiles.inst.RandomEnemy(1, dailyRNG), Character.RandomMood(dailyRNG), 0);                
            foreach (int nextTier in listOfWaveSetup[currentWave - 1].enemyDifficultySpawn)
                yield return CreateEnemy(GameFiles.inst.RandomEnemy(nextTier, dailyRNG), Character.RandomMood(dailyRNG), 0);

            if (FightRules.inst.CheckRule(nameof(AutoTranslate.Charge_Up), 0))
            {
                int randomNum = (dailyRNG != null) ? dailyRNG.Next(0, listOfEnemies.Count) : UnityEngine.Random.Range(0, listOfEnemies.Count);
                Character randomEnemy = listOfEnemies[randomNum];

                yield return randomEnemy.ChangeMaxHealth(5, 1);
                yield return randomEnemy.ChangeHealth(5, 1);
                yield return randomEnemy.ChangeStat(Stats.Power, 2, 1);
                yield return randomEnemy.ChangeStat(Stats.Defense, 2, 1);
                yield return randomEnemy.ChangeEffect(StatusEffect.Stunned, 3, 1);
            }

            StartCoroutine(NewRound(false));
        }
    }
    public IEnumerator NewRound(bool playAnimation)
    {
        if (playAnimation) yield return NewAnimation(false);
        Log.instance.AddText($"");
        Log.instance.AddText(AutoTranslate.Round(currentRound.ToString()));

        speedQueue = AllCharacters();
        bool topsyTurvy = FightRules.inst.CheckRule(nameof(AutoTranslate.Topsy_Turvy), 0);
        foreach (Character character in speedQueue)
        {
            if (character.StatEffectdict[StatusEffect.Protected] >= 1)
                yield return character.ChangeEffect(StatusEffect.Protected, -1, -1);
            if (character.StatEffectdict[StatusEffect.Locked] >= 1)
                yield return character.ChangeEffect(StatusEffect.Locked, -1, -1);
            if (character.StatEffectdict[StatusEffect.Targeted] >= 1)
                yield return character.ChangeEffect(StatusEffect.Targeted, -1, -1);
            
            if (topsyTurvy && character is PlayerCharacter)
            {
                if (character.CurrentPosition == Position.Grounded)
                    yield return character.ChangePosition(Position.Elevated, -1);
                else
                    yield return character.ChangePosition(Position.Grounded, -1);
            }
        }

        while (speedQueue.Count > 0)
        {
            MakeDecision.inst.BlankUI();
            Character nextInLine = speedQueue[0];
            speedQueue.RemoveAt(0);

            if (nextInLine != null && nextInLine.currentHealth > 0)
            {
                Log.instance.AddText($"");
                nextInLine.border.gameObject.SetActive(true);

                yield return nextInLine.MyTurn(0);
                try { nextInLine.border.gameObject.SetActive(false); } catch { /*do nothing*/}
            }

            if (CheckLost())
            {
                GameFinished(AutoTranslate.Game_Lost(), false);
                yield break;
            }
            else if (listOfEnemies.Count == 0)
            {
                if (ScreenOverlay.instance.mode != GameMode.Tutorial)
                {
                    Log.instance.AddText(AutoTranslate.Blank());
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
    public void GameFinished(string message, bool win)
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
        quitButton.gameObject.SetActive(false);

        Log.instance.AddText("");
        Log.instance.AddText(Translator.inst.Translate(message));

        int number = (win) ? currentWave : currentWave - 1;
        Log.instance.AddText(AutoTranslate.Waves_Survived(number.ToString()));

        if (gameTimer != null)
        {
            gameTimer.Stop();
            Log.instance.AddText(AutoTranslate.Time_Taken(MyExtensions.StopwatchTime(gameTimer)));
        }
        MakeDecision.inst.BlankUI();

        MakeDecision.inst.SetTextButtons(Translator.inst.Translate(message), new() { new(AutoTranslate.Title_Screen(), BackToTitle) });
        void BackToTitle()
        {
            SceneManager.LoadScene(titleScreen);
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
    public void CreateVisual(string text, Vector3 position)
    {
        PointsVisual newVisual = (visualStorage.Count > 0) ? visualStorage.Dequeue() : Instantiate(pointsVisual, ScreenOverlay.instance.sceneCanvas);
        newVisual.Setup(text, position, PlayerPrefs.GetFloat("Animation Speed")*2, 1, Color.white);
    }
    public void ReturnVisual(PointsVisual visual)
    {
        visualStorage.Enqueue(visual);
        visual.gameObject.SetActive(false);
    }

#endregion

#region Misc
    public List<Character> AllCharacters()
    {
        List<Character> allTargets = new();
        allTargets.AddRange(listOfPlayers);
        allTargets.AddRange(listOfEnemies);
        return allTargets;
    }
    public IEnumerator CreateEnemy(CharacterData dataFile, Mood startingMood, int logged)
    {
        if (listOfEnemies.Count < 5)
        {
            yield return WaitTime();
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

            nextEnemy.name = Translator.inst.Translate(dataFile.characterName);
            Log.instance.AddText(AutoTranslate.Enter_Fight(nextEnemy.name), logged);
            nextEnemy.SetupCharacter(dataFile, GameFiles.inst.ConvertToAbilityData(dataFile.listOfAbilities, false), startingMood, listOfEnemies.Count, true);
            if (currentWave < 5 && FightRules.inst.CheckRule(nameof(AutoTranslate.Crowds), -1))
                yield return nextEnemy.ChangeMaxHealth(-1, -1);
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
