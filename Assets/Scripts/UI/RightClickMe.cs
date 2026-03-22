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
        int power = character.CalculatePower();
        int defense = character.CalculateDefense();

        statText += $"{character.currentHealth} / {character.data.baseHealth} {AutoTranslate.Health()}\n";
        statText += (power >= 1) ? $"+{power} {AutoTranslate.Power()}," : $"{power} {AutoTranslate.Power()}, ";
        statText += (defense >= 1) ? $"+{defense} {AutoTranslate.Defense()}\n" : $"{defense} {AutoTranslate.Defense()}\n";
        statText += $"{Translator.inst.Translate(character.data.startPosition.ToString())}";

        if (ScreenOverlay.instance != null)
            ScreenOverlay.instance.DisplayCharacterInfo(character, KeywordTooltip.instance.EditText(statText));
    }
}
