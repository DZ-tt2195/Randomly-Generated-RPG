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
    [ReadOnly] public List<Character> friends = new List<Character>();
    [ReadOnly] public List<Character> foes = new List<Character>();

    private void Awake()
    {
        instance = this;
    }

    IEnumerator ChooseSkills()
    {
        yield return null;
    }

    IEnumerator ResolveRound()
    {
        StopCoroutine(ChooseSkills());
        List<Character> speedQueue = CharactersBySpeed();
        yield return null;

        while (speedQueue.Count > 0 && friends.Count > 0 && foes.Count > 0)
        {
            Character nextInLine = speedQueue[0];
            speedQueue.RemoveAt(0);
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
