using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCharacter : Character
{
    protected override IEnumerator ChooseAbility(int logged)
    {
        yield return null;
        List<Ability> allAbilities = new();
        List<Ability> attackingAbilities = new();
        List<Ability> miscAbilities = new();

        foreach (Ability ability in this.listOfRandomAbilities)
        {
            if (ability.CanPlay(this))
            {
                allAbilities.Add(ability);
                if (ability.mainType == AbilityType.Attack)
                    attackingAbilities.Add(ability);
                else
                    miscAbilities.Add(ability);
            }
        }

        if (allAbilities.Count == 0)
        {
            chosenAbility = listOfAutoAbilities[0];
        }
        else if (miscAbilities.Count != 0 && this.CurrentEmotion == Emotion.Happy)
        {
            chosenAbility = miscAbilities[Random.Range(0, miscAbilities.Count)];
        }
        else if (attackingAbilities.Count != 0 && (this.CurrentEmotion == Emotion.Angry ||
            (this.CurrentEmotion == Emotion.Sad && CalculateHealthPercent() < 0.5f)))
        {
            chosenAbility = attackingAbilities[Random.Range(0, attackingAbilities.Count)];
        }
        else
        {
            chosenAbility = allAbilities[Random.Range(0, allAbilities.Count)];
        }
    }

    public override IEnumerator ChooseTarget(Ability ability, TeamTarget target)
    {
        if (ability.singleTarget.Contains(target))
        {
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
        yield return null;
    }
}
