using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class EmotionGuide : MonoBehaviour
{
    public static EmotionGuide instance;
    [SerializeField] Button emotionButton;
    [SerializeField] GameObject emotionTransform;

    private void Awake()
    {
        instance = this;
        emotionButton.onClick.AddListener(SeeEmotions);
    }

    void SeeEmotions()
    {
        emotionTransform.SetActive(true);
        this.transform.SetAsLastSibling();
    }

    private void Update()
    {
        if (emotionTransform.activeSelf && Input.GetMouseButtonDown(0))
        {
            emotionTransform.SetActive(false);
        }
    }
}
