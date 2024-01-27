using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class Ability : MonoBehaviour
{

#region Setup

    public Character self;

    public string myName;
    public string description;
    public string logDescription;

    public string instructions;
    public string nextInstructions;
    public string playCondition;
    public int healthChange;

    public int baseCooldown;
    public int currentCooldown;

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
        logDescription = data.logDescription;
        playCondition = data.playCondition;
        healthChange = data.healthChange;
        baseCooldown = data.cooldown; currentCooldown = baseCooldown;
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
        float roll = Random.Range(0f, 1f);
        bool result = roll <= value;
        return result;
    }

    float RollCritical(float value)
    {
        float roll = Random.Range(0f, 1f);
        bool result = roll <= value;
        if (result)
        {
            Log.instance.AddText("Critical hit!");
            return 1.5f;
        }
        else
            return 1f;
    }

    #endregion

#region Play Condition

    public bool CanPlay()
    {
        if (currentCooldown == 0)
        {
            listOfTargets = GetTargets();

            string divide = playCondition.Replace(" ", "");
            divide = divide.ToUpper();
            string[] methodsInStrings = divide.Split('/');

            for (int i = listOfTargets.Count - 1; i >= 0; i--)
            {
                if (methodsInStrings[0] == "ISDEAD" && listOfTargets[i].CalculateHealth() > 0)
                    listOfTargets.RemoveAt(i);
                else if (listOfTargets[i].CalculateHealth() <= 0)
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
                        if (TurnManager.instance.teammates.Count == 4) return false;
                        break;
                    case "NOTNEUTRAL":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion == Character.Emotion.Neutral) listOfTargets.RemoveAt(i);
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
                foreach (Character foe in TurnManager.instance.enemies) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.instance.teammates) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.AnyOne:
                foreach (Character foe in TurnManager.instance.enemies) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.instance.teammates) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OneTeammate:
                foreach (Character friend in TurnManager.instance.teammates) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OtherTeammate:
                foreach (Character friend in TurnManager.instance.teammates) { if (friend != this.self) listOfTargets.Add(friend); }
                break;
            case TeamTarget.AllTeammates:
                foreach (Character friend in TurnManager.instance.teammates) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OneEnemy:
                foreach (Character foe in TurnManager.instance.enemies) { listOfTargets.Add(foe); }
                break;
            case TeamTarget.OtherEnemy:
                foreach (Character foe in TurnManager.instance.enemies) { if (foe != this.self) listOfTargets.Add(foe); }
                listOfTargets.Remove(this.self);
                break;
            case TeamTarget.AllEnemies:
                foreach (Character foe in TurnManager.instance.enemies) { listOfTargets.Add(foe); }
                break;
        }
        return listOfTargets;
    }

#endregion

