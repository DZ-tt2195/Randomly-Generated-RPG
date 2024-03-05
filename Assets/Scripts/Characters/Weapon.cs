using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Weapon : MonoBehaviour
{

#region Setup

    [ReadOnly] public WeaponData data;
    [ReadOnly] public Character self;

    [ReadOnly] public string myName;
    [ReadOnly] public string description;

    List<Ability> listOfAbilities = new();
    string statCalculation;
    [ReadOnly] public string startOfTurn;
    [ReadOnly] public string endOfTurn;
    [ReadOnly] public string newWave;
    [ReadOnly] public string onDeath;
    [ReadOnly] public string onKill;

    [ReadOnly] public float startingAttack;
    [ReadOnly] public float startingDefense;
    [ReadOnly] public float startingSpeed;
    [ReadOnly] public float startingLuck;
    [ReadOnly] public float startingAccuracy;

    [ReadOnly] public float modifyAttack;
    [ReadOnly] public float modifyDefense;
    [ReadOnly] public float modifySpeed;
    [ReadOnly] public float modifyLuck;
    [ReadOnly] public float modifyAccuracy;

    public void SetupWeapon(WeaponData data)
    {
        self = GetComponent<Character>();
        this.data = data;
        myName = data.myName;
        description = KeywordTooltip.instance.EditText(data.description);

        string[] divideSkillsIntoNumbers = data.skillNumbers.Split(',');
        foreach (string skill in divideSkillsIntoNumbers)
        {
            if (skill.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                listOfAbilities.Add(nextAbility);
                nextAbility.SetupAbility(FileManager.instance.listOfAbilities[int.Parse(skill)], false);
            }
        }

        statCalculation = data.statCalculation;
        startOfTurn = data.startOfTurn;
        endOfTurn = data.endOfTurn;
        newWave = data.newWave;
        onDeath = data.onDeath;
        onKill = data.onKill;

        startingAttack = data.startingAttack;
        startingDefense = data.startingDefense;
        startingSpeed = data.startingSpeed;
        startingLuck = data.startingLuck;
        startingAccuracy = data.startingAccuracy;

        modifyAttack = data.modifyAttack;
        modifyDefense = data.modifyDefense;
        modifySpeed = data.modifySpeed;
        modifyLuck = data.modifyLuck;
        modifyAccuracy = data.modifyAccuracy;
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
                case "USEABILITIES":
                    foreach (Ability ability in listOfAbilities)
                        if (ability.CanPlay(self)) yield return ability.ResolveInstructions(TurnManager.SpliceString(ability.instructions), logged + 1);
                    break;
                default:
                    Debug.LogError($"{this.myName}: {methodName} isn't implemented");
                    break;
            }
        }
    }

    public bool StatCalculation()
    {
        string[] spliced = TurnManager.SpliceString(statCalculation);
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
                    Debug.LogError($"{this.myName}: {methodName} isn't implemented");
                    break;
            }
        }

        return true;
    }

#endregion

}