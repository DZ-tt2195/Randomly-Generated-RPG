using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;

public class ScreenOverlay : MonoBehaviour
{
    public static ScreenOverlay instance;
    [SerializeField] GameObject blackBackground;

    [Foldout("Character display", true)]
        [SerializeField] GameObject characterDisplayBackground;
        [SerializeField] Image characterImage;
        [SerializeField] TMP_Text characterName;
        [SerializeField] TMP_Text characterDescription;
        [SerializeField] Transform weaponStuff;
        [SerializeField] Image weaponImage;
        [SerializeField] TMP_Text weaponName;
        [SerializeField] TMP_Text weaponDescription;
        [SerializeField] TMP_Text emotionText;
        [SerializeField] TMP_Text stats1;
        [SerializeField] TMP_Text stats2;
        [SerializeField] List<AbilityBox> listOfBoxes = new();

    [Foldout("Game settings", true)]
        [SerializeField] GameObject gameSettingsBackground;
        [SerializeField] Button settingsButton;
        [SerializeField] Slider animationSlider;
        [SerializeField] TMP_Text animationText;
        [SerializeField] Toggle undoToggle;

    [Foldout("Emotion triangle", true)]
        [SerializeField] Button emotionButton;
        [SerializeField] GameObject emotionBackground;
        [SerializeField] List<TMP_Text> listOfDescriptions = new();

    private void Awake()
    {
        instance = this;
        settingsButton.onClick.AddListener(SettingsScreen);
        animationSlider.onValueChanged.AddListener(SetAnimationSpeed);
        undoToggle.onValueChanged.AddListener(SetUndo);
    }

    private void Start()
    {
        SetAnimationSpeed(PlayerPrefs.HasKey("Animation Speed") ? PlayerPrefs.GetFloat("Animation Speed") : 1f);
        SetUndo(!PlayerPrefs.HasKey("Confirm Choices") || PlayerPrefs.GetInt("Confirm Choices") == 1);

        emotionButton.onClick.AddListener(SeeEmotions);
        foreach (TMP_Text description in listOfDescriptions)
            description.text = KeywordTooltip.instance.EditText(description.text);
    }

    private void Update()
    {
        if (Input.anyKey && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            blackBackground.SetActive(false);
            characterDisplayBackground.SetActive(false);
            gameSettingsBackground.SetActive(false);
            emotionBackground.SetActive(false);
        }
    }

    public void DisplayCharacterInfo(Character character, string firstStat, string secondStat)
    {
        blackBackground.SetActive(true);
        characterDisplayBackground.transform.SetAsLastSibling();
        characterDisplayBackground.SetActive(true);

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

        emotionText.text = KeywordTooltip.instance.EditText($"{character.currentEmotion}");
        stats1.text = firstStat;
        stats2.text = secondStat;

        int nextBox = 0;
        for (int i = 0; i < character.listOfAbilities.Count; i++)
        {
            Ability nextAbility = character.listOfAbilities[i];
            if (nextAbility.myName != "Skip Turn")
            {
                listOfBoxes[nextBox].gameObject.SetActive(true);
                listOfBoxes[nextBox].ReceiveAbility(nextAbility, character);
                nextBox++;
            }
        }

        for (int i = nextBox; i < listOfBoxes.Count; i++)
        {
            listOfBoxes[i].gameObject.SetActive(false);
        }
    }

    void SettingsScreen()
    {
        gameSettingsBackground.transform.SetAsLastSibling();
        blackBackground.SetActive(true);
        gameSettingsBackground.SetActive(true);
    }

    void SetAnimationSpeed(float value)
    {
        animationSlider.value = value;
        animationText.text = value.ToString("F1");
        PlayerPrefs.SetFloat("Animation Speed", value);
    }

    void SetUndo(bool value)
    {
        undoToggle.isOn = value;
        PlayerPrefs.SetInt("Confirm Choices", value ? 1 : 0);
    }

    void SeeEmotions()
    {
        blackBackground.SetActive(true);
        emotionBackground.SetActive(true);
        emotionBackground.transform.SetAsLastSibling();
    }
}