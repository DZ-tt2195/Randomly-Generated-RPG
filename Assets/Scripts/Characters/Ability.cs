using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Reflection;
using System.Linq;

public enum TeamTarget { None, Self, All, OnePlayer, OtherPlayer, OneEnemy, OtherEnemy, AllPlayers, AllOtherPlayers, AllEnemies, AllOtherEnemies };
public enum AbilityType { None, Attack, StatPlayer, StatEnemy, MoodPlayer, MoodEnemy, PositionPlayer, PositionEnemy, Healing, Misc };

public class Ability
{

#region Setup

    [ReadOnly] public AbilityData data;
    [ReadOnly] public HashSet<TeamTarget> singleTarget = new() { TeamTarget.OnePlayer, TeamTarget.OtherPlayer, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };
    [ReadOnly] public Character self;
    [ReadOnly] public string editedDescription;
    [ReadOnly] public AbilityType mainType;
    [ReadOnly] public int currentCooldown;
    [ReadOnly] public List<List<Character>> listOfTargets = new();
    [ReadOnly] public int damageDealt;
    [ReadOnly] public bool killed;
    [ReadOnly] public bool fullHeal;
    bool runNextMethod;
    public Dictionary<string, MethodInfo> boolDictionary = new();
    public Dictionary<string, MethodInfo> enumeratorDictionary = new();
    public Ability(Character self, AbilityData data, bool startWithCooldown)
    {
        this.self = self;
        this.data = data;
        currentCooldown = (startWithCooldown) ? data.baseCooldown : 0;

        try
        {
            MethodInfo method = typeof(AutoTranslate).GetMethod($"{data.abilityName}_Text", BindingFlags.Static | BindingFlags.Public);
            ParameterInfo[] parameters = method.GetParameters();
            object[] args = new object[parameters.Length];

            for (int i = 0; i<parameters.Length; i++)
            {
                switch (parameters[i].Name)
                {
                    case "MainNum":
                        args[i] = Mathf.Abs(data.mainNumber).ToString(); 
                        break;
                    case "SecNum":
                        args[i] = Mathf.Abs(data.secondNumber).ToString(); 
                        break;
                    case "PowNum":
                        args[i] = Mathf.Abs(data.powerStat).ToString(); 
                        break;
                    case "DefNum":
                        args[i] = Mathf.Abs(data.defenseStat).ToString(); 
                        break;
                    case "MiscNum":
                        args[i] = Mathf.Abs(data.miscNumber).ToString(); 
                        break;
                    case "StatusNum":
                        args[i] = Mathf.Abs(data.statusNumber).ToString(); 
                        break;
                }
            }

            object result = method.Invoke(null, args);
            editedDescription = KeywordTooltip.instance.EditText((string)result);
        }
        catch
        {
            editedDescription = KeywordTooltip.instance.EditText(Translator.inst.Translate($"{data.abilityName}_Text"));
        }

        mainType = AbilityType.Misc;
        foreach (AbilityType type in data.abilityTypes)
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

        foreach (string nextSection in data.playCondition)
        {
            string[] nextSplit = TurnManager.SpliceString(nextSection.Trim(), '/');
            foreach (string small in nextSplit)
            {
                if (small.Equals("None") || small.Equals("") || boolDictionary.ContainsKey(small))
                    continue;

                MethodInfo boolMethod = typeof(Ability).GetMethod(small, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (boolMethod != null && boolMethod.ReturnType == typeof(bool))
                    boolDictionary.Add(small, boolMethod);
                else
                    Debug.LogError($"{data.abilityName}: play condition: {small} doesn't exist");
            }
        }

        foreach (string nextSection in data.instructions)
        {
            string[] nextSplit = TurnManager.SpliceString(nextSection.Trim(), '/');
            foreach (string small in nextSplit)
            {
                if (small.Equals("None") || small.Equals("") || enumeratorDictionary.ContainsKey(small))
                    continue;

                MethodInfo useMethod = typeof(Ability).GetMethod(small, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (useMethod != null && useMethod.ReturnType == typeof(IEnumerator))
                    enumeratorDictionary.Add(small, useMethod);
                else
                    Debug.LogError($"{data.abilityName}: instructions: {small} doesn't exist");
            }
        }
    }

    #endregion

#region Calculations
    public int Effectiveness(Character user, Character target)
    {
        int answer = user.CurrentMood switch
        {
            Mood.Lively => target.CurrentMood switch
            {
                Mood.Focused => 2,
                Mood.Tired => -2,
                _ => 0
            },
            Mood.Focused => target.CurrentMood switch
            {
                Mood.Tired => 2,
                Mood.Lively => -2,
                _ => 0
            },
            Mood.Tired => target.CurrentMood switch
            {
                Mood.Lively => 2,
                Mood.Focused => -2,
                _ => 0
            },
            _ => 0
        };

        return answer;
    }
    int CalculateHealing(Character user, Character target, int number, int logged)
    {
        int finalCalc = number + user.CalculatePower();
        if (FightRules.inst.CheckRule(nameof(AutoTranslate.Aiming), logged))
            finalCalc += (user.CurrentPosition != target.CurrentPosition) ? 1 : -1;
        return Mathf.Max(1, finalCalc);
    }
    int CalculateDamage(Character user, Character target, int number, int logged)
    {
        int effectiveness = Effectiveness(user, target);
        int finalCalc = number + effectiveness + user.CalculatePower() - target.CalculateDefense();

        if (effectiveness > 0)
            Log.instance.AddText(AutoTranslate.Super_Effective(Mathf.Abs(effectiveness).ToString()), logged);
        else if (effectiveness < 0)
            Log.instance.AddText(AutoTranslate.Not_Effective(Mathf.Abs(effectiveness).ToString()), logged);
        if (FightRules.inst.CheckRule(nameof(AutoTranslate.Aiming), logged))
            finalCalc += (user.CurrentPosition != target.CurrentPosition) ? 1 : -1;

        return Mathf.Max(1, finalCalc);
    }

    #endregion

#region Play Condition
    public bool CanPlay()
    {
        if (currentCooldown > 0)
            return false;

        listOfTargets.Clear();

        for (int i = 0; i < data.playCondition.Length; i++)
        {
            string[] methodsInStrings = data.playCondition[i].Split('/');
            if (methodsInStrings[0].Equals("TargetIsDead"))
                listOfTargets.Add(TurnManager.inst.listOfDead);
            else
                listOfTargets.Add(GetTarget(data.toTarget[i]));

            for (int j = 0; j < methodsInStrings.Length; j++)
            {
                if (methodsInStrings[j].Equals("None") || methodsInStrings[j].Equals(""))
                    continue;
                if (!(bool)boolDictionary[methodsInStrings[j]].Invoke(this, new object[1] { i }))
                    return false;
            }
        }

        return true;
    }
    bool TargetIsDead(int currentIndex)
    {
        return listOfTargets[currentIndex].Count > 0;
    }
    bool CanSummon(int currentIndex)
    {
        return TurnManager.inst.listOfEnemies.Count < 5;
    }
    bool TargetAtMaxHealth(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CalculateHealthPercent() < 1f);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetNotMaxHealth(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CalculateHealthPercent() >= 1f);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetHealthOrMore(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.currentHealth <= data.miscNumber);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetHealthOrLess(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.currentHealth >= data.miscNumber);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetInjured(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CalculateHealthPercent() > 0.5f);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetIsLively(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood != Mood.Lively);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetNotLively(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood == Mood.Lively);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetIsFocused(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood != Mood.Focused);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetNotFocused(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood == Mood.Focused);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetIsTired(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood != Mood.Tired);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetNotTired(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood == Mood.Tired);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetSameMood(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood != self.CurrentMood);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetDifferentMood(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentMood == self.CurrentMood);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetIsGrounded(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Grounded);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool AllGrounded(int currentIndex)
    {
        int numberGrounded = (data.toTarget[currentIndex] == TeamTarget.AllPlayers) ? TurnManager.inst.listOfPlayers.Count + TurnManager.inst.listOfDead.Count : TurnManager.inst.listOfEnemies.Count;
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Grounded);
        return (listOfTargets[currentIndex].Count == numberGrounded);
    }
    bool TargetIsElevated(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Elevated);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool AllElevated(int currentIndex)
    {
        int numberGrounded = (data.toTarget[currentIndex] == TeamTarget.AllPlayers) ? TurnManager.inst.listOfPlayers.Count + TurnManager.inst.listOfDead.Count : TurnManager.inst.listOfEnemies.Count;
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Elevated);
        return (listOfTargets[currentIndex].Count == numberGrounded);
    }
    bool TargetSamePosition(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != self.CurrentPosition);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetDifferentPosition(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition == self.CurrentPosition);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetPowerOrMore(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CalculatePower() <= data.miscNumber);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetPowerOrLess(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CalculatePower() >= data.miscNumber);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetDefenseOrMore(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CalculateDefense() <= data.miscNumber);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetDefenseOrLess(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CalculateDefense() >= data.miscNumber);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetIsStunned(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.StatEffectdict[StatusEffect.Stunned] <= 0);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetNotStunned(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.StatEffectdict[StatusEffect.Stunned] >= 1);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool NoTargetedPlayer(int currentIndex)
    {
        return (TurnManager.inst.targetedPlayer == null);
    }
    bool NoTargetedEnemy(int currentIndex)
    {
        return (TurnManager.inst.targetedEnemy == null);
    }
    bool LastAttackerExists(int currentIndex)
    {
        if (self.lastToAttackThis == null) return false;
        if (self.lastToAttackThis.currentHealth == 0) return false;
        return listOfTargets[currentIndex].Contains(self.lastToAttackThis);
    }
    bool TargetZeroCooldown(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => !HasNoCooldown(target));
        return listOfTargets[currentIndex].Count > 0;

