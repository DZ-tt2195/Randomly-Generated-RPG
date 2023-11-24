//modified from code by Teemu Ikonen

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TSVReader
{
	public static string[][] ReadFile(string file)
	{
        TextAsset data = Resources.Load($"File Data/{file}") as TextAsset;

        string editData = data.text;
        editData = editData.Replace("],", "").Replace("{", "").Replace("}", "");

        string[] numCards = editData.Split("[");
        string[][] list = new string[numCards.Length][];

        for (int i = 0; i < numCards.Length; i++)
        {
            list[i] = numCards[i].Split("\",");
        }
        return list;
    }
}