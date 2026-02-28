using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;

public class PlayerCharacter : Character
{
    protected override IEnumerator ChooseAbility(int logged, bool extraAbility)
    {
        this.border.gameObject.SetActive(true);
        
        List<Ability> allAbilities = new();
        allAbilities.AddRange(listOfAutoAbilities);
        allAbilities.AddRange(listOfRandomAbilities);

        MakeDecision.inst.SetAbilities(AutoTranslate.Choose_Ability(this.name), allAbilities, Picked);
        void Picked(Ability ability)
        {
            chosenAbility = ability;
        }
        while (chosenAbility == null)
            yield return null;
        this.border.gameObject.SetActive(false);
    }
    protected override IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        if (ability.singleTarget.Contains(target))
        {
            Character targeted = TurnManager.inst.CheckForTargeted(ability.listOfTargets[index]);
            if (targeted != null)
            {
                ability.listOfTargets[index] = new List<Character> { targeted };
            }
            else
            {
                string abilityName = Translator.inst.Translate(ability.data.abilityName);
                string header = "";
                switch (target)
                {
                    case TeamTarget.OnePlayer:
                        header = AutoTranslate.Choose_One_Player(abilityName);
                        break;
                    case TeamTarget.OtherPlayer:
                        header = AutoTranslate.Choose_Another_Player(abilityName); 
                        break;
                    case TeamTarget.OneEnemy:
                        header = AutoTranslate.Choose_An_Enemy(abilityName); 
                        break;
                }

                MakeDecision.inst.SetCharacters(header, ability.listOfTargets[index], Picked);
                bool waiting = true;

                void Picked(Character character)
                {
                    ability.listOfTargets[index] = new() { character };
                    waiting = false;
                }
                while (waiting)
                    yield return null;
            }
        }
    }
    protected override IEnumerator ConfirmChoice()
    {
        string header = "";
        if (chosenTarget.Count == 0)
        {
            header = AutoTranslate.Confirm_No_Target(Translator.inst.Translate(chosenAbility.data.abilityName));
        }
        else
        {
            header = AutoTranslate.Confirm_Target(Translator.inst.Translate(chosenAbility.data.abilityName));
            header += "\n";
            header += string.Join(" + ", chosenTarget.Select(target => target.name));
        }
        if (PlayerPrefs.GetInt("Confirm Choices") == 1)
        {
            List<TextButtonInfo> textInfo = new() { new(AutoTranslate.Confirm(), Done), new(AutoTranslate.Rechoose(), Repick) };
            bool waiting = true;
            void Done()
            {
                waiting = false;
            }
            void Repick()
            {
                ClearAbility();
                waiting = false;
            }
            MakeDecision.inst.SetTextButtons(header, textInfo);
            while (waiting)
                yield return null;
        }
    }
}
