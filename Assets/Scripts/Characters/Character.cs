using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public enum Stats { Power, Defense, Speed, Luck };
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
        public IReadOnlyDictionary<Stats, int> statModDict => _privStatMod;

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
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}Health");
        this.baseSpeed = data.baseSpeed;

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

    internal void AddAbility(AbilityData ability, bool auto, bool startWithCooldown)
    {
        Ability newAbility = this.gameObject.AddComponent<Ability>();
        newAbility.SetupAbility(ability, startWithCooldown);
        (auto ? listOfAutoAbilities : listOfRandomAbilities).Add(newAbility);
    }

    internal void DropAbility(Ability ability)
    {
        listOfAutoAbilities.Remove(ability);
        listOfRandomAbilities.Remove(ability);
        Destroy(ability);
    }

    #endregion

#region Stats

    public float CalculateHealthPercent()
    {
        return (float)currentHealth / this.baseHealth;
    }

    public int CalculatePower()
    {
        return statModDict[Stats.Power] + (CurrentEmotion == Emotion.Angry ? 2 : 0);
    }

    public int CalculateSpeed()
    {
        return this.baseSpeed + statModDict[Stats.Speed];
    }

    public int CalculateStatTotals()
    {
        return (CalculatePower() + statModDict[Stats.Defense] + CalculateSpeed() + statModDict[Stats.Luck]);
    }

    public IEnumerator ChangeMaxHealth(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        if (statEffectDict[StatusEffect.Protected] > 0 && effect < 0)
        {
            TurnManager.instance.CreateVisual(CarryVariables.instance.Translate("Blocked"), this.transform.localPosition);
            string answer = CarryVariables.instance.Translate("Blocked Stat Drop", new()
            {
                ("This", this.name), ("Num", effect.ToString()), ("StatDrop", CarryVariables.instance.Translate("Max Health"))
            });

            Log.instance.AddText(answer, logged);
            yield break;
        }

        baseHealth += effect;
        TurnManager.instance.CreateVisual($"{effect} {CarryVariables.instance.Translate("Max Health")}", this.transform.localPosition);
        if (effect > 0)
        {
            Log.instance.AddText($"{this.name} has {effect} more Max Health.", logged);
        }
        else
        {
            Log.instance.AddText($"{this.name} has {Mathf.Abs(effect)} less Max Health.", logged);
            if (baseHealth < currentHealth)
                currentHealth = baseHealth;
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}Health");
    }

    public IEnumerator ChangeHealth(int change, int logged, Character attacker = null)
    {
        if (this == null || change == 0) yield break;

        if (change > 0)
        {
            currentHealth = Mathf.Clamp(currentHealth += change, 0, this.baseHealth);
            TurnManager.instance.CreateVisual($"+{change} Health", this.transform.localPosition);
            Log.instance.AddText($"{this.name} gains {change} Health.", logged);
        }
        else if (statEffectDict[StatusEffect.Protected] > 0)
        {
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{this.name} is Protected from {Mathf.Abs(change)} damage.", logged);
        }
        else
        {
            if (attacker != null && ((this is EnemyCharacter && attacker is PlayerCharacter) || (this is PlayerCharacter && attacker is EnemyCharacter)))
                lastToAttackThis = attacker;

            currentHealth -= Mathf.Abs(change);
            TurnManager.instance.CreateVisual($"-{Mathf.Abs(change)} Health", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} takes {Mathf.Abs(change)} damage.", logged);

            if (currentHealth <= 0)
                yield return HasDied(logged);
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}Health");
    }

    public IEnumerator ChangeStat(Stats stat, int change, int logged)
    {
        if (this == null || change == 0) yield break;

        if (statEffectDict[StatusEffect.Protected] > 0 && change < 0)
        {
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} is Protected from losing {Mathf.Abs(change)} {stat}.", logged);
            yield break;
        }

        _privStatMod[stat] = Math.Clamp(_privStatMod[stat] += change, -4, 4);
        CharacterUI();
        if (logged >= 0)
        {
            TurnManager.instance.CreateVisual($"{(change > 0 ? '+' : '-')}{Math.Abs(change)} {stat}", this.transform.localPosition);
            Log.instance.AddText($"{this.name} gets {(change > 0 ? '+' : '-')}{Math.Abs(change)} {stat}.", logged);
        }
    }

    public IEnumerator ChangeEffect(StatusEffect statusEffect, int change, int logged)
    {
        if (this == null || change == 0) yield break;

        _privStatEffect[statusEffect] += change;
        CharacterUI();
        TurnManager.instance.CreateVisual(statusEffect == StatusEffect.Extra ? "EXTRA ABILITY" : statusEffect.ToString().ToUpper(), this.transform.localPosition);
        string pluralSuffix = _privStatEffect[statusEffect] == 1 ? "" : "s";

        switch (statusEffect)
        {
            case StatusEffect.Stunned:
                Log.instance.AddText($"{this.name} is Stunned for {_privStatEffect[statusEffect]} turn{pluralSuffix}.", logged);
                break;
            case StatusEffect.Locked:
                Log.instance.AddText($"{this.name} is Locked for {_privStatEffect[statusEffect]} turn{pluralSuffix}.", logged);
                break;
            case StatusEffect.Targeted:
                if (this is PlayerCharacter)
                    TurnManager.instance.targetedPlayer = this;
                else
                    TurnManager.instance.targetedEnemy = this;
                Log.instance.AddText($"{this.name} is Targeted for {_privStatEffect[statusEffect]} turn{pluralSuffix}.", logged);
                break;
            case StatusEffect.Extra:
                string plural = _privStatEffect[statusEffect] == 1 ? "Ability" : "Abilities";
                Log.instance.AddText($"{this.name} gets {_privStatEffect[statusEffect]} Extra {plural}.", logged);
                break;
            case StatusEffect.Protected:
                Log.instance.AddText($"{this.name} is Protected for the next {_privStatEffect[statusEffect]} attack{pluralSuffix}.", logged);
                break;
        }
    }

    public IEnumerator ChangePosition(Position newPosition, int logged)
    {
        if (this == null || newPosition == Position.Dead || newPosition == CurrentPosition) yield break;

        if (statEffectDict[StatusEffect.Locked] > 0)
        {
            Log.instance.AddText($"{(this.name)}'s Position is Locked and can't change.", logged);
        }
        else
        {
            CurrentPosition = newPosition;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                Log.instance.AddText($"{(this.name)} is now {newPosition}.", logged);
                TurnManager.instance.CreateVisual($"{newPosition.ToString().ToUpper()}", this.transform.localPosition);
            }
        }
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, int logged)
    {
        if (this == null || newEmotion == Emotion.Dead || newEmotion == CurrentEmotion) yield break;

        if (statEffectDict[StatusEffect.Locked] > 0)
        {
            Log.instance.AddText($"{(this.name)}'s Emotion is Locked and can't change.", logged);
            yield break;
        }
        else
        {
            CurrentEmotion = newEmotion;
            CharacterUI();

            if (Log.instance != null && logged >= 0)
            {
                Log.instance.AddText($"{(this.name)} is now {CurrentEmotion}.", logged);
                TurnManager.instance.CreateVisual($"{CurrentEmotion.ToString().ToUpper()}", this.transform.localPosition);
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

        Log.instance.AddText($"{this.name} has died.", logged+1);
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

        Log.instance.AddText($"{(this.name)} comes back to life.", logged);
        TurnManager.instance.listOfPlayers.Add(this);
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
            Log.instance.AddText($"{(this.name)} uses another Ability.", logged);

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

            Log.instance.AddText($"{this.name} runs out of time. (Player Timer)");
            Log.instance.AddText($"{this.name} skips their turn.");
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
            Log.instance.AddText($"{this.name} is Stunned and misses a turn.", 0);
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
                string result = $"{this.name}: Use {chosenAbility.data.myName}" +
                    $"{(chosenTarget.Count > 0 ? " on " + string.Join(" + ", chosenTarget.Select(target => target.data.myName)) : "")}?";

                yield return TurnManager.instance.ConfirmUndo(result, new Vector3(0, 400));
                if (TurnManager.instance.confirmChoice == 1)
                    chosenAbility = null;
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
                string answer = CarryVariables.instance.Translate("Force Cooldown", new()
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
            if (this.CurrentEmotion == Emotion.Angry)
            {
                if (chosenAbility.killed || chosenAbility.fullHeal)
                {
                    Log.instance.AddText($"{this.name} is Angry.", logged);
                    yield return ChangeEffect(StatusEffect.Stunned, 1, logged + 1);
                }
            }
            else if (this.CurrentEmotion == Emotion.Happy)
            {
                if (!extraAbility && chosenAbility.mainType != AbilityType.Attack)
                {
                    Log.instance.AddText($"{this.name} is Happy.", logged);
                    yield return ChangeEffect(StatusEffect.Extra, 1, logged+1);
                }
            }
            else if (this.CurrentEmotion == Emotion.Sad)
            {
                Log.instance.AddText($"{this.name} is Sad.", logged);
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

            AddToTopText(CalculatePower(), "Power");
            AddToTopText(statModDict[Stats.Defense], "Defense");
            AddToTopText(CalculateSpeed(), "Speed");
            AddToTopText(statModDict[Stats.Luck], "Luck");
            void AddToTopText(int amount, string text)
            {
                if (amount > 0)
                    topText.text += $"{amount} {text}";
                else
                    topText.text += $"{amount} {text}";
            }
            topText.text = KeywordTooltip.instance.EditText(topText.text);
        }

        statusText.text = "";

        for (int i = 0; i < statEffectDict[StatusEffect.Extra]; i++)
            statusText.text += "ExtraImage";
        for (int i = 0; i < statEffectDict[StatusEffect.Protected]; i++)
            statusText.text += "ProtectedImage";
        for (int i = 0; i < statEffectDict[StatusEffect.Locked]; i++)
            statusText.text += "LockImage";

        if (TurnManager.instance != null && (TurnManager.instance.targetedPlayer == this || TurnManager.instance.targetedEnemy == this))
        {
            for (int i = 0; i < statEffectDict[StatusEffect.Targeted]; i++)
                statusText.text += "TargetedImage";
        }
        else
        {
            _privStatEffect[StatusEffect.Targeted] = 0;
        }

        for (int i = 0; i < statEffectDict[StatusEffect.Stunned]; i++)
            statusText.text += "StunImage";

        statusText.text = KeywordTooltip.instance.EditText(statusText.text, true);
    }

    #endregion

}