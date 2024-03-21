using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Weapon : MonoBehaviour
{

#region Setup

    [ReadOnly] public WeaponData data;
    [ReadOnly] public Character self;

    List<Ability> listOfAbilities = new();

    public void SetupWeapon(WeaponData data)
    {
        self = GetComponent<Character>();
        this.data = data;
        data.description = KeywordTooltip.instance.EditText(data.description);

        string[] divideSkillsIntoNumbers = data.skillNumbers.Split(',');
        foreach (string skill in divideSkillsIntoNumbers)
        {
            if (skill.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                listOfAbilities.Add(nextAbility);
                nextAbility.SetupAbility(FileManager.instance.listOfOtherAbilities[int.Parse(skill)], false, true);
            }
        }
    }

    #endregion

#region Effects

    public IEnumerator WeaponEffect(string[] listOfMethods, int logged)
    {
        foreach (string methodName in listOfMethods)
        {
            switch (methodName)
            {
                case "":
                    break;
                case "NONE":
                    break;

                case "ABILITYONE":
                    Ability abilityOne = listOfAbilities[0];
                    abilityOne.currentCooldown = 0;
                        if (abilityOne.CanPlay(self)) yield return abilityOne.ResolveInstructions(TurnManager.SpliceString(abilityOne.data.instructions), logged + 1);
                    break;
                case "ABILITYTWO":
                    Ability abilityTwo = listOfAbilities[1];
                    abilityTwo.currentCooldown = 0;
                        if (abilityTwo.CanPlay(self)) yield return abilityTwo.ResolveInstructions(TurnManager.SpliceString(abilityTwo.data.instructions), logged + 1);
                    break;

                default:
                    Debug.LogError($"{data.myName}: {methodName} isn't implemented");
                    break;
            }
        }
    }

    public bool StatCalculation()
    {
        string[] spliced = TurnManager.SpliceString(data.statCalculation);
        foreach (string methodName in spliced)
        {
            switch (methodName)
            {
                case "":
                    break;
                case "NONE":
                    break;
                case "IFAIRBORNE":
                    return self.currentPosition == Position.Airborne;
                default:
                    Debug.LogError($"{data.myName}: {methodName} isn't implemented");
                    break;
            }
        }

        return true;
    }

#endregion

}