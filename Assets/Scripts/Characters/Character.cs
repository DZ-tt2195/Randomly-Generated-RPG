using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;
using System.Linq;

public enum Stats { Power, Defense };
public enum StatusEffect { Stunned, Targeted, Locked, Protected, Extra };
public enum Position { Grounded, Elevated, Dead };
public enum Emotion { Dead, Neutral, Happy, Angry, Sad };

[RequireComponent(typeof(Button))][RequireComponent(typeof(Image))]
public class Character : MonoBehaviour
{

#region Variables

    public static float borderColor;

    [Foldout("Character info", true)]
        protected Ability chosenAbility;
        protected List<Character> chosenTarget = new();
        protected float timer;
        [ReadOnly] public Character lastToAttackThis { get; private set; }
        [ReadOnly] public string editedDescription { get; private set; }
        [ReadOnly] public CharacterData data { get; private set; }
        [ReadOnly] public List<Ability> listOfAutoAbilities = new();
        [ReadOnly] public List<Ability> listOfRandomAbilities = new();

    [Foldout("Stats", true)]
        protected int baseHealth;
        public int currentHealth { get; private set; }
        protected int baseSpeed;

        private Dictionary<StatusEffect, int> _privStatEffect = new();
        public IReadOnlyDictionary<StatusEffect, int> statEffectDict => _privStatEffect;
        private Dictionary<Stats, int> _privStatMod = new();

