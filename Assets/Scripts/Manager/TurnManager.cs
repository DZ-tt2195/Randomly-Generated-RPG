using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public List<AbilityBox> listOfBoxes = new List<AbilityBox>();
    [ReadOnly] public List<Character> friends = new List<Character>();
    [ReadOnly] public List<Character> foes = new List<Character>();
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
            friends.Add(nextFriend);
            nextFriend.transform.SetParent(grouping);
            nextFriend.transform.localPosition = new Vector3(-850 + (600 * i), -550, 0);
        }
        //foes.Add(friends[2]);

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
        foreach (Character character in friends)
            allFriends += character.name + ",";
        Debug.Log(allFriends);
    }

    IEnumerator ResolveRound()
    {
        speedQueue = AllCharacters();

        while (speedQueue.Count > 0)
        //while (speedQueue.Count > 0 && friends.Count > 0 && foes.Count > 0)
        {
            DisableCharacterButtons();
            speedQueue = speedQueue.OrderByDescending(o => o.CalculateSpeed()).ToList();
            TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(false);

            Character nextInLine = speedQueue[0];
            speedQueue.RemoveAt(0);

            nextInLine.border.gameObject.SetActive(true);
            yield return nextInLine.MyTurn();
        }

        foreach (Character character in friends)
        {
            foreach (Ability ability in character.listOfAbilities)
            {
                if (ability.cooldown > 0)
                    ability.cooldown--;
            }
        }

        foreach(Character character in foes)
        {
            foreach (Ability ability in character.listOfAbilities)
            {
                if (ability.cooldown > 0)
                    ability.cooldown--;
            }
        }

        StartCoroutine(ResolveRound());
    }

    public void DisableCharacterButtons()
    {
        foreach (Character character in friends)
        {
            character.button.interactable = false;
            character.border.gameObject.SetActive(false);
        }
        foreach (Character character in foes)
        {
            character.button.interactable = false;
            character.border.gameObject.SetActive(false);
        }
    }

    public List<Character> AllCharacters()
    {
        List<Character> allTargets = new List<Character>();
        allTargets.AddRange(friends);
        allTargets.AddRange(foes);
        return allTargets;
    }
} 
