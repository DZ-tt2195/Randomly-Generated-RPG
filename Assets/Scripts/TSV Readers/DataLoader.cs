using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class WeaponData
{
    public string myName;
    public string description;
    public string skillNumbers;
    public string statCalculation;
    public string startOfTurn;
    public string endOfTurn;
    public string newWave;
    public string onDeath;
    public float startingAttack;
    public float startingDefense;
    public float startingSpeed;
    public float startingLuck;
    public float startingAccuracy;
    public float modifyAttack;
    public float modifyDefense;
    public float modifySpeed;
    public float modifyLuck;
    public float modifyAccuracy;
}

[System.Serializable]
public class CharacterData
{
    public string myName;
    public string description;
    public int baseHealth;
    public float baseAttack;
    public float baseDefense;
    public float baseSpeed;
    public float baseLuck;
    public float baseAccuracy;
    public Character.Position startingPosition;
    public Character.Emotion startingEmotion;
    public string skillNumbers;
    public string aiTargeting;
}

[System.Serializable]
public class AbilityData
{
    public string myName;
    public string description;
    public string logDescription;
    public string instructions;
    public string nextInstructions;
    public string playCondition;
    public float healthChange;
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
        List<CharacterData> nextData = new();
        var data = TSVReader.ReadFile(fileToLoad);

        for (int i = 2; i < data.Length; i++)
        {
            string[] line = data[i];
            CharacterData newCharacter = new();
            nextData.Add(newCharacter);

            for (int j = 0; j < line.Length; j++)
            {
                line[j] = line[j].Trim().Replace("\"", "").Replace("\\", "").Replace("]","");
                //Debug.Log(line[j]);
            }

            newCharacter.myName = line[0];
            newCharacter.description = line[1];
            newCharacter.baseHealth = StringToInt(line[2]);
            newCharacter.baseAttack = StringToFloat(line[3]);
            newCharacter.baseDefense = StringToFloat(line[4]);
            newCharacter.baseSpeed = StringToFloat(line[5]);
            newCharacter.baseLuck = StringToFloat(line[6]);
            newCharacter.baseAccuracy = StringToFloat(line[7]);
            newCharacter.startingPosition = (line[8] == "GROUNDED") ? Character.Position.Grounded : Character.Position.Airborne;
            newCharacter.startingEmotion = StringToEmotion(line[9]);
            newCharacter.skillNumbers = line[10];
            try { newCharacter.aiTargeting = line[11]; } catch (IndexOutOfRangeException) { /*do nothing*/};
        }
        return nextData;
    }

    public static List<AbilityData> ReadAbilityData(string fileToLoad)
    {
        List<AbilityData> nextData = new();
        var data = TSVReader.ReadFile(fileToLoad);
        for (int i = 2; i < data.Length; i++)
        {
            string[] line = data[i];
            AbilityData newAbility = new();
            nextData.Add(newAbility);

            for (int j = 0; j < line.Length; j++)
            {
                line[j] = line[j].Trim().Replace("\"", "").Replace("\\", "").Replace("]", "");
                //Debug.Log(line[j]);
            }

            newAbility.myName = line[1];
            newAbility.description = line[2];
            newAbility.logDescription = line[3];
            newAbility.instructions = line[4];
            newAbility.nextInstructions = line[5];
            newAbility.playCondition = line[6];
            newAbility.healthChange = StringToFloat(line[7]);
            newAbility.cooldown = StringToInt(line[8]);
            newAbility.modifyAttack = StringToFloat(line[9]);
            newAbility.modifyDefense = StringToFloat(line[10]);
            newAbility.modifySpeed = StringToFloat(line[11]);
            newAbility.modifyLuck = StringToFloat(line[12]);
            newAbility.modifyAccuracy = StringToFloat(line[13]);
            newAbility.helperID = StringToInt(line[14]);
            try { newAbility.teamTarget = StringToTeamTarget(line[15]); } catch (IndexOutOfRangeException) { Debug.Log($"{newAbility.myName} has no target"); }
        }
        return nextData;
    }

    public static List<WeaponData> ReadWeaponData(string fileToLoad)
    {
        List<WeaponData> nextData = new();
        var data = TSVReader.ReadFile(fileToLoad);
        for (int i = 2; i < data.Length; i++)
        {
            string[] line = data[i];
            WeaponData newWeapon = new();
            nextData.Add(newWeapon);

            for (int j = 0; j < line.Length; j++)
            {
                line[j] = line[j].Trim().Replace("\"", "").Replace("\\", "").Replace("]", "");
                //Debug.Log(line[j]);
            }

            newWeapon.myName = line[0];
            newWeapon.description = line[1];
            try { newWeapon.skillNumbers = line[2];} catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.statCalculation = line[3];} catch (IndexOutOfRangeException){continue;}
            try { newWeapon.startOfTurn = line[4]; } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.endOfTurn = line[5]; } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.newWave = line[6]; } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.onDeath = line[7]; } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.startingAttack = StringToFloat(line[8]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.startingDefense = StringToFloat(line[9]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.startingSpeed = StringToFloat(line[10]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.startingLuck = StringToFloat(line[11]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.startingAccuracy = StringToFloat(line[12]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.modifyAttack = StringToFloat(line[13]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.modifyDefense = StringToFloat(line[14]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.modifySpeed = StringToFloat(line[15]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.modifyLuck = StringToFloat(line[16]); } catch (IndexOutOfRangeException) { continue; }
            try { newWeapon.modifyAccuracy = StringToFloat(line[17]); } catch (IndexOutOfRangeException) { continue; }
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
            "ONE PLAYER" => Ability.TeamTarget.OnePlayer,
            "ONE ENEMY" => Ability.TeamTarget.OneEnemy,
            "ALL ENEMIES" => Ability.TeamTarget.AllEnemies,
            "ALL PLAYERS" => Ability.TeamTarget.AllPlayers,
            "OTHER PLAYER" => Ability.TeamTarget.OtherPlayer,
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