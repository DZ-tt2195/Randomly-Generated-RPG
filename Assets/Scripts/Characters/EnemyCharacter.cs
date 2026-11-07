using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCharacter : Character
{
    protected override IEnumerator ChooseAbility(int logged)
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

        if (allAbilities.Count == 0)
        {
            chosenAbility = listOfAutoAbilities[0];
            yield break;
        }
        switch (data.aiTargeting)
        {
            case CharacterTargeting.MostTargets:
                allAbilities = allAbilities.OrderByDescending(ability => CountTargets(ability)).ToList();
                chosenAbility = allAbilities[0];
                break;
            case CharacterTargeting.CameOffCooldown:
                allAbilities = allAbilities.OrderByDescending(ability => ability.data.baseCooldown).ToList();
                chosenAbility = allAbilities[0];
                break;
            default:
                if (attackingAbilities.Count == 0)
                    chosenAbility = miscAbilities[0];
                else if (miscAbilities.Count == 0)
                    chosenAbility = attackingAbilities[0];
                else if (data.aiTargeting == CharacterTargeting.ChooseAttack)
                    chosenAbility = attackingAbilities[0];
                else
                    chosenAbility = Random.Range(0, 2) == 1 ? miscAbilities[0] : attackingAbilities[0];
                break;
        }
    }
    protected override IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        yield return null;
        if (ability.singleTarget.Contains(target))
        {
            ability.listOfTargets[index] = ability.listOfTargets[index].Shuffle();
            List<Character> targeted = TurnManager.inst.CheckForTargeted(ability.listOfTargets[index]);

            if (targeted.Count >= 1)
            {
                ability.listOfTargets[index] = new List<Character>(Random.Range(0, targeted.Count));
            }
            else
            {
                switch (data.aiTargeting)
                {
                    case CharacterTargeting.LastAttacker:
                        ability.listOfTargets[index] = new() { lastToAttackThis };
                        break;

                    case CharacterTargeting.ChooseElevated:
                        int numberElevated = ability.listOfTargets[index].Count(target => target.CurrentPosition == Position.Elevated);
                        if (numberElevated > 0) ability.listOfTargets[index].RemoveAll(target => target.CurrentPosition != Position.Elevated);
                        ability.listOfTargets[index] = new() { ability.listOfTargets[index][Random.Range(0, ability.listOfTargets[index].Count)] };
                        break;
                    case CharacterTargeting.ChooseGrounded:
                        int numberGrounded = ability.listOfTargets[index].Count(target => target.CurrentPosition == Position.Grounded);
                        if (numberGrounded > 0) ability.listOfTargets[index].RemoveAll(target => target.CurrentPosition != Position.Grounded);
                        ability.listOfTargets[index] = new() { ability.listOfTargets[index][Random.Range(0, ability.listOfTargets[index].Count)] };
                        break;

                    case CharacterTargeting.LeastHealth:
                        ability.listOfTargets[index].OrderBy(target => target.GetHealth()).ToList();
                        ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                        break;
                    case CharacterTargeting.MostHealth:
                        ability.listOfTargets[index].OrderByDescending(target => target.GetHealth()).ToList();
                        ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                        break;

                    case CharacterTargeting.Effectiveness:
                        ability.listOfTargets[index].OrderByDescending(target => ability.Effectiveness(this, target)).ToList();
                        ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                        break;

                    case CharacterTargeting.Strongest:
                        ability.listOfTargets[index].OrderByDescending(target => target.CalculateStatTotals()).ToList();
                        ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                        break;
                    case CharacterTargeting.Weakest:
                        ability.listOfTargets[index].OrderBy(target => target.CalculateStatTotals()).ToList();
                        ability.listOfTargets[index] = new() { ability.listOfTargets[index][0] };
                        break;

                    default:
                        ability.listOfTargets[index] = new List<Character> { ability.listOfTargets[index][0] };
                        break;
                    }
                }
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