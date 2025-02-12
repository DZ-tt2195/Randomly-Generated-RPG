using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Reflection;
using System.Linq;

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
    bool runNextMethod;

    public Dictionary<string, MethodInfo> boolDictionary = new();
    public Dictionary<string, MethodInfo> enumeratorDictionary = new();

    public void SetupAbility(AbilityData data, bool startWithCooldown)
    {
        self = GetComponent<Character>();
        this.data = data;
        currentCooldown = (startWithCooldown) ? data.baseCooldown : 0;

        editedDescription = data.description
            .Replace("ATK", Mathf.Abs(data.attackDamage).ToString())
            .Replace("REGAIN", Mathf.Abs(data.healthRegain).ToString())
            .Replace("POWERSTAT", Mathf.Abs(data.modifyPower).ToString())
            .Replace("SPEEDSTAT", Mathf.Abs(data.modifySpeed).ToString())
            .Replace("DEFENSESTAT", Mathf.Abs(data.modifyDefense).ToString())
            .Replace("ACCURACYSTAT", Mathf.Abs(data.modifyAccuracy * 100).ToString())
            .Replace("LUCKSTAT", Mathf.Abs(data.modifyLuck * 100).ToString())
            .Replace("MISC", data.miscNumber.ToString());
        editedDescription = KeywordTooltip.instance.EditText(editedDescription);

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

        foreach (string nextSection in data.playCondition)
        {
            string[] nextSplit = TurnManager.SpliceString(nextSection.Trim(), '/');
            foreach (string small in nextSplit)
            {
                if (small.Equals("None") || small.Equals("") || boolDictionary.ContainsKey(small))
                    continue;

                MethodInfo method = typeof(Ability).GetMethod(small, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null && method.ReturnType == typeof(bool))
                    boolDictionary.Add(small, method);
                else
                    Debug.LogError($"{data.myName}: play condition: {small} doesn't exist");
            }
        }

        foreach (string nextSection in data.instructions)
        {
            string[] nextSplit = TurnManager.SpliceString(nextSection.Trim(), '/');
            foreach (string small in nextSplit)
            {
                if (small.Equals("None") || small.Equals("") || enumeratorDictionary.ContainsKey(small))
                    continue;

                MethodInfo method = typeof(Ability).GetMethod(small, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null && method.ReturnType == typeof(IEnumerator))
                    enumeratorDictionary.Add(small, method);
                else
                    Debug.LogError($"{data.myName}: instructions: {small} doesn't exist");
            }
        }
    }

    #endregion

