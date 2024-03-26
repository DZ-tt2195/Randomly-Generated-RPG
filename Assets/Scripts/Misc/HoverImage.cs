using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image background;
    [SerializeField] TMP_Text descriptionTextBox;

    private void Start()
    {
        background.gameObject.SetActive(false);
    }

    public void NewDescription(string description)
    {
        this.descriptionTextBox.text = description;
        background.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
    }
}