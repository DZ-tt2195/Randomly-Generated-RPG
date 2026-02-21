using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class AbilityForcer : MonoBehaviour
{
    [SerializeField] AbilityBox abilityBoxPrefab;
    [SerializeField] GameObject playerPrefab;
    Dictionary<string, List<AbilityBox>> abilityDictionary = new();
    [SerializeField] Button confirmButton;
    [SerializeField] Transform blanks;
    List<AbilityBox> chosenAbilities = new();
    AbilityBox clicked;
    [SerializeField] TMP_Text header;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text confirm;

    void Start()
    {
        if (ScreenOverlay.instance.mode == GameMode.Tutorial)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Log.instance.AddText(AutoTranslate.Defeat_Waves("5"));
            SearchBoxes(AutoTranslate.Wizard());
            SearchBoxes(AutoTranslate.Knight());
            SearchBoxes(AutoTranslate.Angel());

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

            if (ScreenOverlay.instance.mode == GameMode.Daily)
            {
                Confirmed();
            }
            else if (ScreenOverlay.instance.mode == GameMode.Main)
            {
                header.text = AutoTranslate.Customize_Game();
                description.text = AutoTranslate.Customize_Description();
                confirm.text = AutoTranslate.Confirm();
                confirmButton.onClick.AddListener(Confirmed);

                abilityDictionary.Add(nameof(AutoTranslate.Wizard), new());
                abilityDictionary.Add(nameof(AutoTranslate.Knight), new());
                abilityDictionary.Add(nameof(AutoTranslate.Angel), new());

                foreach (var KVP in GameFiles.inst.listOfPlayerAbilities)
                {
                    Ability nextAbility = new Ability(null, KVP.Value, false);
                    AbilityBox nextBox = Instantiate(abilityBoxPrefab, null);
                    nextBox.ReceiveAbility(true, nextAbility);

                    nextBox.button.onClick.AddListener(() => SendAbility(nextAbility));
                    abilityDictionary[KVP.Value.controller].Add(nextBox);
                }
                foreach (var key in abilityDictionary.Keys.ToList())
                    abilityDictionary[key] = abilityDictionary[key].OrderBy(box => box.ability.data.abilityName).ToList();
            }

            Log.instance.AddText(AutoTranslate.Blank());
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
        int counter = 0;
        foreach (var KVP in GameFiles.inst.listOfPlayers)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab).AddComponent<PlayerCharacter>();
            List<AbilityData> abilitiesForPlayer = new();
            for (int j = 0; j<6; j++)
            {
                AbilityBox nextBox = chosenAbilities[counter * 6 + j];
                if (nextBox.ability != null && !abilitiesForPlayer.Contains(nextBox.ability.data))
                    abilitiesForPlayer.Add(nextBox.ability.data);
            }
            abilitiesForPlayer = GameFiles.inst.CompletePlayerAbilities(abilitiesForPlayer, GameFiles.inst.ConvertToAbilityData(KVP.Value.listOfAbilities, true), TurnManager.inst.dailyRNG);

            nextCharacter.SetupCharacter(KVP.Value, abilitiesForPlayer, Character.RandomEmotion(TurnManager.inst.dailyRNG), false);
            TurnManager.inst.AddPlayer(nextCharacter);
            counter++;
        }

        TurnManager.inst.StartCoroutine(TurnManager.inst.NewWave());
        Destroy(this.gameObject);
    }
}
