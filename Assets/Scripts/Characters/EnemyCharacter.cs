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
        var narrowDown = new HashSet<TeamTarget> { TeamTarget.AnyOne, TeamTarget.OneTeammate, TeamTarget.OtherTeammate, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };

        if (narrowDown.Contains(ability.teamTarget))
        {
            List<Character> selectedTarget = new() { ability.listOfTargets[Random.Range(0, ability.listOfTargets.Count)] };
            ability.listOfTargets = selectedTarget;
        }
    }
}
