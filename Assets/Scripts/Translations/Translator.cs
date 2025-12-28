using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;
using System;
using System.Reflection;


public class Translator : MonoBehaviour
{

#region Setup

    public static Translator inst;
    Dictionary<string, Dictionary<string, string>> keyTranslate = new();
    [Scene][SerializeField] string toLoad;

    void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        TextAsset[] languageFiles = Resources.LoadAll<TextAsset>("TSVs");
        foreach (TextAsset language in languageFiles)
        {
            string fileName = ConvertName(language);

            string ConvertName(TextAsset asset)
            {
                //pattern: "0. English"
                string pattern = @"^\d+\.\s*(.+)$";
                Match match = Regex.Match(asset.name, pattern);
                if (match.Success)
                    return match.Groups[1].Value;
                else
                    return asset.name;
            }

            Dictionary<string, string> newDictionary = ReadLanguageFile(language.text);
            keyTranslate.Add(fileName, newDictionary);
        }

        if (!PlayerPrefs.HasKey("English") || !keyTranslate.ContainsKey(PlayerPrefs.GetString("Language")))
            PlayerPrefs.SetString("Language", "English");

        KeywordTooltip.instance.SwitchLanguage();
        ScreenOverlay.instance.UnloadObjects(SceneManager.GetActiveScene().name, toLoad, GameMode.Other);
    }

    public static Dictionary<string, string> ReadLanguageFile(string textToConvert)
    {
        string[] splitUp = textToConvert.Split('\n');
        Dictionary<string, string> toReturn = new();

        foreach (string line in splitUp)
        {
            int index = line.IndexOf('\t');
            string partOne = line[..index].Trim();
            string partTwo = /*partOne.Equals("Blank") ? "" :*/ line[(index + 1)..].Trim();
            toReturn.Add(partOne, partTwo);
        }
        return toReturn;
    }

    #endregion

#region Helpers

    public bool TranslationExists(string key)
    {
        return keyTranslate["English"].ContainsKey(key);
    }

    public string Translate(string key, List<(string, string)> toReplace = null)
    {
        string answer;
        try
        {
            answer = keyTranslate[PlayerPrefs.GetString("Language")][key];
        }
        catch
        {
            try
            {
                answer = keyTranslate[("English")][key];
                //Debug.Log($"{key} failed to translate in {PlayerPrefs.GetString("Language")}");
            }
            catch
            {
                //Debug.Log($"{key} failed to translate at all");
                return key;
            }
        }

        if (toReplace != null)
        {
            foreach ((string one, string two) in toReplace)
                answer = answer.Replace($"${one}$", two);
        }
        return answer;
    }

    public Dictionary<string, Dictionary<string, string>> GetTranslations()
    {
        return keyTranslate;
    }

    public void ChangeLanguage(string newLanguage, Dictionary<string, string> addedTranslation)
    {
        if (addedTranslation != null)
        {
            keyTranslate.Add(newLanguage, addedTranslation);
        }
        if (!PlayerPrefs.GetString("Language").Equals(newLanguage))
        {
            PlayerPrefs.SetString("Language", newLanguage);
            KeywordTooltip.instance.SwitchLanguage();
            ScreenOverlay.instance.UnloadObjects(SceneManager.GetActiveScene().name, toLoad, GameMode.Other);
        }
    }

#endregion

}
