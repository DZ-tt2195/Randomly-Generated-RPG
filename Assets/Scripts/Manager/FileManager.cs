using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;

public class FileManager : MonoBehaviour
{

#region Variables

    public static FileManager instance;
    [ReadOnly] public Transform canvas;
    [SerializeField] bool downloadOn;

    private string ID = "1x5vKp4X4HPKyRix3w0n9aldY6Dh3B0eBegUM0WtfXFY";
    private string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    [Tooltip("store all players")][ReadOnly] public List<Character> listOfPlayers = new List<Character>();
    [Tooltip("store all ability data")][ReadOnly] public List<AbilityData> listOfAbilities;
    [Tooltip("store all other ability data")][ReadOnly] public List<AbilityData> listOfOtherAbilities;
    [Tooltip("store all enemy data")][ReadOnly] public List<CharacterData> listOfEnemies;
    [Tooltip("store all weapon data")][ReadOnly] public List<WeaponData> listOfWeapons;

#endregion

#region Setup

    private void Awake()
    {
        if (instance == null)
        {
            canvas = GameObject.Find("Canvas").transform;
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    internal IEnumerator DownloadFile(string range)
    {
        if (downloadOn)
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
    }

    public AbilityData FindAbility(string name)
    {
        return listOfAbilities.FirstOrDefault(ability => ability.myName == name);
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
        Invoke(nameof(BringBackObjects), 0.2f);
    }

    void BringBackObjects()
    {
        RightClick.instance.transform.SetParent(canvas);
        RightClick.instance.transform.localPosition = new Vector3(0, 0);

        if (FPS.instance != null)
        {
            FPS.instance.transform.SetParent(canvas);
            FPS.instance.transform.localPosition = new Vector3(-1190, 670);
        }

        GameSettings.instance.transform.SetParent(canvas);
        GameSettings.instance.transform.localPosition = new Vector3(0, 0);

        EmotionGuide.instance.transform.SetParent(canvas);
        EmotionGuide.instance.transform.localPosition = new Vector3(0, 0);

        KeywordTooltip.instance.transform.SetParent(canvas);
        KeywordTooltip.instance.transform.localPosition = new Vector3(0, 0);
    }

    public void UnloadObjects(string sceneName)
    {
        if (FPS.instance != null)
            Preserve(FPS.instance.gameObject);

        Preserve(RightClick.instance.gameObject);
        Preserve(GameSettings.instance.gameObject);
        Preserve(EmotionGuide.instance.gameObject);
        Preserve(KeywordTooltip.instance.gameObject);

        listOfPlayers.RemoveAll(item => item == null);
        if (sceneName != "0. Title Screen")
        {
            foreach (Character player in listOfPlayers)
                Preserve(player.gameObject);
        }
    }

    void Preserve(GameObject next)
    {
        next.transform.SetParent(null);
        DontDestroyOnLoad(next);
    }

#endregion

}
