using UnityEditor;
using UnityEngine;
using System.IO;

public static class LineCounter
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
}