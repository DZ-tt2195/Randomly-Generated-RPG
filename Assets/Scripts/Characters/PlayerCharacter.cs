using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class PlayerCharacter : Character
{
    int choice;

    protected override IEnumerator ChooseAbility(int logged, bool extraAbility)
    {
        List<Ability> allAbilities = new();
        allAbilities.AddRange(listOfAutoAbilities);
        allAbilities.AddRange(listOfRandomAbilities);

        EnableAbilityBoxes();
        TurnManager.instance.instructions.text = $"{this.name}: Choose an Ability. {(extraAbility ? "(Extra Turn)" : "")}";

        yield return WaitForChoice();
        if (choice != -1)
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
            box.gameObject.SetActive(true);
            box.ReceiveAbility(listOfAutoAbilities[i].CanPlay(), listOfAutoAbilities[i]);

            box.button.onClick.RemoveAllListeners();
            int buttonNum = boxesFilled;
            box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));

            boxesFilled++;
        }

        for (int i = 0; i<listOfRandomAbilities.Count; i++)
        {
            AbilityBox box = TurnManager.instance.listOfBoxes[boxesFilled];
            box.gameObject.SetActive(true);
            box.ReceiveAbility(listOfRandomAbilities[i].CanPlay(), listOfRandomAbilities[i]);

            box.button.onClick.RemoveAllListeners();
            int buttonNum = boxesFilled;
            box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));

            boxesFilled++;
        }

        for (int i = boxesFilled; i < TurnManager.instance.listOfBoxes.Count; i++)
        {
            AbilityBox box = TurnManager.instance.listOfBoxes[i];
            box.gameObject.SetActive(true);
            box.button.onClick.RemoveAllListeners();
            box.ReceiveAbility(true, null);
        }
    }

    protected override IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        foreach (AbilityBox box in TurnManager.instance.listOfBoxes)
            box.gameObject.SetActive(false);

        if (ability.singleTarget.Contains(target))
        {
            if (TurnManager.instance.CheckForTargeted(ability.listOfTargets[index]))
            {
                TurnManager.instance.instructions.text = $"{TurnManager.instance.targetedEnemy.data.myName} is Targeted.";
                ability.listOfTargets[index] = new() { TurnManager.instance.targetedEnemy };
            }
            else
            {
                switch (target)
                {
                    case TeamTarget.AnyOne:
                        TurnManager.instance.instructions.text = $"Choose someone to target. ({ability.data.myName})";
                        break;
                    case TeamTarget.OnePlayer:
                        TurnManager.instance.instructions.text = $"Choose a Player to target. ({ability.data.myName})";
                        break;
                    case TeamTarget.OtherPlayer:
                        TurnManager.instance.instructions.text = $"Choose another Player to target. ({ability.data.myName})";
                        break;
                    case TeamTarget.OneEnemy:
                        TurnManager.instance.instructions.text = $"Choose an Enemy to target. ({ability.data.myName})";
                        break;
                }

                TurnManager.instance.DisableCharacterButtons();

                for (int i = 0; i < ability.listOfTargets[index].Count; i++)
                {
                    Character character = ability.listOfTargets[index][i];
                    character.border.gameObject.SetActive(true);
                    character.myButton.interactable = true;

                    character.myButton.onClick.RemoveAllListeners();
                    int buttonNum = i;
                    character.myButton.onClick.AddListener(() => ReceiveChoice(buttonNum));
                }

                yield return WaitForChoice();
                if (choice != -1)
                    ability.listOfTargets[index] = new() { ability.listOfTargets[index][choice] };
            }
        }
    }

    IEnumerator WaitForChoice()
    {
        choice = -1;
        while (choice == -1 && timer > 0f)
            yield return null;
    }

    void ReceiveChoice(int n)
    {
        choice = n;
    }
}
