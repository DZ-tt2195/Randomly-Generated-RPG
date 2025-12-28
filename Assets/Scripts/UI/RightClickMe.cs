using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyBox;

public class RightClickMe : MonoBehaviour, IPointerClickHandler
{
    [ReadOnly] public Character character;
    [ReadOnly] public Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (character == null)
                character = GetComponent<Character>();
            RightClickInfo();
        }
    }

    void RightClickInfo()
    {
        string statText = "";

        statText += $"{character.currentHealth} / {character.data.baseHealth} {AutoTranslate.DoEnum(ToTranslate.Health)}, {character.CalculatePower()} {AutoTranslate.DoEnum(ToTranslate.Power)}, {character.CalculateDefense()} {AutoTranslate.DoEnum(ToTranslate.Defense)}\n";
        statText += $"{Translator.inst.Translate(character.data.startPosition.ToString())}";

        if (ScreenOverlay.instance != null)
            ScreenOverlay.instance.DisplayCharacterInfo(character, KeywordTooltip.instance.EditText(statText));
    }
}
