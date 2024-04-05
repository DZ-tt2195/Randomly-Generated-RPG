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
    [ReadOnly] public List<Character> listOfTargets;

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
        mainType = (data.typeOne == AbilityType.Attack || data.typeTwo == AbilityType.Attack) ? AbilityType.Attack :
                   (data.typeOne == AbilityType.Healing || data.typeTwo == AbilityType.Healing) ? AbilityType.Healing :
                   AbilityType.Misc;
    }

#endregion

#region Calculations

    bool RollAccuracy(float value)
    {
        if (FileManager.instance.mode == FileManager.GameMode.Main)
        {
            float roll = Random.Range(0f, 1f);
            return (roll <= value);
        }
        else
        {
            return true;
        }
    }

    int RollCritical(float value, int logged)
    {
        float roll = Random.Range(0f, 1f);
        bool result = roll <= value;
        if (result && FileManager.instance.mode == FileManager.GameMode.Main)
        {
            Log.instance.AddText("Critical hit! (+2 Damage)", logged);
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public int Effectiveness(Character user, Character target, int logged)
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

        if (answer > 0)
            Log.instance.AddText("It's super effective! (+1 Damage)", logged);
        else if (answer < 0)
            Log.instance.AddText("It wasn't very effective... (-1 Damage)", logged);

        return answer;
    }

    int CalculateDamage(Character user, Character target, int logged)
    {
        if (RollAccuracy(user.CalculateAccuracy()))
        {
            int effectiveness = Effectiveness(user, target, logged);
            int critical = RollCritical(user.CalculateLuck(), logged);
            int attack = user.CalculatePower();
            int defense = target.CalculateDefense();

            int finalDamage = Mathf.Max(0, critical + effectiveness + attack + data.attackDamage - defense);
            return finalDamage;
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

    public bool CanPlay(Character user)
    {
        if (data.myName.Equals("Skip Turn"))
            return true;
        if (currentCooldown > 0)
            return false;

        string divide = data.playCondition.Replace(" ", "");
        divide = divide.ToUpper();
        string[] methodsInStrings = divide.Split('/');

        if (methodsInStrings[0].Equals("TARGETSDEAD"))
        {
            listOfTargets = TurnManager.instance.listOfDead;
        }
        else
        {
            listOfTargets = GetTargets();
            for (int i = listOfTargets.Count - 1; i >= 0; i--)
            {
                if (listOfTargets[i].CalculateHealth() <= 0)
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
                case "TARGETSDEAD":
                    break;
                case "SELFDEAD":
                    if (user.CalculateHealth() > 0)
                        return false; break;

                case "SELFMAXHEALTH":
                    if (user.CalculateHealthPercent() < 1f)
                        return false; break;

                case "SELFINJURED":
                    if (user.CalculateHealthPercent() > 0.5f)
                        return false; break;
                case "TARGETSINJURED":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CalculateHealthPercent() > 0.5f) listOfTargets.RemoveAt(i);
                    break;

                case "TARGETSNEUTRAL":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion != Emotion.Neutral) listOfTargets.RemoveAt(i);
                    break;
                case "SELFNEUTRAL":
                    if (user.CurrentEmotion != Emotion.Neutral)
                        return false; break;
                case "TARGETSNOTNEUTRAL":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion == Emotion.Neutral) listOfTargets.RemoveAt(i);
                    break;
                case "SELFNOTNEUTRAL":
                    if (user.CurrentEmotion == Emotion.Neutral)
                        return false; break;

                case "TARGETSHAPPY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion != Emotion.Happy) listOfTargets.RemoveAt(i);
                    break;
                case "SELFHAPPY":
                    if (user.CurrentEmotion != Emotion.Happy)
                        return false; break;
                case "TARGETSNOTHAPPY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion == Emotion.Happy) listOfTargets.RemoveAt(i);
                    break;
                case "SELFNOTHAPPY":
                    if (user.CurrentEmotion == Emotion.Happy)
                        return false; break;

                case "TARGETSANGRY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion != Emotion.Angry) listOfTargets.RemoveAt(i);
                    break;
                case "SELFANGRY":
                    if (user.CurrentEmotion != Emotion.Angry)
                        return false; break;
                case "TARGETSNOTANGRY":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion == Emotion.Angry) listOfTargets.RemoveAt(i);
                    break;
                case "SELFNOTANGRY":
                    if (user.CurrentEmotion == Emotion.Angry)
                        return false; break;

                case "TARGETSSAD":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion != Emotion.Sad) listOfTargets.RemoveAt(i);
                    break;
                case "SELFSAD":
                    if (user.CurrentEmotion != Emotion.Sad)
                        return false; break;
                case "TARGETSNOTSAD":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentEmotion == Emotion.Sad) listOfTargets.RemoveAt(i);
                    break;
                case "SELFNOTSAD":
                    if (user.CurrentEmotion == Emotion.Sad)
                        return false; break;

                case "SELFGROUNDED":
                    if (user.CurrentPosition != Position.Grounded)
                        return false; break;
                case "TARGETSGROUNDED":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentPosition != Position.Grounded) listOfTargets.RemoveAt(i);
                    break;

                case "SELFAIRBORNE":
                    if (user.CurrentPosition != Position.Airborne)
                        return false; break;
                case "TARGETSAIRBORNE":
                    for (int i = listOfTargets.Count - 1; i >= 0; i--)
                        if (listOfTargets[i].CurrentPosition != Position.Airborne) listOfTargets.RemoveAt(i);
                    break;

                default:
                    Debug.LogError($"{self.name}: {methodName} isn't a method");
                    break;
            }
        }

        if (data.teamTarget == TeamTarget.None)
            return true;
        else
            return listOfTargets.Count > 0;
    }

    List<Character> GetTargets()
    {
        List<Character> listOfTargets = new List<Character>();

        switch (data.teamTarget)
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

    public IEnumerator ResolveInstructions(string[] listOfMethods, int logged)
    {
        Log.instance.AddText(Log.Substitute(this, self), logged-1);
        killed = false;
        TurnManager.instance.instructions.transform.parent.gameObject.SetActive(false);

        foreach (Character target in listOfTargets)
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
                        yield return target.TakeDamage(damageDealt, logged);
                        if (target == null || target.CalculateHealth() <= 0) killed = true;
                        break;

                    case "PASSTURN":
                        yield return target.MyTurn(logged, true);
                        break;

                    case "SELFCOPY":
                        TurnManager.instance.CreateEnemy(self.data, (Emotion)Random.Range(1, 5), logged);
                        break;

                    case "SELFHEAL":
                        yield return self.GainHealth(data.healthRegain + self.CalculatePower(), logged);
                        if (self.CalculateHealthPercent() >= 1f) fullHeal = true;
                        break;
                    case "TARGETSHEAL":
                        yield return target.GainHealth(data.healthRegain + self.CalculatePower(), logged);
                        if (target.CalculateHealthPercent() >= 1f) fullHeal = true;
                        break;
                    case "HEALFROMDAMAGE":
                        yield return self.GainHealth(damageDealt, logged);
                        if (self.CalculateHealthPercent() >= 1f) fullHeal = true;
                        break;

                    case "SELFSWAPPOSITION":
                        if (self.CurrentPosition == Position.Airborne)
                            yield return self.ChangePosition(Position.Grounded, logged);
                        else if (self.CurrentPosition == Position.Grounded)
                            yield return self.ChangePosition(Position.Airborne, logged);
                        break;
                    case "TARGETSSWAPPOSITION":
                        if (target.CurrentPosition == Position.Airborne)
                            yield return target.ChangePosition(Position.Grounded, logged);
                        else if (target.CurrentPosition == Position.Grounded)
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

                    case "SELFPOWERSTAT":
                        yield return self.ChangePower(data.modifyPower, logged);
                        break;
                    case "TARGETSPOWERSTAT":
                        yield return target.ChangePower(data.modifyPower, logged);
                        break;

                    case "SELFDEFENSESTAT":
                        yield return self.ChangeDefense(data.modifyDefense, logged);
                        break;
                    case "TARGETSDEFENSESTAT":
                        yield return target.ChangeDefense(data.modifyDefense, logged);
                        break;

                    case "SELFSPEEDSTAT":
                        yield return self.ChangeSpeed(data.modifySpeed, logged);
                        break;
                    case "TARGETSSPEEDSTAT":
                        yield return target.ChangeSpeed(data.modifySpeed, logged);
                        break;

                    case "SELFLUCKSTAT":
                        yield return self.ChangeLuck(data.modifyLuck, logged);
                        break;
                    case "TARGETSLUCKSTAT":
                        yield return target.ChangeLuck(data.modifyLuck, logged);
                        break;

                    case "SELFACCURACYSTAT":
                        yield return self.ChangeAccuracy(data.modifyAccuracy, logged);
                        break;
                    case "TARGETSACCURACYSTAT":
                        yield return target.ChangeAccuracy(data.modifyAccuracy, logged);
                        break;

                    case "LEAVEFIGHT":
                        yield return self.HasDied(-1);
                        break;
                    case "SELFDESTRUCT":
                        yield return self.HasDied(logged);
                        break;

                    case "SELFREVIVE":
                        yield return self.Revive(data.healthRegain, logged);
                        break;
                    case "TARGETSREVIVE":
                        yield return target.Revive(data.healthRegain, logged);
                        break;

                    case "SELFSTUN":
                        yield return self.Stun(data.miscNumber, logged);
                        break;
                    case "TARGETSSTUN":
                        yield return target.Stun(data.miscNumber, logged);
                        break;

                    case "TARGETSINCREASEACTIVECOOLDOWN":
                        foreach (Ability ability in target.listOfAutoAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown+=data.miscNumber;
                        foreach (Ability ability in target.listOfRandomAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown+=data.miscNumber;
                        break;
                    case "TARGETSREDUCEACTIVECOOLDOWN":
                        foreach (Ability ability in target.listOfAutoAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown-=data.miscNumber;
                        foreach (Ability ability in target.listOfRandomAbilities)
                            if (ability.currentCooldown > 0) ability.currentCooldown-=data.miscNumber;
                        break;

                    default:
                        Debug.LogError($"{self.name}: {methodName} isn't a method");
                        break;
                }

                if (!runNextMethod)
                    break;
            }
        }
    }

#endregion

}