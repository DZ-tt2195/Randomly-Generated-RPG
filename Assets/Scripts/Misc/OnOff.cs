using UnityEngine;
using System.Collections.Generic;

public class OnOff : MonoBehaviour
{
    [SerializeField] List<GameObject> forceOn = new();
    [SerializeField] List<GameObject> forceOff = new();
    private void Start() 
    {
        foreach (GameObject next in forceOn)
            if (next != null) next.SetActive(true);
        foreach (GameObject next in forceOff)
            if (next != null) next.SetActive(false);
    }
}