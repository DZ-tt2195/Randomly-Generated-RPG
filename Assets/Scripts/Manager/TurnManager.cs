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
        [SerializeField] Button emotionGuide;
        [SerializeField] GameObject emotionTransform;
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
        emotionGuide.onClick.AddListener(SeeEmotions);
        WaitTime = new WaitForSeconds(PlayerPrefs.GetFloat("Animation Speed"));
    }

    private void Start()
    {
        for (int i = 0; i < FileManager.instance.listOfPlayers.Count; i++)
        {
            Character nextFriend = FileManager.instance.listOfPlayers[i];
            teammates.Add(nextFriend);
            nextFriend.transform.SetParent(FileManager.instance.canvas);
            nextFriend.transform.localPosition = new Vector3(-1000 + (500 * i), -550, 0);
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
        
        Log.instance.AddText($"WAVE {currentWave}", 0);
        if (currentWave > 1) Log.instance.AddText($"Enemies are now {100 * (enemyMultiplier - 1):F0}% stronger.", 0);

        int randomNum = Random.Range(2, 4);
        for (int i = 0; i < randomNum; i++)
        {
            yield return CreateEnemy(Random.Range(0, FileManager.instance.listOfEnemies.Count), enemyMultiplier, 1);
        }

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
                Log.instance.AddText($"", 0);
                nextInLine.border.gameObject.SetActive(true);

                if (nextInLine.weapon != null)
                    yield return nextInLine.weapon.NewWave(1);
                nextInLine.border.gameObject.SetActive(false);
            }

            CheckGameOver();

            if (stillBattling)
            {
                if (enemies.Count == 0)
                {
                    Log.instance.AddText($"", 0);
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
        Log.instance.AddText($"", 0);
        Log.instance.AddText($"ROUND {currentRound}", 0);

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
                Log.instance.AddText($"", 0);
                nextInLine.border.gameObject.SetActive(true);

                yield return nextInLine.MyTurn(0);
                nextInLine.border.gameObject.SetActive(false);
            }

            CheckGameOver();

            if (stillBattling)
            {
                if (enemies.Count == 0)
                {
                    Log.instance.AddText($"", 0);
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

        Log.instance.AddText("", 0);
        Log.instance.AddText("You lost.", 0);
        Log.instance.AddText($"Survived {currentWave-1} waves.", 0);
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

    void SeeEmotions()
    {
        emotionTransform.SetActive(true);
        emotionTransform.transform.SetAsLastSibling();
    }

    private void Update()
    {
        if (emotionTransform.activeSelf && Input.GetMouseButtonDown(0))
        {
            emotionTransform.SetActive(false);
        }
    }

    public IEnumerator CreateHelper(int ID, int logged)
    {
        PlayerCharacter nextCharacter = Instantiate(helperPrefab);
        nextCharacter.transform.SetParent(FileManager.instance.canvas);
        nextCharacter.transform.localPosition = new Vector3(500, -550, 0);
        teammates.Add(nextCharacter);
        Log.instance.AddText($"{Log.Article(FileManager.instance.listOfHelpers[ID].myName)} entered the fight.", logged);
        yield return (nextCharacter.SetupCharacter(Character.CharacterType.Teammate, FileManager.instance.listOfHelpers[ID], true, null));
    }

    public IEnumerator CreateEnemy(int ID, float multiplier, int logged)
    {
        EnemyCharacter nextCharacter = Instantiate(enemyPrefab);
        nextCharacter.transform.SetParent(FileManager.instance.canvas);
        nextCharacter.transform.localPosition = new Vector3(-1000 + (500 * enemies.Count), 300, 0);
        enemies.Add(nextCharacter);
        Log.instance.AddText($"{Log.Article(FileManager.instance.listOfEnemies[ID].myName)} entered the fight.", logged);
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
