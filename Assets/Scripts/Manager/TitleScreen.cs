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

    [Foldout("Misc", true)]
    [SerializeField] GameObject storyObject;
    [SerializeField] GameObject specialThanksObject;
    [SerializeField] TMP_Text timeText;

    [Foldout("RNG", true)]
    [SerializeField] bool randomSeed;
    [SerializeField][ConditionalField(nameof(randomSeed), inverse: true)] int chosenSeed;

    [Foldout("Cheats/Challenges", true)]
    [SerializeField] GameObject cheatChallengeObject;
    List<Toggle> listOfCheats = new();
    List<Toggle> listOfChallenges = new();

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
        TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
        string timezone = (utcOffset.Hours > 0 ? "+" : "") + $"{utcOffset.Hours:D2}:{utcOffset.Minutes:D2}";
        timeText.text = $"{CarryVariables.instance.Translate("Your Timezone", new() { ("Time", timezone)})}";

        DateTime nextUtcMidnight = DateTime.UtcNow.Date.AddDays(1);
        TimeSpan timeUntilMidnightUtc = nextUtcMidnight - DateTime.UtcNow;

        string nextChallenge = $"{timeUntilMidnightUtc.Hours:D2}:{timeUntilMidnightUtc.Minutes:D2}:{timeUntilMidnightUtc.Seconds:D2}";
        timeText.text += $"\n{CarryVariables.instance.Translate("Next Challenge", new() { ("Time", nextChallenge)})}";
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
        PlayerPrefs.Save();
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
