using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RightClick : MonoBehaviour
{
    public static RightClick instance;
    GameObject background;

    Image characterImage;
    TMP_Text characterName;
    TMP_Text characterDescription;

    Transform weaponStuff;
    Image weaponImage;
    TMP_Text weaponName;
    TMP_Text weaponDescription;

    TMP_Text emotion;
    TMP_Text stats1;
    TMP_Text stats2;

    List<AbilityBox> listOfBoxes = new();

    private void Awake()
    {
        instance = this;

        background = transform.Find("Background").gameObject;
        background.SetActive(false);

        Transform characterStuff = background.transform.Find("Character Stuff");
        characterImage = characterStuff.transform.Find("Character Image").GetComponent<Image>();
        characterName = characterStuff.transform.Find("Character Name").GetComponent<TMP_Text>();
        characterDescription = characterStuff.transform.Find("Character Description").GetComponent<TMP_Text>();

        weaponStuff = background.transform.Find("Weapon Stuff");
        weaponImage = weaponStuff.transform.Find("Weapon Image").GetComponent<Image>();
        weaponName = weaponStuff.transform.Find("Weapon Name").GetComponent<TMP_Text>();
        weaponDescription = weaponStuff.transform.Find("Weapon Description").GetComponent<TMP_Text>();

        Transform statsStuff = background.transform.Find("Stats Stuff");
        emotion = statsStuff.transform.Find("Emotion").GetComponent<TMP_Text>();
        stats1 = statsStuff.transform.Find("Stats Part 1").GetComponent<TMP_Text>();
        stats2 = statsStuff.transform.Find("Stats Part 2").GetComponent<TMP_Text>();

        Transform abilityStuff = background.transform.Find("Ability Stuff");
        foreach (Transform child in abilityStuff)
            listOfBoxes.Add(child.GetComponent<AbilityBox>());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            this.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void DisplayInfo(Character character, Sprite sprite, string firstStat, string secondStat)
    {
        this.transform.SetAsLastSibling();
        background.SetActive(true);

        characterImage.sprite = sprite;
        characterName.text = character.name;
        characterDescription.text = character.description;

        if (character.weapon == null)
        {
            weaponStuff.gameObject.SetActive(false);
        }
        else
        {
            weaponStuff.gameObject.SetActive(true);
        }

        emotion.text = character.currentEmotion.ToString();
        stats1.text = firstStat;
        stats2.text = secondStat;

        for (int i = 0; i<listOfBoxes.Count; i++)
        {
            try
            {
                listOfBoxes[i].ReceiveAbility(character.listOfAbilities[i+1], character);
                listOfBoxes[i].gameObject.SetActive(true);
            }
            catch (ArgumentOutOfRangeException)
            {
                listOfBoxes[i].gameObject.SetActive(false);
            }
        }
    }
}
