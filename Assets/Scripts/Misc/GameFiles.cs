using UnityEngine;
using System.Collections.Generic;
using MyBox;
using System;
using System.Linq;
using System.Reflection;

public enum CharacterTargeting { None, MostTargets, CameOffCooldown, LastAttacker, ChooseElevated, ChooseGrounded, LeastHealth, MostHealth, Effectiveness, Strongest, Weakest, ChooseAttack }

[Serializable]
public class CharacterData
{
    public ToTranslate characterName;
    public int baseHealth;
    public Position startPosition;
    public string listOfAbilities;
    public CharacterTargeting aiTargeting;
    public string artCredit;
    public int difficulty;
    public Sprite sprite;
}

[Serializable]
public class AbilityData
{
    public ToTranslate controller;
    public ToTranslate abilityName;
    public string[] instructions;
    public AbilityType[] abilityTypes;
    public string[] playCondition;
    public int baseCooldown;
    public int mainNumber;
    public int secondNumber;
    public int powerStat;
    public int defenseStat;
    public int miscNumber;
    public TeamTarget[] toTarget;
}

public class GameFiles : MonoBehaviour
{
    public static GameFiles inst;
    [Tooltip("store all player ability data")][ReadOnly] public List<AbilityData> listOfPlayerAbilities;
    [Tooltip("store all enemy ability data")][ReadOnly] public List<AbilityData> listOfEnemyAbilities;
    [Tooltip("store all player data")][ReadOnly] public List<CharacterData> listOfPlayers = new();
    [Tooltip("store all enemy data")][ReadOnly] public Dictionary<int, List<CharacterData>> listOfEnemies = new();

    void Awake()
    {
        inst = this;
        listOfPlayerAbilities = ReadTSVFile<AbilityData>(Resources.Load<TextAsset>("Player Ability Data").text);
        listOfEnemyAbilities = ReadTSVFile<AbilityData>(Resources.Load<TextAsset>("Enemy Ability Data").text);
        listOfPlayers = ReadTSVFile<CharacterData>(Resources.Load<TextAsset>("Player Data").text);

        List<CharacterData> allEnemies = ReadTSVFile<CharacterData>(Resources.Load<TextAsset>("Enemy Data").text);
        listOfEnemies = new Dictionary<int, List<CharacterData>>
        {
            { 0, new List<CharacterData>() },
            { 1, new List<CharacterData>() },
            { 2, new List<CharacterData>() },
            { 3, new List<CharacterData>() }
        };
        foreach (CharacterData data in allEnemies)
            listOfEnemies[data.difficulty].Add(data);
    }

    List<T> ReadTSVFile<T>(string textToConvert) where T : new()
    {
        string[] splitUp = textToConvert.Split('\n');
        Dictionary<string, int> columnIndex = new();

        string[] headers = splitUp[0].Split('\t');
        for (int i = 0; i<headers.Length; i++)
            columnIndex[headers[i].Trim()] = i;

        List<T> toReturn = new();
        for (int i = 1; i<splitUp.Length; i++)
        {
            T nextData = new();
            toReturn.Add(nextData);
            string[] thisRow = splitUp[i].Split('\t');

            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (columnIndex.TryGetValue(field.Name, out int index))
                {
                    string sheetValue = thisRow[index].Trim();
                    if (field.FieldType == typeof(int))
                        field.SetValue(nextData, StringToInt(sheetValue));
                    else if (field.FieldType == typeof(bool))
                        field.SetValue(nextData, StringToBool(sheetValue));
                    else if (field.FieldType == typeof(string))
                        field.SetValue(nextData, sheetValue);
                    else if (field.FieldType == typeof(AbilityType[]))
                        field.SetValue(nextData, StringToAbilityType(sheetValue));
                    else if (field.FieldType == typeof(ToTranslate))
                        field.SetValue(nextData, StringToTranslate(sheetValue));
                    else if (field.FieldType == typeof(Position))
                        field.SetValue(nextData, StringToPosition(sheetValue));
                    else if (field.FieldType == typeof(TeamTarget[]))
                        field.SetValue(nextData, StringToTeamTarget(sheetValue));
                    else if (field.FieldType == typeof(CharacterTargeting))
                        field.SetValue(nextData, StringToTargeting(sheetValue));
                    else if (field.FieldType == typeof(string[]))
                        field.SetValue(nextData, SplitStrings(sheetValue));

                    string[] SplitStrings(string line)
                    {
                        return line.Split('-');
                    }

                    int StringToInt(string line)
                    {
                        try
                        {
                            return (line.Equals("")) ? -1 : int.Parse(line);
                        }
                        catch (FormatException)
                        {
                            return 0;
                        }
                    }

                    AbilityType[] StringToAbilityType(string line)
                    {
                        string[] divided = line.Split('/');
                        AbilityType[] toReturn = new AbilityType[divided.Length];

                        for (int i = 0; i<divided.Length; i++)
                            toReturn[i] = (AbilityType)Enum.Parse(typeof(AbilityType), divided[i]);
                        return toReturn;
                        }

                    TeamTarget[] StringToTeamTarget(string line)
                    {
                        string[] divided = line.Split('-');
                        TeamTarget[] toReturn = new TeamTarget[divided.Length];

                        for (int i = 0; i<divided.Length; i++)
                            toReturn[i] = (TeamTarget)Enum.Parse(typeof(TeamTarget), divided[i]);
                        return toReturn;
                    }

                    ToTranslate StringToTranslate(string line)
                    {
                        return (ToTranslate)Enum.Parse(typeof(ToTranslate), line);
                    }

                    CharacterTargeting StringToTargeting(string line)
                    {
                        return (CharacterTargeting)Enum.Parse(typeof(CharacterTargeting), line);
                    }

                    Position StringToPosition(string line)
                    {
                        return line.Equals("GROUNDED") ? Position.Grounded : Position.Elevated;
                    }

                    bool StringToBool(string line)
                    {
                        return line.Equals("TRUE");
                    }
                }
                else
                {
                    if (field.FieldType == typeof(Sprite))
                        field.SetValue(nextData, Resources.Load<Sprite>($"Characters/{thisRow[columnIndex["characterName"]]}"));
                }
            }

        }
        return toReturn;

    }

