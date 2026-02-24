using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using MyBox;

public class Customizer : MonoBehaviour
{
#region  Setup
    [Foldout("Abilities", true)]
    [SerializeField] AbilityBox abilityBoxPrefab;
    [SerializeField] GameObject playerPrefab;
    Dictionary<string, List<AbilityBox>> abilityDictionary = new();
    List<AbilityBox> preSelectedAbilities = new();
    AbilityBox clicked;
    [SerializeField] Transform storeBoxes;
    [Foldout("Rules", true)]
    [SerializeField] RectTransform storeRules;
    [SerializeField] RulesText rulesPrefab;
    HashSet<string> clickedRules = new();
    [Foldout("Misc", true)]
    [SerializeField] Button confirmButton;
    [SerializeField] Slider slider;
    [SerializeField] Transform abilityCustomize;
    [SerializeField] Transform rulesCustomize;
    [Foldout("Translate", true)]
    [SerializeField] TMP_Text header;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text confirm;
    [SerializeField] TMP_Text abilities;
    [SerializeField] TMP_Text rules;
    [SerializeField] TMP_Text rulesReminder;
    void TranslateScreen()
    {
        header.text = AutoTranslate.Customize_Game();
        description.text = AutoTranslate.Customize_Description();
        confirm.text = AutoTranslate.Confirm(); 
        abilities.text = AutoTranslate.Abilities();
        rules.text = AutoTranslate.Rules();     
        rulesReminder.text = AutoTranslate.Rules_Reminder();  
    }
    void Start()
    {
        if (ScreenOverlay.instance.mode == GameMode.Tutorial)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Log.instance.AddText(AutoTranslate.Defeat_Waves("5"));
            if (ScreenOverlay.instance.mode == GameMode.Daily)
            {
                Confirmed();
            }
            else if (ScreenOverlay.instance.mode == GameMode.Main)
            {
                TranslateScreen();
                confirmButton.onClick.AddListener(Confirmed);
                slider.onValueChanged.AddListener(Switch);
                Switch(0);

                void Switch(float value)
                {
                    abilityCustomize.gameObject.SetActive(value == 0);
                    rulesCustomize.gameObject.SetActive(value == 1);
                }
                AbilitySetup();
                RulesSetup();
            }

            Log.instance.AddText(AutoTranslate.Blank());
        }
    }
    void Confirmed()
    {
        AbilitiesConfirmed();
        RulesConfirmed();
        TurnManager.inst.StartCoroutine(TurnManager.inst.NewWave());
        Destroy(this.gameObject);
    }
#endregion

#region  Abilities
    void AbilitySetup()
    {
            SearchBoxes(AutoTranslate.Wizard());
            SearchBoxes(AutoTranslate.Knight());
            SearchBoxes(AutoTranslate.Angel());

            void SearchBoxes(string toFind)
            {
                Transform search = abilityCustomize.transform.Find(toFind);
                foreach (Transform child in search)
                {
                    AbilityBox box = child.GetComponent<AbilityBox>();
                    box.ReceiveAbility(true, null);
                    box.button.onClick.AddListener(() => Summon(box, toFind));
                    preSelectedAbilities.Add(box);
                }
            }

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
    void Summon(AbilityBox clickedBox, string toFind)
    {
        clicked = clickedBox;
        clickedBox.ReceiveAbility(true, null);

        for (int i = storeBoxes.childCount - 1; i >= 0; i--)
            storeBoxes.GetChild(i).SetParent(null);

        foreach (AbilityBox box in abilityDictionary[toFind])
            box.transform.SetParent(storeBoxes);
    }
    void SendAbility(Ability ability)
    {
        clicked.ReceiveAbility(true, ability);
        for (int i = storeBoxes.childCount - 1; i >= 0; i--)
            storeBoxes.GetChild(i).SetParent(null);
    }
    void AbilitiesConfirmed()
    {
        int counter = 0;
        foreach (var KVP in GameFiles.inst.listOfPlayers)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab).AddComponent<PlayerCharacter>();
            List<AbilityData> abilitiesForPlayer = new();
            for (int j = 0; j<Character.maxAbilities; j++)
            {
                int toFind = counter * Character.maxAbilities + j;
                if (toFind < preSelectedAbilities.Count)
                {
                    AbilityBox nextBox = preSelectedAbilities[toFind];
                    if (nextBox.ability != null && !abilitiesForPlayer.Contains(nextBox.ability.data))
                        abilitiesForPlayer.Add(nextBox.ability.data);
                }
            }
            abilitiesForPlayer = GameFiles.inst.CompletePlayerAbilities(abilitiesForPlayer, GameFiles.inst.ConvertToAbilityData(KVP.Value.listOfAbilities, true), TurnManager.inst.dailyRNG);

            nextCharacter.SetupCharacter(KVP.Value, abilitiesForPlayer, Character.RandomEmotion(TurnManager.inst.dailyRNG), counter, false);
            TurnManager.inst.AddPlayer(nextCharacter);
            counter++;
        }        
    }
#endregion

#region  Rules
    void RulesSetup()
    {
        foreach (RulesData data in GameFiles.inst.FinishedRules())
        {
            RulesText nextText = Instantiate(rulesPrefab);
            nextText.AssignRule(data.rulesName);
            nextText.transform.SetParent(storeRules);
            nextText.toggle.isOn = false;
            nextText.toggle.onValueChanged.AddListener(RulesToggle);

            void RulesToggle(bool enabled)
            {
                if (enabled)
                {
                    clickedRules.Add(data.rulesName);
                    if (clickedRules.Count > FightRules.totalRules)
                        nextText.toggle.isOn = false;
                }
                else
                {
                    clickedRules.Remove(data.rulesName);
                }
            }
        }
    }
    void RulesConfirmed()
    {
        FightRules.inst.AssignedRules(clickedRules);
    }
#endregion

}
