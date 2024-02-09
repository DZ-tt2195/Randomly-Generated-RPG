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

    private void Awake()
    {
        instance = this;
        settingsButton.onClick.AddListener(SettingsScreen);
        animationSlider.onValueChanged.AddListener(SetAnimationSpeed);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Animation Speed"))
            SetAnimationSpeed(PlayerPrefs.GetFloat("Animation Speed"));
        else
            SetAnimationSpeed(1f);
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
}
