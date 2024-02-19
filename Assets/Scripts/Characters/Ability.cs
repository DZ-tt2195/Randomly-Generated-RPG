using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public enum TeamTarget { None, Self, AnyOne, All, OnePlayer, OtherPlayer, OneEnemy, OtherEnemy, AllPlayers, AllEnemies };
public enum AbilityType { None, Attack, Stats, Emotion, Position, Healing, Summon, Misc };

public class Ability : MonoBehaviour
{

#region Setup

    [ReadOnly] public Character self;

    [ReadOnly] public string myName;
    [ReadOnly] public string description;
    [ReadOnly] public string logDescription;

    [ReadOnly] public AbilityType typeOne;
    [ReadOnly] public AbilityType typeTwo;

    [ReadOnly] public string instructions;
    [ReadOnly] public string nextInstructions;
    [ReadOnly] public string playCondition;

    [ReadOnly] public float healthChange;

    [ReadOnly] public int baseCooldown;
    [ReadOnly] public int currentCooldown;

    [ReadOnly] public float modifyAttack;
    [ReadOnly] public float modifyDefense;
    [ReadOnly] public float modifySpeed;
    [ReadOnly] public float modifyLuck;
    [ReadOnly] public float modifyAccuracy;

    [ReadOnly] public Emotion? newEmotion;
    [ReadOnly] public Position? newPosition;

    [ReadOnly] public TeamTarget teamTarget;
    [ReadOnly] public HashSet<TeamTarget> singleTarget = new() { TeamTarget.AnyOne, TeamTarget.OnePlayer, TeamTarget.OtherPlayer, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };

    [ReadOnly] public List<Character> listOfTargets;

    [ReadOnly] public int damageDealt;

