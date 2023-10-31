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
    public string instructions;
    public string playCondition;
    public string description;
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
        instructions = data.instructions;
        description = data.description;
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

    bool RollAccuracy(float value)
    {
        float roll = (Random.Range(0f, 1f));
        bool result = roll <= value;
        return result;
    }

    int RollCritical(float value)
    {
        float roll = (Random.Range(0f, 1f));
        bool result = roll <= value;
        if (result)
        {
            return 2;
        }
        else
            return 1;
    }

    float Effectiveness(Character user, Character target)
    {
        float answer = 1;
        if (user.currentEmotion == Character.Emotion.Happy)
        {
            switch (target.currentEmotion)
            {
                case (Character.Emotion.Angry):
                    answer = 1.25f;
                    break;
                case (Character.Emotion.Enraged):
                    answer = 1.25f;
                    break;
                case (Character.Emotion.Sad):
                    answer = 0.75f;
                    break;
                case (Character.Emotion.Depressed):
                    answer = 0.75f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        }
        else if (user.currentEmotion == Character.Emotion.Ecstatic)
        {
            switch (target.currentEmotion)
            {
                case (Character.Emotion.Angry):
                    answer = 1.5f;
                    break;
                case (Character.Emotion.Enraged):
                    answer = 1.5f;
                    break;
                case (Character.Emotion.Sad):
                    answer = 0.5f;
                    break;
                case (Character.Emotion.Depressed):
                    answer = 0.5f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        }
        else if (user.currentEmotion == Character.Emotion.Angry)
        {
            switch (target.currentEmotion)
            {
                case (Character.Emotion.Sad):
                    answer = 1.25f;
                    break;
                case (Character.Emotion.Depressed):
                    answer = 1.25f;
                    break;
                case (Character.Emotion.Happy):
                    answer = 0.75f;
                    break;
                case (Character.Emotion.Ecstatic):
                    answer = 0.75f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        }
        else if (user.currentEmotion == Character.Emotion.Enraged)
        {
            switch (target.currentEmotion)
            {
                case (Character.Emotion.Sad):
                    answer = 1.5f;
                    break;
                case (Character.Emotion.Depressed):
                    answer = 1.5f;
                    break;
                case (Character.Emotion.Happy):
                    answer = 0.5f;
                    break;
                case (Character.Emotion.Ecstatic):
                    answer = 0.5f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        }
        else if (user.currentEmotion == Character.Emotion.Sad)
        {
            switch (target.currentEmotion)
            {
                case (Character.Emotion.Happy):
                    answer = 1.25f;
                    break;
                case (Character.Emotion.Ecstatic):
                    answer = 1.25f;
                    break;
                case (Character.Emotion.Angry):
                    answer = 0.75f;
                    break;
                case (Character.Emotion.Enraged):
                    answer = 0.75f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        }
        else if (user.currentEmotion == Character.Emotion.Depressed)
        {
            switch (target.currentEmotion)
            {
                case (Character.Emotion.Happy):
                    answer = 1.5f;
                    break;
                case (Character.Emotion.Ecstatic):
                    answer = 1.5f;
                    break;
                case (Character.Emotion.Angry):
                    answer = 0.5f;
                    break;
                case (Character.Emotion.Enraged):
                    answer = 0.5f;
                    break;
                default:
                    answer = 1.0f;
                    break;
            }
        }

        return answer;
    }

    int CalculateDamage(Character user, Character target)
    {
        if (RollAccuracy(user.CalculateAccuracy()))
        {
            return (int)(Random.Range(0.75f, 1.25f) * RollCritical(user.CalculateLuck()) * Effectiveness(user, target) * healthChange * user.CalculateAttack() - target.CalculateDefense());
        }
        else
        {
            return 0;
        }
    }
}