        bool HasNoCooldown(Character target)
        {
            foreach (Ability ability in target.listOfRandomAbilities)
                if (ability.currentCooldown == 0) return true;
            return false;
        }
    }
    bool TargetActiveCooldown(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => !HasCooldown(target));
        return listOfTargets[currentIndex].Count > 0;

        bool HasCooldown(Character target)
        {
            foreach (Ability ability in target.listOfAutoAbilities)
                if (ability.currentCooldown > 0) return true;
            foreach (Ability ability in target.listOfRandomAbilities)
                if (ability.currentCooldown > 0) return true;
            return false;
        }
    }
    bool TargetIsStar(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.data.difficulty != data.miscNumber);
        return listOfTargets[currentIndex].Count > 0;
    }
    bool TargetEffective(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => Effectiveness(self, target) < data.miscNumber);
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
                foreach (Character foe in TurnManager.inst.listOfEnemies) { listOfTargets.Add(foe); }
                foreach (Character friend in TurnManager.inst.listOfPlayers) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OnePlayer:
                foreach (Character friend in TurnManager.inst.listOfPlayers) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.OtherPlayer:
                foreach (Character friend in TurnManager.inst.listOfPlayers) { if (friend != this.self) listOfTargets.Add(friend); }
                break;
            case TeamTarget.AllPlayers:
                foreach (Character friend in TurnManager.inst.listOfPlayers) { listOfTargets.Add(friend); }
                break;
            case TeamTarget.AllOtherPlayers:
                foreach (Character friend in TurnManager.inst.listOfPlayers) { if (friend != this.self) listOfTargets.Add(friend); }
                break;
            case TeamTarget.OneEnemy:
                foreach (Character foe in TurnManager.inst.listOfEnemies) { listOfTargets.Add(foe); }
                break;
            case TeamTarget.OtherEnemy:
                foreach (Character foe in TurnManager.inst.listOfEnemies) { if (foe != this.self) listOfTargets.Add(foe); }
                break;
            case TeamTarget.AllEnemies:
                foreach (Character foe in TurnManager.inst.listOfEnemies) { listOfTargets.Add(foe); }
                break;
            case TeamTarget.AllOtherEnemies:
                foreach (Character foe in TurnManager.inst.listOfEnemies) { if (foe != this.self) listOfTargets.Add(foe); }
                break;
        }
        return listOfTargets;
    }

    #endregion

