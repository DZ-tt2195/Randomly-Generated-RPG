using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    [SerializeField] EnemyCharacter enemyPrefab;

    public List<AbilityBox> listOfBoxes = new List<AbilityBox>();
    [ReadOnly] public List<Character> teammates = new List<Character>();
    [ReadOnly] public List<Character> enemies = new List<Character>();
    [ReadOnly] public List<Character> speedQueue = new List<Character>();

    bool decrease = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Transform grouping = GameObject.Find("Group of Players").transform;
        for (int i = 0; i < TitleScreen.instance.listOfPlayers.Count; i++)
        {
            Character nextFriend = TitleScreen.instance.listOfPlayers[i];
            teammates.Add(nextFriend);
            nextFriend.transform.SetParent(grouping);
            nextFriend.transform.localPosition = new Vector3(-850 + (600 * i), -550, 0);
        }

        StartCoroutine(ResolveRound());
    }

    void FixedUpdate()
    {
        Character.borderColor += (decrease) ? -0.05f : 0.05f;
        if (Character.borderColor < 0 || Character.borderColor > 1)
            decrease = !decrease;
    }

    public void ListAllFriends()
    {
        string allFriends = "";
        foreach (Character character in teammates)
            allFriends += character.name + ",";
        Debug.Log(allFriends);
    }

    IEnumerator ResolveRound()
    {
        if (enemies.Count == 0)
        {
            int numEnemies = Random.Range(2, 4);
            for (int i = 0; i<numEnemies; i++)
            {
                EnemyCharacter nextCharacter = Instantiate(enemyPrefab);
                nextCharacter.SetupCharacter(Character.CharacterType.Enemy, TitleScreen.instance.listOfEnemies[Random.Range(0, TitleScreen.instance.listOfEnemies.Count)]);

                enemies.Add(nextCharacter);
                nextCharacter.transform.SetParent(TitleScreen.instance.canvas);
                nextCharacter.transform.localPosition = new Vector3(-850 + (600 * i), 300, 0);
            }
        }

        speedQueue = AllCharacters();

        while (speedQueue.Count > 0)
        {
            DisableCharacterButtons();
            speedQueue = speedQueue.OrderByDescending(o => o.CalculateSpeed()).ToList();
            TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(false);

            Character nextInLine = speedQueue[0];
            speedQueue.RemoveAt(0);

            if (nextInLine != null && nextInLine.CalculateHealth() > 0)
            {
                nextInLine.border.gameObject.SetActive(true);
                yield return nextInLine.MyTurn();
            }
        }

        foreach (Character character in teammates)
        {
            foreach (Ability ability in character.listOfAbilities)
            {
                if (ability.currentCooldown > 0)
                    ability.currentCooldown--;
            }
        }

        foreach(Character character in enemies)
        {
            foreach (Ability ability in character.listOfAbilities)
            {
                if (ability.currentCooldown > 0)
                    ability.currentCooldown--;
            }
        }

        StartCoroutine(ResolveRound());
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
} 
