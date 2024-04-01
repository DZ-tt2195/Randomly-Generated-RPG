using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditCanvas : MonoBehaviour
{
    CanvasScaler scaler;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        float aspectRatio = (float)Screen.width / Screen.height;
        scaler.referenceResolution = new Vector2(scaler.referenceResolution.y * aspectRatio, scaler.referenceResolution.y);
    }
}
