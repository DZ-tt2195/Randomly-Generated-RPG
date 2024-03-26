using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

[Serializable]
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
    public string onKill;
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
    public string artCredit;
}

[Serializable]
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
    public Position startingPosition;
    [ReadOnly] public string skillNumbers;
    public string aiTargeting;
    public string artCredit;
}

[Serializable]
public class AbilityData
{
    public string user;
    public string myName;
    public string description;
    public string logDescription;
    public string instructions;
    public AbilityType typeOne;
    public AbilityType typeTwo;
    public string playCondition;
    public int baseCooldown;
    public float attackPower;
    public float healthRegain;
    public float modifyAttack;
    public float modifyDefense;
    public float modifySpeed;
    public float modifyLuck;
    public float modifyAccuracy;
    public int miscNumber;
    public TeamTarget teamTarget;
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
                line[j] = line[j].Trim().Replace("\"", "").Replace("\\", "").Replace("]", "");
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
            newCharacter.startingPosition = (line[8] == "GROUNDED") ? Position.Grounded : Position.Airborne;
            newCharacter.skillNumbers = line[9].Trim();
            newCharacter.aiTargeting = line[10].Trim();
            newCharacter.artCredit = line[11].Replace("|", "\n");
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

            newAbility.user = line[0];
            newAbility.myName = line[1];
            newAbility.description = line[2];
            newAbility.logDescription = line[3];
            newAbility.typeOne = StringToAbilityType(line[4]);
            newAbility.typeTwo = StringToAbilityType(line[5]);
            newAbility.instructions = line[6];
            newAbility.playCondition = line[7];
            newAbility.baseCooldown = StringToInt(line[8]);
            newAbility.attackPower = StringToFloat(line[9]);
            newAbility.healthRegain = StringToFloat(line[10]);
            newAbility.modifyAttack = StringToFloat(line[11]);
            newAbility.modifyDefense = StringToFloat(line[12]);
            newAbility.modifySpeed = StringToFloat(line[13]);
            newAbility.modifyLuck = StringToFloat(line[14]);
            newAbility.modifyAccuracy = StringToFloat(line[15]);
            newAbility.miscNumber = StringToInt(line[16]);
            newAbility.teamTarget = StringToTeamTarget(line[17]);
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
            newWeapon.skillNumbers = line[2];
            newWeapon.statCalculation = line[3];
            newWeapon.startOfTurn = line[4];
            newWeapon.endOfTurn = line[5];
            newWeapon.newWave = line[6];
            newWeapon.onDeath = line[7];
            newWeapon.onKill = line[8];
            newWeapon.startingAttack = StringToFloat(line[9]);
            newWeapon.startingDefense = StringToFloat(line[10]);
            newWeapon.startingSpeed = StringToFloat(line[11]);
            newWeapon.startingLuck = StringToFloat(line[12]);
            newWeapon.startingAccuracy = StringToFloat(line[13]);
            newWeapon.modifyAttack = StringToFloat(line[14]);
            newWeapon.modifyDefense = StringToFloat(line[15]);
            newWeapon.modifySpeed = StringToFloat(line[16]);
            newWeapon.modifyLuck = StringToFloat(line[17]);
            newWeapon.modifyAccuracy = StringToFloat(line[18]);
            newWeapon.artCredit = line[19].Trim().Replace("|", "\n");
        }
        return nextData;
    }

    static AbilityType StringToAbilityType(string line)
    {
        line = line.ToUpper().Trim();
        return line switch
        {
            "ATTACK" => AbilityType.Attack,
            "STATS" => AbilityType.Stats,
            "EMOTION" => AbilityType.Emotion,
            "HEALING" => AbilityType.Healing,
            "POSITION" => AbilityType.Position,
            "MISC" => AbilityType.Misc,
            "NONE" => AbilityType.None,
            _ => AbilityType.None,
        };
    }

    static TeamTarget StringToTeamTarget(string line)
    {
        line = line.ToUpper().Trim();
        if (line == "")
        {
            UnityEngine.Debug.LogError("missing team target");
        }
        return line switch
        {
            "ANY ONE" => TeamTarget.AnyOne,
            "SELF" => TeamTarget.Self,
            "ALL" => TeamTarget.All,
            "ONE PLAYER" => TeamTarget.OnePlayer,
            "ONE ENEMY" => TeamTarget.OneEnemy,
            "ALL ENEMIES" => TeamTarget.AllEnemies,
            "ALL PLAYERS" => TeamTarget.AllPlayers,
            "OTHER PLAYER" => TeamTarget.OtherPlayer,
            "OTHER ENEMY" => TeamTarget.OtherEnemy,
            "NONE" => TeamTarget.None,
            _ => TeamTarget.None,
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
            UnityEngine.Debug.Log(line);
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
            UnityEngine.Debug.Log(line);
            return -1;
        }
    }
}
