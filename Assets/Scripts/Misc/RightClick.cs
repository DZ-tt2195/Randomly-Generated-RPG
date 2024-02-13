using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static Character;

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

    TMP_Text emotionText;
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
        emotionText = statsStuff.transform.Find("Emotion").GetComponent<TMP_Text>();
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

    public void DisplayInfo(Character character, string firstStat, string secondStat)
    {
        this.transform.SetAsLastSibling();
        background.SetActive(true);

        characterImage.sprite = character.myImage.sprite;
        characterName.text = character.name;
        characterDescription.text = character.description;

        if (character.weapon == null)
        {
            weaponStuff.gameObject.SetActive(false);
        }
        else
        {
            weaponStuff.gameObject.SetActive(true);
            weaponName.text = character.weapon.myName;
            weaponImage.sprite = character.weaponImage.sprite;
            weaponDescription.text = character.weapon.description;
        }

        switch (character.currentEmotion)
        {
            case Emotion.Neutral:
                emotionText.text = TextSubstitute.neutralText;
                break;
            case Emotion.Happy:
                emotionText.text = TextSubstitute.happyText;
                break;
            case Emotion.Ecstatic:
                emotionText.text = TextSubstitute.ecstaticText;
                break;
            case Emotion.Angry:
                emotionText.text = TextSubstitute.angryText;
                break;
            case Emotion.Enraged:
                emotionText.text = TextSubstitute.enragedText;
                break;
            case Emotion.Sad:
                emotionText.text = TextSubstitute.sadText;
                break;
            case Emotion.Depressed:
                emotionText.text = TextSubstitute.depressedText;
                break;
            case Emotion.Dead:
                emotionText.text = TextSubstitute.deadText;
                break;
        }

        stats1.text = firstStat;
        stats2.text = secondStat;

        int nextBox = 0;
        for (int i = 0; i<character.listOfAbilities.Count; i++)
        {
            Ability nextAbility = character.listOfAbilities[i];
            if (nextAbility.myName != "Skip Turn" && nextAbility.myName != "Retreat")
            {
                listOfBoxes[nextBox].gameObject.SetActive(true);
                listOfBoxes[nextBox].ReceiveAbility(nextAbility, character);
                nextBox++;
            }
        }

        for (int i = nextBox; i<listOfBoxes.Count; i++)
        {
            listOfBoxes[i].gameObject.SetActive(false);
        }
    }
}
