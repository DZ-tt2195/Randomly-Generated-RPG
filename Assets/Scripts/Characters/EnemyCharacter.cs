using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ability;

public class EnemyCharacter : Character
{
    protected override IEnumerator ChooseAbility(int logged)
    {
        yield return null;
        List<Ability> availableAbilities = new();
        foreach (Ability ability in this.listOfRandomAbilities)
        {
            if (ability.CanPlay(this))
            {
                availableAbilities.Add(ability);
            }
        }

        chosenAbility = (availableAbilities.Count == 0) ? listOfAutoAbilities[0] : availableAbilities[Random.Range(0, availableAbilities.Count)];
    }

    protected override IEnumerator ChooseTarget(Ability ability)
    {
        yield return null;

        if (ability.singleTarget.Contains(ability.data.teamTarget))
        {
            data.aiTargeting = data.aiTargeting.ToUpper().Trim();
            List<Character> selectedTarget = ability.listOfTargets.Shuffle();

            switch (data.aiTargeting)
            {
                case "":
                    break;

                case "PRIORITIZEAIRBORNE":
                    List<Character> airborneTargets = new();
                    foreach (Character character in selectedTarget)
                    {
                        if (character.CurrentPosition == Position.Airborne)
                            airborneTargets.Add(character);
                    }
                    if (airborneTargets.Count>0)
                    {
                        selectedTarget = airborneTargets;
                    }
                    break;

                case "PRIORITIZEGROUNDED":
                    List<Character> groundedTargets = new();
                    foreach (Character character in selectedTarget)
                    {
                        if (character.CurrentPosition == Position.Grounded)
                            groundedTargets.Add(character);
                    }
                    if (groundedTargets.Count > 0)
                    {
                        selectedTarget = groundedTargets;
                    }
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
                    List<Character> effectiveTargets = selectedTarget.OrderBy(o => ability.Effectiveness(this, o, -1)).ToList();
                    selectedTarget = new() { effectiveTargets[0] };
                    break;

                default:
                    Debug.Log($"not programmed: {data.aiTargeting}");
                    break;
            }

            ability.listOfTargets = new() { selectedTarget[Random.Range(0, selectedTarget.Count)] };
        }
    }
}
