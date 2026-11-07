using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;
using System.Linq;
using System.Diagnostics;
public enum TeamCompare {BothPlayers, BothEnemies, Opposites}
public enum Stats { HealthIcon, Max_HealthIcon, PowerIcon, DefenseIcon };
public enum StatusEffect { Targeted, Locked, Protected, Blessed, Stunned };
public enum Position { Grounded, Elevated };
public enum Mood { Lively, Focused, Tired };
public class StatusRound
{
    public StatusEffect effect;
    public int roundNumber;
    public StatusRound(StatusEffect effect, int roundNumber)
    {
        this.effect = effect;
        this.roundNumber = roundNumber;
    }
}
[RequireComponent(typeof(Button))][RequireComponent(typeof(Image))]
public class Character : MonoBehaviour
{
#region Variables
    public static float borderColor;
    public static int maxAbilities = 6;
    [Foldout("Character info", true)]
        protected Ability chosenAbility;
        protected List<Character> chosenTarget = new();
        [ReadOnly] public Character lastToAttackThis { get; private set; }
        [ReadOnly] public string editedDescription { get; private set; }
        [ReadOnly] public CharacterData data { get; private set; }
        [ReadOnly] public List<Ability> listOfAutoAbilities = new();
        [ReadOnly] public List<Ability> listOfRandomAbilities = new();
        int myPosition;
    [Foldout("Stats", true)]
        private Dictionary<Stats, int> _privStatMod = new();
        public Position CurrentPosition { get; private set; }
        public Mood CurrentMood { get; private set; }
        List<StatusRound> allStatusRounds = new();
    [Foldout("UI", true)]
        [ReadOnly] public Image border;
        [ReadOnly] public Button myButton;
        [ReadOnly] public Image myImage;
        TMP_Text topText;
        TMP_Text healthText;
        TMP_Text nameText;
        TMP_Text statusText;
    #endregion
#region Setup
    public static Mood RandomMood(System.Random dailyRNG)
    {
        int randomRange = Enum.GetValues(typeof(Mood)).Length;
        if (dailyRNG != null)
            return (Mood)dailyRNG.Next(0, randomRange);
        else
            return (Mood)UnityEngine.Random.Range(0, randomRange);
    }
    public static TeamCompare GetTeamCompare(Character one, Character two)
    {
        if (one is PlayerCharacter && two is PlayerCharacter)
            return TeamCompare.BothPlayers;
        else if (one is EnemyCharacter && two is EnemyCharacter)
            return TeamCompare.BothEnemies;
        else
            return TeamCompare.Opposites;
    }
    private void Awake()
    {
        myImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        border = transform.Find("border").GetComponent<Image>();
        statusText = transform.Find("Status Text").GetComponent<TMP_Text>();
        healthText = transform.Find("Health Text").GetChild(0).GetComponent<TMP_Text>();
        nameText = transform.Find("Name Text").GetChild(0).GetComponent<TMP_Text>();
        topText = transform.Find("Top Text").GetComponent<TMP_Text>();

        foreach (Stats value in Enum.GetValues(typeof(Stats)))
            _privStatMod.Add(value, 0);
    }
    public void SetupCharacter(CharacterData characterData, List<AbilityData> listOfAbilityData, Mood startingEmotion, int position, bool abilitiesBeginWithCooldown)
    {
        data = characterData;
        myPosition = position;
        this.name = Translator.inst.Translate(characterData.characterName);
        nameText.text = this.name;
        editedDescription = KeywordTooltip.instance.EditText(Translator.inst.Translate($"{characterData.characterName}_Text"));

        this._privStatMod[Stats.Max_HealthIcon] = data.maxHealth;
        this._privStatMod[Stats.HealthIcon] = data.maxHealth;

        this.myImage.sprite = this.data.sprite;
        AddAbility(GameFiles.inst.FindEnemyAbility(nameof(AutoTranslate.Wait)), true, false);
        if (this is PlayerCharacter)
            AddAbility(GameFiles.inst.FindEnemyAbility(nameof(AutoTranslate.Revive)), true, false);

        StartCoroutine(ChangePosition(data.startPosition, -1, null));
        StartCoroutine(ChangeMood(startingEmotion, -1, null));

        CharacterUI();
        StatusUI();

        foreach (AbilityData data in listOfAbilityData)
            AddAbility(data, false, abilitiesBeginWithCooldown);
        listOfRandomAbilities = listOfRandomAbilities.OrderBy(o => o.mainType).ToList();
    }
    internal void AddAbility(AbilityData abilityFile, bool auto, bool startWithCooldown)
    {
        Ability newAbility = new Ability(this, abilityFile, startWithCooldown);
        (auto ? listOfAutoAbilities : listOfRandomAbilities).Add(newAbility);
    }
    internal void DropAbility(Ability ability)
    {
        listOfAutoAbilities.Remove(ability);
        listOfRandomAbilities.Remove(ability);
    }
    #endregion
#region Stats
    public int GetHealth() => _privStatMod[Stats.HealthIcon];
    public float CalculateHealthPercent()
    {
        return (float)_privStatMod[Stats.HealthIcon] / _privStatMod[Stats.Max_HealthIcon];
    }
    public int CalculatePower()
    {
        int bonus = 0;
        return _privStatMod[Stats.PowerIcon] + bonus;
    }
    public int CalculateDefense()
    {
        int bonus = 0;
        if (FightRules.inst != null && FightRules.inst.CheckRule(nameof(AutoTranslate.Comfort_Zone), -1) is RulesData comfortZone)
            bonus += (CurrentPosition == data.startPosition) ? comfortZone.defenseStat : -comfortZone.defenseStat;
        return _privStatMod[Stats.DefenseIcon] + bonus;
    }
    public int CalculateStatTotals()
    {
        return CalculatePower() + CalculateDefense();
    }
    public bool IsStatus(StatusEffect effect)
    {
        bool isEffect = false;
        for (int i = allStatusRounds.Count-1; i>=0; i--)
        {
            if (allStatusRounds[i].roundNumber < TurnManager.inst.GetRound(0))
                allStatusRounds.RemoveAt(i);
            else if (allStatusRounds[i].effect == effect && TurnManager.inst.GetRound(0) == allStatusRounds[i].roundNumber)
                isEffect = true;
        }
        return isEffect;
    }
    #endregion
#region Change Stats
    public IEnumerator ChangeStat(Stats stat, int change, int logged, Character cause)
    {
        if (this == null || change == 0) yield break;
        string answer = "";

        if (IsStatus(StatusEffect.Protected) && change < 0)
        {
            AudioManager.instance.Blocked();
            TurnManager.inst.CreateVisual($"{AutoTranslate.Blocked()}", this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Drop(this.name, Mathf.Abs(change).ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
            yield break;
        }
        else if (change > 0 && FightRules.inst.CheckRule(nameof(AutoTranslate.Cursed), -1) is RulesData cursed && TurnManager.inst.GetRound(0) % cursed.miscNumber == 0)
        {
            FightRules.inst.CheckRule(nameof(AutoTranslate.Cursed), logged);
            AudioManager.instance.Blocked();
            TurnManager.inst.CreateVisual($"{AutoTranslate.Blocked()}", this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Gain(this.name, change.ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
            yield break;            
        }

        if (stat == Stats.DefenseIcon || stat == Stats.PowerIcon)
        {
            _privStatMod[stat] = Math.Clamp(_privStatMod[stat] + change, -3, 3);
        }
        else
        {
            _privStatMod[stat] += change;
            _privStatMod[Stats.HealthIcon] = Math.Clamp(_privStatMod[Stats.HealthIcon], 0, _privStatMod[Stats.Max_HealthIcon]);

            if (cause != null && stat == Stats.HealthIcon && change < 0 && GetTeamCompare(this, cause) == TeamCompare.Opposites)
                lastToAttackThis = cause;
        }
        
        CharacterUI();
        if (logged >= 0 && change > 0)
        {
            if (stat == Stats.DefenseIcon || stat == Stats.PowerIcon)
                AudioManager.instance.Buff();
            else
                AudioManager.instance.Heal();

            TurnManager.inst.CreateVisual($"+{change} {Translator.inst.Translate(stat.ToString())}", this.transform.localPosition);
            answer = AutoTranslate.Increase_Stat(this.name, change.ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
        }
        else if (logged >= 0 && change < 0)
        {
            if (stat == Stats.DefenseIcon || stat == Stats.PowerIcon)
                AudioManager.instance.Nerf();
            else
                AudioManager.instance.Damage();

            TurnManager.inst.CreateVisual($"{change} {Translator.inst.Translate(stat.ToString())}", this.transform.localPosition);
            answer = AutoTranslate.Decrease_Stat(this.name, Mathf.Abs(change).ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
        }
        if (_privStatMod[Stats.HealthIcon] <= 0)
            yield return HasDied(logged, cause);
    }
    public IEnumerator ChangeEffect(StatusRound newOne, int logged, Character cause)
    {
        if (this == null) yield break;
        allStatusRounds.Add(newOne);

        if (logged >= 0) 
        {
            AudioManager.instance.Change();
            TurnManager.inst.CreateVisual(Translator.inst.Translate(newOne.effect.ToString()), this.transform.localPosition);
        }
        Log.instance.AddText(AutoTranslate.Change_Status(this.name, Translator.inst.Translate(newOne.effect.ToString()), newOne.roundNumber.ToString()), logged);
        if (newOne.roundNumber == TurnManager.inst.GetRound(0))
            StatusUI();
        
        if (FightRules.inst.CheckRule(nameof(AutoTranslate.Aura), logged) is RulesData aura)
        {
            yield return ChangeStat(Stats.DefenseIcon, aura.defenseStat, logged+1, null);
        }
    }
    public IEnumerator ChangePosition(Position newPosition, int logged, Character cause)
    {
        if (this == null || newPosition == CurrentPosition) yield break;

        if (IsStatus(StatusEffect.Locked))
        {
            AudioManager.instance.Blocked();
            Log.instance.AddText(AutoTranslate.Blocked_Position(this.name), logged);
        }
        else
        {
            CurrentPosition = newPosition;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                string change = AutoTranslate.Become_New(this.name, Translator.inst.Translate(newPosition.ToString()));
                Log.instance.AddText(change, logged);
                TurnManager.inst.CreateVisual(Translator.inst.Translate(newPosition.ToString()), this.transform.localPosition);
                AudioManager.instance.Change();
            }

            if (cause != null)
            {
                TeamCompare compare = GetTeamCompare(this, cause);
                if (compare == TeamCompare.Opposites && FightRules.inst.CheckRule(nameof(AutoTranslate.Relocate), logged) is RulesData relocate)
                    yield return this.ChangeStat(Stats.PowerIcon, -relocate.powerStat, logged+1, null);
                else if (FightRules.inst.CheckRule(nameof(AutoTranslate.Vigilance), logged) is RulesData vigilance)
                    yield return this.ChangeStat(Stats.PowerIcon, vigilance.powerStat, logged+1, null);
            }
        }
    }
    public IEnumerator ChangeMood(Mood newMood, int logged, Character cause)
    {
        if (this == null || newMood == CurrentMood) yield break;

        if (IsStatus(StatusEffect.Locked))
        {
            AudioManager.instance.Blocked();
            Log.instance.AddText(AutoTranslate.Blocked_Mood(this.name), logged);
        }
        else
        {
            CurrentMood = newMood;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                string change = AutoTranslate.Become_New(this.name, Translator.inst.Translate(newMood.ToString()));
                Log.instance.AddText(change, logged);
                AudioManager.instance.Change();
                TurnManager.inst.CreateVisual(Translator.inst.Translate(newMood.ToString()), this.transform.localPosition);
            }

            if (cause != null)
            {
                TeamCompare compare = GetTeamCompare(this, cause);
                if (compare == TeamCompare.Opposites && FightRules.inst.CheckRule(nameof(AutoTranslate.Persuade), logged) is RulesData persuade)
                    yield return this.ChangeStat(Stats.DefenseIcon, -persuade.defenseStat, logged+1, null);
                else if (FightRules.inst.CheckRule(nameof(AutoTranslate.Intuition), logged) is RulesData intuition)
                    yield return this.ChangeStat(Stats.DefenseIcon, intuition.defenseStat, logged+1, null);
            }
        }
    }
    public IEnumerator HasDied(int logged, Character cause)
    {
        if (this == null) yield break;
        AudioManager.instance.Dead();

        foreach (Stats value in Enum.GetValues(typeof(Stats)))
            _privStatMod[value] = 0;
        _privStatMod[Stats.Max_HealthIcon] = data.maxHealth;
        allStatusRounds.Clear();
        CharacterUI();
        StatusUI();
        
        Log.instance.AddText(AutoTranslate.Died(this.name), logged+1);
        TurnManager.inst.speedQueue.Remove(this);

        if (this is PlayerCharacter)
        {
            TurnManager.inst.listOfPlayers.Remove(this);
            TurnManager.inst.listOfDead.Add(this);
        }
        else
        {
            TurnManager.inst.listOfEnemies.Remove(this);
            Destroy(this.gameObject);
        }

        if (FightRules.inst.CheckRule(nameof(AutoTranslate.Chain_Reaction), logged) is RulesData chain)
        {
            foreach (Character character in new List<Character>(TurnManager.inst.listOfPlayers))
                yield return character.ChangeStat(Stats.HealthIcon, -chain.mainNumber, logged+1, this);
            foreach (Character character in new List<Character>(TurnManager.inst.listOfEnemies))
                yield return character.ChangeStat(Stats.HealthIcon, -chain.mainNumber, logged+1, this);
        }
    }
    public IEnumerator Revive(int health, int logged)
    {
        if (this == null || this.GetHealth() > 0) yield break;

        Log.instance.AddText(AutoTranslate.Revived(this.name), logged);
        TurnManager.inst.listOfPlayers.Insert(myPosition, this);
        TurnManager.inst.speedQueue.Insert(0, this);
        TurnManager.inst.listOfDead.Remove(this);

        yield return ChangeStat(Stats.HealthIcon, health, logged, null);
        yield return ChangePosition(data.startPosition, -1,null);
        yield return ChangeMood(RandomMood(null), -1, null);
    }
    #endregion
#region Take Turn
    internal IEnumerator MyTurn(int logged)
    {
        yield return ResolveTurn(logged);
        if (GetHealth() == 0) yield break;
        if (IsStatus(StatusEffect.Blessed))
        {
            Log.instance.AddText(AutoTranslate.Use_Extra_Ability(this.name), logged);
            yield return ResolveTurn(logged);            
        }
    }
    IEnumerator ResolveTurn(int logged)
    {
        ClearAbility();
        if (IsStatus(StatusEffect.Stunned))
        {
            yield return TurnManager.inst.WaitTime();
            Log.instance.AddText(AutoTranslate.Stun_Miss_Turn(this.name), logged);
            yield break;
        }
        
        while (chosenAbility == null)
        {
            yield return ChooseAbility(logged);
            for (int i = 0; i < chosenAbility.data.toTarget.Length; i++)
            {
                yield return ChooseTarget(chosenAbility, chosenAbility.data.toTarget[i], i);
                if (chosenAbility.singleTarget.Contains(chosenAbility.data.toTarget[i]))
                    chosenTarget.Add(chosenAbility.listOfTargets[i][0]);
            }
            yield return ConfirmChoice();
        }
        yield return AbilityUse(logged);
    }
    protected virtual IEnumerator ChooseAbility(int logged)
    {
        yield return null;
    }
    protected virtual IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        yield return null;
    }
    protected virtual IEnumerator ConfirmChoice()
    {
        yield return null;
    }
    IEnumerator AbilityUse(int logged)
    {
        Log.instance.AddText(AutoTranslate.Use_Ability(Translator.inst.Translate(this.name), Translator.inst.Translate(chosenAbility.data.abilityName)), logged);
        TurnManager.inst.CreateVisual(Translator.inst.Translate(chosenAbility.data.abilityName), this.transform.localPosition);
        
        chosenAbility.killed = false;
        chosenAbility.fullHeal = false;

        for (int i = 0; i < chosenAbility.data.instructions.Length; i++)
        {
            string[] splicedString = TurnManager.SpliceString(chosenAbility.data.instructions[i], '/');
            yield return chosenAbility.ResolveInstructions(splicedString, i, logged + 1);
        }
        if (chosenAbility.data.abilityName.Equals(nameof(AutoTranslate.Wait)))
        {
            if (FightRules.inst.CheckRule(nameof(AutoTranslate.Enlightenment), logged) is RulesData enlightenment)
                yield return ChangeEffect(new StatusRound(StatusEffect.Blessed, TurnManager.inst.GetRound(enlightenment.statusNumber)), logged+1, null);
        }
        else
        {
            int newCooldown = chosenAbility.data.baseCooldown;
            if (this is PlayerCharacter && FightRules.inst.CheckRule(nameof(AutoTranslate.Bloodied), logged) is RulesData bloodied)
                newCooldown += (GetHealth() >= bloodied.miscNumber) ? bloodied.statusNumber : -bloodied.statusNumber;

            if (newCooldown > 0)
            {
                if (FightRules.inst.CheckRule(nameof(AutoTranslate.Recklessness), logged) is RulesData reckless)
                {
                    yield return ChangeStat(Stats.HealthIcon, -reckless.miscNumber*newCooldown, logged+1, null);
                }
                else
                {
                    chosenAbility.cooldownUntil = TurnManager.inst.GetRound(newCooldown);
                    string answer = AutoTranslate.Apply_Cooldown(this.name, Translator.inst.Translate(chosenAbility.data.abilityName), newCooldown.ToString());
                    Log.instance.AddText(answer, logged);
                }
            }
        }

        if (GetHealth() > 0 && this is PlayerCharacter)
        {
            if (FightRules.inst.CheckRule(nameof(AutoTranslate.Preparation), logged) is RulesData preparation)
            {
                if (chosenAbility.mainType == AbilityType.Attack || chosenAbility.mainType == AbilityType.Healing)
                    yield return ChangeStat(Stats.PowerIcon, -preparation.mainNumber, logged+1, null);
                else
                    yield return ChangeStat(Stats.PowerIcon, preparation.secondNumber, logged+1, null);
            }
            if (FightRules.inst.CheckRule(nameof(AutoTranslate.Doomed), logged) is RulesData doomed)
            {
                yield return ChangeStat(Stats.Max_HealthIcon, -doomed.secondNumber, logged+1, null);
            }
        }
        if (ScreenOverlay.instance.mode == GameMode.Tutorial && TutorialManager.instance.currentCharacter == this)
        {
            TutorialManager.instance.currentCharacter = null;
            yield return TutorialManager.instance.NextStep();
        }
    }
    protected void ClearAbility()
    {
        chosenAbility?.listOfTargets.Clear();
        chosenAbility = null;
        chosenTarget.Clear();        
    }
#endregion
#region UI
    private void FixedUpdate()
    {
        if (ScreenOverlay.instance.mode != GameMode.Other)
        {
            this.border.SetAlpha(borderColor);
            ScreenPosition();
        }
        else if (this.border != null)
        {
            this.border.SetAlpha(0);
        }
    }
    void ScreenPosition()
    {
        if (CurrentPosition == Position.Elevated)
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (this is EnemyCharacter) ? 375 : -500;
            newPosition.y = startingPosition + (20 * Mathf.Cos(Time.time * 3f));
            transform.localPosition = newPosition;
        }
        else
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (this is EnemyCharacter) ? 300 : -560;
            newPosition.y = startingPosition;
            transform.localPosition = newPosition;
        }
    }
    public void CharacterUI()
    {
        if (ScreenOverlay.instance.mode == GameMode.Other)
        {
            this.myImage.color = Color.white;
        }
        else if (GetHealth() <= 0)
        {
            this.myImage.color = Color.gray;
        }
        else
        {
            Color newColor = KeywordTooltip.instance.SearchForKeyword(CurrentMood.ToString()).color;
            this.myImage.color = new Color(newColor.r, newColor.g, newColor.b);
        }
        if (topText == null || statusText == null)
            return;

        topText.text = "";
        healthText.text = KeywordTooltip.instance.EditText($"{_privStatMod[Stats.HealthIcon]}/{_privStatMod[Stats.Max_HealthIcon]} {AutoTranslate.HealthIcon()}");

        if (GetHealth() <= 0)
        {
            topText.text = AutoTranslate.Dead();
        }
        else
        {
            topText.text = $"{Translator.inst.Translate(CurrentMood.ToString())}, {Translator.inst.Translate(CurrentPosition.ToString())}\n";
            
            AddToTopText(CalculatePower(), AutoTranslate.PowerIcon());
            AddToTopText(CalculateDefense(), AutoTranslate.DefenseIcon());

            void AddToTopText(int amount, string text)
            {
                if (amount > 0)
                    topText.text += $"+{amount} {text} ";
                else
                    topText.text += $"{amount} {text} ";
            }
            topText.text = KeywordTooltip.instance.EditText(topText.text);
        }
    }
    public void StatusUI()
    {
        statusText.text = "";
        foreach (StatusEffect value in Enum.GetValues(typeof(StatusEffect)))
        {
            if (IsStatus(value))
                statusText.text += $"{value}Image ";
        }
        statusText.text = KeywordTooltip.instance.EditText(statusText.text, true);        
    }
    #endregion
}