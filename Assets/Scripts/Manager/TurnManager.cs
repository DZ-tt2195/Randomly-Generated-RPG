using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Linq;

public class CharacterPositions
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
        [SerializeField] EnemyCharacter enemyPrefab;
        [SerializeField] PlayerCharacter helperPrefab;

    [Foldout("UI", true)]
        public List<AbilityBox> listOfBoxes = new List<AbilityBox>();
        public TMP_Text instructions;
        bool decrease = true;
        [SerializeField] Button quitButton;
        List<CharacterPositions> teammatePositions = new();
        List<CharacterPositions> enemyPositions = new();

    [Foldout("Character lists", true)]
        [ReadOnly] public List<Character> teammates = new List<Character>();
        [ReadOnly] public List<Character> enemies = new List<Character>();
        [ReadOnly] public List<Character> speedQueue = new List<Character>();

    [Foldout("Info tracking", true)]
        int currentWave;
        int currentRound;
        float enemyMultiplier = 1f;
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
            teammates.Add(nextCharacter);
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

        yield return WaitTime();

        currentWave++;
        enemyMultiplier = 1 + (currentWave - 1) * 0.05f;
        
        Log.instance.AddText($"WAVE {currentWave}");
        if (currentWave > 1)
        {
            Log.instance.AddText($"Enemies are now {100 * (enemyMultiplier - 1):F0}% stronger.");
            Log.instance.AddText("");
        }

        int randomNum = Mathf.Min(currentWave+1, Random.Range(3, 6));
        for (int i = 0; i < randomNum; i++)
        {
            yield return CreateEnemy(Random.Range(0, FileManager.instance.listOfEnemies.Count), enemyMultiplier, 0);
        }

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
                    yield return nextInLine.weapon.NewWave(0);
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
        foreach (Character character in teammates)
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
        teammates.Add(nextCharacter);

        foreach (CharacterPositions position in teammatePositions)
        {
            if (position.character == null)
            {
                nextCharacter.transform.localPosition = position.position;
                position.character = nextCharacter;
                break;
            }
        }

        nextCharacter.name = FileManager.instance.listOfHelpers[ID].myName;
        Log.instance.AddText($"{Log.Article(nextCharacter.name)} entered the fight.", logged);
        yield return (nextCharacter.SetupCharacter(Character.CharacterType.Teammate, FileManager.instance.listOfHelpers[ID], true, null));
    }

    public IEnumerator CreateEnemy(int ID, float multiplier, int logged)
    {
        EnemyCharacter nextCharacter = Instantiate(enemyPrefab);
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

#endregion

}
