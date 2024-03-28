using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    Canvas canvas;
    RectTransform rect;
    float XCap;
    float YCap;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rect = GetComponent<RectTransform>();

        XCap = (Screen.width - rect.sizeDelta.x)/2;
        YCap = (Screen.height - rect.sizeDelta.y) / 2;
    }

    public void DragHangler(BaseEventData data)
    {
        PointerEventData pointer = (PointerEventData)data;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        ((RectTransform)canvas.transform, pointer.position, canvas.worldCamera, out Vector2 position);

        transform.position = canvas.transform.TransformPoint(position);
    }

    private void Update()
    {
        this.transform.localPosition = new Vector3(
            Mathf.Clamp(transform.localPosition.x, -XCap, XCap),
            Mathf.Clamp(transform.localPosition.y, -YCap, YCap),
            0);
    }
}
