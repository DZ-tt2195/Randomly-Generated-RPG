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
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("TSVs/0. English", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "1585553982"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Player Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "2114962038"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Player Ability Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "1268954754"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Enemy Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "567473665"));
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Enemy Ability Data", "1HO4JtV9ukU0UMJfZV7zG5bhExNQqzZwoU3bPh1OfpPk", "949940889"));
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
        TextAsset englishFile = Resources.Load<TextAsset>("TSVs/0. English");
        Dictionary<string, string> newDictionary = Translator.ReadLanguageFile(englishFile.text);
        
        List<string> noConvert = new();
        List<(string, List<string>)> needConvert = new();

        foreach (var KVP in newDictionary)
        {
            string key = KVP.Key;
            string value = KVP.Value;
            
            Regex regex = new(@"\$(.*?)\$");
            List<string> allMatches = new();
            foreach (Match m in regex.Matches(value).Cast<Match>())
            {
                string match = m.Groups[1].Value;
                allMatches.Add(match);
            }

            if (allMatches.Count == 0)
            {
                noConvert.Add(key);
            }
            else
            {
                allMatches = allMatches.Distinct().ToList();
                needConvert.Add((key, allMatches));
            }
        }

        using (StreamWriter writer = new StreamWriter("Assets/Scripts/Translations/AutoTranslate.cs"))
        {
            writer.WriteLine("using System.Collections.Generic;\npublic static class AutoTranslate\n{\n");

            for (int i = 0; i < needConvert.Count; i++)
            {
                (string key, List<string> replace) = needConvert[i];
                string nextCode = $"public static string {key} (";

                for (int j = 0; j < replace.Count; j++)
                {
                    nextCode += $"string {replace[j]}";
                    if (j < replace.Count - 1)
                        nextCode += ",";
                }
                nextCode += ")  { return(";
                nextCode += $"Translator.inst.Translate(\"{key}\", new()";
                nextCode += "{";

                for (int j = 0; j < replace.Count; j++)
                {
                    nextCode += $"(\"{replace[j]}\", {replace[j]})";
                    if (j < replace.Count - 1)
                        nextCode += ",";
                }
                nextCode += "})); }\n";
                writer.WriteLine(nextCode);
            }
            
            writer.WriteLine("public static string DoEnum(ToTranslate thing) {return(Translator.inst.Translate(thing.ToString()));}");
            writer.WriteLine("}");

            writer.WriteLine("public enum ToTranslate {");
            string nextEnum = "";
            for (int i = 0; i < noConvert.Count; i++)
            {
                nextEnum += $"{noConvert[i]}";
                if (i < noConvert.Count - 1)
                    nextEnum += ",";
            }
            writer.WriteLine(nextEnum);
            writer.WriteLine("}");
        }
        Debug.Log($"{noConvert.Count} enum lines, {needConvert.Count} converted lines");
        AssetDatabase.Refresh();
    }
}
