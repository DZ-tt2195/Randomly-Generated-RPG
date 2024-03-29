using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

[Serializable]
public class SaveData
{
    public List<AbilityData> seenKnightAbilities = new();
    public List<AbilityData> seenAngelAbilities = new();
    public List<AbilityData> seenWizardAbilities = new();
    public List<CharacterData> seenEnemies = new();

    public SaveData()
    {
    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    [ReadOnly] public SaveData currentSaveData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if (ES3.FileExists("Random RPG.es3"))
        {
            currentSaveData = ES3.Load<SaveData>("Random RPG", $"{Application.persistentDataPath}/Random RPG.es3");
        }
        else
        {
            currentSaveData = new SaveData();
            ES3.Save("Random RPG", currentSaveData, $"{Application.persistentDataPath}/Random RPG.es3");
        }
    }

    public void SaveEnemy(CharacterData data)
    {
        foreach (CharacterData savedData in currentSaveData.seenEnemies)
            if (savedData.myName.Equals(data.myName))
                return;
        currentSaveData.seenEnemies.Add(data);
        ES3.Save("Random RPG", currentSaveData, $"{Application.persistentDataPath}/Random RPG.es3");
    }

    public void SaveAbility(string character, AbilityData data)
    {
        switch (character)
        {
            case "Knight":
                foreach (AbilityData savedData in currentSaveData.seenKnightAbilities)
                    if (savedData.myName.Equals(data.myName))
                        return;
                currentSaveData.seenKnightAbilities.Add(data);
                break;
            case "Angel":
                foreach (AbilityData savedData in currentSaveData.seenAngelAbilities)
                    if (savedData.myName.Equals(data.myName))
                        return;
                currentSaveData.seenAngelAbilities.Add(data);
                break;
            case "Wizard":
                foreach (AbilityData savedData in currentSaveData.seenWizardAbilities)
                    if (savedData.myName.Equals(data.myName))
                        return;
                currentSaveData.seenWizardAbilities.Add(data);
                break;
        }

        ES3.Save("Random RPG", currentSaveData, $"{Application.persistentDataPath}/Random RPG.es3");
    }
}
