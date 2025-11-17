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
using System.Text.RegularExpressions;

public class CarryVariables : MonoBehaviour
{

#region Setup

    public enum GameMode { Main, Tutorial, Daily, Other };
    public static CarryVariables instance;

    [Foldout("Cheats and Challenges", true)]
        [ReadOnly] public List<string> listOfCheats = new();
        [ReadOnly] public List<string> listOfChallenges = new();

    [Foldout("Files", true)]
        [Tooltip("store all player ability data")][ReadOnly] public List<AbilityData> listOfPlayerAbilities;
        [Tooltip("store all enemy ability data")][ReadOnly] public List<AbilityData> listOfEnemyAbilities;
        [Tooltip("store all enemy data")][ReadOnly] public List<List<CharacterData>> listOfEnemies = new();
        [Tooltip("store all bonus enemy data")][ReadOnly] public List<CharacterData> listOfBonusEnemies;
        Dictionary<string, Dictionary<string, string>> keyTranslate = new();

    [Foldout("Misc info", true)]
        public GameMode mode { get; private set; }
        [ReadOnly] public Transform sceneCanvas;
        [SerializeField] Canvas permanentCanvas;
        [SerializeField] [Scene] string toLoad;

    [Foldout("Scene transition", true)]
        [SerializeField] Image transitionImage;
        [SerializeField] float transitionTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            permanentCanvas.gameObject.SetActive(true);

            if (!PlayerPrefs.HasKey("Language")) PlayerPrefs.SetString("Language", "English");
            List<CharacterData> allEnemies = DataLoader.ReadCharacterData("Enemy Data").OrderBy(data => data.myName).ToList();
            listOfEnemies = new()
        {
            new List<CharacterData>(),
            allEnemies.Where(data => data.difficulty == 1).ToList(),
            allEnemies.Where(data => data.difficulty == 2).ToList(),
            allEnemies.Where(data => data.difficulty == 3).ToList(),
        };

            listOfBonusEnemies = DataLoader.ReadCharacterData("Bonus Enemy Data");
            listOfPlayerAbilities = DataLoader.ReadAbilityData("Player Ability Data").OrderBy(data => data.myName).ToList();
            listOfEnemyAbilities = DataLoader.ReadAbilityData("Enemy Ability Data").OrderBy(data => data.myName).ToList();

            CsvLanguages(TSVReader.ReadFile("Csv Languages"));
            TxtLanguages();
            ChangeLanguage(PlayerPrefs.GetString("Language"));
        }
        else
        {
            Destroy(this.gameObject);
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
        GameObject obj = GameObject.Find("Canvas");
        if (obj != null)
        {
            sceneCanvas = obj.transform;
            StartCoroutine(BringBackObjects());
        }
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

#region Translations

    void TxtLanguages()
    {
        TextAsset[] languageFiles = Resources.LoadAll<TextAsset>("Txt Languages");
        foreach (TextAsset language in languageFiles)
        {
            (bool success, string converted) = ConvertTxtName(language);
            if (success)
            {
                Dictionary<string, string> newDictionary = new();
                keyTranslate.Add(converted, newDictionary);
                string[] lines = language.text.Split('\n');

                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        string[] parts = line.Split('=');
                        newDictionary[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
        }

        (bool, string) ConvertTxtName(TextAsset asset)
        {
            //pattern: "0. English"
            string pattern = @"^\d+\.\s*(.+)$";
            Match match = Regex.Match(asset.name, pattern);
            if (match.Success)
                return (true, match.Groups[1].Value);
            else
                return (false, "");
        }
    }

    void CsvLanguages(string[][] data)
    {
        for (int i = 1; i < data[1].Length; i++)
        {
            data[1][i] = data[1][i].Replace("\"", "").Trim();
            Dictionary<string, string> newDictionary = new();
            keyTranslate.Add(data[1][i], newDictionary);
        }

        for (int i = 2; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                data[i][j] = data[i][j].Replace("\"", "").Replace("\\", "").Replace("]", "").Replace("|", "\n").Trim();
                if (j > 0)
                {
                    string language = data[1][j];
                    string key = data[i][0];
                    keyTranslate[language][key] = data[i][j];
                }
            }
        }
    }

    public string Translate(string key, List<(string, string)> toReplace = null)
    {
        string answer = "";
        if (key == "" || int.TryParse(key, out _))
            return key;

        try
        {
            answer = keyTranslate[PlayerPrefs.GetString("Language")][key];
        }
        catch
        {
            try
            {
                answer = keyTranslate[PlayerPrefs.GetString("English")][key];
                Debug.Log($"{key} failed to translate in {PlayerPrefs.GetString("Language")}");
            }
            catch
            {
                Debug.Log($"{key} failed to translate at all");
                return key;
            }
        }

        if (toReplace != null)
        {
            foreach ((string one, string two) in toReplace)
                answer = answer.Replace($"${one.Replace("$", "")}$", two);
        }
        return answer;
    }

    public Dictionary<string, Dictionary<string, string>> GetTranslations()
    {
        return keyTranslate;
    }

    public void ChangeLanguage(string newLanguage)
    {
        PlayerPrefs.SetString("Language", newLanguage);
        KeywordTooltip.instance.SwitchLanguage();
        SceneManager.LoadScene(toLoad);
    }

    #endregion

#region Character Files

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
