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
    float generationTime = 0f;
    bool _stillGenerating;
    bool stillGenerating { get { return _stillGenerating; } set { { _stillGenerating = value; } Debug.Log(generationTime); } }
    [SerializeField] GameObject playerPrefab;

    [Foldout("RNG", true)]
    [SerializeField] bool randomSeed;
    [SerializeField][ConditionalField(nameof(randomSeed), inverse: true)] int chosenSeed;

    [Foldout("Cheats/Challenges", true)]
    [SerializeField] GameObject cheatChallengeObject;
    [SerializeField] List<Toggle> listOfCheats = new();
    [SerializeField] List<Toggle> listOfChallenges = new();

    [Foldout("Info Screen", true)]
    [SerializeField] List<Button> infoScreenToggles = new();
    [SerializeField] GameObject infoScreen;
    [SerializeField] List<WeaponBox> weaponBoxes = new();
    [SerializeField] List<AbilityBox> abilityBoxes = new();

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

        foreach (Button button in infoScreenToggles)
            button.onClick.AddListener(ToggleInfoScreen);

        foreach (Toggle toggle in listOfCheats)
            InitialToggle(toggle);
        foreach (Toggle toggle in listOfChallenges)
            InitialToggle(toggle);
    }

    private void Update()
    {
        if (stillGenerating)
            generationTime += Time.deltaTime;
    }

    IEnumerator GenerateFiles()
    {
        stillGenerating = true;
        GameObject loadButtons = GameObject.Find("Gameplay Buttons");
        loadButtons.SetActive(false);

        if (Application.isEditor)
        {
            yield return FileManager.instance.DownloadFile("Player Data");
            yield return FileManager.instance.DownloadFile("Enemy Data");
            yield return FileManager.instance.DownloadFile("Bonus Enemy Data");
            yield return FileManager.instance.DownloadFile("Player Ability Data");
            yield return FileManager.instance.DownloadFile("Other Ability Data");
            yield return FileManager.instance.DownloadFile("Weapon Data");
        }

        FileManager.instance.listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        FileManager.instance.listOfBonusEnemies = DataLoader.ReadCharacterData("Bonus Enemy Data");
        FileManager.instance.listOfPlayerAbilities = DataLoader.ReadAbilityData("Player Ability Data");
        FileManager.instance.listOfOtherAbilities = DataLoader.ReadAbilityData("Other Ability Data");
        FileManager.instance.listOfWeapons = DataLoader.ReadWeaponData("Weapon Data");
        FileManager.instance.listOfWeapons = FileManager.instance.listOfWeapons.Shuffle();

        GeneratePlayers();

        GameObject.Find("Loading Text").SetActive(false);
        loadButtons.SetActive(true);
        stillGenerating = false;
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

            nextCharacter.SetupCharacter(CharacterType.Player, playerData[i], characterAbilities, (Emotion)UnityEngine.Random.Range(1,5), false, 1f, randomWeapon);
            FileManager.instance.listOfPlayers.Add(nextCharacter);

            nextCharacter.transform.SetParent(weaponBoxes[i].transform.parent);
            nextCharacter.transform.localPosition = new Vector3(-1050, 0, 0);
            weaponBoxes[i].ReceiveWeapon(nextCharacter.weapon);
            for (int j = 1; j<nextCharacter.listOfAbilities.Count; j++)
                abilityBoxes[i * 5 + j - 1].ReceiveAbility(true, nextCharacter.listOfAbilities[j]);
        }
    }

    void ToggleInfoScreen()
    {
        infoScreen.SetActive(!infoScreen.activeSelf);
    }

    #endregion

#region Cheats/Challenges

    void InitialToggle(Toggle toggle)
    {
        toggle.isOn = PlayerPrefs.HasKey(toggle.name) && PlayerPrefs.GetInt(toggle.name) == 1;
        toggle.onValueChanged.AddListener((bool isOn) => SetPref(isOn, toggle.name));
        SetPref(toggle.isOn, toggle.name);
    }

    void SetPref(bool isOn, string name)
    {
        PlayerPrefs.SetInt(name, (isOn) ? 1 : 0);
    }

    public void CheatChallengeToggle()
    {
        cheatChallengeObject.SetActive(!cheatChallengeObject.activeSelf);
    }

    #endregion

}