#region Character Files

    public List<AbilityData> ConvertToAbilityData(string list, bool player)
    {
        string[] divideIntoNumbers = list.Split(',');
        List<AbilityData> splitAbilities = new();
        for (int j = 0; j < divideIntoNumbers.Length; j++)
        {
            if (!divideIntoNumbers[j].Trim().Equals(""))
            {
                try
                {
                    ToTranslate converted = (ToTranslate)Enum.Parse(typeof(ToTranslate), divideIntoNumbers[j]);
                    AbilityData nextData = (player) ? FindPlayerAbility(converted) : FindEnemyAbility(converted);
                    splitAbilities.Add(nextData);
                }
                catch (NullReferenceException) { continue; }
                catch (ArgumentOutOfRangeException) { break; }
            }
        }
        return splitAbilities;
    }

    public AbilityData FindPlayerAbility(ToTranslate target)
    {
        AbilityData foundData = listOfPlayerAbilities.FirstOrDefault(ability => ability.abilityName == target);
        if (foundData == null)
            Debug.LogError($"failed to find player ability: {target}");
        return foundData;
    }

    public AbilityData FindEnemyAbility(ToTranslate target)
    {
        AbilityData foundData = listOfEnemyAbilities.FirstOrDefault(ability => ability.abilityName == target);
        if (foundData == null)
            Debug.LogError($"failed to find enemy ability: {target}");
        return foundData;
    }

    public CharacterData FindSpecificEnemy(ToTranslate target, int difficulty)
    {
        CharacterData foundData = listOfEnemies[difficulty].FirstOrDefault(character => character.characterName == target);
        if (foundData == null)
            Debug.LogError($"failed to find enemy: {target}, {difficulty}");
        return foundData;
    }

    public List<AbilityData> CompletePlayerAbilities(List<AbilityData> forced, List<AbilityData> toChooseFrom, System.Random dailyRNG)
    {
        ToTranslate playerName = toChooseFrom[0].controller;
        List<AbilityData> newList = new();
        newList.AddRange(forced);

        List<string> toFillOut = playerName switch
        {
            ToTranslate.Knight => new() { "Attack", "Attack", "Attack", "Emotion", "Stats", "Emotion" },
            ToTranslate.Angel => new() { "Heal", "Heal", "Heal", "Position", "Emotion", "Stats" },
            ToTranslate.Wizard => new() { "Attack", "Attack", "Attack", "Position", "Stats", "Position" },
            _ => throw new NotImplementedException(),
        };

        foreach (AbilityData data in forced)
            CheckOff(data);

        bool CheckOff(AbilityData ability)
        {
            bool answer = false;
            if (ability.abilityTypes.Contains(AbilityType.Attack) && toFillOut.Contains("Attack"))
            {
                toFillOut.Remove("Attack");
                answer = true;
            }
            if (ability.abilityTypes.Contains(AbilityType.Healing) && toFillOut.Contains("Heal"))
            {
                toFillOut.Remove("Heal");
                answer = true;
            }
            if ((ability.abilityTypes.Contains(AbilityType.PositionEnemy) || ability.abilityTypes.Contains(AbilityType.PositionPlayer)) && toFillOut.Contains("Position"))
            {
                toFillOut.Remove("Position");
                answer = true;
            }
            if ((ability.abilityTypes.Contains(AbilityType.EmotionEnemy) || ability.abilityTypes.Contains(AbilityType.EmotionPlayer)) && toFillOut.Contains("Emotion"))
            {
                toFillOut.Remove("Emotion");
                answer = true;
            }
            if ((ability.abilityTypes.Contains(AbilityType.StatEnemy) || ability.abilityTypes.Contains(AbilityType.StatPlayer)) && toFillOut.Contains("Stats"))
            {
                toFillOut.Remove("Stats");
                answer = true;
            }
            return answer;
        }

        while (newList.Count < 6)
        {
            int randomNumber = (dailyRNG != null) ? dailyRNG.Next(0, toChooseFrom.Count) : UnityEngine.Random.Range(0, toChooseFrom.Count);
            AbilityData randomAbility = toChooseFrom[randomNumber];

            if (newList.Contains(randomAbility) || !randomAbility.controller.Equals(playerName))
                continue;

            if (toFillOut.Count == 0 || CheckOff(randomAbility))
                newList.Add(randomAbility);
        }
        return newList;
    }

    #endregion

}
