using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System;

public class TitleScreen : MonoBehaviour
{

#region Variables

    public static TitleScreen instance;
    [ReadOnly] public Transform canvas;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] bool randomSeed;
    [SerializeField][ConditionalField(nameof(randomSeed), inverse: true)] int chosenSeed;

#endregion

#region Setup

    void Start()
    {
        if (randomSeed || !Application.isEditor)
        {
            chosenSeed = (int)DateTime.Now.Ticks;
            Debug.Log($"random seed: {chosenSeed}");
        }
        else
        {
            Debug.Log($"manual seed: {chosenSeed}");
        }
        UnityEngine.Random.InitState(chosenSeed);

        StartCoroutine(GenerateStuff());
    }

    IEnumerator GenerateStuff()
    {
        GameObject loadButton = GameObject.Find("Play Button");
        loadButton.SetActive(false);

        #if UNITY_EDITOR
            yield return FileManager.instance.DownloadFile("Player Data");
            yield return FileManager.instance.DownloadFile("Enemy Data");
            yield return FileManager.instance.DownloadFile("Ability Data");
            yield return FileManager.instance.DownloadFile("Other Ability Data");
            yield return FileManager.instance.DownloadFile("Weapon Data");
        #endif

        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        FileManager.instance.listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        FileManager.instance.listOfAbilities = DataLoader.ReadAbilityData("Ability Data");
        FileManager.instance.listOfOtherAbilities = DataLoader.ReadAbilityData("Other Ability Data");
        FileManager.instance.listOfWeapons = DataLoader.ReadWeaponData("Weapon Data");
        FileManager.instance.listOfWeapons = FileManager.instance.listOfWeapons.Shuffle();

        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab).AddComponent<PlayerCharacter>();
            WeaponData randomWeapon;

            if (FileManager.instance.listOfWeapons.Count == 0) randomWeapon = null;
            else randomWeapon = FileManager.instance.listOfWeapons[i];

            nextCharacter.SetupCharacter(CharacterType.Player, playerData[i], false, randomWeapon);
            FileManager.instance.listOfPlayers.Add(nextCharacter);

            nextCharacter.transform.SetParent(FileManager.instance.canvas);
            nextCharacter.transform.localPosition = new Vector3(-1050 + (350 * i), -550, 0);
            nextCharacter.transform.SetAsFirstSibling();
        }

        loadButton.SetActive(true);
    }

#endregion

}
