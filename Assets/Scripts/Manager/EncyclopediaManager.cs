using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public enum PlayerSearch { All, Knight, Angel, Wizard };
public class EncyclopediaManager : MonoBehaviour
{

#region Variables

    [Foldout("Changing searches", true)]
    [SerializeField] List<Button> currentSearch = new();
    [SerializeField] List<GameObject> masterGameObject = new();

    [Foldout("Ability Search", true)]
    List<AbilityBox> knightAbilities = new();
    List<AbilityBox> angelAbilities = new();
    List<AbilityBox> wizardAbilities = new();
    [SerializeField] AbilityBox abilityBoxPrefab;
    [SerializeField] RectTransform storeAbilityBoxes;
    [SerializeField] TMP_InputField abilityInput;
    [SerializeField] TMP_Dropdown characterDropdown;
    [SerializeField] TMP_Dropdown typeDropdown;

    [Foldout("Weapon Search", true)]
    List<WeaponBox> listOfWeaponBoxes = new();
    [SerializeField] WeaponBox weaponBoxPrefab;
    [SerializeField] RectTransform storeWeaponBoxes;
    [SerializeField] TMP_InputField weaponInput;

    [Foldout("Enemy Search", true)]
    List<Character> listOfEnemyBoxes = new();
    [SerializeField] GameObject characterPrefab;
    [SerializeField] RectTransform storeEnemyBoxes;
    [SerializeField] TMP_InputField enemyInput;

    void ChangeMode(int n)
    {
        for (int i = 0; i < currentSearch.Count; i++)
        {
            masterGameObject[i].SetActive(i == n);
        }
    }

