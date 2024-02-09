using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class Ability : MonoBehaviour
{

#region Setup

    [ReadOnly] public Character self;

    [ReadOnly] public string myName;
    [ReadOnly] public string description;
    [ReadOnly] public string logDescription;

    [ReadOnly] public string instructions;
    [ReadOnly] public string nextInstructions;
    [ReadOnly] public string playCondition;
    [ReadOnly] public int healthChange;

    [ReadOnly] public int baseCooldown;
    [ReadOnly] public int currentCooldown;

    [ReadOnly] public float modifyAttack;
    [ReadOnly] public float modifyDefense;
    [ReadOnly] public float modifySpeed;
    [ReadOnly] public float modifyLuck;
    [ReadOnly] public float modifyAccuracy;

    [ReadOnly] public Character.Emotion? newEmotion;
    [ReadOnly] public Character.Position? newPosition;

    public enum TeamTarget { None, Self, AnyOne, All, OneTeammate, OtherTeammate, OneEnemy, OtherEnemy, AllTeammates, AllEnemies };
    [ReadOnly] public TeamTarget teamTarget;

    [ReadOnly] public int summonHelper;
    [ReadOnly] public List<Character> listOfTargets;

    [ReadOnly] public int damageDealt;

    public void SetupAbility(AbilityData data)
    {
        myName = data.myName;
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
            Log.instance.AddText("Critical hit!", 1);
            return 1.5f;
        }
        else
            return 1f;
    }

    #endregion

