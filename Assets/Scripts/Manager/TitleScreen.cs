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

    public static TitleScreen instance;

    [Foldout("UI", true)]
    [SerializeField] Button storyButton;
    [SerializeField] GameObject storyObject;
    [SerializeField] Button specialThanksButton;
    [SerializeField] GameObject specialThanksObject;
    [SerializeField] TMP_Text timeText;
    [SerializeField] Button soundCreditsButton;
    [SerializeField] GameObject soundCreditsObject;
    [SerializeField] Button extrasButton;
    [SerializeField] Transform allExtras;

    [Foldout("RNG", true)]
    [SerializeField] bool randomSeed;
    [SerializeField][ConditionalField(nameof(randomSeed), inverse: true)] int chosenSeed;

    [Foldout("Translate", true)]
    [SerializeField] TMP_Text gameName;
    [SerializeField] TMP_Text author;
    [SerializeField] TMP_Text lastUpdate;
    [SerializeField] TMP_Text tutorial;
    [SerializeField] TMP_Text storyText;
    [SerializeField] TMP_Text play;
    [SerializeField] TMP_Text daily;
    [SerializeField] TMP_Text encyclopedia;
    [SerializeField] TMP_Text actualThanks;
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
        gameName.text = AutoTranslate.Title();
        author.text = AutoTranslate.Author_Credit();
        lastUpdate.text = AutoTranslate.Last_Update();
        tutorial.text = AutoTranslate.Tutorial();
        storyText.text = AutoTranslate.Story_Text();
        play.text = AutoTranslate.Play_Game();
        daily.text = AutoTranslate.Daily_Challenge();
        encyclopedia.text = AutoTranslate.Encyclopedia();
        actualThanks.text = AutoTranslate.All_Thanks();

        extrasButton.onClick.AddListener(() =>
        {
            extrasButton.gameObject.SetActive(false);
            allExtras.gameObject.SetActive(true);            
        });
        extrasButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Extras();

        storyButton.onClick.AddListener(() =>
        {
            AudioManager.instance.Menu();
            storyObject.SetActive(!storyObject.activeSelf);            
        });
        storyButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Story();

        specialThanksButton.onClick.AddListener(() =>
        {
            AudioManager.instance.Menu();
            specialThanksObject.SetActive(!specialThanksObject.activeSelf);            
        });
        specialThanksButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Special_Thanks();

        soundCreditsButton.onClick.AddListener(() =>
        {
            AudioManager.instance.Menu();
            soundCreditsObject.SetActive(!soundCreditsObject.activeSelf);            
        });
        soundCreditsButton.transform.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Sound_Credits();
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
}
