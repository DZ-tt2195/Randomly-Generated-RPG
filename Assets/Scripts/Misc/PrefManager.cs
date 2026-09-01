using System.ComponentModel;
using UnityEngine;

public static class PrefManager
{
    public static int GetSaved(string type, int number) => PlayerPrefs.HasKey($"{type} {number}") ? PlayerPrefs.GetInt($"{type} {number}") : -1;
    public static void SetSaved(string type, int number, int value) => PlayerPrefs.SetInt($"{type} {number}", value);

}