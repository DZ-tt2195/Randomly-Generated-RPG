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
    [SerializeField] GameObject playerPrefab;

#endregion

#region Setup

    void Start()
    {
        StartCoroutine(GenerateStuff());
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
            yield return FileManager.instance.DownloadFile("Other Ability Data");
            yield return FileManager.instance.DownloadFile("Weapon Data");
        #endif

        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        FileManager.instance.listOfHelpers = DataLoader.ReadCharacterData("Helper Data");
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

            yield return nextCharacter.SetupCharacter(Character.CharacterType.Player, playerData[i], false, randomWeapon);
            FileManager.instance.listOfPlayers.Add(nextCharacter);

            nextCharacter.transform.SetParent(FileManager.instance.canvas);
            nextCharacter.transform.localPosition = new Vector3(-1050 + (350 * i), -550, 0);
            nextCharacter.transform.SetAsFirstSibling();
        }

        loadButton.SetActive(true);
    }

#endregion

}
