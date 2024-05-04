using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCharacter : Character
{
    protected override IEnumerator ChooseAbility(int logged, bool extraAbility)
    {
        List<Ability> allAbilities = new();
        List<Ability> attackingAbilities = new();
        List<Ability> miscAbilities = new();

        foreach (Ability ability in this.listOfRandomAbilities)
        {
            if (ability.CanPlay())
            {
                allAbilities.Add(ability);
                if (ability.mainType == AbilityType.Attack)
                    attackingAbilities.Add(ability);
                else
                    miscAbilities.Add(ability);
            }
        }

        allAbilities = allAbilities.Shuffle();
        attackingAbilities = attackingAbilities.Shuffle();
        miscAbilities = miscAbilities.Shuffle();

        Debug.Log($"All: {allAbilities.Count}; Attack: {attackingAbilities.Count}; Misc: {miscAbilities.Count}");
        if (allAbilities.Count == 0)
        {
            chosenAbility = listOfAutoAbilities[0];
            yield break;
        }
        switch (data.aiTargeting)
        {
            case "MOSTTARGETS":
                allAbilities = allAbilities.OrderByDescending(ability => CountTargets(ability)).ToList();
                chosenAbility = allAbilities[0];
                break;
            case "CAMEOFFCOOLDOWN":
                allAbilities = allAbilities.OrderByDescending(ability => ability.data.baseCooldown).ToList();
                chosenAbility = allAbilities[0];
                break;
            default:
                if (attackingAbilities.Count != 0 && (data.aiTargeting.Equals("CHOOSEATTACK") ||
                    this.CurrentEmotion == Emotion.Angry || (this.CurrentEmotion == Emotion.Sad && CalculateHealthPercent() <= 0.6f)))
                    chosenAbility = attackingAbilities[0];
                else if (miscAbilities.Count != 0 && !extraAbility && this.CurrentEmotion == Emotion.Happy)
                    chosenAbility = miscAbilities[0];
                else
                    chosenAbility = allAbilities[0];
                break;
        }
    }

    protected override IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        if (!ability.data.myName.Equals("Skip Turn"))
        {
            ability.listOfTargets[index] = ability.listOfTargets[index].Shuffle();

            if (ability.singleTarget.Contains(target))
            {
                if (TurnManager.instance.CheckForTargeted(ability.listOfTargets[index]))
                {
                    ability.listOfTargets[index] = new List<Character> { TurnManager.instance.targetedPlayer };
                }
                else
                {
                    switch (data.aiTargeting)
                    {
                        case "":
                            ability.listOfTargets[index] = new List<Character> { ability.listOfTargets[index][0] };
                            break;
                        case "NONE":
                            ability.listOfTargets[index] = new List<Character> { ability.listOfTargets[index][0] };
                            break;

                        case "LASTATTACKER":
                            ability.listOfTargets[index].RemoveAll(target => target != lastToAttackThis);
                            break;

                        case "CHOOSEAIRBORNE":
                            int numberAirborne = ability.listOfTargets[index].Count(target => target.CurrentPosition == Position.Airborne);
                            if (numberAirborne > 0) ability.listOfTargets[index].RemoveAll(target => target.CurrentPosition != Position.Airborne);
                            ability.listOfTargets[index] = new() { ability.listOfTargets[index][Random.Range(0, ability.listOfTargets[index].Count)] };
                            break;
                        case "CHOOSEGROUNDED":
                            int numberGrounded = ability.listOfTargets[index].Count(target => target.CurrentPosition == Position.Grounded);
                            if (numberGrounded > 0) ability.listOfTargets[index].RemoveAll(target => target.CurrentPosition != Position.Grounded);
                            ability.listOfTargets[index] = new() { ability.listOfTargets[index][Random.Range(0, ability.listOfTargets[index].Count)] };
                            break;

                        case "LEASTHEALTH":
                            ability.listOfTargets[index].OrderBy(o => o.CalculateHealth()).ToList();
                            ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                            break;
                        case "MOSTHEALTH":
                            ability.listOfTargets[index].OrderByDescending(o => o.CalculateHealth()).ToList();
                            ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                            break;

                        case "EFFECTIVENESS":
                            ability.listOfTargets[index].OrderBy(o => ability.Effectiveness(this, o)).ToList();
                            ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                            break;

                        case "STRONGEST":
                            ability.listOfTargets[index].OrderByDescending(o => o.CalculateStatTotals()).ToList();
                            ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                            break;
                        case "WEAKEST":
                            ability.listOfTargets[index].OrderBy(o => o.CalculateStatTotals()).ToList();
                            ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                            break;
                    }
                }
            }
        }
        else
        {
            yield return null;
        }
    }

    int CountTargets(Ability ability)
    {
        int answer = 0;
        foreach (List<Character> list in ability.listOfTargets)
            answer += list.Count;
        return answer;
    }
}
