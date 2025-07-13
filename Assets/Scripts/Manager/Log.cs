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
    [SerializeField] RectTransform RT;
    GridLayoutGroup gridGroup;
    float startingHeight;
    [SerializeField] TMP_Text textBoxClone;

    private void Awake()
    {
        gridGroup = RT.GetComponent<GridLayoutGroup>();
        startingHeight = RT.sizeDelta.y;
        scroll = this.transform.GetChild(1).GetComponent<Scrollbar>();
        instance = this;
    }

    /*
    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
                AddText($"Test {RT.transform.childCount+1}");
        #endif
    }
    */

    public static string Substitute(Ability ability, Character user, Character target)
    {
        string sentence = CarryVariables.instance.GetText($"{ability.data.myName} Log");
        sentence = sentence.Replace("$This$", user.data.myName);
        try{ sentence = sentence.Replace("$Target", target.data.myName);} catch{/*do nothing*/}
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
            newText.text += "     ";
        newText.text += string.IsNullOrEmpty(logText) ? "" : char.ToUpper(logText[0]) + logText[1..];

        /*
        foreach (Character teammate in TurnManager.instance.listOfPlayers)
        {
            string escapedName = Regex.Escape(teammate.name);
            string pattern = $@"\b{escapedName}(?=\W!$)";
            newText.text = Regex.Replace(newText.text, pattern, $"<color=#00FF00>{teammate.name}</color>");
        }
        foreach (Character enemy in TurnManager.instance.listOfEnemies)
        {
            string escapedName = Regex.Escape(enemy.name);
            string pattern = $@"\b{escapedName}(?=\W!$)";
            newText.text = Regex.Replace(newText.text, pattern, $"<color=#FF0000>{enemy.name}</color>");
        }
        */

        newText.text = KeywordTooltip.instance.EditText(newText.text);

        if (RT.transform.childCount >= (startingHeight / gridGroup.cellSize.y)-1)
        {
            RT.sizeDelta = new Vector2(RT.sizeDelta.x, RT.sizeDelta.y + gridGroup.cellSize.y);

            if (scroll.value <= 0.2f)
            {
                scroll.value = 0;
                RT.transform.localPosition = new Vector3(RT.transform.localPosition.x, RT.transform.localPosition.y + gridGroup.cellSize.y/2, 0);
            }
        }
    }
}
