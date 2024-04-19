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
                    this.CurrentEmotion == Emotion.Angry || (this.CurrentEmotion == Emotion.Sad && CalculateHealthPercent() < 0.5f)))
                    chosenAbility = attackingAbilities[0];
                else if (miscAbilities.Count != 0 && !extraAbility && this.CurrentEmotion == Emotion.Happy)
                    chosenAbility = miscAbilities[0];
                else
                    chosenAbility = allAbilities[0];
                break;
        }
    }

    public override IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        if (ability.singleTarget.Contains(target))
        {
            List<Character> selectedTarget = ability.listOfTargets[index].Shuffle();

            switch (data.aiTargeting)
            {
                case "":
                    break;
                case "NONE":
                    break;

                case "LASTATTACKER":
                    selectedTarget = new List<Character>() { lastToAttackThis };
                    break;

                case "CHOOSEAIRBORNE":
                    List<Character> airborneTargets = new();
                    foreach (Character character in selectedTarget)
                    {
                        if (character.CurrentPosition == Position.Airborne)
                            airborneTargets.Add(character);
                    }
                    if (airborneTargets.Count>0)
                        selectedTarget = airborneTargets;
                    break;

                case "CHOOSEGROUNDED":
                    List<Character> groundedTargets = new();
                    foreach (Character character in selectedTarget)
                    {
                        if (character.CurrentPosition == Position.Grounded)
                            groundedTargets.Add(character);
                    }
                    if (groundedTargets.Count > 0)
                        selectedTarget = groundedTargets;
                    break;

                case "LEASTHEALTH":
                    List<Character> ascendingHealth = selectedTarget.OrderBy(o => o.CalculateHealth()).ToList();
                    selectedTarget = new() { ascendingHealth[0] };
                    break;

                case "MOSTHEALTH":
                    List<Character> descendingHealth = selectedTarget.OrderByDescending(o => o.CalculateHealth()).ToList();
                    selectedTarget = new() { descendingHealth[0] };
                    break;

                case "EFFECTIVENESS":
                    List<Character> effectiveTargets = selectedTarget.OrderBy(o => ability.Effectiveness(this, o)).ToList();
                    selectedTarget = new() { effectiveTargets[0] };
                    break;

                case "STRONGEST":
                    List<Character> strongestTargets = selectedTarget.OrderByDescending(o => o.CalculateStatTotals()).ToList();
                    selectedTarget = new() { strongestTargets[0] };
                    break;

                case "WEAKEST":
                    List<Character> weakestTargets = selectedTarget.OrderBy(o => o.CalculateStatTotals()).ToList();
                    selectedTarget = new() { weakestTargets[0] };
                    break;

                default:
                    Debug.Log($"not programmed: {data.aiTargeting}");
                    break;
            }

            ability.listOfTargets[index] = new() { selectedTarget[Random.Range(0, selectedTarget.Count)] };
        }
        yield return null;
    }

    int CountTargets(Ability ability)
    {
        int answer = 0;
        foreach (List<Character> list in ability.listOfTargets)
            answer += list.Count;
        return answer;
    }
}
