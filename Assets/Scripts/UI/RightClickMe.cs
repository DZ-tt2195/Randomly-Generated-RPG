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

        statText += $"{character.currentHealth} / {character.data.baseHealth} {CarryVariables.instance.Translate("Health")}, {character.CalculatePower()} {CarryVariables.instance.Translate("Power")}, {character.CalculateDefense()} {CarryVariables.instance.Translate("Defense")}\n";
        statText += $"{CarryVariables.instance.Translate(character.data.startingPosition.ToString())}";

        if (ScreenOverlay.instance != null)
            ScreenOverlay.instance.DisplayCharacterInfo(character, KeywordTooltip.instance.EditText(statText));
    }
}
