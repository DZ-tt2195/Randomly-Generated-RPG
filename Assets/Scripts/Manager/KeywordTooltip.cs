using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

[Serializable]
public class KeywordHover
{
    public List<string> keywordVariations;
    [TextArea(5,0)] public string description;
    public Color color = Color.white;
}

public class KeywordTooltip : MonoBehaviour
{
    public static KeywordTooltip instance;
    float XCap;
    float Ydisplace;
    [SerializeField] List<KeywordHover> linkedKeywords = new();
    [SerializeField] List<KeywordHover> spriteKeywords = new();
    [SerializeField] List<KeywordHover> spriteKeywordStatuses = new();
    [SerializeField] TMP_Text tooltipText;

    private void Awake()
    {
        instance = this;
        XCap = tooltipText.rectTransform.sizeDelta.x/2f;
        Ydisplace = tooltipText.rectTransform.sizeDelta.y * 1.25f;
    }

    private void Start()
    {
        foreach (KeywordHover hover in linkedKeywords)
            hover.description = EditText(hover.description);
        foreach (KeywordHover hover in spriteKeywords)
            hover.description = EditText(hover.description);
        foreach (KeywordHover hover in spriteKeywordStatuses)
            hover.description = EditText(hover.description);
    }

    public string EditText(string text)
    {
        string answer = text;
        foreach (KeywordHover link in linkedKeywords)
        {
            foreach (string keyword in link.keywordVariations)
            {
                string pattern = $@"\b{Regex.Escape(keyword)}\b";
                answer = Regex.Replace(answer, pattern, $"<link=\"{keyword}\"><u><color=#{ColorUtility.ToHtmlStringRGB(link.color)}>{keyword}<color=#FFFFFF></u></link>");
            }
        }
        foreach (KeywordHover link in spriteKeywords)
        {
            string toReplace = link.keywordVariations[0];
            answer = answer.Replace(toReplace, $"<link=\"{toReplace}\"><sprite=\"{toReplace}\" name=\"{toReplace}\"></link>");
        }
        foreach (KeywordHover link in spriteKeywordStatuses)
        {
            string toReplace = link.keywordVariations[0];
            answer = answer.Replace(toReplace, $"<link=\"{toReplace}\"><sprite=\"{toReplace}\" name=\"{toReplace}\"></link>");
        }
        return answer;
    }

    public KeywordHover SearchForKeyword(string target)
    {
        foreach (KeywordHover link in linkedKeywords)
        {
            foreach (string keyword in link.keywordVariations)
            {
                if (keyword.Equals(target))
                    return link;
            }
        }
        foreach (KeywordHover link in spriteKeywords)
        {
            if (link.keywordVariations[0].Equals(target))
                return link;
        }
        foreach (KeywordHover link in spriteKeywordStatuses)
        {
            if (link.keywordVariations[0].Equals(target))
                return link;
        }
        Debug.LogError($"{target} couldn't be found");
        return null;
    }

    private void Update()
    {
        tooltipText.transform.parent.gameObject.SetActive(false);
    }

    Vector3 CalculatePosition(Vector3 mousePosition)
    {
        return new Vector3
            (Mathf.Clamp(mousePosition.x, XCap, Screen.width-XCap),
            mousePosition.y + (mousePosition.y > Ydisplace ? -0.5f : 0.5f) * Ydisplace,
            0);
    }

    public void ActivateTextBox(string target, Vector3 mousePosition)
    {
        this.transform.SetAsLastSibling();

        foreach (KeywordHover entry in linkedKeywords)
        {
            foreach (string keyword in entry.keywordVariations)
            {
                if (keyword.Equals(target))
                {
                    tooltipText.text = entry.description;
                    tooltipText.transform.parent.position = CalculatePosition(mousePosition);
                    tooltipText.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
        }
        foreach (KeywordHover entry in spriteKeywords)
        {
            if (entry.keywordVariations[0].Equals(target))
            {
                tooltipText.text = entry.description;
                tooltipText.transform.parent.position = CalculatePosition(mousePosition);
                tooltipText.transform.parent.gameObject.SetActive(true);
                return;
            }
        }
        foreach (KeywordHover entry in spriteKeywordStatuses)
        {
            if (entry.keywordVariations[0].Equals(target))
            {
                tooltipText.text = entry.description;
                tooltipText.transform.parent.position = CalculatePosition(mousePosition);
                tooltipText.transform.parent.gameObject.SetActive(true);
                return;
            }
        }
    }
}
