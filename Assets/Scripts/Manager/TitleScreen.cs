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
    
    [Foldout("Translate", true)]
    [SerializeField] TMP_Text gameName;
    [SerializeField] TMP_Text author;
    [SerializeField] TMP_Text lastUpdate;
    [SerializeField] TMP_Text tutorial;
    [SerializeField] TMP_Text story;
    [SerializeField] TMP_Text storyText;
    [SerializeField] TMP_Text play;
    [SerializeField] TMP_Text daily;
    [SerializeField] TMP_Text encyclopedia;
    [SerializeField] TMP_Text specialThanks;
    [SerializeField] TMP_Text actualThanks;

    #endregion

#region Setup
    void TranslateScreen()
    {
        gameName.text = AutoTranslate.Title();
        author.text = AutoTranslate.Author_Credit();
        lastUpdate.text = AutoTranslate.Last_Update();
        tutorial.text = AutoTranslate.Tutorial();
        story.text = AutoTranslate.Story();
        storyText.text = AutoTranslate.Story_Text();
        play.text = AutoTranslate.Play_Game();
        daily.text = AutoTranslate.Daily_Challenge();
        encyclopedia.text = AutoTranslate.Encyclopedia();
        specialThanks.text = AutoTranslate.Special_Thanks();
        actualThanks.text = AutoTranslate.All_Thanks();
    }
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
        TranslateScreen();
    }

    private void Update()
    {
        TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
        string timezone = (utcOffset.Hours > 0 ? "+" : "") + $"{utcOffset.Hours:D2}:{utcOffset.Minutes:D2}";
        timeText.text = AutoTranslate.Your_Timezone(timezone);

        DateTime nextUtcMidnight = DateTime.UtcNow.Date.AddDays(1);
        TimeSpan timeUntilMidnightUtc = nextUtcMidnight - DateTime.UtcNow;

        string nextChallenge = $"{timeUntilMidnightUtc.Hours:D2}:{timeUntilMidnightUtc.Minutes:D2}:{timeUntilMidnightUtc.Seconds:D2}";
        timeText.text += $"\n{AutoTranslate.Next_Challenge(nextChallenge)}";
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
