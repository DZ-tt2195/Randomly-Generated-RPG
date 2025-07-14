using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;

public enum CurrentScreen { None, Character, Settings, Emotion };
public class ScreenOverlay : MonoBehaviour
{

#region Variables

    public static ScreenOverlay instance;
    public CurrentScreen displayedScreen { get; private set; }

    [SerializeField] GameObject blackBackground;

    [Foldout("Character display", true)]
        [SerializeField] GameObject characterDisplayBackground;
        [SerializeField] Image characterImage;
        [SerializeField] TMP_Text characterName;
        [SerializeField] TMP_Text characterDescription;
        [SerializeField] TMP_Text characterArtCredit;
        [SerializeField] TMP_Text emotionText;
        [SerializeField] TMP_Text stats1;
        [SerializeField] List<AbilityBox> listOfBoxes = new();
        [SerializeField] List<Image> listOfStars = new();

    [Foldout("Game settings", true)]
        [SerializeField] GameObject gameSettingsBackground;
        [SerializeField] Button settingsButton;
        [SerializeField] Slider animationSlider;
        [SerializeField] TMP_Text animationText;
        [SerializeField] Toggle undoToggle;
        [SerializeField] Toggle tooltipToggle;

    [Foldout("Emotion triangle", true)]
        [SerializeField] Button emotionButton;
        [SerializeField] GameObject emotionBackground;

    #endregion

#region Setup

    private void Awake()
    {
        instance = this;
        settingsButton.onClick.AddListener(SettingsScreen);
        animationSlider.onValueChanged.AddListener(SetAnimationSpeed);
        undoToggle.onValueChanged.AddListener(SetUndo);
        tooltipToggle.onValueChanged.AddListener(SetTooltip);
        emotionButton.onClick.AddListener(SeeEmotions);
    }

    private void Start()
    {
        SetAnimationSpeed(PlayerPrefs.HasKey("Animation Speed") ? PlayerPrefs.GetFloat("Animation Speed") : 0.5f);
        SetUndo(!PlayerPrefs.HasKey("Confirm Choices") || PlayerPrefs.GetInt("Confirm Choices") == 1);
        SetTooltip(!PlayerPrefs.HasKey("Keyword Tooltip") || PlayerPrefs.GetInt("Keyword Tooltip") == 1);
        PlayerPrefs.Save();
    }

    void SettingsScreen()
    {
        displayedScreen = CurrentScreen.Settings;
        gameSettingsBackground.transform.SetAsLastSibling();
        blackBackground.SetActive(true);
        gameSettingsBackground.SetActive(true);
    }

    public void SetAnimationSpeed(float value)
    {
        animationSlider.value = value;
        animationText.text = value.ToString("F1");
        PlayerPrefs.SetFloat("Animation Speed", value);
        PlayerPrefs.Save();
    }

    public void SetUndo(bool value)
    {
        undoToggle.isOn = value;
        PlayerPrefs.SetInt("Confirm Choices", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetTooltip(bool value)
    {
        tooltipToggle.isOn = value;
        PlayerPrefs.SetInt("Keyword Tooltip", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    void SeeEmotions()
    {
        displayedScreen = CurrentScreen.Emotion;
        blackBackground.SetActive(true);
        emotionBackground.SetActive(true);
        emotionBackground.transform.SetAsLastSibling();
    }

    #endregion

#region Gameplay

    private void Update()
    {
        this.transform.SetAsLastSibling();
        if ((Input.anyKey && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)) ||
            (!gameSettingsBackground.activeInHierarchy && Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            blackBackground.SetActive(false);
            characterDisplayBackground.SetActive(false);
            gameSettingsBackground.SetActive(false);
            emotionBackground.SetActive(false);
            displayedScreen = CurrentScreen.None;
        }
    }

    public void DisplayCharacterInfo(Character character, string statText)
    {
        displayedScreen = CurrentScreen.Character;
        blackBackground.SetActive(true);
        characterDisplayBackground.transform.SetAsLastSibling();
        characterDisplayBackground.SetActive(true);

        characterImage.sprite = character.myImage.sprite;
        characterName.text = character.name;
        characterDescription.text = character.editedDescription;
        characterArtCredit.text = character.data.artCredit;

        emotionText.text = KeywordTooltip.instance.EditText(CarryVariables.instance.GetText(character.CurrentEmotion.ToString()));
        stats1.text = statText;

        int nextBox = 0;
        for (int i = 0; i < character.listOfRandomAbilities.Count; i++)
        {
            Ability nextAbility = character.listOfRandomAbilities[i];
            listOfBoxes[nextBox].gameObject.SetActive(true);
            listOfBoxes[nextBox].ReceiveAbility(TurnManager.instance != null && nextAbility.CanPlay(), nextAbility);
            nextBox++;
        }
        for (int i = nextBox; i < listOfBoxes.Count; i++)
        {
            listOfBoxes[i].gameObject.SetActive(false);
            //listOfBoxes[i].ReceiveAbility(true, null);
        }

        int nextStar = 0;
        for (int i = 0; i<character.data.difficulty; i++)
        {
            listOfStars[i].gameObject.SetActive(true);
            nextStar++;
        }
        for (int i = nextStar; i<listOfStars.Count; i++)
        {
            listOfStars[i].gameObject.SetActive(false);
        }
    }

    #endregion

}
