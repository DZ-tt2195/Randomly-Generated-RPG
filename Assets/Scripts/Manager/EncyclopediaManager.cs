using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public enum TierSearch { All, Star1, Star2, Star3 };
public enum PlayerSearch { All, Knight, Angel, Wizard };
public class EncyclopediaManager : MonoBehaviour
{

#region Variables

    [Foldout("Changing searches", true)]
        [SerializeField] TMP_Text searchResults;
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
        [SerializeField] TMP_Dropdown type1Dropdown;
        [SerializeField] TMP_Dropdown type2Dropdown;
        [SerializeField] Scrollbar abilityScroll;

    [Foldout("Enemy Search", true)]
        List<Character> listOfEnemyBoxes = new();
        [SerializeField] GameObject characterPrefab;
        [SerializeField] RectTransform storeEnemyBoxes;
        [SerializeField] TMP_InputField enemyInput;
        [SerializeField] TMP_Dropdown tierDropdown;
        [SerializeField] TMP_Dropdown positionDropdown;
        [SerializeField] Scrollbar enemyScroll;

    void ChangeMode(int n)
    {
        searchResults.text = "";
        searchResults.gameObject.SetActive(true);
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
        type1Dropdown.onValueChanged.AddListener(ChangeAbilityDropdown);
        type2Dropdown.onValueChanged.AddListener(ChangeAbilityDropdown);
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

        enemyInput.onValueChanged.AddListener(ChangeEnemyInput);
        tierDropdown.onValueChanged.AddListener(ChangeTierDropdown);
        positionDropdown.onValueChanged.AddListener(ChangeTierDropdown);
        foreach (List<CharacterData> listOfData in FileManager.instance.listOfEnemies)
        {
            foreach (CharacterData data in listOfData)
            {
                Character nextEnemy = Instantiate(characterPrefab).AddComponent<Character>();
                nextEnemy.SetupCharacter(data, FileManager.instance.ConvertToAbilityData(data.listOfSkills, false), Emotion.Neutral, false);
                listOfEnemyBoxes.Add(nextEnemy);
                foreach (Transform child in nextEnemy.transform)
                {
                    if (!(child.name.Equals("Name Text") || child.name.Equals("Health Text")))
                        Destroy(child.gameObject);
                }
            }
        }

        SearchEnemy();
        SearchAbility();
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
        foreach (AbilityType type in data.myTypes)
            if (type == setting) return true;
        return false;
    }

    bool CompareTiers(TierSearch setting, int difficulty)
    {
        return setting switch
        {
            TierSearch.All => true,
            TierSearch.Star1 => difficulty == 1,
            TierSearch.Star2 => difficulty == 2,
            TierSearch.Star3 => difficulty == 3,
            _ => true,
        };
    }

    bool ComparePositions(Position setting, CharacterData data)
    {
        if (setting == Position.Dead)
            return true;
        else
            return setting == data.startingPosition;
    }

    AbilityType ConvertType(string text)
    {
        return text switch
        {
            "Attack" => AbilityType.Attack,
            "Healing" => AbilityType.Healing,
            "Emotion - Player" => AbilityType.EmotionPlayer,
            "Emotion - Enemy" => AbilityType.EmotionEnemy,
            "Position - Player" => AbilityType.PositionPlayer,
            "Position - Enemy" => AbilityType.PositionEnemy,
            "Stats - Player" => AbilityType.StatPlayer,
            "Stats - Enemy" => AbilityType.StatEnemy,
            "Misc" => AbilityType.Misc,
            _ => AbilityType.None,
        };
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

        AbilityType searchType1 = ConvertType(type1Dropdown.options[type1Dropdown.value].text);
        AbilityType searchType2 = ConvertType(type2Dropdown.options[type2Dropdown.value].text);

        foreach (AbilityBox box in knightAbilities)
        {
            if (CompareCharacters(searchPlayer, PlayerSearch.Knight)
                && (CompareStrings(abilityInput.text, box.ability.data.myName) || CompareStrings(abilityInput.text, box.ability.editedDescription))
                && CompareTypes(searchType1, box.ability.data) && CompareTypes(searchType2, box.ability.data))
            {
                box.transform.SetParent(storeAbilityBoxes);
                box.transform.SetAsLastSibling();
            }
            else
            {
                box.transform.SetParent(null);
            }
        }
        foreach (AbilityBox box in angelAbilities)
        {
            if (CompareCharacters(searchPlayer, PlayerSearch.Angel)
                && (CompareStrings(abilityInput.text, box.ability.data.myName) || CompareStrings(abilityInput.text, box.ability.editedDescription))
                && CompareTypes(searchType1, box.ability.data) && CompareTypes(searchType2, box.ability.data))

            {
                box.transform.SetParent(storeAbilityBoxes);
                box.transform.SetAsLastSibling();
            }
            else
            {
                box.transform.SetParent(null);
            }
        }
        foreach (AbilityBox box in wizardAbilities)
        {
            if (CompareCharacters(searchPlayer, PlayerSearch.Wizard)
                && (CompareStrings(abilityInput.text, box.ability.data.myName) || CompareStrings(abilityInput.text, box.ability.editedDescription))
                && CompareTypes(searchType1, box.ability.data) && CompareTypes(searchType2, box.ability.data))
            {
                box.transform.SetParent(storeAbilityBoxes);
                box.transform.SetAsLastSibling();
            }
            else
            {
                box.transform.SetParent(null);
            }
        }

        storeAbilityBoxes.transform.localPosition = new Vector3(0, -1050, 0);
        storeAbilityBoxes.sizeDelta = new Vector3(2560, Math.Max(875, 175 * (2+Mathf.Ceil(storeAbilityBoxes.childCount / 6f))));
        searchResults.text = $"Found {storeAbilityBoxes.childCount} Abilities";
    }

    #endregion

#region Enemy Search

    void ChangeEnemyInput(string text)
    {
        SearchEnemy();
    }

    void ChangeTierDropdown(int n)
    {
        SearchEnemy();
    }

    void SearchEnemy()
    {
        TierSearch searchTier = TierSearch.All;
        Position searchPosition = Position.Dead;

        switch (tierDropdown.options[tierDropdown.value].text)
        {
            case "All":
                searchTier = TierSearch.All;
                break;
            case "1-Star":
                searchTier = TierSearch.Star1;
                break;
            case "2-Star":
                searchTier = TierSearch.Star2;
                break;
            case "3-Star":
                searchTier = TierSearch.Star3;
                break;
        }
        switch (positionDropdown.options[positionDropdown.value].text)
        {
            case "Any":
                searchPosition = Position.Dead;
                break;
            case "Grounded":
                searchPosition = Position.Grounded;
                break;
            case "Elevated":
                searchPosition = Position.Elevated;
                break;
        }

        foreach (Character nextEnemy in listOfEnemyBoxes)
        {
            if (CompareStrings(enemyInput.text, nextEnemy.data.myName) && ComparePositions(searchPosition, nextEnemy.data) && CompareTiers(searchTier, nextEnemy.data.difficulty))
            {
                nextEnemy.transform.SetParent(storeEnemyBoxes);
                nextEnemy.transform.SetAsLastSibling();
            }
            else
            {
                nextEnemy.transform.SetParent(null);
            }
        }
        storeEnemyBoxes.transform.localPosition = new Vector3(0, -1050, 0);
        storeEnemyBoxes.sizeDelta = new Vector3(2560, Math.Max(875, 350 * Mathf.Ceil(storeEnemyBoxes.childCount / 5f)));
        searchResults.text = $"Found {storeEnemyBoxes.childCount} Enemies";
    }

    #endregion

}
