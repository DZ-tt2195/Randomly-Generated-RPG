using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image background;
    public TMP_Text description;

    private void Start()
    {
        background.gameObject.SetActive(false);
    }

    public void NewDescription(string description)
    {
        this.description.text = description;
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
