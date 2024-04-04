using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{

#region Variables

    public static FileManager instance;
    public enum GameMode { Main, Tutorial, Other };

    [Foldout("Misc info", true)]
        public GameMode mode;
        [ReadOnly] public Transform canvas;
        [SerializeField] Canvas permanentCanvas;

    [Foldout("Scene transition", true)]
        [SerializeField] Image transitionImage;
        [SerializeField] float transitionTime;

    [Foldout("Download", true)]
        public bool downloadOn = true;
        private string ID = "1x5vKp4X4HPKyRix3w0n9aldY6Dh3B0eBegUM0WtfXFY";
        private string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
        private string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

        [Tooltip("store all players")][ReadOnly] public List<Character> listOfPlayers = new List<Character>();
        [Tooltip("store all player ability data")][ReadOnly] public List<AbilityData> listOfPlayerAbilities;
        [Tooltip("store all enemy ability data")][ReadOnly] public List<AbilityData> listOfEnemyAbilities;
        [Tooltip("store all enemy data")][ReadOnly] public List<CharacterData> listOfEnemies;
        [Tooltip("store all bonus enemy data")][ReadOnly] public List<CharacterData> listOfBonusEnemies;

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

    private void Start()
    {
        permanentCanvas.gameObject.SetActive(true);
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
                Debug.LogError($"Download failed: {www.error}");
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

    #endregion

#region Helper Methods

    public List<AbilityData> ConvertNumbersToAbilityData(string list, bool player)
    {
        string[] divideIntoNumbers = list.Split(',');
        List<AbilityData> splitAbilities = new();
        for (int j = 0; j < divideIntoNumbers.Length-1; j++)
        {
            try
            {
                string skillNumber = divideIntoNumbers[j];
                skillNumber.Trim();

                if (player)
                    splitAbilities.Add(listOfPlayerAbilities[int.Parse(skillNumber)]);
                else
                    splitAbilities.Add(listOfEnemyAbilities[int.Parse(skillNumber)]);
            }
            catch (FormatException) { continue; }
            catch (ArgumentOutOfRangeException) { break; }
        }
        return splitAbilities;
    }

    public AbilityData FindPlayerAbility(string target)
    {
        return listOfPlayerAbilities.FirstOrDefault(ability => ability.myName == target);
    }

    public AbilityData FindEnemyAbility(string target)
    {
        return listOfEnemyAbilities.FirstOrDefault(ability => ability.myName == target);
    }

    public CharacterData FindEnemy(string target)
    {
        return listOfEnemies.FirstOrDefault(enemy => enemy.myName == target);
    }

    public CharacterData FindBonusEnemy(string target)
    {
        return listOfBonusEnemies.FirstOrDefault(enemy => enemy.myName == target);
    }

#endregion

#region Scenes

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator UnloadObjects(string originalScene, string nextScene, GameMode mode)
    {
        yield return SceneTransitionEffect(0);

        listOfPlayers.RemoveAll(item => item == null);
        if (!originalScene.Equals("1. Battle"))
        {
            foreach (Character player in listOfPlayers)
            {
                player.gameObject.transform.SetParent(null);
                DontDestroyOnLoad(player.gameObject);
            }
        }

        this.mode = mode;
        SceneManager.LoadScene(nextScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canvas = GameObject.Find("Canvas").transform;
        StartCoroutine(BringBackObjects());
    }

    IEnumerator BringBackObjects()
    {
        yield return SceneTransitionEffect(1);
        transitionImage.gameObject.SetActive(false);
    }

    IEnumerator SceneTransitionEffect(float begin)
    {
        transitionImage.gameObject.SetActive(true);
        transitionImage.SetAlpha(begin);

        float waitTime = 0f;
        while (waitTime < transitionTime)
        {
            transitionImage.SetAlpha(Mathf.Abs(begin - (waitTime / transitionTime)));
            waitTime += Time.deltaTime;
            yield return null;
        }

        transitionImage.SetAlpha(Mathf.Abs(begin - 1));
        transitionImage.gameObject.SetActive(true);
    }
    
#endregion

}
