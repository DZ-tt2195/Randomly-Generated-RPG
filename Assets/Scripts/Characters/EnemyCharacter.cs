using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ability;

public class EnemyCharacter : Character
{
    protected override IEnumerator ChooseAbility()
    {
        yield return null;
        List<Ability> availableAbilities = new();
        foreach (Ability ability in this.listOfAbilities)
        {
            if (ability.myName != "Skip Turn" && ability.CanPlay())
                availableAbilities.Add(ability);
        }

        thisTurnAbility = (availableAbilities.Count == 0) ? listOfAbilities[0] : availableAbilities[Random.Range(0, availableAbilities.Count)];
    }

    protected override IEnumerator ChooseTarget(Ability ability)
    {
        yield return null;
        HashSet<TeamTarget> narrowDown = new() { TeamTarget.AnyOne, TeamTarget.OneTeammate, TeamTarget.OtherTeammate, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };

        if (narrowDown.Contains(ability.teamTarget))
        {
            this.aiTargeting = this.aiTargeting.ToUpper().Trim();
            List<Character> selectedTarget = ability.listOfTargets;

            switch (this.aiTargeting)
            {
                case "PRIORITIZEAIRBORNE":
                    List<Character> airborneTargets = new();
                    foreach (Character character in ability.listOfTargets)
                    {
                        if (character.currentPosition == Position.Airborne)
                            airborneTargets.Add(character);
                    }
                    Debug.Log($"found {airborneTargets.Count} airborne");
                    if (airborneTargets.Count>0)
                    {
                        selectedTarget = airborneTargets;
                    }
                    break;

                case "PRIORITIZEGROUNDED":
                    List<Character> groundedTargets = new();
                    foreach (Character character in ability.listOfTargets)
                    {
                        if (character.currentPosition == Position.Grounded)
                            groundedTargets.Add(character);
                    }
                    Debug.Log($"found {groundedTargets.Count} grounded");
                    if (groundedTargets.Count > 0)
                    {
                        selectedTarget = groundedTargets;
                    }
                    break;

                case "LEASTHEALTH":
                    List<Character> ascendingHealth = ability.listOfTargets.OrderBy(o => o.CalculateHealth()).ToList();
                    selectedTarget = new() { ascendingHealth[0] };
                    break;

                case "MOSTHEALTH":
                    List<Character> descendingHealth = ability.listOfTargets.OrderByDescending(o => o.CalculateHealth()).ToList();
                    selectedTarget = new() { descendingHealth[0] };
                    break;

                default:
                    Debug.Log($"not programmed: {this.aiTargeting}");
                    break;
            }

            ability.listOfTargets = new() { selectedTarget[Random.Range(0, selectedTarget.Count)] };
        }
    }
}
