using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public class PlayerCharacter : Character
{
    int choice;

    public override IEnumerator MyTurn()
    {
        TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(true);
        for (int i = 0; i < TurnManager.instance.listOfBoxes.Count; i++)
        {
            AbilityBox box = TurnManager.instance.listOfBoxes[i];
            try
            {
                box.ReceiveAbility(listOfAbilities[i]);
                box.gameObject.SetActive(true);

                box.button.onClick.RemoveAllListeners();
                int buttonNum = i;
                box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));
            }
            catch (ArgumentOutOfRangeException) { box.gameObject.SetActive(false); }
        }

        yield return WaitForChoice();
        Ability chosenAbility = this.listOfAbilities[choice];
        this.border.gameObject.SetActive(false);
        yield return ChooseTarget(chosenAbility);
    }

    protected override IEnumerator ChooseTarget(Ability ability)
    {
        var narrowDown = new HashSet<TeamTarget> { TeamTarget.AnyOne, TeamTarget.OneTeammate, TeamTarget.OtherTeammate, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };

        if (narrowDown.Contains(ability.teamTarget))
        {
            TurnManager.instance.DisableCharacterButtons();
            for (int i = 0; i < ability.listOfTargets.Count; i++)
            {
                Character character = ability.listOfTargets[i];
                character.border.gameObject.SetActive(true);
                character.button.interactable = true;
                character.button.onClick.RemoveAllListeners();
                int buttonNum = i;
                character.button.onClick.AddListener(() => ReceiveChoice(buttonNum));
            }

            yield return WaitForChoice();
            Character chosenCharacter = ability.listOfTargets[choice];
            ability.listOfTargets.Clear();
            ability.listOfTargets.Add(chosenCharacter);
        }
    }

    IEnumerator WaitForChoice()
    {
        choice = -1;
        while (choice == -1)
            yield return null;
    }

    void ReceiveChoice(int n)
    {
        choice = n;
    }
}
