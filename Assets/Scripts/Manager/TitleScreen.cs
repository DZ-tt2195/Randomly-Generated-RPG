using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] string fileToLoad;
    [SerializeField] Character characterPrefab;

    void Start()
    {
        List<CharacterData> data = DataLoader.ReadCharacterData(fileToLoad);
        for (int i = 0; i < data.Count; i++)
        {
            Character nextCharacter = Instantiate(characterPrefab);
            nextCharacter.SetupCharacter(data[i]);
        }

    }

}
