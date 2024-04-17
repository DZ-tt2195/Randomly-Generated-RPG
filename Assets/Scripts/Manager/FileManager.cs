using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;
using UnityEngine.Networking;
using System.IO;
using System;

public class FileManager : MonoBehaviour
{

#region Variables

    public static FileManager instance;

    public bool downloadOn = true;
    private string ID = "1x5vKp4X4HPKyRix3w0n9aldY6Dh3B0eBegUM0WtfXFY";
    private string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    [Tooltip("store all players")][ReadOnly] public List<Character> listOfPlayers = new();
    [Tooltip("store all player ability data")][ReadOnly] public List<AbilityData> listOfPlayerAbilities;
    [Tooltip("store all enemy ability data")][ReadOnly] public List<AbilityData> listOfEnemyAbilities;
    [Tooltip("store all enemy data")][ReadOnly] public List<List<CharacterData>> listOfEnemies = new();
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

    public CharacterData FindEnemy(string target, int tier)
    {
        CharacterData foundData = listOfEnemies[tier].FirstOrDefault(character => character.myName == target); 
        if (foundData == null)
            Debug.LogError($"failed to find enemy in tier {tier}: {target}");
        return foundData;
    }

    public CharacterData FindBonusEnemy(string target)
    {
        CharacterData foundData = listOfBonusEnemies.FirstOrDefault(character => character.myName == target); 
        if (foundData == null)
            Debug.LogError($"failed to find enemy in bonus enemies: {target}");
        return foundData;
    }

#endregion

}
