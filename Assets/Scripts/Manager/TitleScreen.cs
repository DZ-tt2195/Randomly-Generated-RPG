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
    bool stillGenerating { get { return _stillGenerating; } set { { _stillGenerating = value; } if (generationTime>0f) Debug.Log(generationTime); } }

    [Foldout("Misc", true)]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject storyObject;
    [SerializeField] GameObject specialThanksObject;

    [Foldout("RNG", true)]
    [SerializeField] bool randomSeed;
    [SerializeField][ConditionalField(nameof(randomSeed), inverse: true)] int chosenSeed;

    [Foldout("Cheats/Challenges", true)]
    [SerializeField] GameObject cheatChallengeObject;
    List<Toggle> listOfCheats = new();
    List<Toggle> listOfChallenges = new();

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

        Toggle[] allToggles = FindObjectsOfType<Toggle>(includeInactive: true);
        foreach (Toggle toggle in allToggles)
        {
            if (toggle.transform.parent.name.Equals("Cheats"))
            {
                listOfCheats.Add(toggle);
                InitialToggle(toggle, true);
            }
            else if (toggle.transform.parent.name.Equals("Challenges"))
            {
                listOfChallenges.Add(toggle);
                InitialToggle(toggle, false);
            }
        }
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
            FileManager.instance.downloadOn = false;
        }

        List<CharacterData> allEnemies = DataLoader.ReadCharacterData("Enemy Data").OrderBy(data => data.myName).ToList();
        FileManager.instance.listOfEnemies = new()
        {
            new List<CharacterData>(),
            allEnemies.Where(data => data.difficulty == 1).ToList(),
            allEnemies.Where(data => data.difficulty == 2).ToList(),
            allEnemies.Where(data => data.difficulty == 3).ToList(),
        };

        FileManager.instance.listOfBonusEnemies = DataLoader.ReadCharacterData("Bonus Enemy Data");
        FileManager.instance.listOfPlayerAbilities = DataLoader.ReadAbilityData("Player Ability Data").OrderBy(data => data.myName).ToList();
        FileManager.instance.listOfEnemyAbilities = DataLoader.ReadAbilityData("Enemy Ability Data").OrderBy(data => data.myName).ToList();

        GeneratePlayers();

        loadButtons.SetActive(true);
        GameObject.Find("Loading Text").SetActive(false);
        if (!Application.isEditor) GameObject.Find("Play the tutorial").SetActive(false);
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
            nextCharacter.SetupCharacter(CharacterType.Player, playerData[i],
                FileManager.instance.GenerateRandomPlayerAbilities(6, playerData[i].listOfSkills),
                (Emotion)UnityEngine.Random.Range(1,5), false);

            nextCharacter.transform.SetParent(abilityBoxes[i * 6].transform.parent);
            nextCharacter.transform.localPosition = new Vector3(-1050, 0, 0);

            FileManager.instance.listOfPlayers.Add(nextCharacter);
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

        TMP_Text textLabel = toggle.transform.GetChild(0).GetComponent<TMP_Text>();
        textLabel.text = KeywordTooltip.instance.EditText(textLabel.text);
        textLabel.gameObject.AddComponent<KeywordLinkHover>();
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

#region Misc

    public void StoryMenu()
    {
        storyObject.SetActive(!storyObject.activeSelf);
    }

    public void SpecialThanksMenu()
    {
        specialThanksObject.SetActive(!specialThanksObject.activeSelf);
    }

#endregion

}
