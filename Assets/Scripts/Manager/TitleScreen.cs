using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using System.Linq;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    private string ID = "1x5vKp4X4HPKyRix3w0n9aldY6Dh3B0eBegUM0WtfXFY";
    private string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    [SerializeField] PlayerCharacter playerPrefab;
    [Tooltip("0 = Knight, 1 = Angel, 2 = Wizard")] [SerializeField] List<Sprite> playerSprites;

    void Start()
    {
        StartCoroutine(GenerateStuff());
    }

    IEnumerator DownloadFile(string range)
    {
        Debug.Log("trying to download");
        string url = $"{baseUrl}{ID}/values/{range}?key={apiKey}";
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {www.error}");
        }
        else
        {
            string filePath = $"Assets/Resources/{range}.txt";
            File.WriteAllText($"{filePath}", www.downloadHandler.text);
            Debug.Log($"downloaded {range} from the internet");

            string[] allLines = File.ReadAllLines($"{filePath}");
            List<string> modifiedLines = allLines.ToList();
            modifiedLines.RemoveRange(1, 3);
            File.WriteAllLines($"{filePath}", modifiedLines.ToArray());
        }
    }

    IEnumerator GenerateStuff()
    {
        #if UNITY_EDITOR
            yield return DownloadFile("Player Data");
        #endif

        /*
        List<CharacterData> players = DataLoader.ReadCharacterData("Player Data");
        //enemies = DataLoader.ReadEnemyData(enemyFile);
        //abilities = DataLoader.ReadAbilityData(abilityFile);

        for (int i = 0; i < players.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab);
            nextCharacter.SetupCharacter(players[i]);
            nextCharacter.image.sprite = playerSprites[i];
        }
        */
    }
}
