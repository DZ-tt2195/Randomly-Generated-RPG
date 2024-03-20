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

public class TurnManager : MonoBehaviour
{

#region Variables

    public static TurnManager instance;
    [Foldout("Prefabs", true)]
        [SerializeField] GameObject characterPrefab;
        [SerializeField] PointsVisual pointsVisual;
        public TextCollector undoBox;

    [Foldout("UI", true)]
        public List<AbilityBox> listOfBoxes = new();
        [SerializeField] List<TMP_Text> listOfSpeed = new();
        [SerializeField] List<RightClickMe> listOfSpeedImages = new();
        public TMP_Text instructions;
        bool borderDecrease = true;
        [SerializeField] Button quitButton;
        List<CharacterPositions> teammatePositions = new();
        List<CharacterPositions> enemyPositions = new();

    [Foldout("Character lists", true)]
        [ReadOnly] public List<Character> players = new();
        [ReadOnly] public List<Character> enemies = new();
        [ReadOnly] public List<Character> speedQueue = new List<Character>();

    [Foldout("Info tracking", true)]
        int currentWave;
        int currentRound;
        float waveMultiplier = 1f;
        bool isBattling = true;

#endregion

#region Setup

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i<5; i++)
        {
            int nextX = -1050 + (350 * i);
            teammatePositions.Add(new CharacterPositions(new Vector3(nextX, -550, 0)));
            enemyPositions.Add(new CharacterPositions(new Vector3(nextX, 300, 0)));
        }

        if (FileManager.instance.mode == FileManager.GameMode.Main)
        {
            for (int i = 0; i < FileManager.instance.listOfPlayers.Count; i++)
            {
                Character nextCharacter = FileManager.instance.listOfPlayers[i];
                players.Add(nextCharacter);
            }

            StartCoroutine(NewWave());
        }
    }

#endregion

#region Gameplay

    public IEnumerator NewWave()
    {
        instructions.text = "";
        instructions.gameObject.transform.parent.gameObject.SetActive(false);
        DisableCharacterButtons();

        yield return WaitTime();

        currentWave++;
        waveMultiplier = 1 + (currentWave - 1) * 0.02f;

        listOfSpeedImages[0].transform.parent.parent.gameObject.SetActive(false);

        if (currentWave > 5)
        {
            GameFinished("You won!", $"Survived 10 waves.");
        }
        else
        {
            Log.instance.AddText($"WAVE {currentWave} / 5");
            if (currentWave > 1 && PlayerPrefs.GetInt("Scaling Enemies") == 1)
            {
                Log.instance.AddText($"Enemies are now {100 * (waveMultiplier - 1):F0}% stronger.");
                Log.instance.AddText("");
            }

            if (FileManager.instance.mode == FileManager.GameMode.Main)
            {
                for (int i = 0; i < currentWave; i++)
                    CreateEnemy(
                        FileManager.instance.listOfEnemies[UnityEngine.Random.Range(0, FileManager.instance.listOfEnemies.Count)],
                        (Emotion)UnityEngine.Random.Range(1, 5), 0, PlayerPrefs.GetInt("Scaling Enemies") == 1 ? waveMultiplier : 1f);

                speedQueue = AllCharacters();
                while (speedQueue.Count > 0)
                {
                    DisableCharacterButtons();
                    speedQueue = speedQueue.OrderByDescending(o => o.CalculateSpeed()).ToList();

                    Character nextInLine = speedQueue[0];
                    speedQueue.RemoveAt(0);

                    if (nextInLine != null && nextInLine.CalculateHealth() > 0)
                    {
                        instructions.text = "";
                        nextInLine.border.gameObject.SetActive(true);

                        if (nextInLine.weapon != null)
                            yield return nextInLine.weapon.WeaponEffect(SpliceString(nextInLine.weapon.data.newWave), 0);
                        nextInLine.border.gameObject.SetActive(false);
                    }
                }
                StartCoroutine(NewRound());
            }
        }
    }

    void DisplaySpeedQueue()
    {
        listOfSpeedImages[0].transform.parent.parent.gameObject.SetActive(true);
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
        /*
        for (int i = 0; i < listOfSpeed.Count; i++)
        {
            try
            {
                listOfSpeed[i].text = (i == 0) ? $"Current: {speedQueue[i].name}" : $"{i}: {speedQueue[i].name}";
            }
            catch (ArgumentOutOfRangeException)
            {
                listOfSpeed[i].text = "";
            }
            catch (MissingReferenceException)
            {
                listOfSpeed[i].text = "";
            }
        }
        */
    }

    public IEnumerator NewRound()
    {
        currentRound++;
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

            if (nextInLine != null && nextInLine.CalculateHealth() > 0)
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
            else if (enemies.Count == 0 && FileManager.instance.mode == FileManager.GameMode.Main)
            {
                Log.instance.AddText($"");
                StartCoroutine(NewWave());
                yield break;
            }
        }

        if (isBattling && FileManager.instance.mode == FileManager.GameMode.Main)
            StartCoroutine(NewRound());
    }

    bool CheckLost()
    {
        foreach (Character character in players)
        {
            if (character.CalculateHealth() > 0)
                return false;
        }

        return true;
    }

    public void GameFinished(string message1, string message2)
    {
        StopAllCoroutines();
        isBattling = false;
        DisableCharacterButtons();

        instructions.text = "";
        listOfBoxes[0].transform.parent.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);

        Log.instance.AddText("");
        Log.instance.AddText(message1);
        Log.instance.AddText(message2);
        Log.instance.enabled = false;
    }

