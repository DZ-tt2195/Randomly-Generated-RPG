using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    public static FPS instance;
    TMP_Text fpsText;

    private void Awake()
    {
        instance = this;
        fpsText = this.transform.GetChild(0).GetComponent<TMP_Text>();
        Application.targetFrameRate = 60;
    }

    /*
    const float fpsMeasureTimer = 0.5f;
    private int fpsCountInterval = 0;
    private float fpsMeasureTimerNext = 0;
    private int currentFPS;
    const string display = "{0} FPS";

    private void Start()
    {
        fpsMeasureTimerNext = Time.realtimeSinceStartup + fpsMeasureTimer;
    }

    private void Update()
    {
        fpsCountInterval++;
        if (Time.realtimeSinceStartup > fpsMeasureTimerNext)
        {
            currentFPS = (int)(fpsCountInterval / fpsMeasureTimer);
            fpsCountInterval = 0;
            fpsMeasureTimerNext += fpsMeasureTimer;
            fpsText.text = string.Format(display, currentFPS);
        }
    }
    */
    int lastframe = 0;
    float lastupdate = 60;
    float[] framearray = new float[60];

    private void Update()
    {
        fpsText.text = $"FPS: {CalculateFrames():F0}";
    }

    float CalculateFrames()
    {
        framearray[lastframe] = Time.deltaTime;
        lastframe = (lastframe + 1);
        if (lastframe == 60)
        {
            lastframe = 0;
            float total = 0;
            for (int i = 0; i < framearray.Length; i++)
                total += framearray[i];
            lastupdate = (float)(framearray.Length / total);
            return lastupdate;
        }
        return lastupdate;
    }

}