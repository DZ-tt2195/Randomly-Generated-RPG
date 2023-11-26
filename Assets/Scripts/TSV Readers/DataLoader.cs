using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CharacterData
{
    public string name;
    public int baseHealth;
    public float baseAttack;
    public float baseDefense;
    public float baseSpeed;
    public float baseLuck;
    public float baseAccuracy;
    public Character.Position startingPosition;
    public Character.Emotion startingEmotion;
    public string skillNumbers;
}

[System.Serializable]
public class AbilityData
{
    public string name;
    public string description;
    public string instructions;
    public string nextInstructions;
    public string playCondition;
    public int healthChange;
    public int cooldown;
    public float modifyAttack;
    public float modifyDefense;
    public float modifySpeed;
    public float modifyLuck;
    public float modifyAccuracy;
    public int helperID;
    public Ability.TeamTarget teamTarget;
}

public class DataLoader
{
    public static List<CharacterData> ReadCharacterData(string fileToLoad)
    {
        List<CharacterData> nextData = new List<CharacterData>();
        var data = TSVReader.ReadFile(fileToLoad);

        for (int i = 2; i < data.Length; i++)
        {
            string[] line = data[i];
            CharacterData newCharacter = new CharacterData();
            nextData.Add(newCharacter);

            for (int j = 0; j < line.Length; j++)
            {
                line[j] = line[j].Trim().Replace("\"", "").Replace("\\", "").Replace("]","");
                //Debug.Log(line[j]);
            }

            newCharacter.name = line[0];
            newCharacter.baseHealth = StringToInt(line[1]);
            newCharacter.baseAttack = StringToFloat(line[2]);
            newCharacter.baseDefense = StringToFloat(line[3]);
            newCharacter.baseSpeed = StringToFloat(line[4]);
            newCharacter.baseLuck = StringToFloat(line[5]);
            newCharacter.baseAccuracy = StringToFloat(line[6]);
            newCharacter.startingPosition = (line[7] == "GROUNDED") ? Character.Position.Grounded : Character.Position.Airborne;
            newCharacter.startingEmotion = StringToEmotion(line[8]);
            newCharacter.skillNumbers = line[9];
        }
        return nextData;
    }

    public static List<AbilityData> ReadAbilityData(string fileToLoad)
    {
        List<AbilityData> nextData = new List<AbilityData>();
        var data = TSVReader.ReadFile(fileToLoad);
        for (int i = 2; i < data.Length; i++)
        {
            string[] line = data[i];
            AbilityData newAbility = new AbilityData();
            nextData.Add(newAbility);

            for (int j = 0; j < line.Length; j++)
            {
                line[j] = line[j].Trim().Replace("\"", "").Replace("\\", "").Replace("]", "");
                //Debug.Log(line[j]);
            }

            newAbility.name = line[1];
            newAbility.description = line[2];
            newAbility.instructions = line[3];
            newAbility.nextInstructions = line[4];
            newAbility.playCondition = line[5];
            newAbility.healthChange = StringToInt(line[6]);
            newAbility.cooldown = StringToInt(line[7]);
            newAbility.modifyAttack = StringToFloat(line[8]);
            newAbility.modifyDefense = StringToFloat(line[9]);
            newAbility.modifySpeed = StringToFloat(line[10]);
            newAbility.modifyLuck = StringToFloat(line[11]);
            newAbility.modifyAccuracy = StringToFloat(line[12]);
            newAbility.helperID = StringToInt(line[13]);
            newAbility.teamTarget = StringToTeamTarget(line[14]);
        }
        return nextData;
    }

    static Ability.TeamTarget StringToTeamTarget(string line)
    {
        line = line.ToUpper().Trim();
        return line switch
        {
            "ANY ONE" => Ability.TeamTarget.AnyOne,
            "SELF" => Ability.TeamTarget.Self,
            "ALL" => Ability.TeamTarget.All,
            "ONE TEAMMATE" => Ability.TeamTarget.OneTeammate,
            "ONE ENEMY" => Ability.TeamTarget.OneEnemy,
            "ALL ENEMIES" => Ability.TeamTarget.AllEnemies,
            "ALL TEAMMATES" => Ability.TeamTarget.AllTeammates,
            "OTHER TEAMMATE" => Ability.TeamTarget.OtherTeammate,
            "OTHER ENEMY" => Ability.TeamTarget.OtherEnemy,
            "NONE" => Ability.TeamTarget.None,
            _ => Ability.TeamTarget.None,
        };
    }

    static Character.Emotion StringToEmotion(string line)
    {
        line = line.ToUpper().Trim();
        return line switch
        {
            "NEUTRAL" => Character.Emotion.Neutral,
            "HAPPY" => Character.Emotion.Happy,
            "ECSTATIC" => Character.Emotion.Ecstatic,
            "ANGRY" => Character.Emotion.Angry,
            "ENRAGED" => Character.Emotion.Enraged,
            "SAD" => Character.Emotion.Sad,
            "DEPRESSED" => Character.Emotion.Depressed,
            "NONE" => Character.Emotion.Neutral,
            _ => Character.Emotion.Neutral,
        };
    }

    static float StringToFloat(string line)
    {
        line = line.Trim();
        try
        {
            return (line == "") ? 0f : float.Parse(line);
        }
        catch (FormatException)
        {
            Debug.Log(line);
            return -1f;
        }

    }

    static int StringToInt(string line)
    {
        line = line.Trim();
        try
        {
            return (line == "") ? 0 : int.Parse(line);
        }
        catch (FormatException)
        {
            Debug.Log(line);
            return -1;
        }
    }
}