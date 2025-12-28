using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;
using System.Text.RegularExpressions;
using System;
using System.Reflection;

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

    public static string AbilityLogged(Ability ability, Character user, Character target)
    {
        try
        {
            MethodInfo method = typeof(AutoTranslate).GetMethod($"{ability.data.abilityName}_Log", BindingFlags.Static | BindingFlags.Public);
            ParameterInfo[] parameters = method.GetParameters();
            object[] args = new object[parameters.Length];

            for (int i = 0; i<parameters.Length; i++)
            {
                switch (parameters[i].Name)
                {
                    case "This":
                        args[i] = user.name; 
                        break;
                    case "Target":
                        args[i] = (target == null) ? "" : target.name; 
                        break;
                }
            }

            object result = method.Invoke(null, args);
            return (string)result;
        }
        catch
        {
            return Translator.inst.Translate($"{ability.data.abilityName}_Log");
        }
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
