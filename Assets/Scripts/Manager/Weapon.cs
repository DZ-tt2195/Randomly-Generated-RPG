using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Weapon : MonoBehaviour
{

#region Setup

    [ReadOnly] public Character self;

    [ReadOnly] public string myName;
    [ReadOnly] public string description;

    [ReadOnly] public List<Ability> listOfAbilities = new();
    [ReadOnly] public string statCalculation;
    [ReadOnly] public string startOfTurn;
    [ReadOnly] public string endOfTurn;
    [ReadOnly] public string newWave;
    [ReadOnly] public string onDeath;

    [ReadOnly] public float modifyAttack;
    [ReadOnly] public float modifyDefense;
    [ReadOnly] public float modifySpeed;
    [ReadOnly] public float modifyLuck;
    [ReadOnly] public float modifyAccuracy;

    public void SetupWeapon(WeaponData data)
    {
        self = GetComponent<Character>();
        myName = data.name;
        description = data.description;

        string[] divideSkillsIntoNumbers = data.skillNumbers.Split(',');
        foreach (string skill in divideSkillsIntoNumbers)
        {
            if (skill.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                listOfAbilities.Add(nextAbility);
                nextAbility.SetupAbility(FileManager.instance.listOfOtherAbilities[int.Parse(skill)]);
            }
        }

        statCalculation = data.statCalculation;
        startOfTurn = data.startOfTurn;
        endOfTurn = data.endOfTurn;
        newWave = data.newWave;
        onDeath = data.onDeath;

        modifyAttack = data.modifyAttack;
        modifyDefense = data.modifyDefense;
        modifySpeed = data.modifySpeed;
        modifyLuck = data.modifyLuck;
        modifyAccuracy = data.modifyAccuracy;
    }

    #endregion

}