using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class TextButtonInfo
{
    public string myText{get; private set;}
    public Color buttonColor{get; private set;}
    public Action action{get; private set;}

    public TextButtonInfo(string myText, Action action = null)
    {
        this.myText = myText;
        this.buttonColor = Color.white;
        this.action = action;
    }

    public TextButtonInfo(string myText, Color color, Action action = null)
    {
        this.myText = myText;
        this.action = action;
        this.buttonColor = color;
    }
}
public class MakeDecision : MonoBehaviour
{
    public static MakeDecision inst;
    [SerializeField] List<AbilityBox> listOfBoxes = new();
    [SerializeField] TMP_Text instructions;
    [SerializeField] Image centerUI;
    [SerializeField] List<Button> listOfTextButtons = new();

    void Awake()
    {
        inst = this;
        BlankUI();
    }
    public void SetInstruction(string header)
    {
        instructions.text = KeywordTooltip.instance.EditText(header);
        centerUI.gameObject.SetActive(true);        
    }
    public void BlankUI()
    {
        SetCharacters("", new(), null);
        SetAbilities("", new(), null);
        SetTextButtons("", new());
        centerUI.gameObject.SetActive(false);
    }
    public void SetAbilities(string header, List<Ability> abilities, Action<Ability> action)
    {
        SetInstruction(header);
        for (int i = 0; i<listOfBoxes.Count; i++)
        {
            int buttonNum = i;
            AbilityBox box = listOfBoxes[buttonNum];
            box.gameObject.SetActive(abilities.Count >= 1);
            box.button.onClick.RemoveAllListeners();

            if (buttonNum < abilities.Count)
            {
                Ability nextAbility = abilities[i];
                box.ReceiveAbility(nextAbility.CanPlay(), nextAbility);
                box.button.onClick.AddListener(() => Resolve(nextAbility));

                void Resolve(Ability chosen)
                {
                    action?.Invoke(nextAbility);
                    BlankUI();
                }
            }
            else
            {
                box.ReceiveAbility(true, null);
            }
        }
    }
    public void SetCharacters(string header, List<Character> characters, Action<Character> action)
    {
        SetInstruction(header);
        foreach (Character character in TurnManager.inst.AllCharacters())
        {
            character.myButton.interactable = false;
            character.myButton.onClick.RemoveAllListeners();
            character.border.gameObject.SetActive(false);
        }
        for (int i = 0; i<characters.Count; i++)
        {
            Character next = characters[i];
            next.border.gameObject.SetActive(true);
            next.myButton.interactable = true;
            next.myButton.onClick.AddListener(() => Resolve(next));

            void Resolve(Character chosen)
            {
                action?.Invoke(chosen);
                BlankUI();
            }
        }        
    }
    public void SetTextButtons(string header, List<TextButtonInfo> allInfo)
    {
        SetInstruction(header);
        for (int i = 0; i<listOfTextButtons.Count; i++)
        {
            int buttonNum = i;
            Button button = listOfTextButtons[i];
            button.onClick.RemoveAllListeners();

            if (buttonNum < allInfo.Count)
            {
                TextButtonInfo nextInfo = allInfo[i];
                button.gameObject.SetActive(true);

                button.name = nextInfo.myText;
                button.image.color = nextInfo.buttonColor;
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = KeywordTooltip.instance.EditText(nextInfo.myText);
                button.onClick.AddListener(Resolve);

                void Resolve()
                {
                    nextInfo.action?.Invoke();
                    BlankUI();
                }
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}
