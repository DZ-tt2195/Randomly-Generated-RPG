using UnityEditor;
using UnityEngine;
using System.IO;
using TMPro;

public static class TestScripts
{
    [MenuItem("Tools/Count Lines of Code")]
    public static void CountLines()
    {
        string scriptsPath = Path.Combine(Application.dataPath, "Scripts");
        string[] files = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories);
        int total = 0;

        foreach (string file in files)
        {
            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue; // skip blank
                if (trimmed == "{" || trimmed == "}") continue; // skip braces only
                total++;
            }
        }
        Debug.Log($"Total lines of C# code in Scripts/: {total} (across {files.Length} files)");
    }

    [MenuItem("Tools/Change All TMP Fonts")]
    static void ChangeAllFonts()
    {
        TMP_FontAsset newFont = Selection.activeObject as TMP_FontAsset;

        if (newFont == null)
        {
            Debug.LogError("Select a TMP_FontAsset first.");
            return;
        }

        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(
            FindObjectsSortMode.None
        );

        int count = 0;

        foreach (var text in texts)
        {
            if (text.font != newFont)
            {
                Undo.RecordObject(text, "Change TMP Font");
                text.font = newFont;
                EditorUtility.SetDirty(text);
                count++;
            }
        }

        Debug.Log($"Updated {count} TMP_Text components.");
    }

}