    public void SetupAbility(AbilityData data)
    {
        myName = data.myName;
        instructions = data.instructions;
        description = KeywordTooltip.instance.EditText(data.description);
        typeOne = data.typeOne;
        typeTwo = data.typeTwo;
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

                    case "SELFSECONDTIER":
                        return (user.currentEmotion == Emotion.Enraged || user.currentEmotion == Emotion.Ecstatic || user.currentEmotion == Emotion.Depressed);


                    case "TARGETSNEUTRAL":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion != Emotion.Neutral) listOfTargets.RemoveAt(i);
                        break;
                    case "TARGETSNOTNEUTRAL":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion == Emotion.Neutral) listOfTargets.RemoveAt(i);
                        break;
                    case "SELFNEUTRAL":
                        return (user.currentEmotion == Emotion.Neutral);
                    case "SELFNOTNEUTRAL":
                        return (user.currentEmotion != Emotion.Neutral);

                    case "TARGETSHAPPY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion != Emotion.Happy || listOfTargets[i].currentEmotion != Emotion.Ecstatic) listOfTargets.RemoveAt(i);
                        break;
                    case "TARGETSNOTHAPPY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion == Emotion.Happy || listOfTargets[i].currentEmotion == Emotion.Ecstatic) listOfTargets.RemoveAt(i);
                        break;
                    case "SELFHAPPY":
                        return (user.currentEmotion == Emotion.Happy || user.currentEmotion == Emotion.Ecstatic);
                    case "SELFNOTHAPPY":
                        return (user.currentEmotion != Emotion.Happy && user.currentEmotion != Emotion.Ecstatic);

                    case "TARGETSANGRY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion != Emotion.Angry || listOfTargets[i].currentEmotion != Emotion.Enraged) listOfTargets.RemoveAt(i);
                        break;
                    case "TARGETSNOTANGRY":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion == Emotion.Angry || listOfTargets[i].currentEmotion == Emotion.Enraged) listOfTargets.RemoveAt(i);
                        break;
                    case "SELFANGRY":
                        return (user.currentEmotion == Emotion.Angry || user.currentEmotion == Emotion.Enraged);
                    case "SELFNOTANGRY":
                        return (user.currentEmotion != Emotion.Angry && user.currentEmotion != Emotion.Enraged);

                    case "TARGETSSAD":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion != Emotion.Sad || listOfTargets[i].currentEmotion != Emotion.Depressed) listOfTargets.RemoveAt(i);
                        break;
                    case "TARGETSNOTSAD":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentEmotion == Emotion.Sad || listOfTargets[i].currentEmotion == Emotion.Depressed) listOfTargets.RemoveAt(i);
                        break;
                    case "SELFSAD":
                        return (user.currentEmotion == Emotion.Sad || user.currentEmotion == Emotion.Depressed);
                    case "SELFNOTSAD":
                        return (user.currentEmotion != Emotion.Sad && user.currentEmotion != Emotion.Depressed);

                    case "SELFGROUNDED":
                        return user.currentPosition == Position.Grounded;
                    case "TARGETSGROUNDED":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentPosition != Position.Grounded) listOfTargets.RemoveAt(i);
                        break;

                    case "SELFAIRBORNE":
                        return user.currentPosition == Position.Airborne;
                    case "TARGETSAIRBORNE":
                        for (int i = listOfTargets.Count - 1; i >= 0; i--)
                            if (listOfTargets[i].currentPosition != Position.Airborne) listOfTargets.RemoveAt(i);
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
                foreach (Character friend in TurnManager.instance.players) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.AnyOne:
                foreach (Character foe in TurnManager.instance.enemies) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.instance.players) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OnePlayer:
                foreach (Character friend in TurnManager.instance.players) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OtherPlayer:
                foreach (Character friend in TurnManager.instance.players) { if (friend != this.self) listOfTargets.Add(friend); }
                break;
            case TeamTarget.AllPlayers:
                foreach (Character friend in TurnManager.instance.players) { listOfTargets.Add(friend); }
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
            yield return TurnManager.instance.WaitTime();
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
                case "HEALFROMDAMAGE":
                    yield return self.GainHealth(damageDealt, logged);
                    break;

                case "SELFHEAL":
                    yield return self.GainHealth(healthChange, logged);
                    break;
                case "TARGETSHEAL":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        if (listOfTargets[i] != null)
                            yield return listOfTargets[i].GainHealth(healthChange, logged);
                    break;

                case "SELFSWAPPOSITION":
                    if (self.currentPosition == Position.Airborne)
                        yield return self.ChangePosition(Position.Grounded, logged);
                    else if (self.currentPosition == Position.Grounded)
                        yield return self.ChangePosition(Position.Airborne, logged);
                    break;
                case "TARGETSSWAPPOSITION":
                    for (int i = 0; i < listOfTargets.Count; i++)
                    {
                        if (listOfTargets[i].currentPosition == Position.Airborne)
                            yield return listOfTargets[i].ChangePosition(Position.Grounded, logged);
                        else if (listOfTargets[i].currentPosition == Position.Grounded)
                            yield return listOfTargets[i].ChangePosition(Position.Airborne, logged);
                    }
                    break;

                case "SELFGROUNDED":
                    yield return self.ChangePosition(Position.Grounded, logged);
                    break;
                case "TARGETSGROUNDED":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangePosition(Position.Grounded, logged);
                    break;

                case "SELFAIRBORNE":
                    yield return self.ChangePosition(Position.Airborne, logged);
                    break;
                case "TARGETSAIRBORNE":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangePosition(Position.Airborne, logged);
                    break;

                case "SELFHAPPY":
                    yield return self.ChangeEmotion(Emotion.Happy, logged);
                    break;
                case "TARGETSHAPPY":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Emotion.Happy, logged);
                    break;

                case "SELFECSTATIC":
                    yield return self.ChangeEmotion(Emotion.Ecstatic, logged);
                    break;
                case "TARGETSECSTATIC":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Emotion.Ecstatic, logged);
                    break;

                case "SELFSAD":
                    yield return self.ChangeEmotion(Emotion.Sad, logged);
                    break;
                case "TARGETSSAD":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Emotion.Sad, logged);
                    break;

                case "SELFDEPRESSED":
                    yield return self.ChangeEmotion(Emotion.Depressed, logged);
                    break;
                case "TARGETSDEPRESSED":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Emotion.Depressed, logged);
                    break;

                case "SELFANGRY":
                    yield return self.ChangeEmotion(Emotion.Angry, logged);
                    break;
                case "TARGETSANGRY":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Emotion.Angry, logged);
                    break;

                case "SELFENRAGED":
                    yield return self.ChangeEmotion(Emotion.Enraged, logged);
                    break;
                case "TARGETSENRAGED":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Emotion.Enraged, logged);
                    break;

                case "SELFNEUTRAL":
                    yield return self.ChangeEmotion(Emotion.Neutral, logged);
                    break;
                case "TARGETSNEUTRAL":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].ChangeEmotion(Emotion.Neutral, logged);
                    break;

                case "TARGETSAMPLIFY":
                    for (int i = 0; i < listOfTargets.Count; i++)
                    {
                        switch (listOfTargets[i].currentEmotion)
                        {
                            case Emotion.Happy:
                                yield return self.ChangeEmotion(Emotion.Ecstatic, logged);
                                break;
                            case Emotion.Angry:
                                yield return self.ChangeEmotion(Emotion.Enraged, logged);
                                break;
                            case Emotion.Sad:
                                yield return self.ChangeEmotion(Emotion.Depressed, logged);
                                break;
                        }
                    }
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

                case "TARGETSSTUN":
                    for (int i = 0; i < listOfTargets.Count; i++)
                        yield return listOfTargets[i].Stun(1, logged);
                    break;

                case "TARGETSFORCEDCOOLDOWN":
                    for (int i = 0; i < listOfTargets.Count; i++)
                    {
                        foreach (Ability ability in listOfTargets[i].listOfAbilities)
                            if (ability.currentCooldown == 0)
                                ability.currentCooldown++;
                    }
                    break;
                case "TARGETSINCREASEACTIVECOOLDOWN":
                    for (int i = 0; i < listOfTargets.Count; i++)
                    {
                        foreach (Ability ability in listOfTargets[i].listOfAbilities)
                            if (ability.currentCooldown > 0)
                                ability.currentCooldown++;
                    }
                    break;
                case "TARGETSREDUCEACTIVECOOLDOWN":
                    for (int i = 0; i<listOfTargets.Count; i++)
                    {
                        foreach (Ability ability in listOfTargets[i].listOfAbilities)
                            if (ability.currentCooldown > 0)
                                ability.currentCooldown--;
                    }
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
        if (user.currentEmotion == Emotion.Happy)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Angry) => 1.25f,
                (Emotion.Enraged) => 1.25f,
                (Emotion.Sad) => 0.75f,
                (Emotion.Depressed) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Emotion.Ecstatic)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Angry) => 1.5f,
                (Emotion.Enraged) => 1.5f,
                (Emotion.Sad) => 0.5f,
                (Emotion.Depressed) => 0.5f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Emotion.Angry)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Sad) => 1.25f,
                (Emotion.Depressed) => 1.25f,
                (Emotion.Happy) => 0.75f,
                (Emotion.Ecstatic) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Emotion.Enraged)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Sad) => 1.5f,
                (Emotion.Depressed) => 1.5f,
                (Emotion.Happy) => 0.5f,
                (Emotion.Ecstatic) => 0.5f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Emotion.Sad)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Happy) => 1.25f,
                (Emotion.Ecstatic) => 1.25f,
                (Emotion.Angry) => 0.75f,
                (Emotion.Enraged) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Emotion.Depressed)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Happy) => 1.5f,
                (Emotion.Ecstatic) => 1.5f,
                (Emotion.Angry) => 0.5f,
                (Emotion.Enraged) => 0.5f,
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

            int finalDamage = Mathf.Max(0, (int)(damageVariation * critical * effectiveness + (attack * healthChange) - defense));
            return finalDamage;
        }
        else
        {
            Log.instance.AddText($"{user.name}'s attack misses.", 1);
            TurnManager.instance.CreateVisual("MISS", target.transform.localPosition);
            return 0;
        }
    }

#endregion

}