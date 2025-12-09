using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;
using System.Text.RegularExpressions;

public class Log : MonoBehaviour
{    
    public static Log instance;
    Scrollbar scroll;
    float startingHeight;
    [SerializeField] TMP_Text allText;
    private void Awake()
    {
        scroll = this.transform.GetChild(1).GetComponent<Scrollbar>();
        instance = this;
    }

    public static string Substitute(Ability ability, Character user, Character target)
    {
        string sentence = CarryVariables.instance.Translate($"{ability.data.myName} Log", new() { ("This", user.name)});
        if (target != null)
            sentence = sentence.Replace("$Target$", target.name);
        return sentence;
    }

    public void AddText(string logText, int indent = 0)
    {
        if (indent < 0)
            return;

        string targetText = "";
        for (int i = 0; i < indent; i++)
            targetText += "     ";
        targetText += logText;
        allText.text += KeywordTooltip.instance.EditText(targetText) + "\n";

        LayoutRebuilder.ForceRebuildLayoutImmediate(allText.rectTransform);
        Invoke(nameof(ScrollDown), 0.1f);
    }

    void ScrollDown()
    {
        if (scroll.value <= 0.2f)
            scroll.value = 0;
    }
}
