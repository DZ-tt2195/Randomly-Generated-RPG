using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;
public class RulesText : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [ReadOnly] public string thisRule;
    public Toggle toggle;

    public void AssignRule(string rule)
    {
        thisRule = rule;
        nameText.text = Translator.inst.Translate(rule);
        descriptionText.text = KeywordTooltip.instance.EditText(Translator.inst.Translate($"{rule}_Text"));
    }
}
