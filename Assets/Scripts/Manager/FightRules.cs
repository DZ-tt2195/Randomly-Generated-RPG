using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TMPro;
using UnityEngine;

public class FightRules : MonoBehaviour
{
    public static FightRules inst;
    public static int MaxRules = 2;
    [SerializeField] GameObject visuals;
    [SerializeField] TMP_Text rules;
    HashSet<RulesData> selectedRules = new();
    public static int totalRules = 2;
    [SerializeField] List<RulesText> rulesOnScreen = new();

    void Awake()
    {
        inst = this;
        visuals.SetActive(ScreenOverlay.instance.mode != GameMode.Tutorial);
        rules.text = AutoTranslate.Rules();
    }
    void Start()
    {
        if (ScreenOverlay.instance.mode == GameMode.Tutorial)
            return;

        List<RulesData> allRules = GameFiles.inst.SavedRules(TurnManager.inst.dailyRNG).ToList();
        for (int i = 0; i<allRules.Count; i++)
        {
            selectedRules.Add(allRules[i]);
            rulesOnScreen[i].AssignRule(allRules[i]);
            Log.instance.AddText(AutoTranslate.Chosen_Rule(Translator.inst.Translate(allRules[i].rulesName)));            
        }
        Log.instance.AddText(AutoTranslate.Blank());
    }
    public RulesData CheckRule(string rule, int logged)
    {
        foreach (RulesData data in selectedRules)
        {
            if (data.rulesName.Equals(rule))
            {
                Log.instance.AddText(AutoTranslate.Apply_Rule(Translator.inst.Translate(rule)), logged);
                return data;                
            }
        }
        return null;
    }
}