#region Play Instructions

    public IEnumerator ResolveMethod(string methodName)
    {
        switch (methodName)
        {
            case "ATTACK":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].TakeDamage(CalculateDamage(self, listOfTargets[i]));
                break;
            case "SELFHEAL":
                yield return self.GainHealth(healthChange);
                break;
            case "TARGETSHEAL":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].GainHealth(healthChange);
                break;

            case "SELFGROUNDED":
                yield return self.ChangePosition(Character.Position.Grounded, true);
                break;
            case "TARGETSGROUNDED":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangePosition(Character.Position.Grounded, true); 
                break;

            case "SELFAIRBORNE":
                yield return self.ChangePosition(Character.Position.Airborne, true);
                break;
            case "TARGETSAIRBORNE":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangePosition(Character.Position.Airborne, true);
                break;

            case "SELFHAPPY":
                yield return self.ChangeEmotion(Character.Emotion.Happy, true);
                break;
            case "TARGETSHAPPY":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Happy, true);
                break;

            case "SELFECSTATIC":
                yield return self.ChangeEmotion(Character.Emotion.Ecstatic, true);
                break;
            case "TARGETSECSTATIC":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Ecstatic, true);
                break;

            case "SELFSAD":
                yield return self.ChangeEmotion(Character.Emotion.Sad, true);
                break;
            case "TARGETSSAD":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Sad, true);
                break;

            case "SELFDEPRESSED":
                yield return self.ChangeEmotion(Character.Emotion.Depressed, true);
                break;
            case "TARGETSDEPRESSED":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Depressed, true);
                break;

            case "SELFANGRY":
                yield return self.ChangeEmotion(Character.Emotion.Angry, true);
                break;
            case "TARGETSANGRY":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Angry, true);
                break;

            case "SELFENRAGED":
                yield return self.ChangeEmotion(Character.Emotion.Enraged, true);
                break;
            case "TARGETSENRAGED":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Enraged, true);
                break;

            case "SELFNEUTRAL":
                yield return self.ChangeEmotion(Character.Emotion.Neutral, true);
                break;
            case "TARGETSNEUTRAL":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Neutral, true);
                break;

            case "SELFATTACKSTAT":
                yield return self.ChangeAttack(modifyAttack);
                break;
            case "TARGETSATTACKSTAT":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeAttack(modifyAttack);
                break;

            case "SELFDEFENSESTAT":
                yield return self.ChangeDefense(modifyDefense);
                break;
            case "TARGETSDEFENSESTAT":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeDefense(modifyDefense);
                break;

            case "SELFSPEEDSTAT":
                yield return self.ChangeSpeed(modifySpeed);
                break;
            case "TARGETSSPEEDSTAT":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeSpeed(modifySpeed);
                break;

            case "SELFLUCKSTAT":
                yield return self.ChangeLuck(modifyLuck);
                break;
            case "TARGETSLUCKSTAT":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeLuck(modifyLuck);
                break;

            case "SELFACCURACYSTAT":
                yield return self.ChangeAccuracy(modifyAccuracy);
                break;
            case "TARGETSACCURACYSTAT":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].ChangeAccuracy(modifyAccuracy);
                break;

            case "SELFDESTRUCT":
                yield return self.HasDied();
                break;
            case "TARGETSREVIVE":
                for (int i = 0; i < listOfTargets.Count; i++)
                    yield return listOfTargets[i].Revive(healthChange);
                break;

            case "SUMMONHELPER":
                yield return TurnManager.instance.CreateHelper(summonHelper);
                break;

            default:
                Debug.LogError($"{methodName} isn't a method");
                break;
        }
    }

    public float Effectiveness(Character user, Character target)
    {
        float answer = 1;
        if (user.currentEmotion == Character.Emotion.Happy)
        {
            answer = target.currentEmotion switch
            {
                (Character.Emotion.Angry) => 1.25f,
                (Character.Emotion.Enraged) => 1.25f,
                (Character.Emotion.Sad) => 0.75f,
                (Character.Emotion.Depressed) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Character.Emotion.Ecstatic)
        {
            answer = target.currentEmotion switch
            {
                (Character.Emotion.Angry) => 1.5f,
                (Character.Emotion.Enraged) => 1.5f,
                (Character.Emotion.Sad) => 0.5f,
                (Character.Emotion.Depressed) => 0.5f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Character.Emotion.Angry)
        {
            answer = target.currentEmotion switch
            {
                (Character.Emotion.Sad) => 1.25f,
                (Character.Emotion.Depressed) => 1.25f,
                (Character.Emotion.Happy) => 0.75f,
                (Character.Emotion.Ecstatic) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Character.Emotion.Enraged)
        {
            answer = target.currentEmotion switch
            {
                (Character.Emotion.Sad) => 1.5f,
                (Character.Emotion.Depressed) => 1.5f,
                (Character.Emotion.Happy) => 0.5f,
                (Character.Emotion.Ecstatic) => 0.5f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Character.Emotion.Sad)
        {
            answer = target.currentEmotion switch
            {
                (Character.Emotion.Happy) => 1.25f,
                (Character.Emotion.Ecstatic) => 1.25f,
                (Character.Emotion.Angry) => 0.75f,
                (Character.Emotion.Enraged) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Character.Emotion.Depressed)
        {
            answer = target.currentEmotion switch
            {
                (Character.Emotion.Happy) => 1.5f,
                (Character.Emotion.Ecstatic) => 1.5f,
                (Character.Emotion.Angry) => 0.5f,
                (Character.Emotion.Enraged) => 0.5f,
                _ => 1.0f,
            };
        }

        return answer;
    }

    int CalculateDamage(Character user, Character target)
    {
        if (RollAccuracy(user.CalculateAccuracy()))
        {
            float damageVariation = Random.Range(0.8f, 1.2f);
            float critical = RollCritical(user.CalculateLuck());
            float effectiveness = Effectiveness(user, target);
            float attack = user.CalculateAttack();
            float defense = target.CalculateDefense(user);
            return (int)(damageVariation * critical * effectiveness * attack + healthChange - defense);
        }
        else
        {
            Log.instance.AddText($"{user.name}'s attack misses.");
            return 0;
        }
    }

#endregion

}