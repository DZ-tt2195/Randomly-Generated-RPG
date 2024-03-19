using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System;
using System.Linq;

public class TitleScreen : MonoBehaviour
{

#region Variables

    public static TitleScreen instance;
    [ReadOnly] public Transform canvas;
    [SerializeField] GameObject playerPrefab;

    [SerializeField] bool randomSeed;
    [SerializeField][ConditionalField(nameof(randomSeed), inverse: true)] int chosenSeed;

    [SerializeField] GameObject cheatChallengeObject;
    [SerializeField] List<Toggle> listOfCheats = new();
    [SerializeField] List<Toggle> listOfChallenges = new();

    List<AbilityData> playerAbilities = new();
    [SerializeField] TMP_InputField inputField;

    #endregion

#region Setup

    void Start()
    {
        if (randomSeed || !Application.isEditor)
        {
            chosenSeed = (int)DateTime.Now.Ticks;
            Debug.Log($"random seed: {chosenSeed}");
        }
        else
        {
            Debug.Log($"manual seed: {chosenSeed}");
        }
        UnityEngine.Random.InitState(chosenSeed);

        Character.borderColor = 0;
        StartCoroutine(GenerateFiles());

        foreach (Toggle toggle in listOfCheats)
        {
            if (PlayerPrefs.HasKey(toggle.name))
            {
                toggle.isOn = PlayerPrefs.GetInt(toggle.name) == 1;
            }
            else
            {
                toggle.isOn = false;
            }
            toggle.onValueChanged.AddListener((bool isOn) => SetPref(isOn, toggle.name));
            SetPref(toggle.isOn, toggle.name);
        }

        foreach (Toggle toggle in listOfChallenges)
        {
            if (PlayerPrefs.HasKey(toggle.name))
            {
                toggle.isOn = PlayerPrefs.GetInt(toggle.name) == 1;
            }
            else
            {
                toggle.isOn = false;
            }
            toggle.onValueChanged.AddListener((bool isOn) => SetPref(isOn, toggle.name));
            SetPref(toggle.isOn, toggle.name);
        }
    }

    void SetPref(bool isOn, string name)
    {
        PlayerPrefs.SetInt(name, (isOn) ? 1 : 0);
    }

    IEnumerator GenerateFiles()
    {
        GameObject loadButton = GameObject.Find("Gameplay Buttons");
        loadButton.SetActive(false);

        #if UNITY_EDITOR
            yield return FileManager.instance.DownloadFile("Player Data");
            yield return FileManager.instance.DownloadFile("Enemy Data");
            yield return FileManager.instance.DownloadFile("Ability Data");
            yield return FileManager.instance.DownloadFile("Weapon Data");
        #endif

        FileManager.instance.listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        FileManager.instance.listOfAbilities = DataLoader.ReadAbilityData("Ability Data");
        FileManager.instance.listOfWeapons = DataLoader.ReadWeaponData("Weapon Data");
        FileManager.instance.listOfWeapons = FileManager.instance.listOfWeapons.Shuffle();

        GeneratePlayers();
        loadButton.SetActive(true);
        inputField.onValueChanged.AddListener(SearchAbility);
    }

    void GeneratePlayers()
    {
        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab).AddComponent<PlayerCharacter>();
            WeaponData randomWeapon;

            try { randomWeapon = FileManager.instance.listOfWeapons[i]; }
            catch { randomWeapon = null; }

            List<AbilityData> characterAbilities = new();
            string[] divideSkillsIntoNumbers = playerData[i].skillNumbers.Split(',');
            List<string> putIntoList = new();
            foreach (string next in divideSkillsIntoNumbers)
            {
                next.Trim();
                putIntoList.Add(next);
                try
                {
                    playerAbilities.Add(FileManager.instance.listOfAbilities[int.Parse(next)]);
                }
                catch (FormatException)
                {
                    continue;
                }
            }
            putIntoList = putIntoList.Shuffle();

            int counter = -1;
            while (characterAbilities.Count < 5)
            {
                counter++;
                try
                {
                    string skillNumber = putIntoList[counter];
                    characterAbilities.Add(FileManager.instance.listOfAbilities[int.Parse(skillNumber)]);
                }
                catch (FormatException)
                {
                    continue;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.Log($"ran out of skills");
                    break;
                }
            }

            nextCharacter.SetupCharacter(CharacterType.Player, playerData[i], characterAbilities, (Emotion)UnityEngine.Random.Range(1,5), 1f, randomWeapon);
            FileManager.instance.listOfPlayers.Add(nextCharacter);

            nextCharacter.transform.SetParent(FileManager.instance.canvas);
            nextCharacter.transform.localPosition = new Vector3(-1050 + (350 * i), -550, 0);
            nextCharacter.transform.SetAsFirstSibling();
        }
    }

    public void CheatChallengeToggle()
    {
        cheatChallengeObject.SetActive(!cheatChallengeObject.activeSelf);
    }

    void SearchAbility(string newValue)
    {
        IEnumerable<AbilityData> searchName = FileManager.instance.listOfAbilities.Where
            (ability => ability.myName.Contains(newValue, StringComparison.OrdinalIgnoreCase));

        IEnumerable<AbilityData> searchDescription = FileManager.instance.listOfAbilities.Where
            (ability => ability.description.Contains(newValue, StringComparison.OrdinalIgnoreCase));

        Debug.Log($"{newValue}: Names: {searchName.Count()} Descriptions: {searchDescription.Count()}");
    }

    #endregion

}