        public Position CurrentPosition { get; private set; }
        public Emotion CurrentEmotion { get; private set; }

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
            _privStatEffect.Add(value, 0);
        foreach (Stats value in Enum.GetValues(typeof(Stats)))
            _privStatMod.Add(value, 0);
    }

    public void SetupCharacter(CharacterData characterData, List<AbilityData> listOfAbilityData, Emotion startingEmotion, bool abilitiesBeginWithCooldown)
    {
        data = characterData;
        this.name = AutoTranslate.DoEnum(characterData.characterName);
        nameText.text = this.name;
        editedDescription = KeywordTooltip.instance.EditText(Translator.inst.Translate($"{characterData.characterName}_Text"));

        this.baseHealth = data.baseHealth;
        this.currentHealth = this.baseHealth;
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{AutoTranslate.DoEnum(ToTranslate.Health)}");

        this.myImage.sprite = this.data.sprite;
        AddAbility(GameFiles.inst.FindEnemyAbility(ToTranslate.Skip_Turn), true, false);
        if (this is PlayerCharacter)
            AddAbility(GameFiles.inst.FindEnemyAbility(ToTranslate.Revive), true, false);

        StartCoroutine(ChangePosition(data.startPosition, -1));
        StartCoroutine(ChangeEmotion(startingEmotion, -1));

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

    public float CalculateHealthPercent()
    {
        return (float)currentHealth / this.baseHealth;
    }

    public int CalculatePower()
    {
        return _privStatMod[Stats.Power] + (CurrentEmotion == Emotion.Angry ? 1 : 0);
    }

    public int CalculateDefense()
    {
        return _privStatMod[Stats.Defense] + (CurrentEmotion == Emotion.Angry ? 1 : 0);
    }

    public int CalculateStatTotals()
    {
        return (CalculatePower() + CalculateDefense());
    }

    public IEnumerator ChangeMaxHealth(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;
        string answer = "";

        if (statEffectDict[StatusEffect.Protected] > 0 && effect < 0)
        {
            TurnManager.instance.CreateVisual(AutoTranslate.DoEnum(ToTranslate.Blocked), this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Drop(this.name, effect.ToString(), AutoTranslate.DoEnum(ToTranslate.Max_Health));
            Log.instance.AddText(answer, logged);
            yield break;
        }

        baseHealth += effect;
        if (effect > 0)
        {
            TurnManager.instance.CreateVisual($"+{effect} {AutoTranslate.DoEnum(ToTranslate.Max_Health)}", this.transform.localPosition);
            answer = AutoTranslate.Increase_Stat(this.name, effect.ToString(), AutoTranslate.DoEnum(ToTranslate.Max_Health));
            Log.instance.AddText(answer, logged);
        }
        else
        {
            TurnManager.instance.CreateVisual($"{effect} {AutoTranslate.DoEnum(ToTranslate.Max_Health)}", this.transform.localPosition);
            answer = AutoTranslate.Decrease_Stat(this.name, Mathf.Abs(effect).ToString(), AutoTranslate.DoEnum(ToTranslate.Max_Health));
            Log.instance.AddText(answer, logged);

            if (baseHealth < currentHealth)
                currentHealth = baseHealth;
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{AutoTranslate.DoEnum(ToTranslate.Health)}");
    }

    public IEnumerator ChangeHealth(int change, int logged, Character attacker = null)
    {
        if (this == null || change == 0) yield break;
        string answer = "";

        if (change > 0)
        {
            currentHealth = Mathf.Clamp(currentHealth += change, 0, this.baseHealth);
            TurnManager.instance.CreateVisual($"+{change} {AutoTranslate.DoEnum(ToTranslate.Health)}", this.transform.localPosition);

            answer = AutoTranslate.Increase_Stat(this.name, change.ToString(), AutoTranslate.DoEnum(ToTranslate.Health));
            Log.instance.AddText(answer, logged);
        }
        else if (statEffectDict[StatusEffect.Protected] > 0)
        {
            TurnManager.instance.CreateVisual($"{AutoTranslate.DoEnum(ToTranslate.Blocked)}", this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Drop(this.name, change.ToString(), AutoTranslate.DoEnum(ToTranslate.Health));
            Log.instance.AddText(answer, logged);
        }
        else
        {
            if (attacker != null && ((this is EnemyCharacter && attacker is PlayerCharacter) || (this is PlayerCharacter && attacker is EnemyCharacter)))
                lastToAttackThis = attacker;

            currentHealth -= Mathf.Abs(change);
            TurnManager.instance.CreateVisual($"{change} {AutoTranslate.DoEnum(ToTranslate.Health)}", this.transform.localPosition);

            answer = AutoTranslate.Decrease_Stat(this.name, Mathf.Abs(change).ToString(), AutoTranslate.DoEnum(ToTranslate.Health));
            Log.instance.AddText(answer, logged);

            if (currentHealth <= 0)
                yield return HasDied(logged);
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{AutoTranslate.DoEnum(ToTranslate.Health)}");
    }

    public IEnumerator ChangeStat(Stats stat, int change, int logged)
    {
        if (this == null || change == 0) yield break;
        string answer = "";

        if (statEffectDict[StatusEffect.Protected] > 0 && change < 0)
        {
            TurnManager.instance.CreateVisual($"{AutoTranslate.DoEnum(ToTranslate.Blocked)}", this.transform.localPosition);
            answer = AutoTranslate.Blocked_Stat_Drop(this.name, change.ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
            yield break;
        }

        _privStatMod[stat] = Math.Clamp(_privStatMod[stat] += change, -4, 4);
        CharacterUI();
        if (logged >= 0 && change > 0)
        {
            TurnManager.instance.CreateVisual($"+{change} {Translator.inst.Translate(stat.ToString())}", this.transform.localPosition);
            answer = AutoTranslate.Increase_Stat(this.name, change.ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
        }
        else if (logged >= 0 && change < 0)
        {
            TurnManager.instance.CreateVisual($"{change} {Translator.inst.Translate(stat.ToString())}", this.transform.localPosition);
            answer = AutoTranslate.Decrease_Stat(this.name, Mathf.Abs(change).ToString(), Translator.inst.Translate(stat.ToString()));
            Log.instance.AddText(answer, logged);
        }
    }

    public IEnumerator ChangeEffect(StatusEffect statusEffect, int change, int logged)
    {
        if (this == null || change == 0) yield break;

        _privStatEffect[statusEffect] = _privStatEffect[statusEffect]+change;
        TurnManager.instance.CreateVisual(Translator.inst.Translate(statusEffect.ToString()), this.transform.localPosition);

        if (statusEffect == StatusEffect.Extra)
        {
            Log.instance.AddText(AutoTranslate.Gain_Extra_Ability(this.name, _privStatEffect[statusEffect].ToString()), logged);
        }
        else
        {
            Log.instance.AddText(AutoTranslate.Change_Status(this.name, Translator.inst.Translate(statusEffect.ToString()), _privStatEffect[statusEffect].ToString()), logged);
            
            if (statusEffect == StatusEffect.Targeted)
            {
                if (this is PlayerCharacter)
                    TurnManager.instance.targetedPlayer = this;
                else
                    TurnManager.instance.targetedEnemy = this;
            }
        }
        CharacterUI();
    }

    public IEnumerator ChangePosition(Position newPosition, int logged)
    {
        if (this == null || newPosition == Position.Dead || newPosition == CurrentPosition) yield break;

        if (statEffectDict[StatusEffect.Locked] > 0)
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
                TurnManager.instance.CreateVisual(Translator.inst.Translate(newPosition.ToString()), this.transform.localPosition);
            }
        }
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, int logged)
    {
        if (this == null || newEmotion == Emotion.Dead || newEmotion == CurrentEmotion) yield break;

        if (statEffectDict[StatusEffect.Locked] > 0)
        {
            Log.instance.AddText(AutoTranslate.Blocked_Emotion(this.name), logged);
        }
        else
        {
            CurrentEmotion = newEmotion;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                string change = AutoTranslate.Become_New(this.name, Translator.inst.Translate(newEmotion.ToString()));
                Log.instance.AddText(change, logged);
                TurnManager.instance.CreateVisual(Translator.inst.Translate(newEmotion.ToString()), this.transform.localPosition);
            }
        }
    }

    public IEnumerator HasDied(int logged)
    {
        if (this == null) yield break;

        if (this == TurnManager.instance.targetedPlayer)
            TurnManager.instance.targetedPlayer = null;
        if (this == TurnManager.instance.targetedEnemy)
            TurnManager.instance.targetedEnemy = null;

        baseHealth = data.baseHealth;
        currentHealth = 0;

        foreach (StatusEffect value in Enum.GetValues(typeof(StatusEffect)))
            _privStatEffect[value] = 0;
        foreach (Stats value in Enum.GetValues(typeof(Stats)))
            _privStatMod[value] = 0;

        CurrentPosition = Position.Dead;
        CurrentEmotion = Emotion.Dead;
        CharacterUI();

        Log.instance.AddText(AutoTranslate.Died(this.name), logged+1);
        TurnManager.instance.speedQueue.Remove(this);

        if (this is PlayerCharacter)
        {
            TurnManager.instance.listOfPlayers.Remove(this);
            TurnManager.instance.listOfDead.Add(this);
        }
        else
        {
            TurnManager.instance.listOfEnemies.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Revive(int health, int logged)
    {
        if (this == null || this.currentHealth > 0) yield break;

        Log.instance.AddText(AutoTranslate.Revived(this.name), logged);
        TurnManager.instance.listOfPlayers.Add(this);
        TurnManager.instance.speedQueue.Insert(0, this);
        TurnManager.instance.listOfDead.Remove(this);

        yield return ChangeHealth(health, logged);
        yield return ChangePosition(data.startPosition, -1);
        yield return ChangeEmotion((Emotion)UnityEngine.Random.Range(1, 5), -1);
    }

    #endregion

#region Take Turn

    internal IEnumerator MyTurn(int logged)
    {
        chosenAbility = null;
        chosenTarget = new();

        if (_privStatEffect[StatusEffect.Protected] > 0)
            _privStatEffect[StatusEffect.Protected]--;
        if (_privStatEffect[StatusEffect.Targeted] > 0)
            _privStatEffect[StatusEffect.Targeted]--;
        if (_privStatEffect[StatusEffect.Locked] > 0)
            _privStatEffect[StatusEffect.Locked]--;
        CharacterUI();

        yield return ResolveTurn(logged, false);
        while (statEffectDict[StatusEffect.Extra] > 0 && TurnManager.instance.listOfEnemies.Count > 0)
        {
            _privStatEffect[StatusEffect.Extra]--;
            Log.instance.AddText(AutoTranslate.Use_Extra_Ability(this.name), logged);

            CharacterUI();
            yield return ResolveTurn(logged, true);
        }
    }

    IEnumerator Timer()
    {
        timer = 10f;
        if (this is PlayerCharacter && ScreenOverlay.instance.ActiveChallenge(ToTranslate.Player_Timer))
        {
            TurnManager.instance.timerText.gameObject.SetActive(true);
            while (timer > 0f)
            {
                yield return null;
                timer -= Time.deltaTime;
                TurnManager.instance.timerText.text = AutoTranslate.Counting_Down($"{timer:F1}");
            }

            TextCollector[] allCollectors = FindObjectsByType<TextCollector>(FindObjectsSortMode.None);
            foreach (TextCollector collector in allCollectors)
                Destroy(collector.gameObject);

            Log.instance.AddText(AutoTranslate.Out_of_Time(this.name));
            Log.instance.AddText(AutoTranslate.Skip_Turn_Log(this.name));
        }
        else
        {
            TurnManager.instance.timerText.gameObject.SetActive(false);
        }
    }

    IEnumerator ResolveTurn(int logged, bool extraAbility)
    {
        if (statEffectDict[StatusEffect.Stunned] > 0)
        {
            yield return TurnManager.instance.WaitTime();
            _privStatEffect[StatusEffect.Stunned]--;
            CharacterUI();
            Log.instance.AddText(AutoTranslate.Miss_Turn(this.name), 0);
            yield break;
        }

        StartCoroutine(nameof(Timer));
        chosenAbility = null;
        chosenTarget = new();

        while (chosenAbility == null)
        {
            if (timer < 0f)
                yield break;

            yield return ChooseAbility(logged, extraAbility);

            if (timer < 0f)
                yield break;

            for (int i = 0; i < chosenAbility.data.toTarget.Length; i++)
            {
                yield return ChooseTarget(chosenAbility, chosenAbility.data.toTarget[i], i);
                if (chosenAbility.singleTarget.Contains(chosenAbility.data.toTarget[i]))
                    chosenTarget.Add(chosenAbility.listOfTargets[i][0]);
            }

            if (timer < 0f)
                yield break;

            if (this is PlayerCharacter)
            {
                string result = "";
                if (chosenTarget.Count == 0)
                {
                    result = AutoTranslate.Confirm_No_Target(AutoTranslate.DoEnum(chosenAbility.data.abilityName));
                }
                else
                {
                    result = AutoTranslate.Confirm_Target(AutoTranslate.DoEnum(chosenAbility.data.abilityName));
                    result += " ";
                    result += string.Join(" + ", chosenTarget.Select(target => target.name));
                }

                yield return TurnManager.instance.ConfirmUndo(result, new Vector3(0, 400));
                if (TurnManager.instance.confirmChoice == 1)
                {
                    chosenAbility.listOfTargets.Clear();
                    chosenAbility = null;
                    chosenTarget.Clear();
                }
                if (timer < 0f)
                    yield break;
            }
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

    IEnumerator AbilityUse(int logged, bool extraAbility)
    {
        StopCoroutine(nameof(Timer));
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
        if (!chosenAbility.data.abilityName.Equals(ToTranslate.Skip_Turn))
        {
            chosenAbility.killed = false;
            chosenAbility.fullHeal = false;
            chosenAbility.damageDealt = 0;

            for (int i = 0; i < chosenAbility.data.instructions.Length; i++)
            {
                string[] splicedString = TurnManager.SpliceString(chosenAbility.data.instructions[i], '/');
                yield return chosenAbility.ResolveInstructions(splicedString, i, logged + 1);
            }

            int cooldownIncrease = chosenAbility.data.baseCooldown + (CurrentEmotion == Emotion.Happy ? 1 : 0) +
                (ScreenOverlay.instance.ActiveCheat(ToTranslate.Slower_Enemy_Cooldowns) && this is EnemyCharacter ? 1 : 0);

            if (cooldownIncrease > 0)
            {
                chosenAbility.currentCooldown = cooldownIncrease;
                string answer = AutoTranslate.Apply_Cooldown(this.name, AutoTranslate.DoEnum(chosenAbility.data.abilityName), cooldownIncrease.ToString());
                Log.instance.AddText(answer, logged);
            }
            if (ScreenOverlay.instance.mode == GameMode.Tutorial && TutorialManager.instance.currentCharacter == this)
            {
                TutorialManager.instance.currentCharacter = null;
                yield return TutorialManager.instance.NextStep();
            }
        }
        yield return EmotionEffect(logged, extraAbility);
    }

    IEnumerator EmotionEffect(int logged, bool extraAbility)
    {
        if (timer > 0f && chosenAbility != null && !chosenAbility.data.abilityName.Equals("Skip Turn"))
        {
            string answer = AutoTranslate.Emotion_Effect(this.name, Translator.inst.Translate(CurrentEmotion.ToString()));

            if (this.CurrentEmotion == Emotion.Angry)
            {
                if (chosenAbility.killed || chosenAbility.fullHeal)
                {
                    Log.instance.AddText(answer, logged);
                    yield return ChangeEffect(StatusEffect.Stunned, 1, logged + 1);
                }
            }
            else if (this.CurrentEmotion == Emotion.Happy)
            {
                if (!extraAbility && chosenAbility.mainType != AbilityType.Attack)
                {
                    Log.instance.AddText(answer, logged);
                    yield return ChangeEffect(StatusEffect.Extra, 1, logged+1);
                }
            }
            else if (this.CurrentEmotion == Emotion.Sad)
            {
                Log.instance.AddText(answer, logged);
                yield return ChangeHealth(chosenAbility.mainType == AbilityType.Attack ? 2 : -2, logged + 1);
            }
        }
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
        if (topText == null || statusText == null)
            return;

        if (CurrentEmotion == Emotion.Neutral)
        {
            this.myImage.color = Color.white;
        }
        else if (CurrentEmotion == Emotion.Dead)
        {
            this.myImage.color = Color.gray;
        }
        else
        {
            Color newColor = KeywordTooltip.instance.SearchForKeyword(CurrentEmotion.ToString()).color;
            this.myImage.color = new Color(newColor.r, newColor.g, newColor.b);
        }

        topText.text = "";
        if (CurrentPosition == Position.Dead)
        {
            topText.text = AutoTranslate.DoEnum(ToTranslate.Dead);
        }
        else
        {
            topText.text = $"{Translator.inst.Translate(CurrentEmotion.ToString())}, {Translator.inst.Translate(CurrentPosition.ToString())}\n";
            
            AddToTopText(CalculatePower(), AutoTranslate.DoEnum(ToTranslate.Power));
            AddToTopText(CalculateDefense(), AutoTranslate.DoEnum(ToTranslate.Defense));

            void AddToTopText(int amount, string text)
            {
                if (amount > 0)
                    topText.text += $"{amount} {text} ";
                else
                    topText.text += $"{amount} {text} ";
            }
            topText.text = KeywordTooltip.instance.EditText(topText.text);
        }

        statusText.text = "";

        for (int i = 0; i < statEffectDict[StatusEffect.Extra]; i++)
            statusText.text += "ExtraImage";
        for (int i = 0; i < statEffectDict[StatusEffect.Protected]; i++)
            statusText.text += "ProtectedImage";
        for (int i = 0; i < statEffectDict[StatusEffect.Locked]; i++)
            statusText.text += "LockedImage";

        if (TurnManager.instance != null)
        {
            if (statEffectDict[StatusEffect.Targeted] == 0 && TurnManager.instance.targetedPlayer == this)
                TurnManager.instance.targetedPlayer = null;
            if (statEffectDict[StatusEffect.Targeted] == 0 && TurnManager.instance.targetedEnemy == this)
                TurnManager.instance.targetedEnemy = null;
            for (int i = 0; i < statEffectDict[StatusEffect.Targeted]; i++)
                statusText.text += "TargetedImage";
        }
        else
        {
            _privStatEffect[StatusEffect.Targeted] = 0;
        }

        for (int i = 0; i < statEffectDict[StatusEffect.Stunned]; i++)
            statusText.text += "StunnedImage";

        statusText.text = KeywordTooltip.instance.EditText(statusText.text, true);
    }

    #endregion

}