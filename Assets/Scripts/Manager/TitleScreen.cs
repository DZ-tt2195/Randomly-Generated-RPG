using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class TitleScreen : MonoBehaviour
{

#region Variables

    public static TitleScreen instance;
    [ReadOnly] public Transform canvas;
    [SerializeField] PlayerCharacter playerPrefab;

    [SerializeField] Slider slider;
    [SerializeField] TMP_Text sliderText;

#endregion

#region Setup

    void Start()
    {
        StartCoroutine(GenerateStuff());
        slider.onValueChanged.AddListener(UpdateText);

        if (PlayerPrefs.HasKey("Wait"))
            UpdateText(PlayerPrefs.GetFloat("Wait"));
        else
            UpdateText(1f);
    }

    IEnumerator GenerateStuff()
    {
        GameObject loadButton = GameObject.Find("Play Button");
        loadButton.SetActive(false);

        #if UNITY_EDITOR
            yield return FileManager.instance.DownloadFile("Player Data");
            yield return FileManager.instance.DownloadFile("Helper Data");
            yield return FileManager.instance.DownloadFile("Enemy Data");
            yield return FileManager.instance.DownloadFile("Ability Data");
            yield return FileManager.instance.DownloadFile("Enters Fight Data");
        #endif

        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        FileManager.instance.listOfHelpers = DataLoader.ReadCharacterData("Helper Data");
        FileManager.instance.listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        FileManager.instance.listOfAbilities = DataLoader.ReadAbilityData("Ability Data");
        FileManager.instance.listOfEntersFight = DataLoader.ReadAbilityData("Enters Fight Data");

        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab);
            yield return (nextCharacter.SetupCharacter(Character.CharacterType.Teammate, playerData[i], false));
            FileManager.instance.listOfPlayers.Add(nextCharacter);

            nextCharacter.transform.SetParent(FileManager.instance.canvas);
            nextCharacter.transform.localPosition = new Vector3(-1000 + (500 * i), -550, 0);
            nextCharacter.transform.SetAsFirstSibling();
        }

        loadButton.SetActive(true);
    }

    void UpdateText(float value)
    {
        slider.value = value;
        sliderText.text = value.ToString("F1");
        PlayerPrefs.SetFloat("Wait", value);
    }

#endregion

}
