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
    [SerializeField] List<TMP_Text> listOfDescriptions = new();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        emotionButton.onClick.AddListener(SeeEmotions);
        foreach (TMP_Text description in listOfDescriptions)
            description.text = KeywordTooltip.instance.EditText(description.text);
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
