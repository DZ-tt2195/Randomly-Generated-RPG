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
    [SerializeField] EnemyCharacter enemyPrefab;
    [SerializeField] PlayerCharacter helperPrefab;

    public List<AbilityBox> listOfBoxes = new List<AbilityBox>();
    [ReadOnly] public List<Character> teammates = new List<Character>();
    [ReadOnly] public List<Character> enemies = new List<Character>();
    [ReadOnly] public List<Character> speedQueue = new List<Character>();

    bool decrease = true;
    int currentWave;
    int currentRound;
    bool stillBattling = true;

#endregion

#region Setup

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < TitleScreen.instance.listOfPlayers.Count; i++)
        {
            Character nextFriend = TitleScreen.instance.listOfPlayers[i];
            teammates.Add(nextFriend);
            nextFriend.transform.SetParent(TitleScreen.instance.canvas);
            nextFriend.transform.localPosition = new Vector3(-1000 + (500 * i), -550, 0);
        }

        NewWave();
    }

#endregion

#region Gameplay

    void NewWave()
    {
        currentWave++;
        Log.instance.AddText($"WAVE {currentWave}");

        int numEnemies = Random.Range(2, 4);
        for (int i = 0; i < numEnemies; i++)
        {
            CreateEnemy(Random.Range(0, TitleScreen.instance.listOfEnemies.Count));
        }

        StartCoroutine(ResolveRound());
    }

    IEnumerator ResolveRound()
    {
        currentRound++;
        Log.instance.AddText($"");
        Log.instance.AddText($"ROUND {currentRound}");

        if (enemies.Count == 0)
        {
            int numEnemies = Random.Range(2, 4);
            for (int i = 0; i < numEnemies; i++)
            {
                CreateEnemy(Random.Range(0, TitleScreen.instance.listOfEnemies.Count));
            }
        }

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
                Log.instance.AddText($"");
                nextInLine.border.gameObject.SetActive(true);
                yield return nextInLine.MyTurn();
                nextInLine.border.gameObject.SetActive(false);
            }

            CheckTeammates();
        }

        if (stillBattling)
        {
            if (enemies.Count > 0)
                StartCoroutine(ResolveRound());
            else
                NewWave();
        }
    }

    void CheckTeammates()
    {
        foreach (Character character in teammates)
        {
            if (character.CalculateHealth() > 0)
                return;
        }

        stillBattling = false;
        Log.instance.AddText("");
        Log.instance.AddText("You lost.");
    }

    #endregion

#region Misc

    void FixedUpdate()
    {
        Character.borderColor += (decrease) ? -0.05f : 0.05f;
        if (Character.borderColor < 0 || Character.borderColor > 1)
            decrease = !decrease;
    }

    public void CreateHelper(int ID)
    {
        PlayerCharacter nextCharacter = Instantiate(helperPrefab);
        nextCharacter.SetupCharacter(Character.CharacterType.Teammate, TitleScreen.instance.listOfHelpers[ID], true);
        Log.instance.AddText($"{Log.Article(nextCharacter.name)} entered the fight.");

        nextCharacter.transform.SetParent(TitleScreen.instance.canvas);
        nextCharacter.transform.localPosition = new Vector3(500, -550, 0);
        teammates.Add(nextCharacter);
    }

    public void CreateEnemy(int ID)
    {
        EnemyCharacter nextCharacter = Instantiate(enemyPrefab);
        nextCharacter.SetupCharacter(Character.CharacterType.Enemy, TitleScreen.instance.listOfEnemies[ID], false);
        Log.instance.AddText($"{Log.Article(nextCharacter.name)} entered the fight.");

        nextCharacter.transform.SetParent(TitleScreen.instance.canvas);
        nextCharacter.transform.localPosition = new Vector3(-1000 + (500 * enemies.Count), 300, 0);
        enemies.Add(nextCharacter);
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

#endregion

}
