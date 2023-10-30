using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
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
    public Character.Emotion startingEmotion;
}

public class EnemyData
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
    public string listOfAbilities;
}

public class AbilityData
{
    public string name;
    public string playCondition;
    public int healthChange;
    public int energyCost;
    public float modifyAttack;
    public float modifyDefense;
    public float modifySpeed;
    public float modifyLuck;
    public float modifyAccuracy;
    public Character.Emotion? newEmotion = null;
    public Character.Position? positionChange = null;
    public Ability.TeamTarget teamTarget;
    public Ability.PositionTarget positionTarget;
}

public class DataLoader
{
    public static List<PlayerData> ReadPlayerData(string fileToLoad)
    {
        List<PlayerData> nextData = new List<PlayerData>();
        var data = TSVReader.ReadFile(fileToLoad, 1);
        foreach (string[] line in data)
        {
            PlayerData newCharacter = new PlayerData();
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

    public static List<EnemyData> ReadEnemyData(string fileToLoad)
    {
        List<EnemyData> nextData = new List<EnemyData>();
        var data = TSVReader.ReadFile(fileToLoad, 1);
        foreach (string[] line in data)
        {
            EnemyData newEnemy = new EnemyData();
            nextData.Add(newEnemy);

            newEnemy.name = line[0];
            newEnemy.baseHealth = StringToInt(line[1]);
            newEnemy.baseAttack = StringToFloat(line[2]);
            newEnemy.baseDefense = StringToFloat(line[3]);
            newEnemy.baseSpeed = StringToFloat(line[4]);
            newEnemy.baseLuck = StringToFloat(line[5]);
            newEnemy.baseAccuracy = StringToFloat(line[6]);
            newEnemy.startingPosition = (line[7] == "Grounded") ? Character.Position.Grounded : Character.Position.Airborne;
            newEnemy.listOfAbilities = line[8];
        }
        return nextData;
    }

    public static List<AbilityData> ReadAbilityData(string fileToLoad)
    {
        List<AbilityData> nextData = new List<AbilityData>();
        var data = TSVReader.ReadFile(fileToLoad, 1);
        foreach (string[] line in data)
        {
            AbilityData newAbility = new AbilityData();
            nextData.Add(newAbility);

            newAbility.name = line[0];
            newAbility.playCondition = line[1];
            newAbility.healthChange = StringToInt(line[2]);
            newAbility.energyCost = StringToInt(line[3]);
            newAbility.modifyAttack = StringToFloat(line[4]);
            newAbility.modifyDefense = StringToFloat(line[5]);
            newAbility.modifySpeed = StringToFloat(line[6]);
            newAbility.modifyLuck = StringToFloat(line[7]);
            newAbility.modifyAccuracy = StringToFloat(line[8]);
            newAbility.newEmotion = (line[9] == "") ? null : StringToEmotion(line[9]);
            newAbility.positionChange = (line[10] == "") ? null : StringToPosition(line[10]);
            newAbility.teamTarget = StringToTeamTarget(line[11]);
            newAbility.positionTarget = StringToPositionTarget(line[12]);
        }
        return nextData;
    }

    static Ability.PositionTarget StringToPositionTarget(string line)
    {
        line = line.ToUpper();
        return line switch
        {
            "ALL" => Ability.PositionTarget.All,
            "GROUNDED" => Ability.PositionTarget.OnlyGrounded,
            "AIRBORNE" => Ability.PositionTarget.OnlyAirborne,
            _ => Ability.PositionTarget.None,
        };
    }

    static Ability.TeamTarget StringToTeamTarget(string line)
    {
        line = line.ToUpper();
        return line switch
        {
            "SELF" => Ability.TeamTarget.Self,
            "ALL" => Ability.TeamTarget.All,
            "ONE PLAYER" => Ability.TeamTarget.OnePlayer,
            "ONE ENEMY" => Ability.TeamTarget.OneEnemy,
            "ALL ENEMIES" => Ability.TeamTarget.AllEnemies,
            "ALL PLAYERS" => Ability.TeamTarget.AllPlayers,
            _ => Ability.TeamTarget.None,
        };
    }

    static Character.Emotion StringToEmotion(string line)
    {
        line = line.ToUpper();
        return line switch
        {
            "NEUTRAL" => Character.Emotion.Neutral,
            "HAPPY" => Character.Emotion.Happy,
            "ECSTATIC" => Character.Emotion.Ecstatic,
            "ANGRY" => Character.Emotion.Angry,
            "ENRAGED" => Character.Emotion.Enraged,
            "SAD" => Character.Emotion.Sad,
            "DEPRESSED" => Character.Emotion.Depressed,
            _ => Character.Emotion.Neutral,
        };
    }

    static Character.Position StringToPosition(string line)
    {
        line = line.ToUpper();
        return line switch
        {
            "AIRBORNE" => Character.Position.Airborne,
            "GROUNDED" => Character.Position.Grounded,
            _ => Character.Position.Grounded,
        };
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