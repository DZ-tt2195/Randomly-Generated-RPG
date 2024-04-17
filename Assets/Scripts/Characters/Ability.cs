using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum TeamTarget { None, Self, AnyOne, All, OnePlayer, OtherPlayer, OneEnemy, OtherEnemy, AllPlayers, AllEnemies };
public enum AbilityType { None, Attack, StatPlayer, StatEnemy, EmotionPlayer, EmotionEnemy, PositionPlayer, PositionEnemy, Healing, Misc };

public class Ability : MonoBehaviour
{

#region Setup

    [ReadOnly] public AbilityData data;
    [ReadOnly] public HashSet<TeamTarget> singleTarget = new() { TeamTarget.AnyOne, TeamTarget.OnePlayer, TeamTarget.OtherPlayer, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };
    [ReadOnly] public Character self;
    [ReadOnly] public string editedDescription;
    [ReadOnly] public AbilityType mainType;

    [ReadOnly] public int currentCooldown;
    [ReadOnly] public List<List<Character>> listOfTargets = new();

    [ReadOnly] public int damageDealt;
    [ReadOnly] public bool killed;
    [ReadOnly] public bool fullHeal;

    public void SetupAbility(AbilityData data, bool startWithCooldown)
    {
        self = GetComponent<Character>();
        this.data = data;

        editedDescription = data.description
            .Replace("ATK", data.attackDamage.ToString())
            .Replace("REGAIN", data.healthRegain.ToString())
            .Replace("POWERSTAT", Mathf.Abs(data.modifyPower).ToString())
            .Replace("SPEEDSTAT", Mathf.Abs(data.modifySpeed).ToString())
            .Replace("DEFENSESTAT", Mathf.Abs(data.modifyDefense).ToString())
            .Replace("ACCURACYSTAT", Mathf.Abs(data.modifyAccuracy*100).ToString())
            .Replace("LUCKSTAT", Mathf.Abs(data.modifyLuck*100).ToString())
            .Replace("MISC", data.miscNumber.ToString())
        ;
        editedDescription = KeywordTooltip.instance.EditText(editedDescription);

        currentCooldown = (startWithCooldown) ? data.baseCooldown : 0;

        mainType = AbilityType.Misc;
        foreach (AbilityType type in data.myTypes)
        {
            if (type == AbilityType.Attack)
            {
                mainType = AbilityType.Attack;
                break;
            }
            else if (type == AbilityType.Healing)
            {
                mainType = AbilityType.Healing;
                break;
            }
        }
    }

#endregion

#region Calculations

    bool RollAccuracy(float value)
    {
        if (CarryVariables.instance.mode == CarryVariables.GameMode.Main)
        {
            float roll = Random.Range(0f, 1f);
            return (roll <= value);
        }
        else
        {
            return true;
        }
    }

