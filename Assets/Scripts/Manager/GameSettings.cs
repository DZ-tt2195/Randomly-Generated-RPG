using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;
    [SerializeField] GameObject background;
    [SerializeField] Button settingsButton;

    [SerializeField] Slider animationSlider;
    [SerializeField] TMP_Text animationText;
    [SerializeField] Toggle undoToggle;

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
    }

    void SettingsScreen()
    {
        this.transform.SetAsLastSibling();
        background.SetActive(!background.activeSelf);
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
}
