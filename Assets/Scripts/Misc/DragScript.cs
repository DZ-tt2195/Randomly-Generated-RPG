using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragScript : MonoBehaviour
{
    public Canvas canvas;

    public void DragHangler(BaseEventData data)
    {
        PointerEventData pointer = (PointerEventData)data;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        ((RectTransform)canvas.transform, pointer.position, canvas.worldCamera, out position);

        transform.position = canvas.transform.TransformPoint(position);
    }

    private void Update()
    {
        if (this.transform.localPosition.x < -800)
            this.transform.localPosition = new Vector3(-800, transform.localPosition.y, 0);
        else if (this.transform.localPosition.x > 800)
            this.transform.localPosition = new Vector3(800, transform.localPosition.y, 0);

        if (this.transform.localPosition.y < -625)
            this.transform.localPosition = new Vector3(transform.localPosition.x, -625, 0);
        else if (this.transform.localPosition.y > 625)
            this.transform.localPosition = new Vector3(transform.localPosition.x, 625, 0);
    }
}
