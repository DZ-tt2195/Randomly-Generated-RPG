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
        public List<AbilityBox> listOfBoxes = new List<AbilityBox>();
        public TMP_Text instructions;
        bool borderDecrease = true;
        [SerializeField] Button quitButton;
        List<CharacterPositions> teammatePositions = new();
        List<CharacterPositions> enemyPositions = new();

    [Foldout("Character lists", true)]
        [ReadOnly] public List<Character> players = new List<Character>();
        [ReadOnly] public List<Character> enemies = new List<Character>();
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

        for (int i = 0; i < FileManager.instance.listOfPlayers.Count; i++)
        {
            Character nextCharacter = FileManager.instance.listOfPlayers[i];
            players.Add(nextCharacter);
            nextCharacter.transform.SetParent(FileManager.instance.canvas);
            nextCharacter.transform.SetAsFirstSibling();

            foreach (CharacterPositions position in teammatePositions)
            {
                if (position.character == null)
                {
                    nextCharacter.transform.localPosition = position.position;
                    position.character = nextCharacter;
                    break;
                }
            }

            if (nextCharacter.weapon != null)
                SaveManager.instance.AddWeapon(nextCharacter.weapon.data);
            foreach (Ability ability in nextCharacter.listOfAbilities)
                SaveManager.instance.AddAbility(nextCharacter.name, ability.data);
        }

        quitButton.gameObject.SetActive(false);
        StartCoroutine(NewWave());
    }

#endregion

#region Gameplay

    IEnumerator NewWave()
    {
        instructions.text = "";
        instructions.gameObject.transform.parent.gameObject.SetActive(false);
        DisableCharacterButtons();

        yield return WaitTime();

        currentWave++;
        waveMultiplier = 1 + (currentWave - 1) * 0.02f;

        Log.instance.AddText($"WAVE {currentWave}");
        if (currentWave > 1 && PlayerPrefs.GetInt("Scaling Enemies") == 1)
        {
            Log.instance.AddText($"Enemies are now {100 * (waveMultiplier - 1):F0}% stronger.");
            Log.instance.AddText("");
        }

        int randomNum = Mathf.Min(currentWave, UnityEngine.Random.Range(3, 6));
        for (int i = 0; i < randomNum; i++)
            CreateEnemy(UnityEngine.Random.Range(0, FileManager.instance.listOfEnemies.Count), PlayerPrefs.GetInt("Scaling Enemies") == 1 ? waveMultiplier : 1f, 0);
        Log.instance.AddText($"");

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

    IEnumerator NewRound()
    {
        currentRound++;
        Log.instance.AddText($"");
        Log.instance.AddText($"ROUND {currentRound}");

        speedQueue = AllCharacters();

        while (speedQueue.Count > 0)
        {
            DisableCharacterButtons();
            speedQueue = speedQueue.OrderByDescending(o => o.CalculateSpeed()).ToList();
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

            CheckGameOver();

            if (isBattling && enemies.Count == 0)
            {
                Log.instance.AddText($"");
                StartCoroutine(NewWave());
                yield break;
            }
        }

        if (isBattling)
            StartCoroutine(NewRound());
    }

    void CheckGameOver()
    {
        foreach (Character character in players)
        {
            if (character.CalculateHealth() > 0)
                return;
        }

        StopAllCoroutines();
        isBattling = false;
        DisableCharacterButtons();

        instructions.text = "";
        listOfBoxes[0].transform.parent.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);

        Log.instance.AddText("");
        Log.instance.AddText("You lost.");
        Log.instance.AddText($"Survived {currentWave - 1} {(currentWave - 1 == 1 ? "wave" : "waves")}.");
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

    public void CreateEnemy(int ID, float multiplier, int logged)
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

        CharacterData dataFile = FileManager.instance.listOfEnemies[ID];
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

        nextCharacter.SetupCharacter(CharacterType.Enemy, dataFile, characterAbilities, (Emotion)UnityEngine.Random.Range(1, 5), multiplier, null);
        SaveManager.instance.AddEnemy(dataFile);

        if (PlayerPrefs.GetInt("Enemies Stunned") == 1)
            StartCoroutine(nextCharacter.Stun(1, logged + 1));
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

    public TextCollector MakeTextCollector(string header, Vector3 position, List<string> buttons)
    {
        TextCollector collector = Instantiate(undoBox);
        collector.StatsSetup(header, position);

        foreach (string text in buttons)
            collector.AddTextButton(text);
        return collector;
    }

    public void CreateVisual(string text, Vector3 position)
    {
        PointsVisual pv = Instantiate(pointsVisual, FileManager.instance.canvas);
        pv.Setup(text, position);
    }

#endregion

}
