using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using System.Text.RegularExpressions;

[Serializable]
public class CharacterData
{
    public string myName;
    public string description;
    public int baseHealth;
    public int baseSpeed;
    public float baseLuck;
    public float baseAccuracy;
    public Position startingPosition;
    public string listOfSkills;
    public string aiTargeting;
    public string artCredit;
    public int difficulty;
}

[Serializable]
public class AbilityData
{
    public string user;
    public string myName;
    public string description;
    public string logDescription;
    public string[] instructions;
    public AbilityType[] myTypes;
    public string[] playCondition;
    public int baseCooldown;
    public int attackDamage;
    public int healthRegain;
    public int modifyPower;
    public int modifyDefense;
    public int modifySpeed;
    public float modifyLuck;
    public float modifyAccuracy;
    public int miscNumber;
    public TeamTarget[] defaultTargets;
}

public class DataLoader
{
    public static List<CharacterData> ReadCharacterData(string fileToLoad, int difficulty)
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
            }

            newCharacter.myName = line[0];
            newCharacter.description = line[1];
            newCharacter.baseHealth = StringToInt(line[2]);
            newCharacter.baseSpeed = StringToInt(line[3]);
            newCharacter.baseLuck = StringToFloat(line[4]);
            newCharacter.baseAccuracy = StringToFloat(line[5]);
            newCharacter.startingPosition = line[6].Equals("GROUNDED") ? Position.Grounded : Position.Airborne;
            newCharacter.listOfSkills = line[7].Trim();
            newCharacter.aiTargeting = line[8].Trim().ToUpper();
            newCharacter.artCredit = line[9].Replace("|", "\n");
            newCharacter.difficulty = difficulty;
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
                line[j] = line[j].Trim().Replace("\"", "").Replace("\\", "").Replace("]", "");

            newAbility.user = line[0];
            newAbility.myName = line[1];
            newAbility.description = line[2];
            newAbility.logDescription = line[3];

            string[] listOfTargets = (line[4].Equals("") ? new string[1] { "NONE" } : TurnManager.SpliceString(line[4].ToUpper().Trim(), '-'));
            TeamTarget[] convertToTargets = new TeamTarget[listOfTargets.Length];
            for (int j = 0; j < listOfTargets.Length; j++)
                convertToTargets[j] = StringToTeamTarget(listOfTargets[j]);
            newAbility.defaultTargets = convertToTargets;

            newAbility.playCondition = (line[5].Equals("") ? new string[1] { "NONE" } : TurnManager.SpliceString(line[5].ToUpper().Trim(), '-'));
            newAbility.instructions = (line[6].Equals("") ? new string[1] { "NONE" } : TurnManager.SpliceString(line[6].ToUpper().Trim(), '-'));

            newAbility.baseCooldown = StringToInt(line[7]);
            newAbility.attackDamage = StringToInt(line[8]); VerifyNumbers(newAbility, "ATK", newAbility.attackDamage.ToString());
            newAbility.healthRegain = StringToInt(line[9]); VerifyNumbers(newAbility, "REGAIN", newAbility.healthRegain.ToString());

            newAbility.modifyPower = StringToInt(line[10]); VerifyNumbers(newAbility, "POWERSTAT", newAbility.modifyPower.ToString());
            newAbility.modifyDefense = StringToInt(line[11]); VerifyNumbers(newAbility, "DEFENSESTAT", newAbility.modifyDefense.ToString());
            newAbility.modifySpeed = StringToInt(line[12]); VerifyNumbers(newAbility, "SPEEDSTAT", newAbility.modifySpeed.ToString());
            newAbility.modifyLuck = StringToFloat(line[13]); VerifyNumbers(newAbility, "LUCKSTAT", newAbility.modifyLuck.ToString());
            newAbility.modifyAccuracy = StringToFloat(line[14]); VerifyNumbers(newAbility, "ACCURACYSTAT", newAbility.modifyAccuracy.ToString());
            newAbility.miscNumber = StringToInt(line[15]); VerifyNumbers(newAbility, "MISC", newAbility.miscNumber.ToString());

            string[] listOfTypes = TurnManager.SpliceString(line[16].ToUpper().Trim(), '/');
            AbilityType[] convertToTypes = new AbilityType[listOfTypes.Length];
            for (int j = 0; j < listOfTypes.Length; j++)
                convertToTypes[j] = StringToAbilityType(listOfTypes[j]);
            newAbility.myTypes = convertToTypes;

            if (!(newAbility.defaultTargets.Length == newAbility.playCondition.Length &&
                newAbility.playCondition.Length == newAbility.instructions.Length))
                Debug.LogError($"{newAbility.user}: {newAbility.myName} has inconsistent targets / instructions / conditions");
        }
        return nextData;
    }

    static AbilityType StringToAbilityType(string line)
    {
        return line switch
        {
            "ATTACK" => AbilityType.Attack,
            "STATPLAYER" => AbilityType.StatPlayer,
            "STATENEMY" => AbilityType.StatEnemy,
            "EMOTIONPLAYER" => AbilityType.EmotionPlayer,
            "EMOTIONENEMY" => AbilityType.EmotionEnemy,
            "HEALING" => AbilityType.Healing,
            "POSITIONPLAYER" => AbilityType.PositionPlayer,
            "POSITIONENEMY" => AbilityType.PositionEnemy,
            "MISC" => AbilityType.Misc,
            "NONE" => AbilityType.None,
            _ => AbilityType.None,
        };
    }

    static TeamTarget StringToTeamTarget(string line)
    {
        if (line.Equals(""))
        {
            Debug.LogError("missing team target");
        }
        return line switch
        {
            "ANYONE" => TeamTarget.AnyOne,
            "SELF" => TeamTarget.Self,
            "ALL" => TeamTarget.All,
            "ONEPLAYER" => TeamTarget.OnePlayer,
            "ONEENEMY" => TeamTarget.OneEnemy,
            "ALLENEMIES" => TeamTarget.AllEnemies,
            "ALLPLAYERS" => TeamTarget.AllPlayers,
            "OTHERPLAYER" => TeamTarget.OtherPlayer,
            "OTHERENEMY" => TeamTarget.OtherEnemy,
            "NONE" => TeamTarget.None,
            _ => TeamTarget.None,
        };
    }

    static float StringToFloat(string line)
    {
        line = line.Trim();
        try
        {
            return (line.Equals("")) ? 0f : float.Parse(line);
        }
        catch (FormatException)
        {
            Debug.LogError(line);
            return -1f;
        }
    }

    static int StringToInt(string line)
    {
        line = line.Trim();
        try
        {
            return (line.Equals("")) ? 0 : int.Parse(line);
        }
        catch (FormatException)
        {
            Debug.LogError(line);
            return -1;
        }
    }

    static void VerifyNumbers(AbilityData data, string toReplace, string newText)
    {
        int count = Regex.Matches(data.description, toReplace).Count;
        if (count > 0)
        {
            try
            {
                if (int.Parse(newText) == 0)
                    Debug.LogError($"{data.user}: {data.myName} has wrong {toReplace}");
            }
            catch (FormatException)
            {
                if (float.Parse(newText) == 0f)
                    Debug.LogError($"{data.user}: {data.myName} has wrong {toReplace}");
            }
        }
    }
}
