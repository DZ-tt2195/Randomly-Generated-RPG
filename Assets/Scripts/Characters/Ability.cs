using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Reflection;
using System.Linq;

public enum TeamTarget { None, Self, All, OnePlayer, OtherPlayer, OneEnemy, OtherEnemy, AllPlayers, AllOtherPlayers, AllEnemies, AllOtherEnemies };
public enum AbilityType { None, Attack, StatPlayer, StatEnemy, EmotionPlayer, EmotionEnemy, PositionPlayer, PositionEnemy, Healing, Misc };

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
                    case "Num":
                        args[i] = Mathf.Abs(data.mainNumber).ToString(); 
                        break;
                    case "Sec":
                        args[i] = Mathf.Abs(data.secondNumber).ToString(); 
                        break;
                    case "PowerStat":
                        args[i] = Mathf.Abs(data.powerStat).ToString(); 
                        break;
                    case "DefenseStat":
                        args[i] = Mathf.Abs(data.defenseStat).ToString(); 
                        break;
                    case "MiscStat":
                        args[i] = Mathf.Abs(data.miscNumber).ToString(); 
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
        int answer = user.CurrentEmotion switch
        {
            Emotion.Happy => target.CurrentEmotion switch
            {
                Emotion.Angry => 2,
                Emotion.Sad => -2,
                _ => 0
            },
            Emotion.Angry => target.CurrentEmotion switch
            {
                Emotion.Sad => 2,
                Emotion.Happy => -2,
                _ => 0
            },
            Emotion.Sad => target.CurrentEmotion switch
            {
                Emotion.Happy => 2,
                Emotion.Angry => -2,
                _ => 0
            },
            _ => 0
        };

        return answer;
    }

    int CalculateHealing(Character user, int number, int logged)
    {
        bool enemyNumberCap = self is EnemyCharacter && ScreenOverlay.instance.ActiveCheat(ToTranslate.Number_Cap);
        int finalCalc = number + user.CalculatePower();

        if (finalCalc > 4 && enemyNumberCap)
        {
            Log.instance.AddText(AutoTranslate.DoEnum(ToTranslate.Apply_Number_Cap), logged);
            return 4;
        }
        else if (finalCalc < 1 && number > 1)
        {
            Log.instance.AddText(AutoTranslate.DoEnum(ToTranslate.Apply_Minimum), logged);
            return 1;
        }
        else
        {
            return finalCalc;
        }
    }

    int CalculateDamage(Character user, Character target, int number, int logged)
    {
        int effectiveness = Effectiveness(user, target);

        if (effectiveness < 0 && ScreenOverlay.instance.ActiveChallenge(ToTranslate.Ineffectives_Fail) && user is PlayerCharacter)
        {
            Log.instance.AddText(AutoTranslate.Apply_Ineffectives_Fail(user.name), logged);
            TurnManager.instance.CreateVisual(AutoTranslate.DoEnum(ToTranslate.Failed), target.transform.localPosition);
            return 0;
        }

        bool enemyNumberCap = self is EnemyCharacter && ScreenOverlay.instance.ActiveCheat(ToTranslate.Number_Cap);
        int finalCalc = number + effectiveness + user.CalculatePower() - target.CalculateDefense();

        if (effectiveness > 0)
            Log.instance.AddText(AutoTranslate.Super_Effective(Mathf.Abs(effectiveness).ToString()), logged);
        else if (effectiveness < 0)
            Log.instance.AddText(AutoTranslate.Not_Effective(Mathf.Abs(effectiveness).ToString()), logged);

        if (finalCalc > 4 && enemyNumberCap)
        {
            Log.instance.AddText(AutoTranslate.DoEnum(ToTranslate.Apply_Number_Cap), logged);
            return 4;
        }
        else if (finalCalc < 1 && number > 1)
        {
            Log.instance.AddText(AutoTranslate.DoEnum(ToTranslate.Apply_Minimum), logged);
            return 1;
        }
        else
        {
            return finalCalc;
        }
    }

    #endregion

