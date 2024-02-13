using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using System.Drawing;

[Serializable]
public class KeywordHover
{
    public string keyword;
    public string description;
    public Color color = Color.white;
}

public static class TextSubstitute
{
    public static string neutralText = $"<color=#FFFFFF>Neutral</color>";
    public static string deadText = $"<color=#D3D3D3>Dead</color>";

    public static string happyText = $"<color=#00FF00>Happy</color>";
    public static string ecstaticText = $"<color=#00FF00>Ecstatic</color>";

    public static string angryText = $"<color=#FF4C4C>Angry</color>";
    public static string enragedText = $"<color=#FF4C4C>Enraged</color>";

    public static string sadText = $"<color=#9999FF>Sad</color>";
    public static string depressedText = $"<color=#9999FF>Depressed</color>";
}

public class KeywordTooltip : MonoBehaviour
{
    public static KeywordTooltip instance;
    [SerializeField] List<KeywordHover> linkedKeywords = new();
    [SerializeField] List<KeywordHover> spriteKeywords = new();
    [SerializeField] TMP_Text tooltipText;

    private void Awake()
    {
        instance = this;
    }

    public string EditText(string text)
    {
        string answer = text;
        foreach (KeywordHover link in linkedKeywords)
            answer = answer.Replace(link.keyword, $"<link=\"{link.keyword}\"><u><color=#{ColorUtility.ToHtmlStringRGB(link.color)}><b>{link.keyword}</b><color=#FFFFFF></u></link>");
        foreach (KeywordHover link in spriteKeywords)
            answer = answer.Replace(link.keyword, $"<link=\"{link.keyword}\"><sprite=\"Symbols\"name=\"{link.keyword}\"></link>");

        return answer;
    }

    private void Update()
    {
        tooltipText.transform.parent.gameObject.SetActive(false);
    }

    public void ActivateTextBox(string keyword, Vector3 position)
    {
        tooltipText.transform.parent.gameObject.SetActive(true);
        this.transform.SetAsLastSibling();

        foreach (KeywordHover entry in linkedKeywords)
        {
            if (entry.keyword == keyword)
            {
                tooltipText.transform.parent.position = position + new Vector3(0, -150, 0);
                tooltipText.text = entry.description;
            }
        }
    }
}
