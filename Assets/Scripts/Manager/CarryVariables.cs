using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class CarryVariables : MonoBehaviour
{

#region Setup

    public enum GameMode { Main, Tutorial, Daily, Other };
    public static CarryVariables instance;

    [Foldout("Cheats and Challenges", true)]
        [ReadOnly] public List<string> listOfCheats = new();
        [ReadOnly] public List<string> listOfChallenges = new();

    [Foldout("Files", true)]
        public bool downloadOn = true;
        private string ID = "1hMvLkthvrosGe7GUuUReFIcvK7NNX4SRE1Z40bbjRyI";
        private string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
        private string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

        [Tooltip("store all player ability data")][ReadOnly] public List<AbilityData> listOfPlayerAbilities;
        [Tooltip("store all enemy ability data")][ReadOnly] public List<AbilityData> listOfEnemyAbilities;
        [Tooltip("store all enemy data")][ReadOnly] public List<List<CharacterData>> listOfEnemies = new();
        [Tooltip("store all bonus enemy data")][ReadOnly] public List<CharacterData> listOfBonusEnemies;

    [Foldout("Misc info", true)]
        public GameMode mode { get; private set; }
        [ReadOnly] public Transform sceneCanvas;
        [SerializeField] Canvas permanentCanvas;

    [Foldout("Scene transition", true)]
        [SerializeField] Image transitionImage;
        [SerializeField] float transitionTime;

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

    private void Start()
    {
        permanentCanvas.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

#region Scenes

    public IEnumerator UnloadObjects(string originalScene, string nextScene, GameMode mode)
    {
        yield return SceneTransitionEffect(0);
        this.mode = mode;
        SceneManager.LoadScene(nextScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneCanvas = GameObject.Find("Canvas").transform;
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

#region Cheats/Challenges

    public bool ActiveCheat(string cheat)
    {
        return (mode == GameMode.Main && listOfCheats.Contains(cheat));
    }

    public bool ActiveChallenge(string challenge)
    {
        return (mode == GameMode.Main && listOfChallenges.Contains(challenge));
    }

    #endregion

#region Files

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

    public List<AbilityData> ConvertToAbilityData(string list, bool player)
    {
        string[] divideIntoNumbers = list.Split(',');
        List<AbilityData> splitAbilities = new();
        for (int j = 0; j < divideIntoNumbers.Length; j++)
        {
            if (!divideIntoNumbers[j].Trim().Equals(""))
            {
                try
                {
                    AbilityData nextData = (player) ? FindPlayerAbility(divideIntoNumbers[j]) : FindEnemyAbility(divideIntoNumbers[j]);
                    splitAbilities.Add(nextData);
                }
                catch (NullReferenceException) { continue; }
                catch (ArgumentOutOfRangeException) { break; }
            }
        }
        return splitAbilities;
    }

    public AbilityData FindPlayerAbility(string target)
    {
        AbilityData foundData = listOfPlayerAbilities.FirstOrDefault(ability => ability.myName == target);
        if (foundData == null)
            Debug.LogError($"failed to find player ability: {target}");
        return foundData;
    }

    public AbilityData FindEnemyAbility(string target)
    {
        AbilityData foundData = listOfEnemyAbilities.FirstOrDefault(ability => ability.myName == target);
        if (foundData == null)
            Debug.LogError($"failed to find enemy ability: {target}");
        return foundData;
    }

    public CharacterData FindBonusEnemy(string target)
    {
        CharacterData foundData = listOfBonusEnemies.FirstOrDefault(character => character.myName == target);
        if (foundData == null)
            Debug.LogError($"failed to find enemy in bonus enemies: {target}");
        return foundData;
    }

    public List<AbilityData> CompletePlayerAbilities(List<AbilityData> current, string player, System.Random dailyRNG)
    {
        List<AbilityData> newList = new();
        newList.AddRange(current);

        while (newList.Count < 6)
        {
            int randomNumber = (dailyRNG != null) ? dailyRNG.Next(0, listOfPlayerAbilities.Count) : UnityEngine.Random.Range(0, listOfPlayerAbilities.Count);
            AbilityData randomAbility = listOfPlayerAbilities[randomNumber];
            if (!newList.Contains(randomAbility) && randomAbility.user.Equals(player))
                newList.Add(randomAbility);
        }
        return newList;
    }

    #endregion

}
