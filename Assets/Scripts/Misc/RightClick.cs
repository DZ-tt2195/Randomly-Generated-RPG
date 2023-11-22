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
    [SerializeField] List<AbilityBox> listOfBoxes = new List<AbilityBox>();

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

    public void DisplayInfo(Character character)
    {
        background.SetActive(true);
        image.sprite = character.image.sprite;
        nameText.text = character.name;
        for (int i = 0; i<listOfBoxes.Count; i++)
        {
            try
            {
                listOfBoxes[i].ReceiveAbility(character.listOfAbilities[i]);
                listOfBoxes[i].gameObject.SetActive(true);
            }
            catch (ArgumentOutOfRangeException)
            {
                listOfBoxes[i].gameObject.SetActive(false);
            }
        }
    }
}
