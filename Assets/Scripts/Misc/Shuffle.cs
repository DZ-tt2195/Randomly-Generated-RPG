using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyExtensions
{
    public static void Shuffle(this Transform originalTransform)
    {
        for (int k = 0; k < 5; k++)
        {
            List<int> indexes = new();
            List<Transform> items = new();

            for (int i = 0; i < originalTransform.childCount; i++)
            {
                indexes.Add(i);
                items.Add(originalTransform.GetChild(i));
            }

            foreach (var x in items)
            {
                x.SetSiblingIndex(indexes[Random.Range(0, indexes.Count)]);
            }
        }
    }

    public static List<T> Shuffle<T>(this List<T> originalList)
    {
        int originalCount = originalList.Count;
        for (int k = 0; k < 5; k++)
        {
            List<T> newList = new();

            for (int i = 0; i<originalCount; i++)
            {
                int randomNumber = Random.Range(0, originalList.Count);
                newList.Add(originalList[randomNumber]);
                originalList.RemoveAt(randomNumber);
            }

            originalList = newList;
        }
        return originalList;
    }

}