#region Play Instructions
    public IEnumerator ResolveInstructions(string[] listOfMethods, int index, int logged)
    {
        runNextMethod = true;
        MakeDecision.inst.BlankUI();

        foreach (Character target in listOfTargets[index])
        {
            if (target == null)
                continue;

            for (int i = 0; i<listOfMethods.Count(); i++)
            {
                string methodName = listOfMethods[i];

                if (methodName.Equals("None") || methodName.Equals(""))
                    continue;

                if (target != null && target.currentHealth >= 0)
                    yield return (IEnumerator)enumeratorDictionary[methodName].Invoke(this, new object[2] { target, logged });
                if (!runNextMethod) break;
                yield return TurnManager.inst.WaitTime();
            }
        }
    }
    IEnumerator DealtKill(Character target, int logged)
    {
        if (!killed)
            runNextMethod = false;
        yield return null;
    }
    IEnumerator TargetAttack(Character target, int logged)
    {
        damageDealt = CalculateDamage(self, target, data.mainNumber, logged);
        yield return target.ChangeHealth(-1*damageDealt, logged, self);
        if (target == null || target.currentHealth <= 0)
            killed = true;
    }
    IEnumerator TargetAttackSec(Character target, int logged)
    {
        damageDealt = CalculateDamage(self, target, data.secondNumber, logged);
        yield return target.ChangeHealth(-1*damageDealt, logged, self);
        if (target == null || target.currentHealth <= 0)
            killed = true;
    }
    IEnumerator TargetHeal(Character target, int logged)
    {
        yield return target.ChangeHealth(CalculateHealing(self, target, data.mainNumber, logged), logged);
        if (target.CalculateHealthPercent() >= 1f)
            fullHeal = true;
    }
    IEnumerator TargetHealSec(Character target, int logged)
    {
        yield return target.ChangeHealth(CalculateHealing(self, target, data.secondNumber, logged), logged);
        if (target.CalculateHealthPercent() >= 1f)
            fullHeal = true;
    }
    IEnumerator TargetMaxHealth(Character target, int logged)
    {
        yield return target.ChangeMaxHealth(data.miscNumber, logged);
    }
    IEnumerator TargetSwitchPosition(Character target, int logged)
    {
        if (target.CurrentPosition == Position.Grounded)
            yield return target.ChangePosition(Position.Elevated, logged);
        else
            yield return target.ChangePosition(Position.Grounded, logged);
    }
    IEnumerator TargetBecomeGrounded(Character target, int logged)
    {
        yield return target.ChangePosition(Position.Grounded, logged);
    }
    IEnumerator TargetBecomeElevated(Character target, int logged)
    {
        yield return target.ChangePosition(Position.Elevated, logged);
    }
    IEnumerator TargetBecomeLively(Character target, int logged)
    {
        yield return target.ChangeMood(Mood.Lively, logged);
    }
    IEnumerator TargetBecomeFocused(Character target, int logged)
    {
        yield return target.ChangeMood(Mood.Focused, logged);
    }
    IEnumerator TargetBecomeTired(Character target, int logged)
    {
        yield return target.ChangeMood(Mood.Tired, logged);
    }
    IEnumerator TargetRandomMood(Character target, int logged)
    {
        yield return target.ChangeMood(Character.RandomMood(null), logged);
    }
    IEnumerator TargetGainPower(Character target, int logged)
    {
        yield return target.ChangeStat(Stats.Power, data.powerStat, logged);
    }
    IEnumerator TargetLosePower(Character target, int logged)
    {
        yield return target.ChangeStat(Stats.Power, -1*data.powerStat, logged);
    }
    IEnumerator TargetGainDefense(Character target, int logged)
    {
        yield return target.ChangeStat(Stats.Defense, data.defenseStat, logged);
    }
    IEnumerator TargetLoseDefense(Character target, int logged)
    {
        yield return target.ChangeStat(Stats.Defense, -1*data.defenseStat, logged);
    }
    IEnumerator TargetDeath(Character target, int logged)
    {
        yield return target.HasDied(logged);
    }
    IEnumerator TargetRevive(Character target, int logged)
    {
        yield return target.Revive(CalculateHealing(self, target, data.mainNumber, logged), logged);
    }
    IEnumerator TargetBecomeStunned(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Stunned, data.statusNumber, logged);
    }
    IEnumerator TargetBecomeProtected(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Protected, data.statusNumber, logged);
    }
    IEnumerator TargetBecomeTargeted(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Targeted, data.statusNumber, logged);
    }
    IEnumerator TargetBecomeLocked(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Locked, data.statusNumber, logged);
    }
    IEnumerator TargetGetExtraAbility(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Extra, data.statusNumber, logged);
    }
    IEnumerator TargetIncreaseAllCooldown(Character target, int logged)
    {
        int total = 0;
        foreach (Ability ability in target.listOfAutoAbilities)
        {
            if (ability.currentCooldown > 0)
            {
                ability.currentCooldown += data.miscNumber;
                total++;
            }
        }
        foreach (Ability ability in target.listOfRandomAbilities)
        {
            if (ability.currentCooldown > 0)
            {
                ability.currentCooldown += data.miscNumber;
                total++;
            }
        }

        string answer = AutoTranslate.Increase_Cooldown(total.ToString(), target.name, data.miscNumber.ToString());
        Log.instance.AddText(answer, logged);
        yield return null;
    }
    IEnumerator TargetDecreaseAllCooldown(Character target, int logged)
    {
        int total = 0;
        foreach (Ability ability in target.listOfAutoAbilities)
        {
            if (ability.currentCooldown > 0)
            {
                ability.currentCooldown -= data.miscNumber;
                total++;
            }
        }
        foreach (Ability ability in target.listOfRandomAbilities)
        {
            if (ability.currentCooldown > 0)
            {
                ability.currentCooldown -= data.miscNumber;
                total++;
            }
        }

        string answer = AutoTranslate.Decrease_Cooldown(total.ToString(), target.name, data.miscNumber.ToString());
        Log.instance.AddText(answer, logged);
        yield return null;
    }
    IEnumerator TargetForceOneCooldown(Character target, int logged)
    {
        List<Ability> hasNoCooldown = target.listOfRandomAbilities.Where(ability => ability.currentCooldown == 0).ToList();
        if (hasNoCooldown.Count > 0)
        {
            Ability chosenAbility = hasNoCooldown[Random.Range(0, hasNoCooldown.Count)];
            chosenAbility.currentCooldown += this.data.miscNumber;
            string answer = AutoTranslate.Apply_Cooldown(target.name, Translator.inst.Translate(chosenAbility.data.abilityName), this.data.miscNumber.ToString());
            Log.instance.AddText(answer, logged);
        }
        yield return null;
    }
    IEnumerator TargetCopy(Character target, int logged)
    {
        yield return TurnManager.inst.CreateEnemy(target.data, Character.RandomMood(null), logged);
    }
    IEnumerator SummonStar(Character target, int logged)
    {
        yield return TurnManager.inst.CreateEnemy(GameFiles.inst.RandomEnemy(data.miscNumber, null), Character.RandomMood(null), logged);
    }

    #endregion

}