#region Play Condition

    public bool CanPlay(Character user)
    {
        if (currentCooldown == 0)
        {
            listOfTargets = GetTargets();

            string divide = playCondition.Replace(" ", "");
            divide = divide.ToUpper();
            string[] methodsInStrings = divide.Split('/');

            for (int i = listOfTargets.Count - 1; i >= 0; i--)
            {
                if (methodsInStrings[0] == "ISDEAD")
                {
                    if (listOfTargets[i].CalculateHealth() > 0)
                    {
                        listOfTargets.RemoveAt(i);
                    }
                }
                else if (listOfTargets[i].CalculateHealth() <= 0)
                {
                    listOfTargets.RemoveAt(i);
                }
            }

            foreach (string nextMethod in methodsInStrings)
            {
                switch (nextMethod)
                {
                    case "":
                        break;
                    case "NONE":
                        break;
                    case "ISDEAD":
                        break;

                    case "SELFGROUNDED":
                        return user.currentPosition == Character.Position.Grounded;
                    case "SELFAIRBORNE":
                        return user.currentPosition == Character.Position.Airborne;

                    case "NOTHAPPY":
                        return (user.currentEmotion != Character.Emotion.Happy && user.currentEmotion != Character.Emotion.Ecstatic);
                    case "SELFHAPPY":
                        return (user.currentEmotion == Character.Emotion.Happy || user.currentEmotion == Character.Emotion.Ecstatic);
                    case "SELFECSTATIC":
                        return (user.currentEmotion == Character.Emotion.Ecstatic);

                    case "NOTANGRY":
                        return (user.currentEmotion != Character.Emotion.Angry && user.currentEmotion != Character.Emotion.Enraged);
                    case "SELFANGRY":
                        return (user.currentEmotion == Character.Emotion.Angry || user.currentEmotion == Character.Emotion.Enraged);
                    case "SELFENRAGED":
                        return (user.currentEmotion == Character.Emotion.Enraged);

                    case "NOTSAD":
                        return (user.currentEmotion != Character.Emotion.Sad && user.currentEmotion != Character.Emotion.Depressed);
                    case "SELFSAD":
                        return (user.currentEmotion == Character.Emotion.Sad || user.currentEmotion == Character.Emotion.Depressed);
                    case "SELFDEPRESSED":
                        return (user.currentEmotion == Character.Emotion.Depressed);

                    case "GROUNDEDONLY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentPosition != Character.Position.Grounded) listOfTargets.RemoveAt(i);
                        break;
                    case "AIRBORNEONLY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentPosition != Character.Position.Airborne) listOfTargets.RemoveAt(i);
                        break;
                    case "CANSUMMON":
                        if (TurnManager.instance.teammates.Count >= 5) return false;
                        break;
                    case "NOTNEUTRAL":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion == Character.Emotion.Neutral) listOfTargets.RemoveAt(i);
                        break;

                    default:
                        Debug.LogError($"{nextMethod} isn't a method");
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

    public IEnumerator ResolveInstructions(string[] listOfMethods, int logged)
    {
        Log.instance.AddText(Log.Substitute(this, self), logged-1);
        foreach (string methodName in listOfMethods)
        {
            yield return TurnManager.instance.WaitTime;
            TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(false);

            switch (methodName)
            {
                case "":
                    break;
                case "NONE":
                    break;
                case "DEALTDAMAGE":
                    if (damageDealt == 0)
                        yield break;
                    break;

                case "ATTACK":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        if (listOfTargets[i] != null)
                        {
                            damageDealt = CalculateDamage(self, listOfTargets[i], logged);
                            yield return listOfTargets[i].TakeDamage(damageDealt, logged);
                        }
                    break;
                case "SELFHEAL":
                    yield return self.GainHealth(healthChange, logged);
                    break;
                case "HEALFROMDAMAGE":
                    yield return self.GainHealth(damageDealt, logged);
                    break;
                case "TARGETSHEAL":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        if (listOfTargets[i] != null)
                            yield return listOfTargets[i].GainHealth(healthChange, logged);
                    break;

                case "SELFGROUNDED":
                    yield return self.ChangePosition(Character.Position.Grounded, logged);
                    break;
                case "TARGETSGROUNDED":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangePosition(Character.Position.Grounded, logged);
                    break;

                case "SELFAIRBORNE":
                    yield return self.ChangePosition(Character.Position.Airborne, logged);
                    break;
                case "TARGETSAIRBORNE":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangePosition(Character.Position.Airborne, logged);
                    break;

                case "SELFHAPPY":
                    yield return self.ChangeEmotion(Character.Emotion.Happy, logged);
                    break;
                case "TARGETSHAPPY":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Happy, logged);
                    break;

                case "SELFECSTATIC":
                    yield return self.ChangeEmotion(Character.Emotion.Ecstatic, logged);
                    break;
                case "TARGETSECSTATIC":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Ecstatic, logged);
                    break;

                case "SELFSAD":
                    yield return self.ChangeEmotion(Character.Emotion.Sad, logged);
                    break;
                case "TARGETSSAD":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Sad, logged);
                    break;

                case "SELFDEPRESSED":
                    yield return self.ChangeEmotion(Character.Emotion.Depressed, logged);
                    break;
                case "TARGETSDEPRESSED":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Depressed, logged);
                    break;

                case "SELFANGRY":
                    yield return self.ChangeEmotion(Character.Emotion.Angry, logged);
                    break;
                case "TARGETSANGRY":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Angry, logged);
                    break;

                case "SELFENRAGED":
                    yield return self.ChangeEmotion(Character.Emotion.Enraged, logged);
                    break;
                case "TARGETSENRAGED":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Enraged, logged);
                    break;

                case "SELFNEUTRAL":
                    yield return self.ChangeEmotion(Character.Emotion.Neutral, logged);
                    break;
                case "TARGETSNEUTRAL":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Character.Emotion.Neutral, logged);
                    break;

                case "SELFATTACKSTAT":
                    yield return self.ChangeAttack(modifyAttack, logged);
                    break;
                case "TARGETSATTACKSTAT":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeAttack(modifyAttack, logged);
                    break;

                case "SELFDEFENSESTAT":
                    yield return self.ChangeDefense(modifyDefense, logged);
                    break;
                case "TARGETSDEFENSESTAT":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeDefense(modifyDefense, logged);
                    break;

                case "SELFSPEEDSTAT":
                    yield return self.ChangeSpeed(modifySpeed, logged);
                    break;
                case "TARGETSSPEEDSTAT":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeSpeed(modifySpeed, logged);
                    break;

                case "SELFLUCKSTAT":
                    yield return self.ChangeLuck(modifyLuck, logged);
                    break;
                case "TARGETSLUCKSTAT":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeLuck(modifyLuck, logged);
                    break;

                case "SELFACCURACYSTAT":
                    yield return self.ChangeAccuracy(modifyAccuracy, logged);
                    break;
                case "TARGETSACCURACYSTAT":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeAccuracy(modifyAccuracy, logged);
                    break;

                case "LEAVEFIGHT":
                    yield return self.HasDied(-1);
                    break;
                case "SELFDESTRUCT":
                    yield return self.HasDied(logged);
                    break;
                case "TARGETSREVIVE":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].Revive(healthChange, logged);
                    break;

                case "SUMMONHELPER":
                    yield return TurnManager.instance.CreateHelper(summonHelper, logged);
                    break;

                default:
                    Debug.LogError($"{methodName} isn't a method");
                    break;
            }
        }
    }

    public float Effectiveness(Character user, Character target, int logged)
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

        if (answer > 1)
            Log.instance.AddText("It's super effective!", logged);
        else if (answer < 1)
            Log.instance.AddText("It's not very effective...", logged);

        return answer;
    }

    int CalculateDamage(Character user, Character target, int logged)
    {
        if (RollAccuracy(user.CalculateAccuracy()))
        {
            float damageVariation = Random.Range(0.8f, 1.2f);
            float effectiveness = Effectiveness(user, target, logged);
            float critical = RollCritical(user.CalculateLuck());
            float attack = user.CalculateAttack();
            float defense = target.CalculateDefense(user);

            int finalDamage = (int)(damageVariation * critical * effectiveness * attack + healthChange - defense);
            return finalDamage > 0 ? finalDamage : 0;
        }
        else
        {
            Log.instance.AddText($"{user.name}'s attack misses.", 1);
            return 0;
        }
    }

#endregion

}