    int RollCritical(float value)
    {
        float roll = Random.Range(0f, 1f);
        bool result = roll <= value;
        if (result && CarryVariables.instance.mode == CarryVariables.GameMode.Main)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public int Effectiveness(Character user, Character target)
    {
        int answer = user.CurrentEmotion switch
        {
            Emotion.Happy => target.CurrentEmotion switch
            {
                Emotion.Angry => 1,
                Emotion.Sad => -1,
                _ => 0
            },
            Emotion.Angry => target.CurrentEmotion switch
            {
                Emotion.Sad => 1,
                Emotion.Happy => -1,
                _ => 0
            },
            Emotion.Sad => target.CurrentEmotion switch
            {
                Emotion.Happy => 1,
                Emotion.Angry => -1,
                _ => 0
            },
            _ => 0
        };

        return answer;
    }

    int CalculateDamage(Character user, Character target, int logged)
    {
        int effectiveness = Effectiveness(user, target);

        if (effectiveness < 0 && CarryVariables.instance.ActiveChallenge("Ineffectives Miss") && self.myType == CharacterType.Player)
        {
            Log.instance.AddText($"{user.name}'s attack misses {target.name} (Ineffectives Miss).", logged);
            TurnManager.instance.CreateVisual("MISS", target.transform.localPosition);
            return 0;
        }

        if (RollAccuracy(user.CalculateAccuracy()))
        {
            int critical = RollCritical(user.CalculateLuck());
            int damage = critical + effectiveness + user.CalculatePower() + data.attackDamage - target.CalculateDefense();

            if (effectiveness > 0)
                Log.instance.AddText($"It's super effective! (+{effectiveness} Damage)", logged);
            else if (effectiveness < 0)
                Log.instance.AddText($"It was ineffective...({effectiveness} Damage)", logged);

            if (damage <= 1 && critical > 0)
                Log.instance.AddText($"Critical hit! (+{critical} Damage)", logged);

            return Mathf.Max(1, damage);
        }
        else
        {
            Log.instance.AddText($"{user.name}'s attack misses {target.name}.", logged);
            TurnManager.instance.CreateVisual("MISS", target.transform.localPosition);
            return 0;
        }
    }

    #endregion

#region Play Condition

    public bool CanPlay()
    {
        if (data.myName.Equals("Skip Turn"))
            return true;
        if (data.myName.Equals("Revive") && CarryVariables.instance.ActiveChallenge("No Revives"))
            return false;
        if (currentCooldown > 0)
            return false;

        listOfTargets.Clear();

        for (int i = 0; i < data.playCondition.Length; i++)
        {
            string[] methodsInStrings = data.playCondition[i].Split('/');
            if (methodsInStrings[0].Equals("TARGETDEAD"))
                listOfTargets.Add(TurnManager.instance.listOfDead);
            else
                listOfTargets.Add(GetTarget(data.defaultTargets[i]));

            if (!CheckMethod(methodsInStrings, i))
                return false;
        }

        return true;
    }

    bool CheckMethod(string[] listOfMethods, int currentIndex)
    { 
        foreach (string methodName in listOfMethods)
        {
            switch (methodName)
            {
                case "":
                    break;
                case "NONE":
                    break;

                case "CANSUMMON":
                    if (TurnManager.instance.listOfEnemies.Count >= 5)
                        return false; break;

                case "SELFMAXHEALTH":
                    if (self.CalculateHealthPercent() < 1f)
                        return false; break;
                case "SELFNOTMAXHEALTH":
                    if (self.CalculateHealthPercent() >= 1f)
                        return false; break;

                case "SELFINJURED":
                    if (self.CalculateHealthPercent() > 0.5f)
                        return false; break;
                case "TARGETINJURED":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CalculateHealthPercent() > 0.5f) listOfTargets[currentIndex].RemoveAt(i);
                    break;

                case "TARGETNEUTRAL":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion != Emotion.Neutral) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFNEUTRAL":
                    if (self.CurrentEmotion != Emotion.Neutral)
                        return false; break;
                case "TARGETNOTNEUTRAL":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion == Emotion.Neutral) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFNOTNEUTRAL":
                    if (self.CurrentEmotion == Emotion.Neutral)
                        return false; break;

                case "TARGETHAPPY":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion != Emotion.Happy) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFHAPPY":
                    if (self.CurrentEmotion != Emotion.Happy)
                        return false; break;
                case "TARGETNOTHAPPY":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion == Emotion.Happy) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFNOTHAPPY":
                    if (self.CurrentEmotion == Emotion.Happy)
                        return false; break;

                case "TARGETANGRY":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion != Emotion.Angry) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFANGRY":
                    if (self.CurrentEmotion != Emotion.Angry)
                        return false; break;
                case "TARGETNOTANGRY":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion == Emotion.Angry) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFNOTANGRY":
                    if (self.CurrentEmotion == Emotion.Angry)
                        return false; break;

                case "TARGETSAD":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion != Emotion.Sad) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFSAD":
                    if (self.CurrentEmotion != Emotion.Sad)
                        return false; break;
                case "TARGETNOTSAD":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion == Emotion.Sad) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SELFNOTSAD":
                    if (self.CurrentEmotion == Emotion.Sad)
                        return false; break;

                case "SELFGROUNDED":
                    if (self.CurrentPosition != Position.Grounded)
                        return false; break;
                case "TARGETGROUNDED":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentPosition != Position.Grounded) listOfTargets[currentIndex].RemoveAt(i);
                    break;

                case "SELFAIRBORNE":
                    if (self.CurrentPosition != Position.Airborne)
                        return false; break;
                case "TARGETAIRBORNE":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentPosition != Position.Airborne) listOfTargets[currentIndex].RemoveAt(i);
                    break;

                case "SAMEPOSITION":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentPosition != self.CurrentPosition) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "DIFFERENTPOSITION":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentPosition == self.CurrentPosition) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "SAMEEMOTION":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion != self.CurrentEmotion) listOfTargets[currentIndex].RemoveAt(i);
                    break;
                case "DIFFERENTEMOTION":
                    for (int i = listOfTargets[currentIndex].Count - 1; i >= 0; i--)
                        if (listOfTargets[currentIndex][i].CurrentEmotion == self.CurrentEmotion) listOfTargets[currentIndex].RemoveAt(i);
                    break;

                case "LASTATTACKEREXISTS":
                    if (!listOfTargets[currentIndex].Contains(self.lastToAttackThis))
                        return false;
                    break;

                default:
                    Debug.LogError($"{this.data.myName}: {methodName} isn't a condition");
                    break;
            }
        }

        if (data.defaultTargets[currentIndex] == TeamTarget.None)
            return true;
        else
            return listOfTargets[currentIndex].Count > 0;
    }

    List<Character> GetTarget(TeamTarget target)
    {
        List<Character> listOfTargets = new();

        switch (target)
        {
            case TeamTarget.None:
                listOfTargets.Add(this.self);
                break;
            case TeamTarget.Self:
                listOfTargets.Add(this.self);
                break;
            case TeamTarget.All:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.instance.listOfPlayers) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.AnyOne:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.instance.listOfPlayers) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OnePlayer:
                foreach (Character friend in TurnManager.instance.listOfPlayers) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OtherPlayer:
                foreach (Character friend in TurnManager.instance.listOfPlayers) { if (friend != this.self) listOfTargets.Add(friend); }
                break;
            case TeamTarget.AllPlayers:
                foreach (Character friend in TurnManager.instance.listOfPlayers) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OneEnemy:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { listOfTargets.Add(foe); }
                break;
            case TeamTarget.OtherEnemy:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { if (foe != this.self) listOfTargets.Add(foe); }
                listOfTargets.Remove(this.self);
                break;
            case TeamTarget.AllEnemies:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { listOfTargets.Add(foe); }
                break;
        }
        return listOfTargets;
    }

