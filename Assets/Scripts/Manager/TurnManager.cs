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
            TitleScreen.instance.listOfPlayers[i].transform.SetParent(grouping);
            TitleScreen.instance.listOfPlayers[i].transform.localPosition = new Vector3(-850 + (600 * i), -550, 0);
        }
    }

    void FixedUpdate()
    {
        Character.borderColor += (decrease) ? -0.05f : 0.05f;
        if (Character.borderColor < 0 || Character.borderColor > 1)
            decrease = !decrease;
    }

    IEnumerator ResolveRound()
    {
        List<Character> speedQueue = CharactersBySpeed();
        yield return null;

        while (speedQueue.Count > 0 && friends.Count > 0 && foes.Count > 0)
        {
            Character nextInLine = speedQueue[0];
            speedQueue.RemoveAt(0);
            yield return nextInLine.ChooseAbility();
        }
    }

    List<Character> CharactersBySpeed()
    {
        List<Character> allTargets = new List<Character>();

        for (int i = 0; i < friends.Count; i++)
            allTargets.Add(friends[i]);
        for (int i = 0; i < foes.Count; i++)
            allTargets.Add(foes[i]);

        allTargets = allTargets.OrderByDescending(o => o.CalculateSpeed()).ToList();
        return allTargets;
    }
} 
