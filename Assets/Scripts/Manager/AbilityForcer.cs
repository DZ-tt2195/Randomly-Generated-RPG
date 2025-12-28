using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class AbilityForcer : MonoBehaviour
{
    System.Random dailyRNG;
    [SerializeField] AbilityBox abilityBoxPrefab;
    [SerializeField] GameObject playerPrefab;
    Dictionary<ToTranslate, List<AbilityBox>> abilityDictionary = new();
    [SerializeField] Button confirmButton;
    [SerializeField] Transform blanks;
    [SerializeField] List<TranslateDropdown> emotionDropdowns = new();
    List<AbilityBox> chosenAbilities = new();

    AbilityBox clicked;

    void Start()
    {
        if (ScreenOverlay.instance.mode == GameMode.Tutorial)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Log.instance.AddText(AutoTranslate.Defeat_Waves("5"));
            SearchBoxes(ToTranslate.Wizard);
            SearchBoxes(ToTranslate.Knight);
            SearchBoxes(ToTranslate.Angel);

            void SearchBoxes(ToTranslate toFind)
            {
                Transform search = this.transform.Find(toFind.ToString());
                foreach (Transform child in search)
                {
                    AbilityBox box = child.GetComponent<AbilityBox>();
                    box.ReceiveAbility(true, null);
                    box.button.onClick.AddListener(() => Summon(box, toFind));
                    chosenAbilities.Add(box);
                }
            }

            if (ScreenOverlay.instance.mode == GameMode.Daily)
            {
                DateTime day = DateTime.UtcNow.Date;
                int seed = day.Year * 10000 + day.Month * 100 + day.Day;
                dailyRNG = new System.Random(seed);

                Log.instance.AddText(AutoTranslate.DoEnum(ToTranslate.Daily_Challenge), 0);
                Log.instance.AddText(AutoTranslate.Current_Date(Translator.inst.Translate($"Month_{day.Month}"), day.Day.ToString(), day.Year.ToString()), 1);
                Confirmed();
            }
            else if (ScreenOverlay.instance.mode == GameMode.Main)
            {
                confirmButton.onClick.AddListener(Confirmed);
                foreach (ToTranslate cheat in ScreenOverlay.instance.listOfCheats)
                    Log.instance.AddText($"<color=#00FF00>{AutoTranslate.DoEnum(ToTranslate.Cheat)}: {AutoTranslate.DoEnum(cheat)}</color>", 1);
                foreach (ToTranslate challenge in ScreenOverlay.instance.listOfChallenges)
                    Log.instance.AddText($"<color=#FF0000>{AutoTranslate.DoEnum(ToTranslate.Challenge)}: {AutoTranslate.DoEnum(challenge)}</color>", 1);

                abilityDictionary.Add(ToTranslate.Wizard, new());
                abilityDictionary.Add(ToTranslate.Knight, new());
                abilityDictionary.Add(ToTranslate.Angel, new());

                foreach (AbilityData data in GameFiles.inst.listOfPlayerAbilities)
                {
                    Ability nextAbility = new Ability(null, data, false);
                    AbilityBox nextBox = Instantiate(abilityBoxPrefab, null);
                    nextBox.ReceiveAbility(true, nextAbility);

                    nextBox.button.onClick.AddListener(() => SendAbility(nextAbility));
                    abilityDictionary[data.controller].Add(nextBox);
                }

                foreach (var key in abilityDictionary.Keys.ToList())
                    abilityDictionary[key] = abilityDictionary[key].OrderBy(box => box.ability.data.abilityName).ToList();
            }

            Log.instance.AddText(AutoTranslate.DoEnum(ToTranslate.Blank));
        }
    }

    void Summon(AbilityBox clickedBox, ToTranslate toFind)
    {
        clicked = clickedBox;
        clickedBox.ReceiveAbility(true, null);

        for (int i = blanks.childCount - 1; i >= 0; i--)
            blanks.GetChild(i).SetParent(null);

        foreach (AbilityBox box in abilityDictionary[toFind])
            box.transform.SetParent(blanks);
    }

    void SendAbility(Ability ability)
    {
        clicked.ReceiveAbility(true, ability);
        for (int i = blanks.childCount - 1; i >= 0; i--)
            blanks.GetChild(i).SetParent(null);
    }

    void Confirmed()
    {
        for (int i = 0; i < GameFiles.inst.listOfPlayers.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab).AddComponent<PlayerCharacter>();

            Emotion startingEmotion = Emotion.Dead;
            switch (emotionDropdowns[i].GetOriginal())
            {
                case "Random":
                    int randomValue = dailyRNG != null ? dailyRNG.Next(1, 5) : UnityEngine.Random.Range(1, 5);
                    startingEmotion = (Emotion)randomValue;
                    break;
                case "Neutral":
                    startingEmotion = Emotion.Neutral;
                    break;
                case "Happy":
                    startingEmotion = Emotion.Happy;
                    break;
                case "Angry":
                    startingEmotion = Emotion.Angry;
                    break;
                case "Sad":
                    startingEmotion = Emotion.Sad;
                    break;
            }

            List<AbilityData> abilitiesForPlayer = new();
            for (int j = 0; j<6; j++)
            {
                AbilityBox nextBox = chosenAbilities[i * 6 + j];
                if (nextBox.ability != null && !abilitiesForPlayer.Contains(nextBox.ability.data))
                    abilitiesForPlayer.Add(nextBox.ability.data);
            }
            abilitiesForPlayer = GameFiles.inst.CompletePlayerAbilities(abilitiesForPlayer, GameFiles.inst.ConvertToAbilityData(GameFiles.inst.listOfPlayers[i].listOfAbilities, true), dailyRNG);

            nextCharacter.SetupCharacter(GameFiles.inst.listOfPlayers[i], abilitiesForPlayer, startingEmotion, false);
            TurnManager.instance.AddPlayer(nextCharacter);
        }
        TurnManager.instance.StartCoroutine(TurnManager.instance.NewWave());
        Destroy(this.gameObject);
    }
}
