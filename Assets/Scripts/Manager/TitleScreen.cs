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
using static UnityEditor.Experimental.GraphView.GraphView;

public class TitleScreen : MonoBehaviour
{
    public static TitleScreen instance;
    Transform canvas;
    private string ID = "1x5vKp4X4HPKyRix3w0n9aldY6Dh3B0eBegUM0WtfXFY";
    private string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";


    [SerializeField] PlayerCharacter playerPrefab;
    [Tooltip("0 = Knight, 1 = Angel, 2 = Wizard")] [SerializeField] List<Sprite> playerSprites;

    [Tooltip("store all players")][ReadOnly] public List<Character> listOfPlayers = new List<Character>();
    [Tooltip("store all abilities")] [ReadOnly] public List<AbilityData> listOfAbilities;
    [Tooltip("store all enemies")][ReadOnly] public List<CharacterData> listOfEnemies;
    [Tooltip("store all helpers")][ReadOnly] public List<CharacterData> listOfHelpers;

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
            yield return DownloadFile("Ability Data");
            yield return DownloadFile("Helper Data");
            yield return DownloadFile("Enemy Data");
        #endif

        List<CharacterData> players = DataLoader.ReadCharacterData("Player Data");
        listOfHelpers = DataLoader.ReadCharacterData("Helper Data");
        listOfEnemies = DataLoader.ReadCharacterData("Enemy Data");
        listOfAbilities = DataLoader.ReadAbilityData("Ability Data");

        for (int i = 0; i < players.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab);
            nextCharacter.SetupCharacter(players[i]);
            nextCharacter.image.sprite = playerSprites[i];

            listOfPlayers.Add(nextCharacter);
            nextCharacter.transform.SetParent(canvas);
            nextCharacter.transform.localPosition = new Vector3(-750 + (750 * i), 0, 0);
            nextCharacter.transform.SetAsFirstSibling();
        }

    }

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
        yield return new WaitForSeconds(0.5f);

        RightClick.instance.transform.SetParent(canvas);
        RightClick.instance.transform.localPosition = new Vector3(0, 0);

        FPS.instance.transform.SetParent(canvas);
        FPS.instance.transform.localPosition = new Vector3(-1200, 666);

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
}
