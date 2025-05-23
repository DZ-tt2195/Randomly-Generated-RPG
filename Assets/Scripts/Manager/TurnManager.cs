using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Linq;
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
        [SerializeField] List<RightClickMe> listOfSpeedImages = new();
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

    private void Awake()
    {
        instance = this;
        isBattling = true;
        waveText.transform.parent.localPosition = new Vector3(0, 1200, 0);

        resignButton.onClick.AddListener(() => GameFinished("You quit.", $"Survived {currentWave - 1} {(currentWave - 1 == 1 ? "wave" : "waves")}."));

        if (CarryVariables.instance.mode == CarryVariables.GameMode.Daily)
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

        if (currentWave > 5)
        {
            GameFinished("You won!", $"Survived 5 waves.");
        }
        else if (CarryVariables.instance.mode == CarryVariables.GameMode.Tutorial)
        {
            yield return NewAnimation(true);
            Log.instance.AddText($"WAVE {currentWave} / 3");
        }
        else
        {
            yield return NewAnimation(true);
            Log.instance.AddText($"WAVE {currentWave} / 5");

            if (currentWave >= 2 && CarryVariables.instance.ActiveCheat("New Abilities"))
            {
                Log.instance.AddText($"Players have new abilities.");
                foreach (Character player in listOfPlayers)
                {
                    for (int i = player.listOfRandomAbilities.Count - 1; i >= 0; i--)
                        player.DropAbility(player.listOfRandomAbilities[i]);

                    List<AbilityData> newAbilties = CarryVariables.instance.CompletePlayerAbilities(new(), player.name, dailyRNG);
                    foreach (AbilityData data in newAbilties)
                        player.AddAbility(data, false, false);
                }
                Log.instance.AddText("");
            }

            foreach (int nextTier in listOfWaveSetup[currentWave - 1].enemyDifficultySpawn)
                yield return MakeNewEnemy(CarryVariables.instance.listOfEnemies[nextTier]);
            if (currentWave <= 4 && CarryVariables.instance.ActiveChallenge("More Enemies"))
                yield return MakeNewEnemy(CarryVariables.instance.listOfEnemies[1]);

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
            yield return NewAnimation(false);

        Log.instance.AddText($"");
        Log.instance.AddText($"ROUND {currentRound}");

        speedQueue = AllCharacters();

        while (speedQueue.Count > 0)
        {
            DisableCharacterButtons();
            DisplaySpeedQueue();
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
                GameFinished("You lost.", $"Survived {currentWave - 1} {(currentWave - 1 == 1 ? "wave" : "waves")}.");
                yield break;
            }
            else if (listOfEnemies.Count == 0)
            {
                if (CarryVariables.instance.mode != CarryVariables.GameMode.Tutorial)
                {
                    Log.instance.AddText($"");
                    StartCoroutine(NewWave());
                }
                yield break;
            }
        }

        if (isBattling && CarryVariables.instance.mode != CarryVariables.GameMode.Tutorial)
            StartCoroutine(NewRound(true));
    }

    IEnumerator NewAnimation(bool increaseWave)
    {
        Vector3 originalPos = new Vector3(0, 1200, 0);
        Vector3 finalPos = new Vector3(0, -1200, 0);

        waveText.transform.parent.localPosition = originalPos;
        waveText.text = $"WAVE {currentWave} / {(CarryVariables.instance.mode == CarryVariables.GameMode.Tutorial ? "3" : "5")}";
        roundText.text = $"ROUND {currentRound}";

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
        roundText.text = $"ROUND {currentRound}";
        if (increaseWave)
        {
            currentWave++;
            waveText.text = $"WAVE {currentWave} / {(CarryVariables.instance.mode == CarryVariables.GameMode.Tutorial ? "3" : "5")}";
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

    public void GameFinished(string message1, string message2)
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
        listOfSpeedImages[0].transform.parent.gameObject.SetActive(false);

        Log.instance.AddText("");
        Log.instance.AddText(message1);
        Log.instance.AddText(message2);
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

        try
        {
            foreach (string text in buttons)
                collector.AddTextButton(text);
        }
        catch (NullReferenceException)
        {
            //do nothing
        }
        return collector;
    }

    public void CreateVisual(string text, Vector3 position)
    {
        PointsVisual pv = Instantiate(pointsVisual, CarryVariables.instance.sceneCanvas);
        pv.Setup(text, position);
    }

    void DisplaySpeedQueue()
    {
        listOfSpeedImages[0].transform.parent.gameObject.SetActive(true);
        speedQueue = speedQueue.OrderByDescending(o => o.CalculateSpeed()).ToList();

        for (int i = 0; i < listOfSpeedImages.Count; i++)
        {
            try
            {
                listOfSpeedImages[i].gameObject.SetActive(true);
                listOfSpeedImages[i].character = speedQueue[i];
                listOfSpeedImages[i].image.sprite = speedQueue[i].myImage.sprite;
                listOfSpeedImages[i].image.color = speedQueue[i].myImage.color;
            }
            catch (ArgumentOutOfRangeException)
            {
                listOfSpeedImages[i].gameObject.SetActive(false);
            }
            catch (MissingReferenceException)
            {
                listOfSpeedImages[i].gameObject.SetActive(false);
            }
        }
    }

#endregion

#region Misc

    public void CreateEnemy(CharacterData dataFile, Emotion startingEmotion, int logged)
    {
        if (listOfEnemies.Count < 5)
        {
            EnemyCharacter nextEnemy = Instantiate(characterPrefab).AddComponent<EnemyCharacter>();
            nextEnemy.transform.SetParent(CarryVariables.instance.sceneCanvas);
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

            nextEnemy.name = dataFile.myName;
            Log.instance.AddText($"{Log.Article(nextEnemy.name)} entered the fight.", logged);

            nextEnemy.SetupCharacter(dataFile, CarryVariables.instance.ConvertToAbilityData(dataFile.listOfSkills, false), startingEmotion, true);
            if (CarryVariables.instance.ActiveCheat("Weaker Enemies"))
                StartCoroutine(nextEnemy.ChangeMaxHealth(-2, logged + 1));
            if (CarryVariables.instance.ActiveChallenge("Extra Enemy Turns"))
                StartCoroutine(nextEnemy.ChangeEffect(StatusEffect.Extra, 1, logged+1));
        }
    }

    public void AddPlayer(Character character)
    {
        listOfPlayers.Add(character);
        character.transform.SetParent(CarryVariables.instance.sceneCanvas);
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

    List<Character> AllCharacters()
    {
        List<Character> allTargets = new();
        allTargets.AddRange(listOfPlayers);
        allTargets.AddRange(listOfEnemies);
        return allTargets.Shuffle();
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

    public bool CheckForTargeted(List<Character> possibleTargets)
    {
        if (targetedPlayer != null && possibleTargets.Contains(targetedPlayer))
            return true;

        return (targetedEnemy != null && possibleTargets.Contains(targetedEnemy));
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
