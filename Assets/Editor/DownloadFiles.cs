using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

public static class FileManager
{
    private static string sheetURL = "1dzI-REK9mWqcYQMd7uS7JPx_CdkTwnbDr0L8vgJcWuk";
    private static string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private static string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    [MenuItem("Tools/Download From Spreadsheet")]
    public static void DownloadFiles()
    {
        Debug.Log($"starting downloads");
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Csv Languages"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Player Data"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Enemy Data"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Bonus Enemy Data"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Player Ability Data"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Enemy Ability Data"));
    }

    static IEnumerator Download(string range)
    {
        string url = $"{baseUrl}{sheetURL}/values/{range}?key={apiKey}";
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Download failed: {www.error}");
        }
        else
        {
            string filePath = $"Assets/Resources/{range}.txt";
            File.WriteAllText($"{filePath}", www.downloadHandler.text);

            string[] allLines = File.ReadAllLines($"{filePath}");
            List<string> modifiedLines = allLines.ToList();
            modifiedLines.RemoveRange(1, 3);
            File.WriteAllLines($"{filePath}", modifiedLines.ToArray());
            Debug.Log($"downloaded {range}");
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Tools/Create base txt")]
    public static void CreateBaseTxtFile()
    {
        string baseText = "";
        string[][] csvFile = TSVReader.ReadFile("Csv Languages");

        for (int i = 2; i < csvFile.Length; i++)
        {
            string key = FixLine(csvFile[i][0]);
            string text = FixLine(csvFile[i][1]);
            baseText += $"{key}={text}\n";
        }

        string FixLine(string line)
        {
            return line.Replace("\"", "").Replace("\\", "").Replace("]", "").Replace("|", "\n").Trim();
        }

        string filePath = $"Assets/Resources/BaseTxtFile.txt";
        File.WriteAllText($"{filePath}", baseText);

        /*
        string filePath = Path.Combine(Application.persistentDataPath, "BaseTxtFile.txt");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string input in listOfKeys)
                writer.WriteLine($"{input}=");
        }*/
        Debug.Log($"converted English csv to txt file");
        AssetDatabase.Refresh();
    }
}
