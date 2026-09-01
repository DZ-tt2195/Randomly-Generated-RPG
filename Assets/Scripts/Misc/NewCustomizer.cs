using UnityEngine;
using TMPro;
using MyBox;
using UnityEngine.UI;
using System.Collections.Generic;
[System.Serializable]
public class ToCustomize
{
    public string toTranslate;
    public TMP_Text label;
    public GameObject toEnable;
    public Transform storeThings;
    public List<int> toSave = new();
}

public class NewCustomizer : MonoBehaviour
{
    [Foldout("UI", true)]
        [SerializeField] Button openCustomizer;
        [SerializeField] TMP_Text customizerText;
        [SerializeField] Transform customizerScreen;
        [SerializeField] Button confirmButton;
        [SerializeField] TMP_Text confirmText;
        [SerializeField] TMP_Text chooseCustomizing;

    [Foldout("Customize", true)]
        [SerializeField] Slider slider;
        [SerializeField] List<ToCustomize> toCustomizeList = new();
        [SerializeField] AbilityBox boxWithToggle;
        [SerializeField] RulesText rulesWithToggle;

    void Awake()
    {
        SetupUI();
        SetupSlider();
        SetupCustomizations();
        confirmButton.onClick.AddListener(Done);
    }
    void SetupUI()
    {
        chooseCustomizing.text = AutoTranslate.Choose_Customizing(Character.maxAbilities.ToString(),FightRules.MaxRules.ToString());
        confirmText.text = AutoTranslate.Confirm();
        customizerText.text = AutoTranslate.Open_Customizer();
        customizerScreen.gameObject.SetActive(false);

        openCustomizer.onClick.AddListener(() =>
        {
            AudioManager.instance.Menu();
            customizerScreen.gameObject.SetActive(true);
        });
    }
    void SetupSlider()
    {
        slider.onValueChanged.AddListener(ChangeScreen);
        slider.value = 0;
        ChangeScreen(0);
        void ChangeScreen(float value)
        {
            int index = (int)value;
            for (int i = 0; i < toCustomizeList.Count; i++)
                toCustomizeList[i].toEnable.SetActive(i == index);
        }
    }
    void SetupCustomizations()
    {
        foreach (ToCustomize customize in toCustomizeList)
        {
            customize.label.text = Translator.inst.Translate(customize.toTranslate);

            bool isRules = customize.toTranslate == "Rules";
            int max = isRules ? FightRules.MaxRules : Character.maxAbilities;

            if (isRules)
                CreateRuleOptions(customize, max);
            else
                CreateAbilityOptions(customize, max);
        }
    }
    void CreateRuleOptions(ToCustomize customize, int max)
    {
        List<RulesData> rules = GameFiles.inst.AllRules();

        for (int i = 0; i < rules.Count; i++)
        {
            int index = i;

            RulesText rule = Instantiate(rulesWithToggle, customize.storeThings);
            rule.AssignRule(rules[index].rulesName);

            SetupToggle(rule.toggle,customize,index,max);
        }
    }

    void CreateAbilityOptions(ToCustomize customize, int max)
    {
        List<AbilityData> abilities = GameFiles.inst.ConvertToAbilityData(GameFiles.inst.listOfPlayers[customize.toTranslate].listOfAbilities,true);

        for (int i = 0; i < abilities.Count; i++)
        {
            int index = i;

            AbilityBox box = Instantiate(boxWithToggle, customize.storeThings);
            Ability ability = new Ability(null, abilities[index], false);
            box.ReceiveAbility(true, ability);

            SetupToggle(box.toggle, customize,index,max);
        }
    }

    void SetupToggle(Toggle toggle,ToCustomize customize,int index,int max)
    {
        toggle.isOn = AlreadySaved(customize, index, max);

        if (toggle.isOn)
            customize.toSave.Add(index);

        toggle.onValueChanged.AddListener(enabled =>
        {
            if (enabled)
            {
                customize.toSave.Add(index); 
                if (customize.toSave.Count > max) 
                    toggle.isOn = false; 
                else 
                    AudioManager.instance.Menu();            
            }
            else
            {
                customize.toSave.Remove(index);
                AudioManager.instance.Menu();
            }
        });
    }

    bool AlreadySaved(ToCustomize customize, int number, int max)
    {
        for (int i = 0; i < max; i++)
        {
            if (PrefManager.GetSaved(customize.toTranslate, i) == number)
                return true;
        }

        return false;
    }

    void Done()
    {
        AudioManager.instance.Menu();
        foreach (ToCustomize customize in toCustomizeList)
        {
            int max = customize.toTranslate == "Rules" ? FightRules.MaxRules: Character.maxAbilities;

            for (int i = 0; i < max; i++)
            {
                int value = i < customize.toSave.Count? customize.toSave[i]: -1;
                PrefManager.SetSaved(customize.toTranslate,i,value);
            }
        }

        PlayerPrefs.Save();
        customizerScreen.gameObject.SetActive(false);
    }
}