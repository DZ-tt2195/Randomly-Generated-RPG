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
    public void AddText(string logText, int indent = 0)
    {
        if (indent < 0)
            return;

        string targetText = "";
        for (int i = 0; i < indent; i++)
            targetText += "     ";
        targetText += logText;
        allText.text += KeywordTooltip.instance.EditText(targetText) + "\n";

        if (scroll.value <= 0.2f)
            Invoke(nameof(ScrollDown), 0.1f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(allText.rectTransform);
    }
    void ScrollDown()
    {
        scroll.value = 0;
    }
}
