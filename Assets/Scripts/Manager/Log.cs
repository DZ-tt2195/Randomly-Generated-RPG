using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;

public class Log : MonoBehaviour
{    
    public static Log instance;
    [ReadOnly] Scrollbar scroll;
    [SerializeField] RectTransform RT;
    [SerializeField] TMP_Text textBoxClone;

    private void Awake()
    {
        scroll = this.transform.GetChild(1).GetComponent<Scrollbar>();
        instance = this;
    }

    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
                AddText($"Test {RT.transform.childCount+1}");
        #endif
    }

    public static string Substitute(Ability ability, Character user)
    {
        string sentence = ability.logDescription;
        sentence = sentence.Replace("THIS", user.name);
        try{ sentence = sentence.Replace("TARGET", ability.listOfTargets[0].name);} catch{/*do nothing*/}
        return sentence;
    }

    public static string Article(string followingWord)
    {
        if (followingWord.StartsWith('A')
            || followingWord.StartsWith('E')
            || followingWord.StartsWith('I')
            || followingWord.StartsWith('O')
            || followingWord.StartsWith('U'))
        {
            return $"an {followingWord}";
        }
        else
        {
            return $"a {followingWord}";
        }
    }
    
    public void AddText(string logText, int indent = 0)
    {
        if (indent < 0)
            return;

        TMP_Text newText = Instantiate(textBoxClone, RT.transform);
        newText.text = "";
        for (int i = 0; i < indent; i++)
            newText.text += "        ";
        newText.text += string.IsNullOrEmpty(logText) ? "" : char.ToUpper(logText[0]) + logText[1..];

        foreach (Character teammate in TurnManager.instance.teammates)
            newText.text = newText.text.Replace(teammate.name, $"<color=#00FF00><b>{teammate.name}</b></color>");

        foreach (Character enemy in TurnManager.instance.enemies)
            newText.text = newText.text.Replace(enemy.name, $"<color=#FF0000><b>{enemy.name}</b></color>");

        if (RT.transform.childCount >= 23)
        {
            RT.sizeDelta = new Vector2(640, RT.sizeDelta.y+60);

            if (scroll.value <= 0.2f)
            {
                scroll.value = 0;
                RT.transform.localPosition = new Vector3(-30, RT.transform.localPosition.y + 25, 0);
            }
        }
    }
}
