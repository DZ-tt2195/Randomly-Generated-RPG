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

        statText += $"{character.currentHealth} / {character.data.baseHealth} Health, {character.CalculatePower()} Power, {character.statModDict[Stats.Defense]} Defense\n";
        statText += $"{character.CalculateSpeed()} Speed, {character.statModDict[Stats.Luck]} Luck, {CarryVariables.instance.Translate(character.data.startingPosition.ToString())}";

        if (ScreenOverlay.instance != null)
            ScreenOverlay.instance.DisplayCharacterInfo(character, KeywordTooltip.instance.EditText(statText));
    }
}
