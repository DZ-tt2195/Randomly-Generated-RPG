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
        string stats1 = "";
        string stats2 = "";

        stats1 += $"Health: {character.CalculateHealth()} / {character.data.baseHealth}\n";
        stats1 += $"Power: {character.CalculatePower()}\n";
        stats1 += $"Defense: {character.modifyDefense}\n";

        stats2 += $"Speed: {character.CalculateSpeed()}\n";
        stats2 += $"Luck: {character.modifyLuck}\n";

        ScreenOverlay.instance.DisplayCharacterInfo(character, KeywordTooltip.instance.EditText(stats1), KeywordTooltip.instance.EditText(stats2));
    }
}
