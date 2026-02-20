using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using System.Text.RegularExpressions;
public static class FileManager
{
    [MenuItem("Tools/Download from spreadsheet")]
    public static void DownloadTSV()
    {
        Debug.Log($"starting downloads");
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Languages/0. English", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "1585553982"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Data/Player Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "2114962038"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Data/Player Ability Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "1268954754"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Data/Enemy Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "567473665"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Data/Enemy Ability Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "949940889"));
    }
    static IEnumerator Download(string fileName, string spreadsheetID, string sheetGID)
    {
        string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetID}/export?format=tsv&gid={sheetGID}";        
        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.error);
            yield break;
        }
        else
        {
            File.WriteAllText($"Assets/Resources/{fileName}.txt", req.downloadHandler.text);
            Debug.Log($"downloaded {fileName}");
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Tools/Make enums and functions")]
    public static void EnumsAndFunctions()
    {
        TextAsset englishFile = Resources.Load<TextAsset>("Languages/0. English");
        Dictionary<string, string> newDictionary = Translator.ReadLanguageFile(englishFile.text);
        
        List<(string, List<string>)> normal = new();
        List<(string, List<string>)> online = new();

        foreach (var KVP in newDictionary)
        {
            string key = KVP.Key;
            string value = KVP.Value;

            Regex regex = new(@"\$(.*?)\$");
            List<string> allMatches = new();
            foreach (Match m in regex.Matches(value).Cast<Match>())
            {
                string match = m.Groups[1].Value;
                if (!allMatches.Contains(match))
                    allMatches.Add(match);
            }

            if (key.Contains("Online"))
            {
                online.Add((key, allMatches));
            }
            else
            {
                normal.Add((key, allMatches));
            }
        }

        using (StreamWriter writer = new StreamWriter("Assets/Scripts/Translations/AutoTranslate.cs"))
        {
            writer.WriteLine("public static class AutoTranslate \n{");

            string needSubEnum = "";
            for (int i = 0; i < normal.Count; i++)
            {
                (string key, List<string> replace) = normal[i];
                needSubEnum += key;
                if (i < normal.Count - 1)
                    needSubEnum += ",";

                string nextCode = $"public static string {key} (";
                for (int j = 0; j < replace.Count; j++)
                {
                    nextCode += $"string {replace[j]}";
                    if (j < replace.Count - 1)
                        nextCode += ",";
                }
                nextCode += ") => ";
                nextCode += $"Translator.inst.Translate(\"{key}\", new() ";
                nextCode += "{";

                for (int j = 0; j < replace.Count; j++)
                {
                    nextCode += $"(\"{replace[j]}\", {replace[j]})";
                    if (j < replace.Count - 1)
                        nextCode += ",";
                }
                nextCode += "});";
                writer.WriteLine(nextCode);
            }
            writer.WriteLine("}");
        }

        using (StreamWriter writer = new StreamWriter("Assets/Scripts/Translations/OnlineTranslate.cs"))
        {
            writer.WriteLine("public static class OnlineTranslate \n{");
            string onlineEnum = "";

            for (int i = 0; i < online.Count; i++)
            {
                (string key, List<string> replace) = online[i];
                onlineEnum += key;
                if (i < online.Count - 1)
                    onlineEnum += ",";

                string nextCode = $"public static string {key} (";
                for (int j = 0; j < replace.Count; j++)
                {
                    nextCode += $"string {replace[j]}";
                    if (j < replace.Count - 1)
                        nextCode += ",";
                }
                nextCode += $") => $\"{key}";
                for (int j = 0; j<replace.Count; j++)
                {
                    nextCode += "\\t" + replace[j];
                    nextCode += "\\t" + "{" + replace[j] + "}";
                }

                nextCode += $"\";";
                writer.WriteLine(nextCode);
            }

            writer.WriteLine("}");
            //writer.WriteLine("public enum OnlinePackage {" + onlineEnum + "}");
        }

        Debug.Log($"{normal.Count} lines, {online.Count} online lines");
        AssetDatabase.Refresh();
    }

}
