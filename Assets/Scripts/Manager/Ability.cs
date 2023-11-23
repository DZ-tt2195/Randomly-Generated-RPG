using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using UnityEngine.UIElements;

public class Ability : MonoBehaviour
{
#region Setup
    public Character self;

    public string myName;
    public string instructions;
    public string nextInstructions;
    public string playCondition;
    public string description;
    public int healthChange;
    public int cooldown;

    public float modifyAttack;
    public float modifyDefense;
    public float modifySpeed;
    public float modifyLuck;
    public float modifyAccuracy;

    public Character.Emotion? newEmotion;
    public Character.Position? newPosition;

    public enum TeamTarget { None, Self, AnyOne, All, OneTeammate, OtherTeammate, OneEnemy, OtherEnemy, AllTeammates, AllEnemies };
    public TeamTarget teamTarget;

    public int summonHelper;

    public List<Character> listOfTargets;

    public void SetupAbility(AbilityData data)
    {
        myName = data.name;
        instructions = data.instructions;
        nextInstructions = data.nextInstructions;
        description = data.description;
        playCondition = data.playCondition;
        healthChange = data.healthChange;
        cooldown = data.cooldown;
        modifyAttack = data.modifyAttack;
        modifyDefense = data.modifyDefense;
        modifySpeed = data.modifySpeed;
        modifyLuck = data.modifyLuck;
        modifyAccuracy = data.modifyAccuracy;
        teamTarget = data.teamTarget;
        summonHelper = data.helperID;
        self = GetComponent<Character>();
    }

    #endregion

#region Stats

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

    #endregion

#region Play Condition

    public bool CanPlay()
    {
        if (cooldown == 0)
        {
            listOfTargets = GetTargets();

            string divide = playCondition.Replace(" ", "");
            divide = divide.ToUpper();
            string[] methodsInStrings = divide.Split('/');

            for (int i = listOfTargets.Count - 1; i >= 0; i--)
            {
                if (methodsInStrings[0] == "ISDEAD" && listOfTargets[i].GetHealth() > 0)
                    listOfTargets.RemoveAt(i);
                else if (listOfTargets[i].GetHealth() < 0)
                    listOfTargets.RemoveAt(i);
            }

            foreach (string nextMethod in methodsInStrings)
            {
                switch (nextMethod)
                {
                    case "GROUNDEDONLY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentPosition != Character.Position.Grounded) listOfTargets.RemoveAt(i);
                        break;
                    case "AIRBORNEONLY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentPosition != Character.Position.Airborne) listOfTargets.RemoveAt(i);
                        break;
                    case "NOHELPER":
                        if (TurnManager.instance.friends.Count == 4) return false;
                        break;
                    case "NOTNEUTRAL":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion != Character.Emotion.Neutral) listOfTargets.RemoveAt(i);
                        break;
                }
            }

            if (teamTarget == TeamTarget.None)
                return true;
            else
                return listOfTargets.Count > 0;
        }
        else
        {
            return false;
        }
    }

    List<Character> GetTargets()
    {
        List<Character> listOfTargets = new List<Character>();

        switch (teamTarget)
        {
            case TeamTarget.None:
                break;
            case TeamTarget.Self:
                listOfTargets.Add(this.self);
                break;
            case TeamTarget.All:
                foreach (Character foe in TurnManager.instance.foes) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.instance.friends) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.AnyOne:
                foreach (Character foe in TurnManager.instance.foes) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.instance.friends) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OneTeammate:
                foreach (Character friend in TurnManager.instance.friends) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OtherTeammate:
                foreach (Character friend in TurnManager.instance.friends) { if (friend != this.self) listOfTargets.Add(friend); }
                break;
            case TeamTarget.AllTeammates:
                foreach (Character friend in TurnManager.instance.friends) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OneEnemy:
                foreach (Character foe in TurnManager.instance.foes) { listOfTargets.Add(foe); }
                break;
            case TeamTarget.OtherEnemy:
                foreach (Character foe in TurnManager.instance.foes) { if (foe != this.self) listOfTargets.Add(foe); }
                listOfTargets.Remove(this.self);
                break;
            case TeamTarget.AllEnemies:
                foreach (Character foe in TurnManager.instance.foes) { listOfTargets.Add(foe); }
                break;
        }
        return listOfTargets;
    }

    #endregion
}