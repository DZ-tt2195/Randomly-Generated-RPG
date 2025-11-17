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
        TextAsset data = Resources.Load($"{file}") as TextAsset;

        string editData = data.text;
        editData = editData.Replace("],", "").Replace("{", "").Replace("}", "");

        string[] numLines = editData.Split("[");
        string[][] list = new string[numLines.Length][];

        for (int i = 0; i < numLines.Length; i++)
        {
            list[i] = numLines[i].Split("\",");
        }
        return list;
    }
}