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
        TurnManager.inst.instructions.text = AutoTranslate.Choose_Ability(this.name);

        yield return WaitForChoice();
        if (choice != -1)
            chosenAbility = allAbilities[choice];

        this.border.gameObject.SetActive(false);
    }

    void EnableAbilityBoxes()
    {
        TurnManager.inst.listOfBoxes[0].transform.parent.gameObject.SetActive(true);
        int boxesFilled = 0;

        for (int i = 0; i<listOfAutoAbilities.Count; i++)
        {
            AbilityBox box = TurnManager.inst.listOfBoxes[boxesFilled];
            box.gameObject.SetActive(true);
            box.ReceiveAbility(listOfAutoAbilities[i].CanPlay(), listOfAutoAbilities[i]);

            box.button.onClick.RemoveAllListeners();
            int buttonNum = boxesFilled;
            box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));

            boxesFilled++;
        }

        for (int i = 0; i<listOfRandomAbilities.Count; i++)
        {
            AbilityBox box = TurnManager.inst.listOfBoxes[boxesFilled];
            box.gameObject.SetActive(true);
            box.ReceiveAbility(listOfRandomAbilities[i].CanPlay(), listOfRandomAbilities[i]);

            box.button.onClick.RemoveAllListeners();
            int buttonNum = boxesFilled;
            box.button.onClick.AddListener(() => ReceiveChoice(buttonNum));

            boxesFilled++;
        }

        for (int i = boxesFilled; i < TurnManager.inst.listOfBoxes.Count; i++)
        {
            AbilityBox box = TurnManager.inst.listOfBoxes[i];
            box.gameObject.SetActive(true);
            box.button.onClick.RemoveAllListeners();
            box.ReceiveAbility(true, null);
        }
    }

    protected override IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        foreach (AbilityBox box in TurnManager.inst.listOfBoxes)
            box.gameObject.SetActive(false);

        if (ability.singleTarget.Contains(target))
        {
            Character targeted = TurnManager.inst.CheckForTargeted(ability.listOfTargets[index]);
            if (targeted != null)
            {
                TurnManager.inst.instructions.text = AutoTranslate.Must_Choose_Targeted(targeted.name);
                ability.listOfTargets[index] = new List<Character> { targeted };
            }
            else
            {
                string abilityName = Translator.inst.Translate(ability.data.abilityName);
                switch (target)
                {
                    case TeamTarget.OnePlayer:
                        TurnManager.inst.instructions.text = AutoTranslate.Choose_One_Player(abilityName);
                        break;
                    case TeamTarget.OtherPlayer:
                        TurnManager.inst.instructions.text = AutoTranslate.Choose_Another_Player(abilityName); 
                        break;
                    case TeamTarget.OneEnemy:
                        TurnManager.inst.instructions.text = AutoTranslate.Choose_An_Enemy(abilityName); 
                        break;
                }

                TurnManager.inst.DisableCharacterButtons();

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
        while (choice == -1)
            yield return null;
    }

    void ReceiveChoice(int n)
    {
        choice = n;
    }
}