    private void Start()
    {
        for (int i = 0; i<currentSearch.Count; i++)
        {
            int k = i;
            currentSearch[i].onClick.AddListener(() => ChangeMode(k));
        }

        abilityInput.onValueChanged.AddListener(ChangeAbilityInput);
        characterDropdown.onValueChanged.AddListener(ChangeAbilityDropdown);
        typeDropdown.onValueChanged.AddListener(ChangeAbilityDropdown);
        foreach (AbilityData data in FileManager.instance.listOfPlayerAbilities)
        {
            Ability nextAbility = this.gameObject.AddComponent<Ability>();
            nextAbility.SetupAbility(data, false);

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

        weaponInput.onValueChanged.AddListener(ChangeWeaponInput);
        FileManager.instance.listOfWeapons = FileManager.instance.listOfWeapons.OrderBy(data => data.myName).ToList();
        foreach (WeaponData data in FileManager.instance.listOfWeapons)
        {
            Weapon nextWeapon = this.gameObject.AddComponent<Weapon>();
            nextWeapon.SetupWeapon(data);

            WeaponBox nextBox = Instantiate(weaponBoxPrefab, null);
            listOfWeaponBoxes.Add(nextBox);
            nextBox.ReceiveWeapon(nextWeapon);
        }
        SearchWeapon();

        FileManager.instance.listOfEnemies = FileManager.instance.listOfEnemies.OrderBy(data => data.myName).ToList();
        enemyInput.onValueChanged.AddListener(ChangeEnemyInput);
        foreach (CharacterData data in FileManager.instance.listOfEnemies)
        {
            Character nextEnemy = Instantiate(characterPrefab).AddComponent<Character>();
            List<AbilityData> characterAbilities = new();
            string[] divideSkillsIntoNumbers = data.skillNumbers.Split(',');
            for (int j = 0; j < divideSkillsIntoNumbers.Length; j++)
            {
                try
                {
                    string skillNumber = divideSkillsIntoNumbers[j];
                    skillNumber.Trim();
                    characterAbilities.Add(FileManager.instance.listOfOtherAbilities[int.Parse(skillNumber)]);
                }
                catch (FormatException) { continue; }
                catch (ArgumentOutOfRangeException) { break; }
            }

            nextEnemy.SetupCharacter(CharacterType.Enemy, data, characterAbilities, Emotion.Neutral);
            listOfEnemyBoxes.Add(nextEnemy);
            foreach (Transform child in nextEnemy.transform)
                Destroy(child.gameObject);
        }
        SearchEnemy();
    }

#endregion

#region Helper Methods

    bool CompareCharacters(PlayerSearch setting, PlayerSearch current)
    {
        if (setting == PlayerSearch.All)
            return true;
        return setting == current;
    }

    bool CompareStrings(string searchBox, string comparison)
    {
        if (searchBox.IsNullOrEmpty())
            return true;
        return (comparison.IndexOf(searchBox, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    bool CompareTypes(AbilityType setting, AbilityData data)
    {
        if (setting == AbilityType.None)
            return true;
        return (data.typeOne == setting || data.typeTwo == setting);
    }

    #endregion

#region Ability Search

    void ChangeAbilityInput(string text)
    {
        SearchAbility();
    }

    void ChangeAbilityDropdown(int n)
    {
        SearchAbility();
    }

    void SearchAbility()
    {
        PlayerSearch searchPlayer = PlayerSearch.All;
        switch (characterDropdown.options[characterDropdown.value].text)
        {
            case "All":
                searchPlayer = PlayerSearch.All;
                break;
            case "Knight":
                searchPlayer = PlayerSearch.Knight;
                break;
            case "Angel":
                searchPlayer = PlayerSearch.Angel;
                break;
            case "Wizard":
                searchPlayer = PlayerSearch.Wizard;
                break;
        }

        AbilityType searchType = AbilityType.None;
        switch (typeDropdown.options[typeDropdown.value].text)
        {
            case "All":
                searchType = AbilityType.None;
                break;
            case "Attack":
                searchType = AbilityType.Attack;
                break;
            case "Healing":
                searchType = AbilityType.Healing;
                break;
            case "Emotion":
                searchType = AbilityType.Emotion;
                break;
            case "Position":
                searchType = AbilityType.Position;
                break;
            case "Stats":
                searchType = AbilityType.Stats;
                break;
        }

        foreach (AbilityBox box in knightAbilities)
        {
            if (CompareCharacters(searchPlayer, PlayerSearch.Knight)
                && (CompareStrings(abilityInput.text, box.ability.data.myName) || CompareStrings(abilityInput.text, box.ability.editedDescription))
                && CompareTypes(searchType, box.ability.data))
                box.transform.SetParent(storeAbilityBoxes);
            else
                box.transform.SetParent(null);
        }
        foreach (AbilityBox box in angelAbilities)
        {
            if (CompareCharacters(searchPlayer, PlayerSearch.Angel)
                && (CompareStrings(abilityInput.text, box.ability.data.myName) || CompareStrings(abilityInput.text, box.ability.editedDescription))
                && CompareTypes(searchType, box.ability.data))
                box.transform.SetParent(storeAbilityBoxes);
            else
                box.transform.SetParent(null);
        }
        foreach (AbilityBox box in wizardAbilities)
        {
            if (CompareCharacters(searchPlayer, PlayerSearch.Wizard)
                && (CompareStrings(abilityInput.text, box.ability.data.myName) || CompareStrings(abilityInput.text, box.ability.editedDescription))
                && CompareTypes(searchType, box.ability.data))
                box.transform.SetParent(storeAbilityBoxes);
            else
                box.transform.SetParent(null);
        }

        storeAbilityBoxes.sizeDelta = new Vector3(2560, Math.Max(875, 175 * (1+(storeAbilityBoxes.childCount / 6))));
    }

    #endregion

#region Weapon Search

    void ChangeWeaponInput(string text)
    {
        SearchWeapon();
    }

    void SearchWeapon()
    {
        foreach (WeaponBox nextBox in listOfWeaponBoxes)
        {
            if (CompareStrings(weaponInput.text, nextBox.weapon.data.myName) || CompareStrings(weaponInput.text, nextBox.weapon.editedDescription))
                nextBox.transform.SetParent(storeWeaponBoxes);
            else
                nextBox.transform.SetParent(null);
        }
        storeWeaponBoxes.sizeDelta = new Vector3(2560, Math.Max(875, 175 * (1 + (storeWeaponBoxes.childCount / 6))));
    }

    #endregion

#region Enemy Search

    void ChangeEnemyInput(string text)
    {
        SearchEnemy();
    }

    void SearchEnemy()
    {
        foreach (Character nextEnemy in listOfEnemyBoxes)
        {
            if (CompareStrings(enemyInput.text, nextEnemy.data.myName) || CompareStrings(enemyInput.text, nextEnemy.data.description))
                nextEnemy.transform.SetParent(storeEnemyBoxes);
            else
                nextEnemy.transform.SetParent(null);
        }
        storeEnemyBoxes.sizeDelta = new Vector3(2560, Math.Max(875, 175 * (1 + (storeEnemyBoxes.childCount / 6))));
    }

    #endregion
}