#region Calculations

    bool RollAccuracy(float value)
    {
        if (CarryVariables.instance.mode == CarryVariables.GameMode.Main)
        {
            return (UnityEngine.Random.Range(0f, 1f) <= value);
        }
        else
        {
            return true;
        }
    }

    int RollCritical(float value)
    {
        float roll = UnityEngine.Random.Range(0f, 1f);
        bool result = roll <= value;
        return (result && CarryVariables.instance.mode == CarryVariables.GameMode.Main) ? 2 : 0;
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

    int CalculateHealing(Character user, int logged)
    {
        int critical = RollCritical(user.CalculateLuck());
        int health = critical + user.CalculatePower() + data.healthRegain;

        if (health > 1 && critical > 0)
            Log.instance.AddText($"{user.data.myName} has good luck! (+{critical} Health)", logged);

        return Mathf.Max(1, health);
    }

    int CalculateDamage(Character user, Character target, int logged)
    {
        int effectiveness = Effectiveness(user, target);

        if (effectiveness < 0 && CarryVariables.instance.ActiveChallenge("Ineffectives Miss") && user is PlayerCharacter)
        {
            Log.instance.AddText($"{user.name}'s misses (Ineffectives Miss).", logged);
            TurnManager.instance.CreateVisual("MISS", target.transform.localPosition);
            return 0;
        }

        if (RollAccuracy(user.CalculateAccuracy()))
        {
            int critical = RollCritical(user.CalculateLuck());
            int damage = (critical + effectiveness + user.CalculatePower() + data.attackDamage) - target.CalculateDefense();

            if (effectiveness > 0)
                Log.instance.AddText($"It's super effective! (+{effectiveness} Damage)", logged);
            else if (effectiveness < 0)
                Log.instance.AddText($"It was ineffective...({effectiveness} Damage)", logged);

            if (damage > 1 && critical > 0)
                Log.instance.AddText($"{user.data.myName} has good luck! (+{critical} Damage)", logged);

            if (self is EnemyCharacter && CarryVariables.instance.ActiveCheat("Damage Cap"))
                damage = Mathf.Min(damage, 4);

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
        if (data.myName.Equals("Revive") && CarryVariables.instance.ActiveChallenge("No Revives"))
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
                listOfTargets.Add(GetTarget(data.defaultTargets[i]));

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

    bool SetGroundedPlayers(int currentIndex)
    {
        return (SetValues(listOfTargets[currentIndex].Count(target => target.CurrentPosition == Position.Grounded)) > 0);
    }

    bool SetAirbornePlayers(int currentIndex)
    {
        return (SetValues(listOfTargets[currentIndex].Count(target => target.CurrentPosition == Position.Airborne)) > 0);
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

    bool TargetIsGrounded(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Grounded);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool AllGrounded(int currentIndex)
    {
        int numberGrounded = (data.defaultTargets[currentIndex] == TeamTarget.AllPlayers) ? TurnManager.instance.listOfPlayers.Count + TurnManager.instance.listOfDead.Count : TurnManager.instance.listOfEnemies.Count;
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Grounded);
        return (listOfTargets[currentIndex].Count == numberGrounded);
    }

    bool TargetIsAirborne(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Airborne);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool AllAirborne(int currentIndex)
    {
        int numberGrounded = (data.defaultTargets[currentIndex] == TeamTarget.AllPlayers) ? TurnManager.instance.listOfPlayers.Count + TurnManager.instance.listOfDead.Count : TurnManager.instance.listOfEnemies.Count;
        listOfTargets[currentIndex].RemoveAll(target => target.CurrentPosition != Position.Airborne);
        return (listOfTargets[currentIndex].Count == numberGrounded);
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

    bool TargetIsStunned(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.TurnsStunned <= 0);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool TargetNotStunned(int currentIndex)
    {
        listOfTargets[currentIndex].RemoveAll(target => target.TurnsStunned >= 1);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool NotTargeted(int currentIndex)
    {
        listOfTargets[currentIndex].Remove(TurnManager.instance.targetedPlayer);
        listOfTargets[currentIndex].Remove(TurnManager.instance.targetedEnemy);
        return listOfTargets[currentIndex].Count > 0;
    }

    bool LastAttackerExists(int currentIndex)
    {
        if (self.lastToAttackThis == null) return false;
        if (self.lastToAttackThis.CalculateHealth() == 0) return false;
        return listOfTargets[currentIndex].Contains(self.lastToAttackThis);
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

    int SetValues(int number)
    {
        data.attackDamage = number;
        data.healthRegain = number;
        return number;
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

    IEnumerator DealtDamage(Character target, int logged)
    {
        if (damageDealt == 0)
            runNextMethod = false;
        yield return null;
    }

    IEnumerator TargetAttack(Character target, int logged)
    {
        damageDealt = CalculateDamage(self, target, logged);
        yield return target.TakeDamage(damageDealt, logged, self);
        if (target == null || target.CalculateHealth() <= 0)
            killed = true;
    }

    IEnumerator TargetHeal(Character target, int logged)
    {
        yield return target.GainHealth(CalculateHealing(self, logged), logged);
        if (target.CalculateHealthPercent() >= 1f) fullHeal = true;
    }

    IEnumerator TargetMaxHealth(Character target, int logged)
    {
        yield return target.ChangeMaxHealth(data.healthRegain, logged);
    }

    IEnumerator TargetExtraTurn(Character target, int logged)
    {
        yield return target.Extra(data.miscNumber, logged);
    }

    IEnumerator TargetCopy(Character target, int logged)
    {
        TurnManager.instance.CreateEnemy(target.data, (Emotion)UnityEngine.Random.Range(1, 5), logged);
        yield return null;
    }

    IEnumerator SummonStar(Character target, int logged)
    {
        TurnManager.instance.CreateEnemy(FileManager.instance.RandomEnemy(data.miscNumber), (Emotion)UnityEngine.Random.Range(1, 5), logged);
        yield return null;
    }

    IEnumerator TargetSwapPosition(Character target, int logged)
    {
        if (target.CurrentPosition == Position.Airborne)
            yield return target.ChangePosition(Position.Grounded, logged);
        else if (target.CurrentPosition == Position.Grounded)
            yield return target.ChangePosition(Position.Airborne, logged);
    }

    IEnumerator TargetBecomeGrounded(Character target, int logged)
    {
        yield return target.ChangePosition(Position.Grounded, logged);
    }

    IEnumerator TargetBecomeAirborne(Character target, int logged)
    {
        yield return target.ChangePosition(Position.Airborne, logged);
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

    IEnumerator TargetPowerStat(Character target, int logged)
    {
        yield return target.ChangePower(data.modifyPower, logged);
    }

    IEnumerator TargetDefenseStat(Character target, int logged)
    {
        yield return target.ChangeDefense(data.modifyDefense, logged);
    }

    IEnumerator TargetSpeedStat(Character target, int logged)
    {
        yield return target.ChangeSpeed(data.modifySpeed, logged);
    }

    IEnumerator TargetLuckStat(Character target, int logged)
    {
        yield return target.ChangeLuck(data.modifyLuck, logged);
    }

    IEnumerator TargetAccuracyStat(Character target, int logged)
    {
        yield return target.ChangeAccuracy(data.modifyAccuracy, logged);
    }

    IEnumerator TargetDeath(Character target, int logged)
    {
        yield return target.HasDied(logged);
    }

    IEnumerator TargetRevive(Character target, int logged)
    {
        yield return target.Revive(data.healthRegain, logged);
    }

    IEnumerator TargetBecomeStunned(Character target, int logged)
    {
        yield return target.Stun(data.miscNumber, logged);
    }

    IEnumerator TargetBecomeProtected(Character target, int logged)
    {
        yield return target.Protected(data.miscNumber, logged);
    }

    IEnumerator TargetBecomeTargeted(Character target, int logged)
    {
        yield return target.Targeted(data.miscNumber, logged);
    }

    IEnumerator TargetBecomeLocked(Character target, int logged)
    {
        yield return target.Locked(data.miscNumber, logged);
    }

    IEnumerator TargetIncreaseCooldown(Character target, int logged)
    {
        foreach (Ability ability in target.listOfAutoAbilities)
            if (ability.currentCooldown > 0) ability.currentCooldown += data.miscNumber;
        foreach (Ability ability in target.listOfRandomAbilities)
            if (ability.currentCooldown > 0) ability.currentCooldown += data.miscNumber;
        yield return null;
    }

    IEnumerator TargetDecreaseCooldown(Character target, int logged)
    {
        foreach (Ability ability in target.listOfAutoAbilities)
            if (ability.currentCooldown > 0) ability.currentCooldown -= data.miscNumber;
        foreach (Ability ability in target.listOfRandomAbilities)
            if (ability.currentCooldown > 0) ability.currentCooldown -= data.miscNumber;
        yield return null;
    }

    IEnumerator TargetIncreaseRandomCooldown(Character target, int logged)
    {
        List<Ability> hasNoCooldown = target.listOfRandomAbilities.Where(ability => ability.currentCooldown == 0).ToList();
        if (hasNoCooldown.Count > 0)
        {
            Ability chosenAbility = hasNoCooldown[UnityEngine.Random.Range(0, hasNoCooldown.Count)];
            chosenAbility.currentCooldown += data.miscNumber;
            Log.instance.AddText($"{chosenAbility.data.myName} is placed on Cooldown.", logged);
        }
        yield return null;
    }

#endregion

}