#endregion

#region Misc

    void FixedUpdate()
    {
        Character.borderColor += (borderDecrease) ? -0.05f : 0.05f;
        if (Character.borderColor < 0 || Character.borderColor > 1)
            borderDecrease = !borderDecrease;
    }

    public Character CreateEnemy(CharacterData dataFile, Emotion startingEmotion, int logged, float multiplier = 1f)
    {
        EnemyCharacter nextCharacter = Instantiate(characterPrefab).AddComponent<EnemyCharacter>();
        nextCharacter.transform.SetParent(FileManager.instance.canvas);
        nextCharacter.transform.SetAsFirstSibling();
        enemies.Add(nextCharacter);

        foreach (CharacterPositions position in enemyPositions)
        {
            if (position.character == null)
            {
                nextCharacter.transform.localPosition = position.position;
                position.character = nextCharacter;
                break;
            }
        }

        nextCharacter.name = dataFile.myName;
        Log.instance.AddText($"{Log.Article(nextCharacter.name)} entered the fight.", logged);

        List<AbilityData> characterAbilities = new();
        string[] divideSkillsIntoNumbers = dataFile.skillNumbers.Split(',');
        for (int j = 0; j < divideSkillsIntoNumbers.Length; j++)
        {
            try
            {
                string skillNumber = divideSkillsIntoNumbers[j];
                skillNumber.Trim();
                characterAbilities.Add(FileManager.instance.listOfAbilities[int.Parse(skillNumber)]);
            }
            catch (FormatException) { continue; }
            catch (ArgumentOutOfRangeException) { break; }
        }

        nextCharacter.SetupCharacter(CharacterType.Enemy, dataFile, characterAbilities, startingEmotion, multiplier, null);
        SaveManager.instance.AddEnemy(dataFile);

        if (FileManager.instance.mode == FileManager.GameMode.Main && PlayerPrefs.GetInt("Enemies Stunned") == 1)
            StartCoroutine(nextCharacter.Stun(1, logged + 1));
        return nextCharacter;
    }

    public void AddPlayer(Character character)
    {
        players.Add(character);
        character.transform.SetParent(FileManager.instance.canvas);
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

        if (character.weapon != null)
            SaveManager.instance.AddWeapon(character.weapon.data);
        foreach (Ability ability in character.listOfAbilities)
            SaveManager.instance.AddAbility(character.name, ability.data);
    }

    public void DisableCharacterButtons()
    {
        foreach (Character character in players)
        {
            character.myButton.interactable = false;
            character.border.gameObject.SetActive(false);
        }
        foreach (Character character in enemies)
        {
            character.myButton.interactable = false;
            character.border.gameObject.SetActive(false);
        }
    }

    public List<Character> AllCharacters()
    {
        List<Character> allTargets = new List<Character>();
        allTargets.AddRange(players);
        allTargets.AddRange(enemies);
        return allTargets;
    }

    public static string[] SpliceString(string text)
    {
        if (!text.IsNullOrEmpty())
        {
            string divide = text.Replace(" ", "");
            divide = divide.ToUpper().Trim();
            string[] splitIntoStrings = divide.Split('/');
            return splitIntoStrings;
        }

        return new string[0];
    }

    public IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(PlayerPrefs.GetFloat("Animation Speed"));
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
        PointsVisual pv = Instantiate(pointsVisual, FileManager.instance.canvas);
        pv.Setup(text, position);
    }

#endregion

}
