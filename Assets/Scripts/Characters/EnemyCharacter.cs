using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Ability;

public class EnemyCharacter : Character
{
    public override IEnumerator MyTurn()
    {
        List<Ability> availableAbilities = new List<Ability>();
        foreach (Ability ability in this.listOfAbilities)
        {
            if (ability.myName != "Skip Turn" && ability.CanPlay())
                availableAbilities.Add(ability);
        }

        if (availableAbilities.Count == 0)
        {
            yield return ResolveAbility(listOfAbilities[0]);
        }
        else
        {
            Ability chosenAbility = availableAbilities[Random.Range(0, availableAbilities.Count)];
            yield return ChooseTarget(chosenAbility);
            chosenAbility.currentCooldown = chosenAbility.baseCooldown;
            yield return ResolveAbility(chosenAbility);
        }
    }

    protected override IEnumerator ChooseTarget(Ability ability)
    {
        yield return null;
        var narrowDown = new HashSet<TeamTarget> { TeamTarget.AnyOne, TeamTarget.OneTeammate, TeamTarget.OtherTeammate, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };

        if (narrowDown.Contains(ability.teamTarget))
        {
            List<Character> selectedTarget = new List<Character> { ability.listOfTargets[Random.Range(0, ability.listOfTargets.Count)] };
            ability.listOfTargets = selectedTarget;
        }
    }
}
