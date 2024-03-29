using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    int choice;

    protected override IEnumerator ChooseAbility(int logged)
    {
        List<Ability> allAbilities = new();
        allAbilities.AddRange(listOfAutoAbilities);
        allAbilities.AddRange(listOfRandomAbilities);

        EnableAbilityBoxes();
        TurnManager.instance.instructions.text = $"{this.name}'s Turn: Choose an ability.";
        yield return WaitForChoice();
        chosenAbility = allAbilities[choice];
        this.border.gameObject.SetActive(false);
    }

    void EnableAbilityBoxes()
    {
        TurnManager.instance.listOfBoxes[0].transform.parent.gameObject.SetActive(true);
        int boxesFilled = 0;

        for (int i = 0; i<listOfAutoAbilities.Count; i++)
        {
            AbilityBox box = TurnManager.instance.listOfBoxes[boxesFilled];
            box.ReceiveAbility(listOfAutoAbilities[i].CanPlay(this), listOfAutoAbilities[i]);

            box.button.onClick.RemoveAllListeners();
            int buttonNum = i;
            box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));

            boxesFilled++;
        }

        for (int i = 0; i<listOfRandomAbilities.Count; i++)
        {
            AbilityBox box = TurnManager.instance.listOfBoxes[boxesFilled];
            box.ReceiveAbility(listOfRandomAbilities[i].CanPlay(this), listOfRandomAbilities[i]);

            box.button.onClick.RemoveAllListeners();
            int buttonNum = i;
            box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));

            boxesFilled++;
        }

        for (int i = boxesFilled; i < TurnManager.instance.listOfBoxes.Count; i++)
        {
            TurnManager.instance.listOfBoxes[i].ReceiveAbility(true, null);
        }
    }

    protected override IEnumerator ChooseTarget(Ability ability)
    {
        foreach (AbilityBox box in TurnManager.instance.listOfBoxes)
            box.gameObject.SetActive(false);

        if (ability.singleTarget.Contains(ability.data.teamTarget))
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
