using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public class PlayerCharacter : Character
{
    int choice;

    protected override IEnumerator ChooseAbility()
    {
        EnableAbilityBoxes();
        TurnManager.instance.instructions.text = $"{this.name}'s Turn: Choose an ability.";
        yield return WaitForChoice();
        chosenAbility = this.listOfAbilities[choice];
        this.border.gameObject.SetActive(false);
    }

    void EnableAbilityBoxes()
    {
        TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(true);
        for (int i = 0; i < TurnManager.instance.listOfBoxes.Count; i++)
        {
            AbilityBox box = TurnManager.instance.listOfBoxes[i];
            try
            {
                box.ReceiveAbility(listOfAbilities[i], this);
                box.gameObject.SetActive(true);

                box.button.onClick.RemoveAllListeners();
                int buttonNum = i;
                box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));
            }
            catch (ArgumentOutOfRangeException) { box.gameObject.SetActive(false); }
        }
    }

    protected override IEnumerator ChooseTarget(Ability ability)
    {
        TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(false);
        HashSet<TeamTarget> narrowDown = new() { TeamTarget.AnyOne, TeamTarget.OneTeammate, TeamTarget.OtherTeammate, TeamTarget.OneEnemy, TeamTarget.OtherEnemy };

        if (narrowDown.Contains(ability.teamTarget))
        {
            TurnManager.instance.instructions.text = $"Choose a character to target.";
            TurnManager.instance.DisableCharacterButtons();
            for (int i = 0; i < ability.listOfTargets.Count; i++)
            {
                Character character = ability.listOfTargets[i];
                character.border.gameObject.SetActive(true);
                character.myButton.interactable = true;
                character.myButton.onClick.RemoveAllListeners();
                int buttonNum = i;
                character.myButton.onClick.AddListener(() => ReceiveChoice(buttonNum));
            }

            yield return WaitForChoice();
            List<Character> selectedTarget = new() { ability.listOfTargets[choice]};
            ability.listOfTargets = selectedTarget;
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
