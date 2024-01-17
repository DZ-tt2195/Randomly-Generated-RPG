using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RightClick : MonoBehaviour
{
    public static RightClick instance;
    [SerializeField] GameObject background;

    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text emotionText;
    [SerializeField] List<AbilityBox> listOfBoxes = new();

    private void Awake()
    {
        instance = this;
        background.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            this.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void DisplayInfo(Character character, Sprite sprite)
    {
        background.SetActive(true);
        image.sprite = sprite;
        nameText.text = character.name;
        for (int i = 0; i<listOfBoxes.Count; i++)
        {
            try
            {
                listOfBoxes[i].ReceiveAbility(character.listOfAbilities[i+1]);
                listOfBoxes[i].gameObject.SetActive(true);
            }
            catch (ArgumentOutOfRangeException)
            {
                listOfBoxes[i].gameObject.SetActive(false);
            }
        }

        switch (character.currentEmotion)
        {
            case Character.Emotion.Dead:
                emotionText.color = Color.white;
                emotionText.text = "";
                break;
            case Character.Emotion.Neutral:
                emotionText.color = Color.white;
                emotionText.text = "NEUTRAL";
                break;
            case Character.Emotion.Happy:
                emotionText.color = Color.yellow;
                emotionText.text = "HAPPY: 15% more luck and speed; abilities used have 1 extra turn of cooldown. Becomes Ecstatic when stacked.";
                break;
            case Character.Emotion.Ecstatic:
                emotionText.color = Color.yellow;
                emotionText.text = "ECSTATIC: 30% more luck and speed; abilities used have 2 extra turns of cooldown.";
                break;
            case Character.Emotion.Angry:
                emotionText.color = Color.red;
                emotionText.text = "ANGRY: 15% more attack; lose 5% health at end of turn. Becomes Enraged when stacked.";
                break;
            case Character.Emotion.Enraged:
                emotionText.color = Color.red;
                emotionText.text = "ENRAGED: 30% more attack; lose 10% health at end of turn.";
                break;
            case Character.Emotion.Sad:
                emotionText.color = Color.blue;
                emotionText.text = "SAD: 15% more defense against non-Angry attacks; 10% less accuracy. Becomes Depressed when stacked.";
                break;
            case Character.Emotion.Depressed:
                emotionText.color = Color.blue;
                emotionText.text = "DEPRESSED: 30% more defense against non-Angry attacks; 20% less accuracy.";
                break;
        }
    }
}
