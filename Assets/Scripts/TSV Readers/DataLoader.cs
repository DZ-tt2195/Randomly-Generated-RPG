using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityData
{
}

public class CharacterData
{
    public string name;
    public int baseHealth;
    public int baseEnergy;
    public float baseAttack;
    public float baseDefense;
    public float baseSpeed;
    public float baseLuck;
    public float baseAccuracy;
    public Character.Position startingPosition;
}

public class DataLoader
{
    public static List<AbilityData> ReadAbilityData(string fileToLoad)
    {
        List<AbilityData> nextData = new List<AbilityData>();
        var data = TSVReader.ReadFile(fileToLoad, 1);
        foreach (string[] line in data)
        {
            AbilityData newAbility = new AbilityData();
            nextData.Add(newAbility);

        }
        return nextData;
    }

    public static List<CharacterData> ReadCharacterData(string fileToLoad)
    {
        List<CharacterData> nextData = new List<CharacterData>();
        var data = TSVReader.ReadFile(fileToLoad, 1);
        foreach (string[] line in data)
        {
            CharacterData newCharacter = new CharacterData();
            nextData.Add(newCharacter);

            newCharacter.name = line[0];
            newCharacter.baseHealth = StringToInt(line[1]);
            newCharacter.baseEnergy = StringToInt(line[2]);
            newCharacter.baseAttack = StringToFloat(line[3]);
            newCharacter.baseDefense = StringToFloat(line[4]);
            newCharacter.baseSpeed = StringToFloat(line[5]);
            newCharacter.baseLuck = StringToFloat(line[6]);
            newCharacter.baseAccuracy = StringToFloat(line[7]);
            newCharacter.startingPosition = (line[8] == "Grounded") ? Character.Position.Grounded : Character.Position.Airborne;
        }
        return nextData;
    }

    static float StringToFloat(string line)
    {
        return (line == "") ? 0f : float.Parse(line);
    }

    static int StringToInt(string line)
    {
        return (line == "") ? 0 : int.Parse(line);
    }
}