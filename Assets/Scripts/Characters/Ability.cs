using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum TeamTarget { None, Self, AnyOne, All, OnePlayer, OtherPlayer, OneEnemy, OtherEnemy, AllPlayers, AllEnemies };
public enum AbilityType { None, Attack, Stats, Emotion, Position, Healing, Misc };

public class Ability : MonoBehaviour
{

#region Setup

    [ReadOnly] public AbilityData data;
    [ReadOnly] public Character self;

    [ReadOnly] public string myName;
    [ReadOnly] public string description;
    [ReadOnly] public string logDescription;

    [ReadOnly] public AbilityType typeOne;
    [ReadOnly] public AbilityType typeTwo;

    [ReadOnly] public string instructions;
    [ReadOnly] public string nextInstructions;
    [ReadOnly] public string playCondition;

    [ReadOnly] public float attackPower;
    [ReadOnly] public float healthRegain;
    [ReadOnly] public int miscNumber;

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
    [ReadOnly] public bool killed;

    public void SetupAbility(AbilityData data, bool startWithCooldown)
    {
        this.data = data;
        myName = data.myName;
        instructions = data.instructions;
        description = KeywordTooltip.instance.EditText(data.description);
        typeOne = data.typeOne;
        typeTwo = data.typeTwo;
        logDescription = data.logDescription;
        playCondition = data.playCondition;
        baseCooldown = data.cooldown;
        currentCooldown = (startWithCooldown) ? baseCooldown : 0;
        attackPower = data.attackPower;
        healthRegain = data.healthRegain;
        modifyAttack = data.modifyAttack;
        modifyDefense = data.modifyDefense;
        modifySpeed = data.modifySpeed;
        modifyLuck = data.modifyLuck;
        modifyAccuracy = data.modifyAccuracy;
        teamTarget = data.teamTarget;
        miscNumber = data.miscNumber;
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
        if (this.myName == "Skip Turn")
            return true;
        if (currentCooldown > 0)
            return false;

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

        foreach (string methodName in methodsInStrings)
        {
            switch (methodName)
            {
                case "":
                    break;
                case "NONE":
                    break;
                case "ISDEAD":
                    break;

                case "TARGETSNEUTRAL":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion != Emotion.Neutral) listOfTargets.RemoveAt(i);
                    break;
                case "TARGETSNOTNEUTRAL":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion == Emotion.Neutral) listOfTargets.RemoveAt(i);
                    break;
                case "SELFNEUTRAL":
                    if (user.currentEmotion == Emotion.Neutral)
                        return false; break;
                case "SELFNOTNEUTRAL":
                    if (user.currentEmotion != Emotion.Neutral)
                        return false; break;

                case "TARGETSHAPPY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion != Emotion.Happy) listOfTargets.RemoveAt(i);
                    break;
                case "TARGETSNOTHAPPY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion == Emotion.Happy) listOfTargets.RemoveAt(i);
                    break;
                case "SELFHAPPY":
                    if (user.currentEmotion == Emotion.Happy)
                        return false; break;
                case "SELFNOTHAPPY":
                    if (user.currentEmotion != Emotion.Happy)
                        return false; break;

                case "TARGETSANGRY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion != Emotion.Angry) listOfTargets.RemoveAt(i);
                    break;
                case "TARGETSNOTANGRY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion == Emotion.Angry) listOfTargets.RemoveAt(i);
                    break;
                case "SELFANGRY":
                    if (user.currentEmotion == Emotion.Angry)
                        return false; break;
                case "SELFNOTANGRY":
                    if (user.currentEmotion != Emotion.Angry)
                        return false; break;

                case "TARGETSSAD":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion != Emotion.Sad) listOfTargets.RemoveAt(i);
                    break;
                case "TARGETSNOTSAD":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentEmotion == Emotion.Sad) listOfTargets.RemoveAt(i);
                    break;
                case "SELFSAD":
                    if (user.currentEmotion == Emotion.Sad)
                        return false; break;
                case "SELFNOTSAD":
                    if (user.currentEmotion != Emotion.Sad)
                        return false; break;

                case "SELFGROUNDED":
                    if (user.currentPosition == Position.Grounded)
                        return false; break;
                case "TARGETSGROUNDED":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentPosition != Position.Grounded) listOfTargets.RemoveAt(i);
                    break;

                case "SELFAIRBORNE":
                    if (user.currentPosition == Position.Airborne)
                        return false; break;
                case "TARGETSAIRBORNE":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].currentPosition != Position.Airborne) listOfTargets.RemoveAt(i);
                    break;

                default:
                    Debug.LogError($"{self.name}: {methodName} isn't a method");
                    break;
            }
        }

        if (teamTarget == TeamTarget.None)
            return true;
        else
            return listOfTargets.Count > 0;
    }

    List<Character> GetTargets()
    {
        List<Character> listOfTargets = new List<Character>();

        switch (teamTarget)
        {
            case TeamTarget.None:
                listOfTargets.Add(this.self);
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
        killed = false;
        TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(false);

        foreach (Character target in listOfTargets)
        {
            if (target == null)
                continue;

            foreach (string methodName in listOfMethods)
            {
                yield return TurnManager.instance.WaitTime();

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
                        damageDealt = CalculateDamage(self, target, logged);
                        yield return target.TakeDamage(damageDealt, logged);
                        if (target == null || target.CalculateHealth() <= 0) killed = true;
                        break;
                    case "HEALFROMDAMAGE":
                        yield return self.GainHealth(damageDealt, logged);
                        break;

                    case "SELFHEAL":
                        yield return self.GainHealth(healthRegain, logged);
                        break;
                    case "TARGETSHEAL":
                        yield return target.GainHealth(healthRegain, logged);
                        break;

                    case "SELFSWAPPOSITION":
                        if (self.currentPosition == Position.Airborne)
                            yield return self.ChangePosition(Position.Grounded, logged);
                        else if (self.currentPosition == Position.Grounded)
                            yield return self.ChangePosition(Position.Airborne, logged);
                        break;
                    case "TARGETSSWAPPOSITION":
                        if (target.currentPosition == Position.Airborne)
                            yield return target.ChangePosition(Position.Grounded, logged);
                        else if (target.currentPosition == Position.Grounded)
                            yield return target.ChangePosition(Position.Airborne, logged);
                        break;

                    case "SELFGROUNDED":
                        yield return self.ChangePosition(Position.Grounded, logged);
                        break;
                    case "TARGETSGROUNDED":
                        yield return target.ChangePosition(Position.Grounded, logged);
                        break;

                    case "SELFAIRBORNE":
                        yield return self.ChangePosition(Position.Airborne, logged);
                        break;
                    case "TARGETSAIRBORNE":
                        yield return target.ChangePosition(Position.Airborne, logged);
                        break;

                    case "SELFHAPPY":
                        yield return self.ChangeEmotion(Emotion.Happy, logged);
                        break;
                    case "TARGETSHAPPY":
                        yield return target.ChangeEmotion(Emotion.Happy, logged);
                        break;

                    case "SELFSAD":
                        yield return self.ChangeEmotion(Emotion.Sad, logged);
                        break;
                    case "TARGETSSAD":
                        yield return target.ChangeEmotion(Emotion.Sad, logged);
                        break;

                    case "SELFANGRY":
                        yield return self.ChangeEmotion(Emotion.Angry, logged);
                        break;
                    case "TARGETSANGRY":
                        yield return target.ChangeEmotion(Emotion.Angry, logged);
                        break;

                    case "SELFNEUTRAL":
                        yield return self.ChangeEmotion(Emotion.Neutral, logged);
                        break;
                    case "TARGETSNEUTRAL":
                        yield return target.ChangeEmotion(Emotion.Neutral, logged);
                        break;

                    case "SELFATTACKSTAT":
                        yield return self.ChangeAttack(modifyAttack, logged);
                        break;
                    case "TARGETSATTACKSTAT":
                        yield return target.ChangeAttack(modifyAttack, logged);
                        break;

                    case "SELFDEFENSESTAT":
                        yield return self.ChangeDefense(modifyDefense, logged);
                        break;
                    case "TARGETSDEFENSESTAT":
                        yield return target.ChangeDefense(modifyDefense, logged);
                        break;

                    case "SELFSPEEDSTAT":
                        yield return self.ChangeSpeed(modifySpeed, logged);
                        break;
                    case "TARGETSSPEEDSTAT":
                        yield return target.ChangeSpeed(modifySpeed, logged);
                        break;

                    case "SELFLUCKSTAT":
                        yield return self.ChangeLuck(modifyLuck, logged);
                        break;
                    case "TARGETSLUCKSTAT":
                        yield return target.ChangeLuck(modifyLuck, logged);
                        break;

                    case "SELFACCURACYSTAT":
                        yield return self.ChangeAccuracy(modifyAccuracy, logged);
                        break;
                    case "TARGETSACCURACYSTAT":
                        yield return target.ChangeAccuracy(modifyAccuracy, logged);
                        break;

                    case "LEAVEFIGHT":
                        yield return self.HasDied(-1);
                        break;
                    case "SELFDESTRUCT":
                        yield return self.HasDied(logged);
                        break;

                    case "TARGETSREVIVE":
                        yield return target.Revive(healthRegain, logged);
                        break;

                    case "TARGETSSTUN":
                        yield return target.Stun(miscNumber, logged);
                        break;

                    case "TARGETSINCREASEACTIVECOOLDOWN":
                        foreach (Ability ability in target.listOfAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown++;
                        break;
                    case "TARGETSREDUCEACTIVECOOLDOWN":
                        foreach (Ability ability in target.listOfAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown--;
                        break;

                    default:
                        Debug.LogError($"{self.name}: {methodName} isn't a method");
                        break;
                }
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
                (Emotion.Sad) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Emotion.Angry)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Sad) => 1.25f,
                (Emotion.Happy) => 0.75f,
                _ => 1.0f,
            };
        }
        else if (user.currentEmotion == Emotion.Sad)
        {
            answer = target.currentEmotion switch
            {
                (Emotion.Happy) => 1.25f,
                (Emotion.Angry) => 0.75f,
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
            float defense = target.CalculateDefense();

            int finalDamage = Mathf.Max(0, (int)(damageVariation * critical * effectiveness + (attack * attackPower) - defense));
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