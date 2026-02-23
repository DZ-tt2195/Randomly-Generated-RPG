using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FightRules : MonoBehaviour
{
    public static FightRules inst;
    [SerializeField] TMP_Text rules;
    HashSet<string> selectedRules = new();
    public static int totalRules = 2;
    [SerializeField] List<RulesText> rulesOnScreen = new();

    void Awake()
    {
        inst = this;
        if (ScreenOverlay.instance.mode == GameMode.Tutorial)
        {
            this.transform.Find("Visuals").gameObject.SetActive(false);
        }
        else
        {
            rules.text = AutoTranslate.Rules();
        }
    }

    public void AssignedRules(HashSet<string> rules)
    {
        selectedRules = rules;
        List<RulesData> allRules = GameFiles.inst.FinishedRules();
        System.Random dailyRNG = TurnManager.inst.dailyRNG;

        while (ScreenOverlay.instance.mode != GameMode.Tutorial && selectedRules.Count < totalRules)
        {
            int randomNumber = (dailyRNG != null) ? dailyRNG.Next(0, allRules.Count) : UnityEngine.Random.Range(0, allRules.Count);
            selectedRules.Add(allRules[randomNumber].rulesName);
        }

        List<string> tempList = selectedRules.ToList();
        for (int i = 0; i<tempList.Count; i++)
        {            
            rulesOnScreen[i].AssignRule(tempList[i]);
            Log.instance.AddText(AutoTranslate.Chosen_Rule(tempList[i]));
        }
    }

    public bool CheckRule(string rule) => selectedRules.Contains(rule);
}
