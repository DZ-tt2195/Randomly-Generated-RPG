using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
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
    public static string neutralText = $"<link=\"Neutral\"><u><color=#FFFFFF><b>Neutral</b><color=#FFFFFF></u></link>";
    public static string deadText = $"<color=#D3D3D3>Dead</color>";

    public static string happyText = $"<link=\"Happy\"><u><color=#00FF00><b>Happy</b><color=#FFFFFF></u></link>";
    public static string ecstaticText = $"<link=\"Ecstatic\"><u><color=#00FF00><b>Ecstatic</b><color=#FFFFFF></u></link>";

    public static string angryText = $"<link=\"Angry\"><u><color=#FF4C4C><b>Angry</b><color=#FFFFFF></u></link>";
    public static string enragedText = $"<link=\"Enraged\"><u><color=#FF4C4C><b>Enraged</b><color=#FFFFFF></u></link>";

    public static string sadText = $"<link=\"Sad\"><u><color=#9999FF><b>Sad</b><color=#FFFFFF></u></link>";
    public static string depressedText = $"<link=\"Depressed\"><u><color=#9999FF><b>Depressed</b><color=#FFFFFF></u></link>";
}

public class KeywordTooltip : MonoBehaviour
{
    public static KeywordTooltip instance;
    float displace;
    [SerializeField] List<KeywordHover> linkedKeywords = new();
    [SerializeField] List<KeywordHover> spriteKeywords = new();
    [SerializeField] TMP_Text tooltipText;

    private void Awake()
    {
        instance = this;
        displace = tooltipText.rectTransform.sizeDelta.y * 1.25f;
    }

    public string EditText(string text)
    {
        string answer = text;
        foreach (KeywordHover link in linkedKeywords)
        {
            string pattern = $@"\b{Regex.Escape(link.keyword)}\b";
            answer = Regex.Replace(answer, pattern, $"<link=\"{link.keyword}\"><u><color=#{ColorUtility.ToHtmlStringRGB(link.color)}><b>{link.keyword}</b><color=#FFFFFF></u></link>");
        }
        foreach (KeywordHover link in spriteKeywords)
        {
            answer = answer.Replace(link.keyword, $"<link=\"{link.keyword}\"><sprite=\"Symbols\"name=\"{link.keyword}\"></link>");
        }
        return answer;
    }

    private void Update()
    {
        tooltipText.transform.parent.gameObject.SetActive(false);
    }

    public void ActivateTextBox(string keyword, Vector3 mousePosition)
    {
        tooltipText.transform.parent.gameObject.SetActive(true);
        this.transform.SetAsLastSibling();

        foreach (KeywordHover entry in linkedKeywords)
        {
            if (entry.keyword == keyword)
            {
                tooltipText.text = entry.description;
                tooltipText.transform.parent.position = mousePosition + (mousePosition.y > (displace)
                    ? new Vector3(0, -0.5f*displace, 0)
                    : new Vector3(0, 0.5f*displace, 0));
            }
        }
    }
}
