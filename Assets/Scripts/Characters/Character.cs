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
        this.name = CarryVariables.instance.Translate(characterData.myName);
        nameText.text = this.name;
        editedDescription = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate($"{characterData.myName} Text"));

        this.baseHealth = data.baseHealth;
        this.currentHealth = this.baseHealth;
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{CarryVariables.instance.Translate("Health")}");

        this.myImage.sprite = Resources.Load<Sprite>($"Characters/{characterData.myName}");
        AddAbility(CarryVariables.instance.FindEnemyAbility("Skip Turn"), true, false);
        if (this is PlayerCharacter)
            AddAbility(CarryVariables.instance.FindEnemyAbility("Revive"), true, false);

        StartCoroutine(ChangePosition(data.startingPosition, -1));
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
            TurnManager.instance.CreateVisual(CarryVariables.instance.Translate("Blocked"), this.transform.localPosition);
            answer = CarryVariables.instance.Translate("Blocked Stat Drop", new()
            {
                ("This", this.name), ("Num", effect.ToString()), ("Stat", CarryVariables.instance.Translate("Max Health"))
            });

            Log.instance.AddText(answer, logged);
            yield break;
        }

        baseHealth += effect;
        if (effect > 0)
        {
            TurnManager.instance.CreateVisual($"+{effect} {CarryVariables.instance.Translate("Max Health")}", this.transform.localPosition);
            answer = CarryVariables.instance.Translate("Increase Stat", new()
            { ("This", this.name), ("Num", effect.ToString()), ("Stat", CarryVariables.instance.Translate("Max Health"))});
            Log.instance.AddText(answer, logged);
        }
        else
        {
            TurnManager.instance.CreateVisual($"{effect} {CarryVariables.instance.Translate("Max Health")}", this.transform.localPosition);
            answer = CarryVariables.instance.Translate("Decrease Stat", new()
            { ("This", this.name), ("Num", Mathf.Abs(effect).ToString()), ("Stat", CarryVariables.instance.Translate("Max Health"))});
            Log.instance.AddText(answer, logged);

            if (baseHealth < currentHealth)
                currentHealth = baseHealth;
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{CarryVariables.instance.Translate("Health")}");
    }

    public IEnumerator ChangeHealth(int change, int logged, Character attacker = null)
    {
        if (this == null || change == 0) yield break;
        string answer = "";

        if (change > 0)
        {
            currentHealth = Mathf.Clamp(currentHealth += change, 0, this.baseHealth);
            TurnManager.instance.CreateVisual($"+{change} {CarryVariables.instance.Translate("Health")}", this.transform.localPosition);

            answer = CarryVariables.instance.Translate("Increase Stat", new()
            { ("This", this.name), ("Num", change.ToString()), ("Stat", CarryVariables.instance.Translate("Health"))});
            Log.instance.AddText(answer, logged);
        }
        else if (statEffectDict[StatusEffect.Protected] > 0)
        {
            TurnManager.instance.CreateVisual(CarryVariables.instance.Translate("Blocked"), this.transform.localPosition);
            answer = CarryVariables.instance.Translate("Blocked Stat Drop", new()
            { ("This", this.name), ("Num", change.ToString()), ("Stat", CarryVariables.instance.Translate("Health"))});
            Log.instance.AddText(answer, logged);
        }
        else
        {
            if (attacker != null && ((this is EnemyCharacter && attacker is PlayerCharacter) || (this is PlayerCharacter && attacker is EnemyCharacter)))
                lastToAttackThis = attacker;

            currentHealth -= Mathf.Abs(change);
            TurnManager.instance.CreateVisual($"{change} {CarryVariables.instance.Translate("Health")}", this.transform.localPosition);

            answer = CarryVariables.instance.Translate("Decrease Stat", new()
            { ("This", this.name), ("Num", Mathf.Abs(change).ToString()), ("Stat", CarryVariables.instance.Translate("Health"))});
            Log.instance.AddText(answer, logged);

            if (currentHealth <= 0)
                yield return HasDied(logged);
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}{CarryVariables.instance.Translate("Health")}");
    }

    public IEnumerator ChangeStat(Stats stat, int change, int logged)
    {
        if (this == null || change == 0) yield break;
        string answer = "";

        if (statEffectDict[StatusEffect.Protected] > 0 && change < 0)
        {
            TurnManager.instance.CreateVisual(CarryVariables.instance.Translate("Blocked"), this.transform.localPosition);
            answer = CarryVariables.instance.Translate("Blocked Stat Drop", new()
            { ("This", this.name), ("Num", change.ToString()), ("Stat", CarryVariables.instance.Translate(stat.ToString()))});
            Log.instance.AddText(answer, logged);
            yield break;
        }

        _privStatMod[stat] = Math.Clamp(_privStatMod[stat] += change, -4, 4);
        CharacterUI();
        if (logged >= 0 && change > 0)
        {
            TurnManager.instance.CreateVisual($"+{change} {CarryVariables.instance.Translate(stat.ToString())}", this.transform.localPosition);
            answer = CarryVariables.instance.Translate("Increase Stat", new()
            { ("This", this.name), ("Num", change.ToString()), ("Stat", CarryVariables.instance.Translate(stat.ToString()))});
            Log.instance.AddText(answer, logged);
        }
        else if (logged >= 0 && change < 0)
        {
            TurnManager.instance.CreateVisual($"{change} {CarryVariables.instance.Translate(stat.ToString())}", this.transform.localPosition);
            answer = CarryVariables.instance.Translate("Decrease Stat", new()
            { ("This", this.name), ("Num", Mathf.Abs(change).ToString()), ("Stat", CarryVariables.instance.Translate(stat.ToString()))});
            Log.instance.AddText(answer, logged);
        }
    }

    public IEnumerator ChangeEffect(StatusEffect statusEffect, int change, int logged)
    {
        if (this == null || change == 0) yield break;

        _privStatEffect[statusEffect] = _privStatEffect[statusEffect]+change;
        TurnManager.instance.CreateVisual(CarryVariables.instance.Translate(statusEffect.ToString()), this.transform.localPosition);

        if (statusEffect == StatusEffect.Extra)
        {
            Log.instance.AddText(CarryVariables.instance.Translate("Gain Extra Ability",
                new() { ("This", this.name), ("Num", _privStatEffect[statusEffect].ToString()) }), logged);
        }
        else
        {
            Log.instance.AddText(CarryVariables.instance.Translate("Change Status",
                new() { ("This", this.name), ("Status", CarryVariables.instance.Translate(statusEffect.ToString())), ("Num", _privStatEffect[statusEffect].ToString()) }), logged);
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
            Log.instance.AddText(CarryVariables.instance.Translate("Blocked Position", new() { ("This", this.name)}), logged);
        }
        else
        {
            CurrentPosition = newPosition;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                string change = CarryVariables.instance.Translate("Become New", new()
                { ("This", this.name), ("Change", CarryVariables.instance.Translate(newPosition.ToString())) });
                Log.instance.AddText(change, logged);
                TurnManager.instance.CreateVisual(CarryVariables.instance.Translate(newPosition.ToString()), this.transform.localPosition);
            }
        }
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, int logged)
    {
        if (this == null || newEmotion == Emotion.Dead || newEmotion == CurrentEmotion) yield break;

        if (statEffectDict[StatusEffect.Locked] > 0)
        {
            Log.instance.AddText(CarryVariables.instance.Translate("Blocked Emotion", new() { ("This", this.name) }), logged);
        }
        else
        {
            CurrentEmotion = newEmotion;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                string change = CarryVariables.instance.Translate("Become New", new()
                { ("This", this.name), ("Change", CarryVariables.instance.Translate(newEmotion.ToString())) });
                Log.instance.AddText(change, logged);
                TurnManager.instance.CreateVisual(CarryVariables.instance.Translate(newEmotion.ToString()), this.transform.localPosition);
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

        Log.instance.AddText(CarryVariables.instance.Translate("Died", new() { ("This", this.name)}), logged+1);
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

        Log.instance.AddText(CarryVariables.instance.Translate("Revived", new() { ("This", this.name)}), logged);
        TurnManager.instance.listOfPlayers.Add(this);
        TurnManager.instance.speedQueue.Insert(0, this);
        TurnManager.instance.listOfDead.Remove(this);

        yield return ChangeHealth(health, logged);
        yield return ChangePosition(data.startingPosition, -1);
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
            Log.instance.AddText(CarryVariables.instance.Translate("Use Extra Ability", new() { ("This", this.name) }), logged);

            CharacterUI();
            yield return ResolveTurn(logged, true);
        }
    }

    IEnumerator Timer()
    {
        timer = 10f;
        if (this is PlayerCharacter && CarryVariables.instance.ActiveChallenge("Player Timer"))
        {
            TurnManager.instance.timerText.gameObject.SetActive(true);
            while (timer > 0f)
            {
                yield return null;
                timer -= Time.deltaTime;
                TurnManager.instance.timerText.text = $"Timer: {timer:F1}";
            }

            TextCollector[] allCollectors = FindObjectsByType<TextCollector>(FindObjectsSortMode.None);
            foreach (TextCollector collector in allCollectors)
                Destroy(collector.gameObject);

            Log.instance.AddText(CarryVariables.instance.Translate("Out of Time", new() { ("This", this.name) }), 0);
            Log.instance.AddText(CarryVariables.instance.Translate("Skip Turn Log", new() { ("This", this.name)}), 0);
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
            Log.instance.AddText(CarryVariables.instance.Translate("Miss Turn", new() { ("This", this.name)}), 0);
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

            for (int i = 0; i < chosenAbility.data.defaultTargets.Length; i++)
            {
                yield return ChooseTarget(chosenAbility, chosenAbility.data.defaultTargets[i], i);
                if (chosenAbility.singleTarget.Contains(chosenAbility.data.defaultTargets[i]))
                    chosenTarget.Add(chosenAbility.listOfTargets[i][0]);
            }

            if (timer < 0f)
                yield break;

            if (this is PlayerCharacter)
            {
                string result = "";
                if (chosenTarget.Count == 0)
                {
                    result = CarryVariables.instance.Translate("Confirm No Target", new() { ("Ability", CarryVariables.instance.Translate(chosenAbility.data.myName)) });
                }
                else
                {
                    result = CarryVariables.instance.Translate("Confirm Target", new() { ("Ability", CarryVariables.instance.Translate(chosenAbility.data.myName)) });
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

        Log.instance.AddText(Log.Substitute(chosenAbility, this, (chosenTarget.Count > 0) ? chosenTarget[0] : null), logged);
        if (!chosenAbility.data.myName.Equals("Skip Turn"))
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
                (CarryVariables.instance.ActiveCheat("Slower Enemy Cooldowns") && this is EnemyCharacter ? 1 : 0);

            if (cooldownIncrease > 0)
            {
                chosenAbility.currentCooldown = cooldownIncrease;
                string answer = CarryVariables.instance.Translate("Apply Cooldown", new()
                {
                    ("Target", this.name), ("Ability", CarryVariables.instance.Translate(chosenAbility.data.myName)), ("MiscStat", cooldownIncrease.ToString())
                });
                Log.instance.AddText(answer, logged);
            }
            if (CarryVariables.instance.mode == CarryVariables.GameMode.Tutorial && TutorialManager.instance.currentCharacter == this)
            {
                TutorialManager.instance.currentCharacter = null;
                yield return TutorialManager.instance.NextStep();
            }
        }
        yield return EmotionEffect(logged, extraAbility);
    }

    IEnumerator EmotionEffect(int logged, bool extraAbility)
    {
        if (timer > 0f && chosenAbility != null && !chosenAbility.data.myName.Equals("Skip Turn"))
        {
            string answer = CarryVariables.instance.Translate("Emotion Effect",
                new() { ("This", this.name), ("Emotion", CarryVariables.instance.Translate(this.CurrentEmotion.ToString())) });

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
        if (CarryVariables.instance.mode != CarryVariables.GameMode.Other)
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
            topText.text = CarryVariables.instance.Translate("Dead");
        }
        else
        {
            topText.text = $"{CarryVariables.instance.Translate(CurrentEmotion.ToString())}, " +
                $"{CarryVariables.instance.Translate(CurrentPosition.ToString())}\n";

            AddToTopText(CalculatePower(), CarryVariables.instance.Translate("Power"));
            AddToTopText(CalculateDefense(), CarryVariables.instance.Translate("Defense"));

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