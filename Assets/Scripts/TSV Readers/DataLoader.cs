using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string helperName;
    public Ability.TeamTarget teamTarget;
}

public class DataLoader
{
    public static List<CharacterData> ReadCharacterData(string fileToLoad)
    {
        List<CharacterData> nextData = new List<CharacterData>();
        var data = TSVReader.ReadFile(fileToLoad);
        foreach (string[] line in data)
        {
            CharacterData newCharacter = new CharacterData();
            nextData.Add(newCharacter);

            newCharacter.name = line[0];
            newCharacter.baseHealth = StringToInt(line[1]);
            newCharacter.baseAttack = StringToFloat(line[2]);
            newCharacter.baseDefense = StringToFloat(line[3]);
            newCharacter.baseSpeed = StringToFloat(line[4]);
            newCharacter.baseLuck = StringToFloat(line[5]);
            newCharacter.baseAccuracy = StringToFloat(line[6]);
            newCharacter.startingPosition = (line[7] == "Grounded") ? Character.Position.Grounded : Character.Position.Airborne;
            newCharacter.startingEmotion = StringToEmotion(line[8]);
            newCharacter.skillNumbers = line[9];
        }
        return nextData;
    }

    public static List<AbilityData> ReadAbilityData(string fileToLoad)
    {
        List<AbilityData> nextData = new List<AbilityData>();
        var data = TSVReader.ReadFile(fileToLoad);
        foreach (string[] line in data)
        {
            AbilityData newAbility = new AbilityData();
            nextData.Add(newAbility);

            newAbility.name = line[1];
            newAbility.description = line[2];
            newAbility.instructions = line[3];
            newAbility.nextInstructions = line[4];
            newAbility.playCondition = line[5];
            newAbility.healthChange = StringToInt(line[5]);
            newAbility.cooldown = StringToInt(line[6]);
            newAbility.cooldown = StringToInt(line[6]);
            newAbility.modifyAttack = StringToFloat(line[7]);
            newAbility.modifyDefense = StringToFloat(line[8]);
            newAbility.modifySpeed = StringToFloat(line[9]);
            newAbility.modifyLuck = StringToFloat(line[10]);
            newAbility.modifyAccuracy = StringToFloat(line[11]);
            newAbility.helperName = line[12];
            newAbility.teamTarget = StringToTeamTarget(line[13]);
        }
        return nextData;
    }

    static Ability.TeamTarget StringToTeamTarget(string line)
    {
        line = line.ToUpper();
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
            "NONE" => Character.Emotion.Neutral,
            _ => Character.Emotion.Neutral,
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