using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class MyExtensions
{
    public static string StopwatchTime(Stopwatch stopwatch)
    {
        TimeSpan time = stopwatch.Elapsed;
        string seconds = time.Seconds < 10 ? $"0{time.Seconds}" : $"{time.Seconds}";
        return $"{(int)time.TotalMinutes}:{seconds}.{time.Milliseconds}";
    }

    public static void SetAlpha(SpriteRenderer target, float alpha)
    {
        Color newColor = target.color;
        newColor.a = alpha;
        target.color = newColor;
    }

    public static void Shuffle(this Transform originalTransform)
    {
        List<int> indexes = new();
        List<Transform> items = new();

        for (int i = 0; i < originalTransform.childCount; i++)
        {
            indexes.Add(i);
            items.Add(originalTransform.GetChild(i));
        }

        foreach (var next in items)
        {
            int randomNumber = UnityEngine.Random.Range(0, indexes.Count);
            next.SetSiblingIndex(indexes[randomNumber]);
        }
    }

    public static List<T> Shuffle<T>(this List<T> originalList)
    {
        List<T> newList = new();

        while (originalList.Count > 0)
        {
            int randomNumber = UnityEngine.Random.Range(0, originalList.Count);
            newList.Add(originalList[randomNumber]);
            originalList.RemoveAt(randomNumber);
        }

        return newList;
    }
}