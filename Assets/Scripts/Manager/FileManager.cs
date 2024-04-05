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

}
