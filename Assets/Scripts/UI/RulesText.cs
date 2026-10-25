using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;
public class RulesText : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [ReadOnly] public RulesData thisRule;
    public Toggle toggle;

    public void AssignRule(RulesData rule)
    {
        thisRule = rule;
        nameText.text = Translator.inst.Translate(rule.rulesName);
        descriptionText.text = Ability.TranslatedDescription(rule.rulesName, rule.mainNumber, rule.secondNumber, rule.powerStat, rule.defenseStat, rule.statusNumber, rule.miscNumber);
    }
}
