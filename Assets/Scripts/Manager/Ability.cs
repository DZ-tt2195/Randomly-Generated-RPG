using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class StringAndMethod
{
    [ReadOnly] public Dictionary<string, IEnumerator> dictionary = new Dictionary<string, IEnumerator>();

    public StringAndMethod(Ability ability)
    {
        //dictionary["DRAWCARDS"] = ability.DrawCards();
    }
}

public class Ability : MonoBehaviour
{
    public string playCondition;
    public int healthChange;
    public int energyCost;

    public float modifyAttack;
    public float modifyDefense;
    public float modifySpeed;
    public float modifyLuck;
    public float modifyAccuracy;

    public Character.Emotion? newEmotion;
    public Character.Position? newPosition;

    public enum TeamTarget { None, Self, All, OnePlayer, OneEnemy, AllPlayers, AllEnemies};
    public TeamTarget teamTarget;
    public enum PositionTarget { None, All, OnlyGrounded, OnlyAirborne };
    public PositionTarget positionTarget;

    public void SetupAbility(AbilityData data)
    {
        this.name = data.name;
        playCondition = data.playCondition;
        healthChange = data.healthChange;
        energyCost = data.energyCost;
        modifyAttack = data.modifyAttack;
        modifyDefense = data.modifyDefense;
        modifySpeed = data.modifySpeed;
        modifyLuck = data.modifyLuck;
        modifyAccuracy = data.modifyAccuracy;
        newEmotion = data.newEmotion;
        newPosition = data.positionChange;
        teamTarget = data.teamTarget;
        positionTarget = data.positionTarget;
    }
}
