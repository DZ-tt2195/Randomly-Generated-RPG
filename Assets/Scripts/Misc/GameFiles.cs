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
    public string characterName;
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
    public string controller;
    public string abilityName;
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
    [Tooltip("store all player ability data")][ReadOnly] public Dictionary<string, AbilityData> listOfPlayerAbilities;
    [Tooltip("store all enemy ability data")][ReadOnly] public Dictionary<string, AbilityData> listOfEnemyAbilities;
    [Tooltip("store all player data")][ReadOnly] public Dictionary<string, CharacterData> listOfPlayers = new();
    [Tooltip("store all enemy data")][ReadOnly] public Dictionary<int, Dictionary<string, CharacterData>> listOfEnemies = new();

    void Awake()
    {
        inst = this;
        listOfPlayerAbilities = ReadTSVFile<AbilityData>(Resources.Load<TextAsset>("Data/Player Ability Data").text);
        listOfEnemyAbilities = ReadTSVFile<AbilityData>(Resources.Load<TextAsset>("Data/Enemy Ability Data").text);
        listOfPlayers = ReadTSVFile<CharacterData>(Resources.Load<TextAsset>("Data/Player Data").text);

        Dictionary<string, CharacterData> allEnemies = ReadTSVFile<CharacterData>(Resources.Load<TextAsset>("Data/Enemy Data").text);
        foreach (var KVP in allEnemies)
        {
            if (!listOfEnemies.ContainsKey(KVP.Value.difficulty))
                listOfEnemies.Add(KVP.Value.difficulty, new Dictionary<string, CharacterData>());
            listOfEnemies[KVP.Value.difficulty].Add(KVP.Key, KVP.Value);
        }
    }

    Dictionary<string, T> ReadTSVFile<T>(string textToConvert) where T : new()
    {
        string[] splitUp = textToConvert.Split('\n');
        Dictionary<string, int> columnIndex = new();

        string[] headers = splitUp[0].Split('\t');
        for (int i = 0; i<headers.Length; i++)
            columnIndex[headers[i].Trim()] = i;

        Dictionary<string, T> toReturn = new();
        for (int i = 1; i<splitUp.Length; i++)
        {
            T nextData = new();
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
            if (nextData is CharacterData character)
                toReturn.Add(character.characterName, nextData);
            else if (nextData is AbilityData ability)
                toReturn.Add(ability.abilityName, nextData);
        }
        return toReturn;

    }

#region Character Files

    public CharacterData RandomEnemy(int difficulty, System.Random dailyRNG)
    {
        Dictionary<string, CharacterData> enemyList = listOfEnemies[difficulty];
        List<string> convertedKeys = enemyList.Keys.ToList();
        string randomKey = "";

        if (dailyRNG != null)
            randomKey = convertedKeys[dailyRNG.Next(0, convertedKeys.Count)];
        else
            randomKey = convertedKeys[UnityEngine.Random.Range(0, convertedKeys.Count)];
        return enemyList[randomKey];
    }
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
                    AbilityData nextData = (player) ? FindPlayerAbility(divideIntoNumbers[j]) : FindEnemyAbility(divideIntoNumbers[j]);
                    splitAbilities.Add(nextData);
                }
                catch (NullReferenceException) { continue; }
                catch (ArgumentOutOfRangeException) { break; }
            }
        }
        return splitAbilities;
    }
    public AbilityData FindPlayerAbility(string target) => listOfPlayerAbilities[target];
    public AbilityData FindEnemyAbility(string target) => listOfEnemyAbilities[target];
    public CharacterData FindSpecificEnemy(string target, int difficulty) => listOfEnemies[difficulty][target];
    public List<AbilityData> CompletePlayerAbilities(List<AbilityData> forced, List<AbilityData> toChooseFrom, System.Random dailyRNG)
    {
        string playerName = toChooseFrom[0].controller;
        List<AbilityData> newList = new();
        newList.AddRange(forced);

        List<string> toFillOut = new();
        if (playerName.Equals(AutoTranslate.Knight()))
            toFillOut = new() { "Attack", "Attack", "Attack", "Emotion", "Stats", "Emotion" };
        else if (playerName.Equals(AutoTranslate.Angel()))
            toFillOut = new() { "Heal", "Heal", "Heal", "Position", "Emotion", "Stats" };
        else if (playerName.Equals(AutoTranslate.Wizard()))
            toFillOut = new() { "Attack", "Attack", "Attack", "Position", "Stats", "Position" };

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
