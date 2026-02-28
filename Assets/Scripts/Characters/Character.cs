using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;
using System.Linq;

public enum Stats { Power, Defense };
public enum StatusEffect { Targeted, Locked, Protected, Extra, Stunned };
public enum Position { Grounded, Elevated };
public enum Mood { Lively, Focused, Tired };

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
        protected int baseHealth;
        public int currentHealth { get; private set; }
        protected int baseSpeed;
        private Dictionary<StatusEffect, int> _privStateffect = new();
        public IReadOnlyDictionary<StatusEffect, int> StatEffectdict => _privStateffect;
        private Dictionary<Stats, int> _privStatMod = new();
        public Position CurrentPosition { get; private set; }
        public Mood CurrentMood { get; private set; }

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
    private void Awake()
    {
        myImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        border = transform.Find("border").GetComponent<Image>();
        statusText = transform.Find("Status Text").GetComponent<TMP_Text>();
        healthText = transform.Find("Health Text").GetChild(0).GetComponent<TMP_Text>();
        nameText = transform.Find("Name Text").GetChild(0).GetComponent<TMP_Text>();
        topText = transform.Find("Top Text").GetComponent<TMP_Text>();

        foreach (StatusEffect value in Enum.GetValues(typeof(StatusEffect)))
            _privStateffect.Add(value, 0);
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

        this.baseHealth = data.baseHealth;
        this.currentHealth = this.baseHealth;
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{AutoTranslate.Health()}");

        this.myImage.sprite = this.data.sprite;
        AddAbility(GameFiles.inst.FindEnemyAbility(nameof(AutoTranslate.Skip_Turn)), true, false);
        if (this is PlayerCharacter)
            AddAbility(GameFiles.inst.FindEnemyAbility(nameof(AutoTranslate.Revive)), true, false);

        StartCoroutine(ChangePosition(data.startPosition, -1));
        StartCoroutine(ChangeMood(startingEmotion, -1));

        foreach (AbilityData data in listOfAbilityData)
            AddAbility(data, false, abilitiesBeginWithCooldown);
        listOfRandomAbilities = listOfRandomAbilities.OrderBy(o => o.mainType).ToList();
        CharacterUI();
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
    public float CalculateHealthPercent()
    {
        return (float)currentHealth / this.baseHealth;
    }
    public int CalculatePower()
    {
        return _privStatMod[Stats.Power] /*+ (CurrentEmotion == Emotion.Busy ? 1 : 0)*/;
    }
    public int CalculateDefense()
    {
        return _privStatMod[Stats.Defense] /*+ (CurrentEmotion == Emotion.Busy ? 1 : 0)*/;
    }
    public int CalculateStatTotals()
    {
        return CalculatePower() + CalculateDefense();
    }
    public IEnumerator ChangeMaxHealth(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;
        string answer = "";

        if (_privStateffect[StatusEffect.Protected] > 0 && effect < 0)
        {
            TurnManager.inst.CreateVisual(AutoTranslate.Blocked(), this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Drop(this.name, effect.ToString(), AutoTranslate.Max_Health());
            Log.instance.AddText(answer, logged);
            yield break;
        }

        baseHealth += effect;

        if (effect > 0)
        {
            if (logged >= 0)
                TurnManager.inst.CreateVisual($"+{effect} {AutoTranslate.Max_Health()}", this.transform.localPosition);
            answer = AutoTranslate.Increase_Stat(this.name, effect.ToString(), AutoTranslate.Max_Health());
            Log.instance.AddText(answer, logged);
        }
        else
        {
            if (logged >= 0)
                TurnManager.inst.CreateVisual($"{effect} {AutoTranslate.Max_Health()}", this.transform.localPosition);
            answer = AutoTranslate.Decrease_Stat(this.name, Mathf.Abs(effect).ToString(), AutoTranslate.Max_Health());
            Log.instance.AddText(answer, logged);

            if (baseHealth < currentHealth)
                currentHealth = baseHealth;
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{AutoTranslate.Health()}");
    }
    public IEnumerator ChangeHealth(int change, int logged, Character attacker = null)
    {
        if (this == null || change == 0) yield break;
        string answer = "";

        if (change > 0)
        {
            currentHealth = Mathf.Clamp(currentHealth + change, 0, this.baseHealth);
            TurnManager.inst.CreateVisual($"+{change} {AutoTranslate.Health()}", this.transform.localPosition);

            answer = AutoTranslate.Increase_Stat(this.name, change.ToString(), AutoTranslate.Health());
            Log.instance.AddText(answer, logged);
        }
        else if (_privStateffect[StatusEffect.Protected] > 0)
        {
            TurnManager.inst.CreateVisual($"{AutoTranslate.Blocked()}", this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Drop(this.name, Mathf.Abs(change).ToString(), AutoTranslate.Health());
            Log.instance.AddText(answer, logged);
        }
        else
        {
            if (attacker != null && ((this is EnemyCharacter && attacker is PlayerCharacter) || (this is PlayerCharacter && attacker is EnemyCharacter)))
                lastToAttackThis = attacker;

            currentHealth -= Mathf.Abs(change);
            TurnManager.inst.CreateVisual($"{change} {AutoTranslate.Health()}", this.transform.localPosition);

            answer = AutoTranslate.Decrease_Stat(this.name, Mathf.Abs(change).ToString(), AutoTranslate.Health());
            Log.instance.AddText(answer, logged);

            if (currentHealth <= 0)
                yield return HasDied(logged);
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{AutoTranslate.Health()}");
    }
    public IEnumerator ChangeStat(Stats stat, int change, int logged)
    {
        if (this == null || change == 0) yield break;
        string answer = "";

        if (_privStateffect[StatusEffect.Protected] > 0 && change < 0)
        {
            TurnManager.inst.CreateVisual($"{AutoTranslate.Blocked()}", this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Drop(this.name, change.ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
            yield break;
        }

        _privStatMod[stat] = Math.Clamp(_privStatMod[stat] + change, -3, 3);
        CharacterUI();
        if (logged >= 0 && change > 0)
        {
            TurnManager.inst.CreateVisual($"+{change} {Translator.inst.Translate(stat.ToString())}", this.transform.localPosition);
            answer = AutoTranslate.Increase_Stat(this.name, change.ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
        }
        else if (logged >= 0 && change < 0)
        {
            TurnManager.inst.CreateVisual($"{change} {Translator.inst.Translate(stat.ToString())}", this.transform.localPosition);
            answer = AutoTranslate.Decrease_Stat(this.name, Mathf.Abs(change).ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
        }
    }
    public IEnumerator ChangeEffect(StatusEffect statusEffect, int change, int logged)
    {
        if (this == null || change == 0) yield break;

        _privStateffect[statusEffect] = _privStateffect[statusEffect]+change;
        if (logged >= 0) TurnManager.inst.CreateVisual(Translator.inst.Translate(statusEffect.ToString()), this.transform.localPosition);

        if (statusEffect == StatusEffect.Extra)
        {
            Log.instance.AddText(AutoTranslate.Gain_Extra_Ability(this.name, _privStateffect[statusEffect].ToString()), logged);
        }
        else
        {
            Log.instance.AddText(AutoTranslate.Change_Status(this.name, Translator.inst.Translate(statusEffect.ToString()), _privStateffect[statusEffect].ToString()), logged);
            
            if (statusEffect == StatusEffect.Targeted)
            {
                if (this is PlayerCharacter)
                    TurnManager.inst.targetedPlayer = this;
                else
                    TurnManager.inst.targetedEnemy = this;
            }
        }
        CharacterUI();
    }
    public IEnumerator ChangePosition(Position newPosition, int logged)
    {
        if (this == null || newPosition == CurrentPosition) yield break;

        if (_privStateffect[StatusEffect.Locked] > 0)
        {
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
            }
        }
    }
    public IEnumerator ChangeMood(Mood newEmotion, int logged)
    {
        if (this == null || newEmotion == CurrentMood) yield break;

        if (_privStateffect[StatusEffect.Locked] > 0)
        {
            Log.instance.AddText(AutoTranslate.Blocked_Mood(this.name), logged);
        }
        else
        {
            CurrentMood = newEmotion;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                string change = AutoTranslate.Become_New(this.name, Translator.inst.Translate(newEmotion.ToString()));
                Log.instance.AddText(change, logged);
                TurnManager.inst.CreateVisual(Translator.inst.Translate(newEmotion.ToString()), this.transform.localPosition);

                if (newEmotion == Mood.Focused && FightRules.inst.CheckRule(nameof(AutoTranslate.Frustration), logged))
                {
                    yield return ChangeStat(Stats.Power, 1, logged+1);
                    yield return ChangeStat(Stats.Defense, 1, logged+1);
                }
                else if (newEmotion == Mood.Lively && FightRules.inst.CheckRule(nameof(AutoTranslate.Lucky), logged))
                {
                    yield return ChangeEffect(StatusEffect.Extra, 1, logged+1);
                }
                else if (newEmotion == Mood.Tired && FightRules.inst.CheckRule(nameof(AutoTranslate.Rest), logged))
                {
                    yield return ChangeHealth(2, logged+1);
                }
            }
        }
    }
    public IEnumerator HasDied(int logged)
    {
        if (this == null) yield break;

        if (this == TurnManager.inst.targetedPlayer)
            TurnManager.inst.targetedPlayer = null;
        if (this == TurnManager.inst.targetedEnemy)
            TurnManager.inst.targetedEnemy = null;

        baseHealth = data.baseHealth;
        currentHealth = 0;

        foreach (StatusEffect value in Enum.GetValues(typeof(StatusEffect)))
            _privStateffect[value] = 0;
        foreach (Stats value in Enum.GetValues(typeof(Stats)))
            _privStatMod[value] = 0;

        CharacterUI();
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
    }
    public IEnumerator Revive(int health, int logged)
    {
        if (this == null || this.currentHealth > 0) yield break;

        Log.instance.AddText(AutoTranslate.Revived(this.name), logged);
        TurnManager.inst.listOfPlayers.Insert(myPosition, this);
        TurnManager.inst.speedQueue.Insert(0, this);
        TurnManager.inst.listOfDead.Remove(this);

        yield return ChangeHealth(health, logged);
        yield return ChangePosition(data.startPosition, -1);
        yield return ChangeMood(RandomMood(null), -1);
    }

    #endregion

#region Take Turn
    internal IEnumerator MyTurn(int logged)
    {
        yield return ResolveTurn(logged, false);
        while (_privStateffect[StatusEffect.Extra] > 0 && TurnManager.inst.listOfEnemies.Count > 0)
        {
            _privStateffect[StatusEffect.Extra]--;
            Log.instance.AddText(AutoTranslate.Use_Extra_Ability(this.name), logged);

            CharacterUI();
            yield return ResolveTurn(logged, true);
        }
    }
    IEnumerator ResolveTurn(int logged, bool extraAbility)
    {
        if (_privStateffect[StatusEffect.Stunned] > 0)
        {
            yield return TurnManager.inst.WaitTime();
            _privStateffect[StatusEffect.Stunned]--;
            CharacterUI();
            Log.instance.AddText(AutoTranslate.Skip_Turn_Log(this.name), logged);
            yield break;
        }

        ClearAbility();
        while (chosenAbility == null)
        {
            yield return ChooseAbility(logged, extraAbility);
            for (int i = 0; i < chosenAbility.data.toTarget.Length; i++)
            {
                yield return ChooseTarget(chosenAbility, chosenAbility.data.toTarget[i], i);
                if (chosenAbility.singleTarget.Contains(chosenAbility.data.toTarget[i]))
                    chosenTarget.Add(chosenAbility.listOfTargets[i][0]);
            }
            yield return ConfirmChoice();
        }
        yield return AbilityUse(logged, extraAbility);
    }
    protected virtual IEnumerator ChooseAbility(int logged, bool extraAbility)
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
    IEnumerator AbilityUse(int logged, bool extraAbility)
    {
        if (!extraAbility)
        {
            foreach (Ability ability in listOfAutoAbilities)
            {
                if (ability.currentCooldown > 0)
                    ability.currentCooldown--;
            }
            foreach (Ability ability in listOfRandomAbilities)
            {
                if (ability.currentCooldown > 0)
                    ability.currentCooldown--;
            }
        }

        Log.instance.AddText(Log.AbilityLogged(chosenAbility, this, (chosenTarget.Count > 0) ? chosenTarget[0] : null), logged);
        chosenAbility.killed = false;
        chosenAbility.fullHeal = false;
        chosenAbility.damageDealt = 0;

        for (int i = 0; i < chosenAbility.data.instructions.Length; i++)
        {
            string[] splicedString = TurnManager.SpliceString(chosenAbility.data.instructions[i], '/');
            yield return chosenAbility.ResolveInstructions(splicedString, i, logged + 1);
        }
        if (!chosenAbility.data.abilityName.Equals(nameof(AutoTranslate.Skip_Turn)))
        {
            int newCooldown = chosenAbility.data.baseCooldown;
            if (FightRules.inst.CheckRule(nameof(AutoTranslate.Bloodied), logged))
                newCooldown += (currentHealth >= 6) ? 1 : -1;

            if (newCooldown > 0)
            {
                chosenAbility.currentCooldown = newCooldown;
                string answer = AutoTranslate.Apply_Cooldown(this.name, Translator.inst.Translate(chosenAbility.data.abilityName), newCooldown.ToString());
                Log.instance.AddText(answer, logged);
            }
        }
        if (FightRules.inst.CheckRule(nameof(AutoTranslate.Preparation), logged))
        {
            if (chosenAbility.mainType == AbilityType.Attack || chosenAbility.mainType == AbilityType.Healing)
                yield return ChangeStat(Stats.Power, -1, logged+1);
            else
                yield return ChangeStat(Stats.Power, 2, logged+1);
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
        else if (currentHealth <= 0)
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
        if (currentHealth <= 0)
        {
            topText.text = AutoTranslate.Dead();
        }
        else
        {
            topText.text = $"{Translator.inst.Translate(CurrentMood.ToString())}, {Translator.inst.Translate(CurrentPosition.ToString())}\n";
            
            AddToTopText(CalculatePower(), AutoTranslate.Power());
            AddToTopText(CalculateDefense(), AutoTranslate.Defense());

            void AddToTopText(int amount, string text)
            {
                if (amount > 0)
                    topText.text += $"+{amount} {text} ";
                else
                    topText.text += $"{amount} {text} ";
            }
            topText.text = KeywordTooltip.instance.EditText(topText.text);
        }

        statusText.text = "";

        for (int i = 0; i < _privStateffect[StatusEffect.Extra]; i++)
            statusText.text += "ExtraImage";
        for (int i = 0; i < _privStateffect[StatusEffect.Protected]; i++)
            statusText.text += "ProtectedImage";
        for (int i = 0; i < _privStateffect[StatusEffect.Locked]; i++)
            statusText.text += "LockedImage";

        if (TurnManager.inst != null)
        {
            if (_privStateffect[StatusEffect.Targeted] == 0 && TurnManager.inst.targetedPlayer == this)
                TurnManager.inst.targetedPlayer = null;
            if (_privStateffect[StatusEffect.Targeted] == 0 && TurnManager.inst.targetedEnemy == this)
                TurnManager.inst.targetedEnemy = null;
            for (int i = 0; i < _privStateffect[StatusEffect.Targeted]; i++)
                statusText.text += "TargetedImage";
        }
        else
        {
            _privStateffect[StatusEffect.Targeted] = 0;
        }

        for (int i = 0; i < _privStateffect[StatusEffect.Stunned]; i++)
            statusText.text += "StunnedImage";

        statusText.text = KeywordTooltip.instance.EditText(statusText.text, true);
    }

    #endregion

}