#endregion

#region Play Instructions

    public IEnumerator ResolveInstructions(string[] listOfMethods, int index, int logged)
    {
        TurnManager.instance.instructions.transform.parent.gameObject.SetActive(false);

        foreach (Character target in listOfTargets[index])
        {
            if (target == null)
                continue;

            foreach (string methodName in listOfMethods)
            {
                yield return TurnManager.instance.WaitTime();
                bool runNextMethod = true;

                switch (methodName)
                {
                    case "":
                        break;
                    case "NONE":
                        break;

                    case "DEALTDAMAGE":
                        if (damageDealt == 0)
                            runNextMethod = false;
                        break;

                    case "ATTACK":
                        damageDealt = CalculateDamage(self, target, logged);
                        yield return target.TakeDamage(damageDealt, logged, self);
                        if (target == null || target.CalculateHealth() <= 0) killed = true;
                        break;

                    case "PASSTURN":
                        yield return target.MyTurn(logged, true);
                        break;

                    case "SELFCOPY":
                        TurnManager.instance.CreateEnemy(self.data, (Emotion)Random.Range(1, 5), logged);
                        break;
                    case "SUMMON1STAR":
                        TurnManager.instance.CreateEnemy(FileManager.instance.listOfEnemies[0][Random.Range(0, FileManager.instance.listOfEnemies[0].Count)], (Emotion)Random.Range(1, 5), logged);
                        break;
                    case "SUMMON2STAR":
                        TurnManager.instance.CreateEnemy(FileManager.instance.listOfEnemies[1][Random.Range(0, FileManager.instance.listOfEnemies[1].Count)], (Emotion)Random.Range(1, 5), logged);
                        break;
                    case "SUMMON3STAR":
                        TurnManager.instance.CreateEnemy(FileManager.instance.listOfEnemies[2][Random.Range(0, FileManager.instance.listOfEnemies[2].Count)], (Emotion)Random.Range(1, 5), logged);
                        break;

                    case "SELFHEAL":
                        yield return self.GainHealth(Mathf.Max(data.healthRegain + self.CalculatePower(), 1), logged);
                        if (self.CalculateHealthPercent() >= 1f) fullHeal = true;
                        break;
                    case "TARGETHEAL":
                        yield return target.GainHealth(Mathf.Max(data.healthRegain + self.CalculatePower(), 1), logged);
                        if (target.CalculateHealthPercent() >= 1f) fullHeal = true;
                        break;

                    case "SELFSWAPPOSITION":
                        if (self.CurrentPosition == Position.Airborne)
                            yield return self.ChangePosition(Position.Grounded, logged);
                        else if (self.CurrentPosition == Position.Grounded)
                            yield return self.ChangePosition(Position.Airborne, logged);
                        break;
                    case "TARGETSWAPPOSITION":
                        if (target.CurrentPosition == Position.Airborne)
                            yield return target.ChangePosition(Position.Grounded, logged);
                        else if (target.CurrentPosition == Position.Grounded)
                            yield return target.ChangePosition(Position.Airborne, logged);
                        break;

                    case "SELFGROUNDED":
                        yield return self.ChangePosition(Position.Grounded, logged);
                        break;
                    case "TARGETGROUNDED":
                        yield return target.ChangePosition(Position.Grounded, logged);
                        break;

                    case "SELFAIRBORNE":
                        yield return self.ChangePosition(Position.Airborne, logged);
                        break;
                    case "TARGETAIRBORNE":
                        yield return target.ChangePosition(Position.Airborne, logged);
                        break;

                    case "SELFHAPPY":
                        yield return self.ChangeEmotion(Emotion.Happy, logged);
                        break;
                    case "TARGETHAPPY":
                        yield return target.ChangeEmotion(Emotion.Happy, logged);
                        break;

                    case "SELFSAD":
                        yield return self.ChangeEmotion(Emotion.Sad, logged);
                        break;
                    case "TARGETSAD":
                        yield return target.ChangeEmotion(Emotion.Sad, logged);
                        break;

                    case "SELFANGRY":
                        yield return self.ChangeEmotion(Emotion.Angry, logged);
                        break;
                    case "TARGETANGRY":
                        yield return target.ChangeEmotion(Emotion.Angry, logged);
                        break;

                    case "SELFNEUTRAL":
                        yield return self.ChangeEmotion(Emotion.Neutral, logged);
                        break;
                    case "TARGETNEUTRAL":
                        yield return target.ChangeEmotion(Emotion.Neutral, logged);
                        break;

                    case "SELFRANDOMEMOTION":
                        yield return self.ChangeEmotion((Emotion)Random.Range(1,5), logged);
                        break;
                    case "TARGETRANDOMEMOTION":
                        yield return target.ChangeEmotion((Emotion)Random.Range(1, 5), logged);
                        break;

                    case "SELFPOWERSTAT":
                        yield return self.ChangePower(data.modifyPower, logged);
                        break;
                    case "TARGETPOWERSTAT":
                        yield return target.ChangePower(data.modifyPower, logged);
                        break;
                    case "TARGETINVERTPOWERSTAT":
                        yield return target.ChangePower(target.modifyPower * -2, logged);
                        break;

                    case "SELFDEFENSESTAT":
                        yield return self.ChangeDefense(data.modifyDefense, logged);
                        break;
                    case "TARGETDEFENSESTAT":
                        yield return target.ChangeDefense(data.modifyDefense, logged);
                        break;
                    case "TARGETINVERTDEFENSESTAT":
                        yield return target.ChangeDefense(target.modifyDefense * -2, logged);
                        break;

                    case "SELFSPEEDSTAT":
                        yield return self.ChangeSpeed(data.modifySpeed, logged);
                        break;
                    case "TARGETSPEEDSTAT":
                        yield return target.ChangeSpeed(data.modifySpeed, logged);
                        break;
                    case "TARGETINVERTSPEEDSTAT":
                        yield return target.ChangeSpeed(target.modifySpeed * -2, logged);
                        break;

                    case "SELFLUCKSTAT":
                        yield return self.ChangeLuck(data.modifyLuck, logged);
                        break;
                    case "TARGETLUCKSTAT":
                        yield return target.ChangeLuck(data.modifyLuck, logged);
                        break;
                    case "TARGETINVERTLUCKSTAT":
                        yield return target.ChangeLuck(target.modifyLuck * -2, logged);
                        break;

                    case "SELFACCURACYSTAT":
                        yield return self.ChangeAccuracy(data.modifyAccuracy, logged);
                        break;
                    case "TARGETACCURACYSTAT":
                        yield return target.ChangeAccuracy(data.modifyAccuracy, logged);
                        break;
                    case "TARGETINVERTACCURACYSTAT":
                        yield return target.ChangeAccuracy(target.modifyAccuracy * -2, logged);
                        break;

                    case "TARGETDEATH":
                        yield return target.HasDied(logged);
                        break;
                    case "SELFDESTRUCT":
                        yield return self.HasDied(logged);
                        break;

                    case "SELFREVIVE":
                        yield return self.Revive(data.healthRegain, logged);
                        break;
                    case "TARGETREVIVE":
                        yield return target.Revive(data.healthRegain, logged);
                        break;

                    case "SELFSTUN":
                        yield return self.Stun(data.miscNumber, logged);
                        break;
                    case "TARGETSTUN":
                        yield return target.Stun(data.miscNumber, logged);
                        break;
                    case "TARGETNOSTUN":
                        yield return target.NoStun(logged);
                        break;

                    case "SELFPROTECTED":
                        yield return self.Protected(data.miscNumber, logged);
                        break;
                    case "TARGETPROTECTED":
                        yield return target.Protected(data.miscNumber, logged);
                        break;

                    case "TARGETINCREASEACTIVECOOLDOWN":
                        foreach (Ability ability in target.listOfAutoAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown+=data.miscNumber;
                        foreach (Ability ability in target.listOfRandomAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown+=data.miscNumber;
                        break;
                    case "TARGETREDUCEACTIVECOOLDOWN":
                        foreach (Ability ability in target.listOfAutoAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown-=data.miscNumber;
                        foreach (Ability ability in target.listOfRandomAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown-=data.miscNumber;
                        break;

                    default:
                        Debug.LogError($"{this.data.myName}: {methodName} isn't a method");
                        break;
                }

                if (!runNextMethod)
                    break;
            }
        }
    }

#endregion

}