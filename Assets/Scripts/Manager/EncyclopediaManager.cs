using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using System;
using System.Linq;

public enum PlayerSearch { All, Knight, Angel, Wizard };
public class EncyclopediaManager : MonoBehaviour
{

#region Variables

    [Foldout("Ability Search", true)]
    List<AbilityBox> knightAbilities = new();
    List<AbilityBox> angelAbilities = new();
    List<AbilityBox> wizardAbilities = new();
    [SerializeField] AbilityBox abilityBoxPrefab;
    [SerializeField] Transform storeAbilityBoxes;
    [SerializeField] TMP_InputField abilitySearch;
    [SerializeField] TMP_Dropdown characterDropdown;
    [SerializeField] TMP_Dropdown typeDropdown;

    [Foldout("Weapon Search", true)]
    List<HoverImage> weaponBoxes = new();
    [SerializeField] HoverImage weaponBoxPrefab;
    [SerializeField] Transform storeWeaponBoxes;
    [SerializeField] TMP_InputField weaponSearch;

    private void Start()
    {
        abilitySearch.onValueChanged.AddListener(ChangeInput);
        characterDropdown.onValueChanged.AddListener(ChangeDropdown);
        typeDropdown.onValueChanged.AddListener(ChangeDropdown);

        foreach (AbilityData data in FileManager.instance.listOfPlayerAbilities)
        {
            Ability nextAbility = this.gameObject.AddComponent<Ability>();
            nextAbility.SetupAbility(data, false, false);
            AbilityBox nextBox = Instantiate(abilityBoxPrefab, null);
            nextBox.ReceiveAbility(true, nextAbility);

            switch (data.user)
            {
                case "Knight":
                    knightAbilities.Add(nextBox);
                    break;
                case "Angel":
                    angelAbilities.Add(nextBox);
                    break;
                case "Wizard":
                    wizardAbilities.Add(nextBox);
                    break;
            }
        }
        knightAbilities = knightAbilities.OrderBy(box => box.ability.data.myName).ToList();
        angelAbilities = angelAbilities.OrderBy(box => box.ability.data.myName).ToList();
        wizardAbilities = wizardAbilities.OrderBy(box => box.ability.data.myName).ToList();
        SearchAbility();

        FileManager.instance.listOfWeapons = FileManager.instance.listOfWeapons.OrderBy(data => data.myName).ToList();



        FileManager.instance.listOfEnemies = FileManager.instance.listOfEnemies.OrderBy(data => data.myName).ToList();
    }

    #endregion

#region Ability Search

    void ChangeInput(string text)
    {
        SearchAbility();
    }

    void ChangeDropdown(int n)
    {
        SearchAbility();
    }

    bool CompareCharacters(PlayerSearch setting, PlayerSearch current)
    {
        if (setting == PlayerSearch.All)
            return true;
        return setting == current;
    }

    bool CompareStrings(string searchBox, AbilityData data)
    {
        if (searchBox == "")
            return true;
        return data.myName.IndexOf(searchBox, StringComparison.OrdinalIgnoreCase) >= 0
            || data.description.IndexOf(searchBox, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    bool CompareTypes(AbilityType setting, AbilityData data)
    {
        if (setting == AbilityType.None)
            return true;
        return (data.typeOne == setting || data.typeTwo == setting);
    }

    void SearchAbility()
    {
        PlayerSearch player = PlayerSearch.All;
        switch (characterDropdown.options[characterDropdown.value].text)
        {
            case "All":
                player = PlayerSearch.All;
                break;
            case "Knight":
                player = PlayerSearch.Knight;
                break;
            case "Angel":
                player = PlayerSearch.Angel;
                break;
            case "Wizard":
                player = PlayerSearch.Wizard;
                break;
        }

        AbilityType type = AbilityType.None;
        switch (typeDropdown.options[typeDropdown.value].text)
        {
            case "All":
                type = AbilityType.None;
                break;
            case "Attack":
                type = AbilityType.Attack;
                break;
            case "Healing":
                type = AbilityType.Healing;
                break;
            case "Emotion":
                type = AbilityType.Emotion;
                break;
            case "Position":
                type = AbilityType.Position;
                break;
            case "Stats":
                type = AbilityType.Stats;
                break;
        }

        foreach (AbilityBox box in knightAbilities)
        {
            if (CompareCharacters(player, PlayerSearch.Knight) && CompareStrings(abilitySearch.text, box.ability.data) && CompareTypes(type, box.ability.data))
                box.transform.SetParent(storeAbilityBoxes);
            else
                box.transform.SetParent(null);
        }
        foreach (AbilityBox box in angelAbilities)
        {
            if (CompareCharacters(player, PlayerSearch.Angel) && CompareStrings(abilitySearch.text, box.ability.data) && CompareTypes(type, box.ability.data))
                box.transform.SetParent(storeAbilityBoxes);
            else
                box.transform.SetParent(null);
        }
        foreach (AbilityBox box in wizardAbilities)
        {
            if (CompareCharacters(player, PlayerSearch.Wizard) && CompareStrings(abilitySearch.text, box.ability.data) && CompareTypes(type, box.ability.data))
                box.transform.SetParent(storeAbilityBoxes);
            else
                box.transform.SetParent(null);
        }
    }

#endregion

}
