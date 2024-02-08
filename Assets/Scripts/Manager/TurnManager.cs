using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Linq;

public class TurnManager : MonoBehaviour
{

#region Variables

    public static TurnManager instance;
    [Foldout("Prefabs", true)]
        [SerializeField] EnemyCharacter enemyPrefab;
        [SerializeField] PlayerCharacter helperPrefab;
        [ReadOnly] public WaitForSeconds WaitTime;

    [Foldout("UI", true)]
        public List<AbilityBox> listOfBoxes = new List<AbilityBox>();
        public TMP_Text instructions;
        bool decrease = true;
        [SerializeField] Button quitButton;

    [Foldout("Character lists", true)]
        [ReadOnly] public List<Character> teammates = new List<Character>();
        [ReadOnly] public List<Character> enemies = new List<Character>();
        [ReadOnly] public List<Character> speedQueue = new List<Character>();

    [Foldout("Info tracking", true)]
        int currentWave;
        int currentRound;
        float enemyMultiplier = 1f;
        bool stillBattling = true;

#endregion

#region Setup

    private void Awake()
    {
        instance = this;
        WaitTime = new WaitForSeconds(PlayerPrefs.GetFloat("Animation Speed"));
    }

    private void Start()
    {
        for (int i = 0; i < FileManager.instance.listOfPlayers.Count; i++)
        {
            Character nextCharacter = FileManager.instance.listOfPlayers[i];
            teammates.Add(nextCharacter);
            nextCharacter.transform.SetParent(FileManager.instance.canvas);
            nextCharacter.transform.SetAsFirstSibling();
            nextCharacter.transform.localPosition = new Vector3(-1050 + (350 * i), -550, 0);
        }

        quitButton.gameObject.SetActive(false);
        StartCoroutine(NewWave());
    }

#endregion

#region Gameplay

    IEnumerator NewWave()
    {
        instructions.text = "";
        listOfBoxes[0].transform.parent.gameObject.SetActive(false);
        DisableCharacterButtons();

        yield return WaitTime;

        currentWave++;
        enemyMultiplier = 1 + (currentWave - 1) * 0.05f;
        
        Log.instance.AddText($"WAVE {currentWave}");
        if (currentWave > 1)
        {
            Log.instance.AddText($"Enemies are now {100 * (enemyMultiplier - 1):F0}% stronger.");
            Log.instance.AddText("");
        }

        int randomNum = Mathf.Min(currentWave, Random.Range(3, 6));
        for (int i = 0; i < randomNum; i++)
        {
            yield return CreateEnemy(Random.Range(0, FileManager.instance.listOfEnemies.Count), enemyMultiplier, 0);
        }

        Log.instance.AddText($"");

        speedQueue = AllCharacters();
        while (speedQueue.Count > 0 && stillBattling)
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
                    yield return nextInLine.weapon.NewWave(0);
                nextInLine.border.gameObject.SetActive(false);
            }

            CheckGameOver();

            if (stillBattling)
            {
                if (enemies.Count == 0)
                {
                    StartCoroutine(NewWave());
                    yield break;
                }
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

        while (speedQueue.Count > 0 && stillBattling)
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

            if (stillBattling)
            {
                if (enemies.Count == 0)
                {
                    Log.instance.AddText($"");
                    StartCoroutine(NewWave());
                    yield break;
                }
            }
        }

        StartCoroutine(NewRound());
    }

    void CheckGameOver()
    {
        foreach (Character character in teammates)
        {
            if (character.CalculateHealth() > 0)
                return;
        }

        stillBattling = false;
        DisableCharacterButtons();

        instructions.text = "";
        listOfBoxes[0].transform.parent.gameObject.SetActive(false);

        Log.instance.AddText("");
        Log.instance.AddText("You lost.");
        Log.instance.AddText($"Survived {currentWave-1} waves.");
        quitButton.gameObject.SetActive(true);
    }

#endregion

#region Misc

    void FixedUpdate()
    {
        Character.borderColor += (decrease) ? -0.05f : 0.05f;
        if (Character.borderColor < 0 || Character.borderColor > 1)
            decrease = !decrease;
    }

    public IEnumerator CreateHelper(int ID, int logged)
    {
        PlayerCharacter nextCharacter = Instantiate(helperPrefab);
        nextCharacter.transform.SetParent(FileManager.instance.canvas);
        nextCharacter.transform.SetAsFirstSibling();
        nextCharacter.transform.localPosition = new Vector3(-1050 + (350 * teammates.Count), -550, 0);
        teammates.Add(nextCharacter);

        nextCharacter.name = FileManager.instance.listOfHelpers[ID].myName;
        Log.instance.AddText($"{Log.Article(nextCharacter.name)} entered the fight.", logged);
        yield return (nextCharacter.SetupCharacter(Character.CharacterType.Teammate, FileManager.instance.listOfHelpers[ID], true, null));
    }

    public IEnumerator CreateEnemy(int ID, float multiplier, int logged)
    {
        EnemyCharacter nextCharacter = Instantiate(enemyPrefab);
        nextCharacter.transform.SetParent(FileManager.instance.canvas);
        nextCharacter.transform.SetAsFirstSibling();
        nextCharacter.transform.localPosition = new Vector3(-1050 + (350 * enemies.Count), 300, 0);
        enemies.Add(nextCharacter);

        nextCharacter.name = FileManager.instance.listOfEnemies[ID].myName;
        Log.instance.AddText($"{Log.Article(nextCharacter.name)} entered the fight.", logged);
        yield return (nextCharacter.SetupCharacter(Character.CharacterType.Enemy, FileManager.instance.listOfEnemies[ID], false, null, multiplier));
    }

    public void DisableCharacterButtons()
    {
        foreach (Character character in teammates)
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
        allTargets.AddRange(teammates);
        allTargets.AddRange(enemies);
        return allTargets;
    }

    public static string[] SpliceString(string text)
    {
        string divide = text.Replace(" ", "");
        divide = divide.ToUpper();
        string[] splitIntoStrings = divide.Split('/');
        return splitIntoStrings;
    }

#endregion

}
