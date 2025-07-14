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
    Dictionary<string, List<AbilityBox>> abilityDictionary = new();
    [SerializeField] Button confirmButton;
    [SerializeField] Transform blanks;
    [SerializeField] List<TMP_Dropdown> emotionDropdowns = new();
    List<AbilityBox> chosenAbilities = new();

    AbilityBox clicked;

    void Start()
    {
        if (CarryVariables.instance.mode == CarryVariables.GameMode.Tutorial)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Log.instance.AddText(CarryVariables.instance.GetText("Defeat Waves").Replace("$Num$", "5"));
            SearchBoxes("Knight");
            SearchBoxes("Angel");
            SearchBoxes("Wizard");

            void SearchBoxes(string toFind)
            {
                Transform search = this.transform.Find(toFind);
                foreach (Transform child in search)
                {
                    AbilityBox box = child.GetComponent<AbilityBox>();
                    box.ReceiveAbility(true, null);
                    box.button.onClick.AddListener(() => Summon(box, toFind));
                    chosenAbilities.Add(box);
                }
            }

            if (CarryVariables.instance.mode == CarryVariables.GameMode.Daily)
            {
                DateTime day = DateTime.UtcNow.Date;
                int seed = day.Year * 10000 + day.Month * 100 + day.Day;
                dailyRNG = new System.Random(seed);
                Log.instance.AddText($"Daily Challenge: {day:MMMM dd, yyyy}", 1);
                Confirmed();
            }
            else if (CarryVariables.instance.mode == CarryVariables.GameMode.Main)
            {
                confirmButton.onClick.AddListener(Confirmed);
                foreach (string cheat in CarryVariables.instance.listOfCheats)
                    Log.instance.AddText($"<color=#00FF00>Cheat: {cheat}</color>", 1);
                foreach (string challenge in CarryVariables.instance.listOfChallenges)
                    Log.instance.AddText($"<color=#FF0000>Challenge: {challenge}</color>", 1);

                abilityDictionary.Add("Knight", new());
                abilityDictionary.Add("Angel", new());
                abilityDictionary.Add("Wizard", new());

                foreach (AbilityData data in CarryVariables.instance.listOfPlayerAbilities)
                {
                    Ability nextAbility = this.gameObject.AddComponent<Ability>();
                    nextAbility.SetupAbility(data, false);

                    AbilityBox nextBox = Instantiate(abilityBoxPrefab, null);
                    nextBox.ReceiveAbility(true, nextAbility);

                    nextBox.button.onClick.AddListener(() => SendAbility(nextAbility));
                    abilityDictionary[data.user].Add(nextBox);
                }

                foreach (var key in abilityDictionary.Keys.ToList())
                    abilityDictionary[key] = abilityDictionary[key].OrderBy(box => box.ability.data.myName).ToList();
            }

            Log.instance.AddText("");
        }
    }

    void Summon(AbilityBox clickedBox, string toFind)
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
        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab).AddComponent<PlayerCharacter>();

            Emotion startingEmotion = Emotion.Dead;
            string dropdownValue = emotionDropdowns[i].options[emotionDropdowns[i].value].text;

            if (dropdownValue == CarryVariables.instance.GetText("Random"))
            {
                int randomValue = dailyRNG != null ? dailyRNG.Next(1, 5) : UnityEngine.Random.Range(1, 5);
                startingEmotion = (Emotion)randomValue;
            }
            else if (dropdownValue == CarryVariables.instance.GetText("Neutral"))
                startingEmotion = Emotion.Neutral;
            else if (dropdownValue == CarryVariables.instance.GetText("Happy"))
                startingEmotion = Emotion.Happy;
            else if (dropdownValue == CarryVariables.instance.GetText("Angry"))
                startingEmotion = Emotion.Angry;
            else if (dropdownValue == CarryVariables.instance.GetText("Sad"))
                startingEmotion = Emotion.Sad;

            List<AbilityData> abilitiesForPlayer = new();
            for (int j = 0; j<6; j++)
            {
                AbilityBox nextBox = chosenAbilities[i * 6 + j];
                if (nextBox.ability != null && !abilitiesForPlayer.Contains(nextBox.ability.data))
                    abilitiesForPlayer.Add(nextBox.ability.data);
            }
            abilitiesForPlayer = CarryVariables.instance.CompletePlayerAbilities(abilitiesForPlayer, playerData[i].myName, dailyRNG);

            nextCharacter.SetupCharacter(playerData[i], abilitiesForPlayer, startingEmotion, false);
            TurnManager.instance.AddPlayer(nextCharacter);
        }
        TurnManager.instance.StartCoroutine(TurnManager.instance.NewWave());
        Destroy(this.gameObject);
    }
}
