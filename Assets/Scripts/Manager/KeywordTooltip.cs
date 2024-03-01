using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

[Serializable]
public class KeywordHover
{
    public string keyword;
    public string description;
    public Color color = Color.white;
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
            answer = Regex.Replace(answer, pattern, $"<link=\"{link.keyword}\"><u><color=#{ColorUtility.ToHtmlStringRGB(link.color)}>{link.keyword}<color=#FFFFFF></u></link>");
        }
        foreach (KeywordHover link in spriteKeywords)
        {
            answer = answer.Replace(link.keyword, $"<link=\"{link.keyword}\"><sprite=\"Symbols\"name=\"{link.keyword}\"></link>");
        }
        return answer;
    }

    public KeywordHover SearchForKeyword(string keyword)
    {
        foreach (KeywordHover link in linkedKeywords)
        {
            if (link.keyword == keyword)
                return link;
        }
        foreach (KeywordHover link in spriteKeywords)
        {
            if (link.keyword == keyword)
                return link;
        }
        Debug.LogError($"{keyword} couldn't be found");
        return null;
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
                return;
            }
        }
        foreach (KeywordHover entry in spriteKeywords)
        {
            if (entry.keyword == keyword)
            {
                tooltipText.text = entry.description;
                tooltipText.transform.parent.position = mousePosition + (mousePosition.y > (displace)
                    ? new Vector3(0, -0.5f * displace, 0)
                    : new Vector3(0, 0.5f * displace, 0));
                return;
            }
        }
    }
}
