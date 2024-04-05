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
            InitialToggle(toggle, true);
        foreach (Toggle toggle in listOfChallenges)
            InitialToggle(toggle, false);
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
            yield return FileManager.instance.DownloadFile("Enemy Ability Data");
            yield return FileManager.instance.DownloadFile("Weapon Data");
            FileManager.instance.downloadOn = false;
        }

        FileManager.instance.listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        FileManager.instance.listOfBonusEnemies = DataLoader.ReadCharacterData("Bonus Enemy Data");
        FileManager.instance.listOfPlayerAbilities = DataLoader.ReadAbilityData("Player Ability Data");
        FileManager.instance.listOfEnemyAbilities = DataLoader.ReadAbilityData("Enemy Ability Data");

        GeneratePlayers();

        GameObject.Find("Loading Text").SetActive(false);
        loadButtons.SetActive(true);
        stillGenerating = false;
    }

    void GeneratePlayers()
    {
        try
        {
            foreach (Character player in FileManager.instance.listOfPlayers)
                Destroy(player.gameObject);
            FileManager.instance.listOfPlayers.Clear();
        }
        catch
        {
            //do nothing
        }

        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab).AddComponent<PlayerCharacter>();
            List<AbilityData> allAbilities = FileManager.instance.ConvertNumbersToAbilityData(playerData[i].skillNumbers, true).Shuffle();
            List<AbilityData> usedAbilities = new();

            int counter = -1;
            while (usedAbilities.Count < 6)
            {
                counter++;
                try
                {
                    usedAbilities.Add(allAbilities[counter]);
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

            nextCharacter.SetupCharacter(CharacterType.Player, playerData[i], usedAbilities, (Emotion)UnityEngine.Random.Range(1,5), false);
            FileManager.instance.listOfPlayers.Add(nextCharacter);

            nextCharacter.transform.SetParent(abilityBoxes[i*6].transform.parent);
            nextCharacter.transform.localPosition = new Vector3(-1050, 0, 0);
            for (int j = 0; j<nextCharacter.listOfRandomAbilities.Count; j++)
                abilityBoxes[i * 6 + j].ReceiveAbility(true, nextCharacter.listOfRandomAbilities[j]);
        }
    }

    void ToggleInfoScreen()
    {
        infoScreen.SetActive(!infoScreen.activeSelf);
    }

    #endregion

#region Cheats/Challenges

    void InitialToggle(Toggle toggle, bool cheat)
    {
        toggle.isOn = PlayerPrefs.HasKey(toggle.name) && PlayerPrefs.GetInt(toggle.name) == 1;
        toggle.onValueChanged.AddListener((bool isOn) => SetPref(isOn, toggle.name, cheat));
        SetPref(toggle.isOn, toggle.name, cheat);
    }

    void SetPref(bool isOn, string name, bool cheat)
    {
        PlayerPrefs.SetInt(name, (isOn) ? 1 : 0);
        if (cheat && isOn)
            CarryVariables.instance.listOfCheats.Add(name);
        else if (cheat && !isOn)
            CarryVariables.instance.listOfCheats.Remove(name);

        else if (!cheat && isOn)
            CarryVariables.instance.listOfChallenges.Add(name);
        else if (!cheat && !isOn)
            CarryVariables.instance.listOfChallenges.Remove(name);
    }

    public void CheatChallengeMenu()
    {
        cheatChallengeObject.SetActive(!cheatChallengeObject.activeSelf);
    }

    public void ClearAll()
    {
        foreach (Toggle cheat in listOfCheats)
        {
            cheat.isOn = false;
            SetPref(false, cheat.name, true);
        }
        foreach (Toggle challenge in listOfChallenges)
        {
            challenge.isOn = false;
            SetPref(false, challenge.name, false);
        }
    }

    #endregion

}
