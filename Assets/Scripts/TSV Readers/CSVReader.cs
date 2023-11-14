//modified from code by Teemu Ikonen

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TSVReader
{
	static string SPLIT_RE = @"\t(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

	/// <summary>
	/// Reads a TSV from a file in the Resources folder and outputs a jagged array of strings
	/// </summary>
	/// <param name="file">The path of the file to load</param>
	/// <param name="headerLines">The number of lines at the top to skip over as headers</param>
	/// <returns></returns>
	public static string[][] ReadFile(string file)
	{
        TextAsset data = Resources.Load(file) as TextAsset;

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