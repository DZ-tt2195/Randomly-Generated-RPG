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
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{

#region Variables

    public static TitleScreen instance;
    [ReadOnly] public Transform canvas;

    private string ID = "1x5vKp4X4HPKyRix3w0n9aldY6Dh3B0eBegUM0WtfXFY";
    private string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    [SerializeField] PlayerCharacter playerPrefab;
    [Tooltip("store all players")][ReadOnly] public List<Character> listOfPlayers = new List<Character>();
    [Tooltip("store all ability data")] [ReadOnly] public List<AbilityData> listOfAbilities;
    [Tooltip("store all enters fight data")][ReadOnly] public List<AbilityData> listOfEntersFight;
    [Tooltip("store all enemy data")][ReadOnly] public List<CharacterData> listOfEnemies;
    [Tooltip("store all helper data")][ReadOnly] public List<CharacterData> listOfHelpers;

#endregion

#region Setup

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(GenerateStuff());
    }

    IEnumerator DownloadFile(string range)
    {
        string url = $"{baseUrl}{ID}/values/{range}?key={apiKey}";
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {www.error}");
        }
        else
        {
            string filePath = $"Assets/Resources/File Data/{range}.txt";
            File.WriteAllText($"{filePath}", www.downloadHandler.text);

            string[] allLines = File.ReadAllLines($"{filePath}");
            List<string> modifiedLines = allLines.ToList();
            modifiedLines.RemoveRange(1, 3);
            File.WriteAllLines($"{filePath}", modifiedLines.ToArray());
        }
    }

    IEnumerator GenerateStuff()
    {
        GameObject loadButton = GameObject.Find("Play Button");
        loadButton.SetActive(false);

        #if UNITY_EDITOR
            yield return DownloadFile("Player Data");
            yield return DownloadFile("Helper Data");
            yield return DownloadFile("Enemy Data");
            yield return DownloadFile("Ability Data");
            yield return DownloadFile("Enters Fight Data");
        #endif

        List<CharacterData> players = DataLoader.ReadCharacterData("Player Data");
        listOfHelpers = DataLoader.ReadCharacterData("Helper Data");
        listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        listOfAbilities = DataLoader.ReadAbilityData("Ability Data");
        listOfEntersFight = DataLoader.ReadAbilityData("Enters Fight Data");

        for (int i = 0; i < players.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab);
            yield return (nextCharacter.SetupCharacter(Character.CharacterType.Teammate, players[i], false));
            listOfPlayers.Add(nextCharacter);
            nextCharacter.transform.SetParent(canvas);
            nextCharacter.transform.localPosition = new Vector3(-1000 + (500 * i), -550, 0);
            nextCharacter.transform.SetAsFirstSibling();
        }

        loadButton.SetActive(true);
    }

#endregion

#region Loading Scenes

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canvas = GameObject.Find("Canvas").transform;
        StartCoroutine(BringBackObjects());
    }

    IEnumerator BringBackObjects()
    { 
        yield return new WaitForSeconds(0.25f);

        RightClick.instance.transform.SetParent(canvas);
        RightClick.instance.transform.localPosition = new Vector3(0, 0);

        FPS.instance.transform.SetParent(canvas);
        FPS.instance.transform.localPosition = new Vector3(-1190, 666);
    }

    public void UnloadObjects()
    {
        Preserve(FPS.instance.gameObject);
        Preserve(RightClick.instance.gameObject);
        foreach (Character player in listOfPlayers)
            Preserve(player.gameObject);
    }

    void Preserve(GameObject next)
    {
        next.transform.SetParent(null);
        DontDestroyOnLoad(next);
    }

    #endregion

}