#region Play Condition

    public bool CanPlay()
    {
        if (data.abilityName.Equals(ToTranslate.Revive) && ScreenOverlay.instance.ActiveChallenge(ToTranslate.No_Revives))
            return false;
        if (currentCooldown > 0)
            return false;

        listOfTargets.Clear();

        for (int i = 0; i < data.playCondition.Length; i++)
        {
            string[] methodsInStrings = data.playCondition[i].Split('/');
            if (methodsInStrings[0].Equals("TargetIsDead"))
                listOfTargets.Add(TurnManager.instance.listOfDead);
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
        return TurnManager.instance.listOfEnemies.Count < 5;
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

    bool TargetIsNeutral(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion != Emotion.Neutral);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetNotNeutral(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion == Emotion.Neutral);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetIsHappy(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion != Emotion.Happy);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetNotHappy(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion == Emotion.Happy);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetIsAngry(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion != Emotion.Angry);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetNotAngry(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion == Emotion.Angry);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetIsSad(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion != Emotion.Sad);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetNotSad(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion == Emotion.Sad);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetSameEmotion(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion != self.CurrentEmotion);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetDifferentEmotion(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentEmotion == self.CurrentEmotion);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetIsGrounded(int currentIndex)
    {
        if (!(self.name.Equals(ToTranslate.Knight) && ScreenOverlay.instance.ActiveCheat(ToTranslate.Knight_Reach)))
            listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Grounded);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool AllGrounded(int currentIndex)
    {
        int numberGrounded = (data.toTarget[currentIndex] == TeamTarget.AllPlayers) ? TurnManager.instance.listOfPlayers.Count + TurnManager.instance.listOfDead.Count : TurnManager.instance.listOfEnemies.Count;
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
        int numberGrounded = (data.toTarget[currentIndex] == TeamTarget.AllPlayers) ? TurnManager.instance.listOfPlayers.Count + TurnManager.instance.listOfDead.Count : TurnManager.instance.listOfEnemies.Count;
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
        listOfTargets[currentIndex].RemoveAll(target => target.statEffectDict[StatusEffect.Stunned] <= 0);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetNotStunned(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.statEffectDict[StatusEffect.Stunned] >= 1);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool NoTargetedPlayer(int currentIndex)
    {
        return (TurnManager.instance.targetedPlayer == null);
    }

    bool NoTargetedEnemy(int currentIndex)
    {
        return (TurnManager.instance.targetedEnemy == null);
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
            case TeamTarget.AllOtherPlayers:
                foreach (Character friend in TurnManager.instance.listOfPlayers) { if (friend != this.self) listOfTargets.Add(friend); }
                break;
            case TeamTarget.OneEnemy:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { listOfTargets.Add(foe); }
                break;
            case TeamTarget.OtherEnemy:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { if (foe != this.self) listOfTargets.Add(foe); }
                break;
            case TeamTarget.AllEnemies:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { listOfTargets.Add(foe); }
                break;
            case TeamTarget.AllOtherEnemies:
                foreach (Character foe in TurnManager.instance.listOfEnemies) { if (foe != this.self) listOfTargets.Add(foe); }
                break;
        }
        return listOfTargets;
    }

    #endregion

#region Play Instructions

    public IEnumerator ResolveInstructions(string[] listOfMethods, int index, int logged)
    {
        runNextMethod = true;
        TurnManager.instance.instructions.transform.parent.gameObject.SetActive(false);

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
                    yield return ((IEnumerator)enumeratorDictionary[methodName].Invoke(this, new object[2] { target, logged }));
                if (!runNextMethod) break;
                yield return TurnManager.instance.WaitTime();
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
        yield return target.ChangeHealth(CalculateHealing(self, data.mainNumber, logged), logged);
        if (target.CalculateHealthPercent() >= 1f)
            fullHeal = true;
    }

    IEnumerator TargetHealSec(Character target, int logged)
    {
        yield return target.ChangeHealth(CalculateHealing(self, data.secondNumber, logged), logged);
        if (target.CalculateHealthPercent() >= 1f)
            fullHeal = true;
    }

    IEnumerator TargetMaxHealth(Character target, int logged)
    {
        yield return target.ChangeMaxHealth(data.miscNumber, logged);
    }

    IEnumerator TargetSwitchPosition(Character target, int logged)
    {
        yield return target.ChangePosition((target.CurrentPosition == Position.Grounded) ? Position.Elevated : Position.Grounded, logged);
    }

    IEnumerator TargetBecomeGrounded(Character target, int logged)
    {
        yield return target.ChangePosition(Position.Grounded, logged);
    }

    IEnumerator TargetBecomeElevated(Character target, int logged)
    {
        yield return target.ChangePosition(Position.Elevated, logged);
    }

    IEnumerator TargetBecomeHappy(Character target, int logged)
    {
        yield return target.ChangeEmotion(Emotion.Happy, logged);
    }

    IEnumerator TargetBecomeAngry(Character target, int logged)
    {
        yield return target.ChangeEmotion(Emotion.Angry, logged);
    }

    IEnumerator TargetBecomeSad(Character target, int logged)
    {
        yield return target.ChangeEmotion(Emotion.Sad, logged);
    }

    IEnumerator TargetBecomeNeutral(Character target, int logged)
    {
        yield return target.ChangeEmotion(Emotion.Neutral, logged);
    }

    IEnumerator TargetRandomEmotion(Character target, int logged)
    {
        yield return target.ChangeEmotion((Emotion)UnityEngine.Random.Range(1, 5), logged);
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
        yield return target.Revive(data.mainNumber, logged);
    }

    IEnumerator TargetBecomeStunned(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Stunned, data.miscNumber, logged);
    }

    IEnumerator TargetBecomeProtected(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Protected, data.miscNumber, logged);
    }

    IEnumerator TargetBecomeTargeted(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Targeted, data.miscNumber, logged);
    }

    IEnumerator TargetBecomeLocked(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Locked, data.miscNumber, logged);
    }

    IEnumerator TargetGetExtraAbility(Character target, int logged)
    {
        yield return target.ChangeEffect(StatusEffect.Extra, data.miscNumber, logged);
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

        string answer = AutoTranslate.Increase_Cooldown(target.name, total.ToString(), data.miscNumber.ToString());
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

        string answer = AutoTranslate.Decrease_Cooldown(target.name, total.ToString(), data.miscNumber.ToString());
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
            string answer = AutoTranslate.Apply_Cooldown(target.name, AutoTranslate.DoEnum(chosenAbility.data.abilityName), this.data.miscNumber.ToString());
            Log.instance.AddText(answer, logged);
        }
        yield return null;
    }

    IEnumerator TargetCopy(Character target, int logged)
    {
        TurnManager.instance.CreateEnemy(target.data, (Emotion)UnityEngine.Random.Range(1, 5), logged);
        yield return null;
    }

    IEnumerator SummonStar(Character target, int logged)
    {
        List<CharacterData> fromList = GameFiles.inst.listOfEnemies[data.miscNumber];
        CharacterData randomEnemy = fromList[Random.Range(0, fromList.Count)];
        TurnManager.instance.CreateEnemy(randomEnemy, (Emotion)Random.Range(1, 5), logged);
        yield return null;
    }

    #endregion

}