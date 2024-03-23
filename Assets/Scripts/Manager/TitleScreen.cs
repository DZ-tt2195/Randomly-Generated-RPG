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
    [SerializeField] GameObject playerPrefab;

    [SerializeField] bool randomSeed;
    [SerializeField][ConditionalField(nameof(randomSeed), inverse: true)] int chosenSeed;

    [SerializeField] GameObject cheatChallengeObject;
    [SerializeField] List<Toggle> listOfCheats = new();
    [SerializeField] List<Toggle> listOfChallenges = new();

    List<AbilityData> playerAbilities = new();

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
            yield return FileManager.instance.DownloadFile("Bonus Enemy Data");
            yield return FileManager.instance.DownloadFile("Player Ability Data");
            yield return FileManager.instance.DownloadFile("Other Ability Data");
            yield return FileManager.instance.DownloadFile("Weapon Data");
        #endif

        FileManager.instance.listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        FileManager.instance.listOfBonusEnemies = DataLoader.ReadCharacterData("Bonus Enemy Data");
        FileManager.instance.listOfPlayerAbilities = DataLoader.ReadAbilityData("Player Ability Data");
        FileManager.instance.listOfOtherAbilities = DataLoader.ReadAbilityData("Other Ability Data");
        FileManager.instance.listOfWeapons = DataLoader.ReadWeaponData("Weapon Data");
        FileManager.instance.listOfWeapons = FileManager.instance.listOfWeapons.Shuffle();

        GeneratePlayers();
        loadButton.SetActive(true);
    }

    void GeneratePlayers()
    {
        foreach (Character player in FileManager.instance.listOfPlayers)
            Destroy(player.gameObject);
        FileManager.instance.listOfPlayers.Clear();

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
                    playerAbilities.Add(FileManager.instance.listOfPlayerAbilities[int.Parse(next)]);
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
                    characterAbilities.Add(FileManager.instance.listOfPlayerAbilities[int.Parse(skillNumber)]);
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
        }
    }

    public void CheatChallengeToggle()
    {
        cheatChallengeObject.SetActive(!cheatChallengeObject.activeSelf);
    }

#endregion

}
