using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;
using System.Linq;

public enum Position { Grounded, Elevated, Dead };
public enum Emotion { Dead, Neutral, Happy, Angry, Sad };

[RequireComponent(typeof(Button))][RequireComponent(typeof(Image))]
public class Character : MonoBehaviour
{

#region Variables

    public static float borderColor;

    [Foldout("Character info", true)]
        protected Ability chosenAbility;
        protected Character chosenTarget;
        protected float timer;
        [ReadOnly] public Character lastToAttackThis { get; private set; }
        [ReadOnly] public string editedDescription { get; private set; }
        [ReadOnly] public CharacterData data { get; private set; }
        [ReadOnly] public List<Ability> listOfAutoAbilities = new();
        [ReadOnly] public List<Ability> listOfRandomAbilities = new();

    [Foldout("Base Stats", true)]
        protected int baseHealth;
        protected int baseSpeed;

    [Foldout("Current Stats", true)]
        public int currentHealth { get; private set; }
        public int modifyPower { get; private set; }
        public int modifyDefense { get; private set; }
        public int modifySpeed { get; private set; }
        public int modifyLuck { get; private set; }

        private Position _currentPosition;
        [ReadOnly] public Position CurrentPosition
        {
            get { return _currentPosition; }
            private set {
               _currentPosition = value; CharacterUI(); }
        }
        private Emotion _currentEmotion;
        [ReadOnly] public Emotion CurrentEmotion
        {
            get { return _currentEmotion; }
            private set {
                _currentEmotion = value; CharacterUI(); }
        }
        private int _turnsStunned;
        [ReadOnly]
        public int TurnsStunned
        {
            get { return _turnsStunned; }
            private set { _turnsStunned = value; CharacterUI(); }
        }
        private int _turnsProtected;
        [ReadOnly]
        public int TurnsProtected
        {
            get { return _turnsProtected; }
            private set { _turnsProtected = value; CharacterUI(); }
        }
        private int _turnsTargeted;
        [ReadOnly]
        public int TurnsTargeted
        {
            get { return _turnsTargeted; }
            private set { _turnsTargeted = value; CharacterUI(); }
        }
        private int _extraTurns;
        [ReadOnly] public int ExtraTurns
        {
            get { return _extraTurns; }
            private set { _extraTurns = value; CharacterUI(); }
        }
        private int _turnsLocked;
        [ReadOnly] public int TurnsLocked
        {
            get { return _turnsLocked; }
            private set { _turnsLocked = value; CharacterUI(); }
        }

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
    }

    public void SetupCharacter(CharacterData characterData, List<AbilityData> listOfAbilityData, Emotion startingEmotion, bool abilitiesBeginWithCooldown)
    {
        data = characterData;
        this.name = characterData.myName;
        editedDescription = KeywordTooltip.instance.EditText(data.description);

        this.baseHealth = data.baseHealth;
        this.currentHealth = this.baseHealth;
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}Health");
        this.baseSpeed = data.baseSpeed;

        this.myImage.sprite = Resources.Load<Sprite>($"Characters/{this.name}");
        AddAbility(FileManager.instance.FindEnemyAbility("Skip Turn"), true, false);
        if (this is PlayerCharacter)
            AddAbility(FileManager.instance.FindEnemyAbility("Revive"), true, false);

        StartCoroutine(ChangePosition(data.startingPosition, -1));
        StartCoroutine(ChangeEmotion(startingEmotion, -1));
        nameText.text = this.name;

        foreach (AbilityData data in listOfAbilityData)
            AddAbility(data, false, abilitiesBeginWithCooldown);
        listOfRandomAbilities = listOfRandomAbilities.OrderBy(o => o.mainType).ToList();

        modifyPower = 0;
        modifyDefense = 0;
        modifySpeed = 0;
        modifyLuck = 0;
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
        return modifyPower + (CurrentEmotion == Emotion.Angry ? 2 : 0);
    }

    public int CalculateSpeed()
    {
        return this.baseSpeed + modifySpeed;
    }

    public float CalculateStatTotals()
    {
        return (float)(CalculatePower() + modifyDefense + CalculateSpeed() + modifyLuck);
    }

#endregion

#region Change Stats

    public IEnumerator ChangeMaxHealth(int effect, int logged)
    {
        if (this == null) yield break;
        baseHealth += effect;

        if (effect > 0)
        {
            TurnManager.instance.CreateVisual($"+{effect} MAX HEALTH", this.transform.localPosition);
            Log.instance.AddText($"{this.name} has {effect} more Max Health.", logged);
        }
        else
        {
            TurnManager.instance.CreateVisual($"{effect} MAX Health", this.transform.localPosition);
            Log.instance.AddText($"{this.name} has {Mathf.Abs(effect)} less Max Health.", logged);
            if (baseHealth < currentHealth)
                currentHealth = baseHealth;
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}Health");
    }

    public IEnumerator GainHealth(int health, int logged)
    {
        if (this == null) yield break;

        currentHealth = Mathf.Clamp(currentHealth += health, 0, this.baseHealth);
        TurnManager.instance.CreateVisual($"+{health} Health", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} regains {health} Health.", logged);
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}Health");
    }

    public IEnumerator TakeDamage(int damage, int logged, Character attacker = null)
    {
        if (attacker != null) lastToAttackThis = attacker;
        if (this == null || damage == 0) yield break;

        if (TurnsProtected > 0)
        {
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} is Protected from {damage} damage.", logged);
        }
        else
        {
            currentHealth -= damage;
            TurnManager.instance.CreateVisual($"-{damage} Health", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} takes {damage} damage.", logged);

            if (currentHealth <= 0)
                yield return HasDied(logged);
        }
        healthText.text = KeywordTooltip.instance.EditText($"{currentHealth}/{baseHealth}Health");
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
        TurnsStunned = 0;
        TurnsProtected = 0;
        TurnsLocked = 0;
        CurrentPosition = Position.Dead;
        CurrentEmotion = Emotion.Dead;

        Log.instance.AddText($"{this.name} has died.", logged);
        TurnManager.instance.speedQueue.Remove(this);

        if (this is PlayerCharacter)
        {
            TurnManager.instance.listOfPlayers.Remove(this);
            TurnManager.instance.listOfDead.Add(this);
            myImage.color = Color.gray;
        }
        else
        {
            TurnManager.instance.listOfEnemies.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ChangePower(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        if (TurnsProtected > 0 && effect < 0)
        {
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} is Protected from losing {Mathf.Abs(effect)} Power.", logged);
        }
        else
        {
            modifyPower = Math.Clamp(modifyPower += effect, -3, 3);
            if (logged >= 0)
            {
                TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Power", this.transform.localPosition);
                Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Power.", logged);
            }
        }
    }

    public IEnumerator ChangeDefense(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        if (TurnsProtected > 0 && effect < 0)
        {
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} is Protected from losing {Mathf.Abs(effect)} Defense.", logged);
        }
        else
        {
            modifyDefense = Math.Clamp(modifyDefense += effect, -3, 3);
            if (logged >= 0)
            {
                TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Defense", this.transform.localPosition);
                Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Defense.", logged);
            }
        }
    }

    public IEnumerator ChangeSpeed(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        if (TurnsProtected > 0 && effect < 0)
        {
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} is Protected from losing {Mathf.Abs(effect)} Speed.", logged);
        }
        else
        {
            modifySpeed = Math.Clamp(modifySpeed += effect, -5, 5);

            if (logged >= 0)
            {
                TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Speed", this.transform.localPosition);
                Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Speed.", logged);
            }
        }
    }

    public IEnumerator ChangeLuck(int effect, int logged)
    {
        if (this == null || effect == 0f) yield break;

        if (TurnsProtected > 0 && effect < 0)
        {
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} is Protected from losing {Mathf.Abs(effect)} Luck.", logged);
        }
        else
        {
            modifyLuck = Mathf.Clamp(modifyLuck += effect, -3, 3);
            if (logged >= 0)
            {
                TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Luck", this.transform.localPosition);
                Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Luck.", logged);
            }
        }
    }

    #endregion

#region Other Stats

    public IEnumerator ChangePosition(Position newPosition, int logged)
    {
        if (this == null || newPosition == Position.Dead || newPosition == CurrentPosition) yield break;

        if (TurnsLocked > 0)
        {
            Log.instance.AddText($"{(this.name)}'s Position can't change.", logged);
        }
        else
        {
            CurrentPosition = newPosition;
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

        if (TurnsLocked > 0)
        {
            Log.instance.AddText($"{(this.name)}'s Emotion can't change.", logged);
        }
        else
        {
            CurrentEmotion = newEmotion;
            if (Log.instance != null && logged >= 0)
            {
                Log.instance.AddText($"{(this.name)} is now {CurrentEmotion}.", logged);
                TurnManager.instance.CreateVisual($"{CurrentEmotion.ToString().ToUpper()}", this.transform.localPosition);
            }
        }

        if (newEmotion == Emotion.Neutral)
        {
            this.myImage.color = Color.white;
        }
        else
        {
            Color newColor = KeywordTooltip.instance.SearchForKeyword(CurrentEmotion.ToString()).color;
            this.myImage.color = new Color(newColor.r, newColor.g, newColor.b);
        }
    }

    public IEnumerator Stunned(int amount, int logged)
    {
        if (this == null) yield break;

        TurnsStunned += amount;
        TurnManager.instance.CreateVisual($"STUNNED", this.transform.localPosition);
        Log.instance.AddText($"{this.name} is Stunned for {TurnsStunned} turn{(TurnsStunned == 1 ? "" : "s")}.", logged);
    }
     
    public IEnumerator Protected(int amount, int logged)
    {
        if (this == null) yield break;

        TurnsProtected += amount;
        TurnManager.instance.CreateVisual($"PROTECTED", this.transform.localPosition);
        Log.instance.AddText($"{this.name} is Protected for the next {TurnsProtected} attack{(TurnsProtected == 1 ? "" : "s")}..", logged);
    }

    public IEnumerator Targeted(int amount, int logged)
    {
        if (this == null) yield break;

        if (this is PlayerCharacter)
            TurnManager.instance.targetedPlayer = this;
        else if (this is EnemyCharacter)
            TurnManager.instance.targetedEnemy = this;

        TurnsTargeted += amount;
        TurnManager.instance.CreateVisual($"TARGETED", this.transform.localPosition);
        Log.instance.AddText($"{this.name} is Targeted for {TurnsTargeted} turn{(TurnsTargeted == 1 ? "" : "s")}.", logged);
    }

    public IEnumerator Extra(int amount, int logged)
    {
        if (this == null) yield break;
        ExtraTurns += amount;
        TurnManager.instance.CreateVisual($"EXTRA TURN", this.transform.localPosition);
        Log.instance.AddText($"{this.name} will get {ExtraTurns} Extra turn{(ExtraTurns == 1 ? "" : "s")}.", logged);
    }

    public IEnumerator Locked(int amount, int logged)
    {
        if (this == null) yield break;
        TurnsLocked += amount;
        TurnManager.instance.CreateVisual($"LOCKED", this.transform.localPosition);
        Log.instance.AddText($"{this.name}'s Position / Emotion are Locked for {TurnsLocked} turn{(TurnsLocked == 1 ? "" : "s")}.", logged);
    }

    public IEnumerator Revive(int health, int logged)
    {
        if (this == null || this.currentHealth > 0) yield break;

        Log.instance.AddText($"{(this.name)} comes back to life.", logged);
        TurnManager.instance.listOfPlayers.Add(this);
        TurnManager.instance.listOfDead.Remove(this);

        yield return GainHealth(health, logged);
        yield return ChangePosition(data.startingPosition, -1);
        yield return ChangeEmotion((Emotion)UnityEngine.Random.Range(1, 5), -1);

        modifyPower = 0;
        modifyDefense = 0;
        modifySpeed = 0;
        modifyLuck = 0;
    }

    #endregion

#region Turns

    internal IEnumerator MyTurn(int logged)
    {
        chosenAbility = null;
        chosenTarget = null;

        if (TurnsProtected > 0)
            TurnsProtected--;
        if (TurnsTargeted > 0)
            TurnsTargeted--;
        if (TurnsLocked > 0)
            TurnsLocked--;

        yield return ResolveTurn(logged, false);
        yield return EmotionEffect(logged, false);

        while (ExtraTurns > 0)
        {
            ExtraTurns--;
            yield return ResolveTurn(logged, true);
            yield return EmotionEffect(logged, true);
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
        if (TurnsStunned > 0)
        {
            yield return TurnManager.instance.WaitTime();
            TurnsStunned--;
            Log.instance.AddText($"{this.name} is Stunned.", 0);
            yield break;
        }

        if (TurnManager.instance.listOfEnemies.Count > 0)
        {
            StartCoroutine(nameof(Timer));
            if (extraAbility)
            {
                foreach (Ability ability in listOfAutoAbilities)
                {
                    if (ability.currentCooldown > 0)
                        ability.currentCooldown++;
                }
                foreach (Ability ability in listOfRandomAbilities)
                {
                    if (ability.currentCooldown > 0)
                        ability.currentCooldown++;
                }
            }

            chosenAbility = null;
            chosenTarget = null;

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
                        chosenTarget = chosenAbility.listOfTargets[i][0];
                }

                if (timer < 0f)
                    yield break;

                if (this is PlayerCharacter)
                {
                    string part1 = $"{this.name}: Use {chosenAbility.data.myName}";
                    string part2 = chosenTarget != null ? $" on {chosenTarget.data.myName}?" : "?";

                    yield return TurnManager.instance.ConfirmUndo(part1 + part2, new Vector3(0, 400));
                    if (TurnManager.instance.confirmChoice == 1)
                        chosenAbility = null;
                    if (timer < 0f)
                        yield break;
                }
            }

            StopCoroutine(nameof(Timer));
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

            Log.instance.AddText(Log.Substitute(chosenAbility, this, chosenTarget), logged);
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

                chosenAbility.currentCooldown = chosenAbility.data.baseCooldown + (CurrentEmotion == Emotion.Happy ? 1 : 0) +
                    (CarryVariables.instance.ActiveCheat("Slower Enemy Cooldowns") && this is EnemyCharacter ? 1 : 0);

                if (CarryVariables.instance.mode == CarryVariables.GameMode.Tutorial && TutorialManager.instance.currentCharacter == this)
                {
                    TutorialManager.instance.currentCharacter = null;
                    yield return TutorialManager.instance.NextStep();
                }
            }
        }
    }

    protected virtual IEnumerator ChooseAbility(int logged, bool extraAbility)
    {
        yield return null;
    }

    protected virtual IEnumerator ChooseTarget(Ability ability, TeamTarget target, int index)
    {
        yield return null;
    }

    IEnumerator EmotionEffect(int logged, bool extraTurn)
    {
        if (timer > 0f && chosenAbility != null && !chosenAbility.data.myName.Equals("Skip Turn"))
        {
            if (this.CurrentEmotion == Emotion.Angry)
            {
                if (chosenAbility.killed || chosenAbility.fullHeal)
                {
                    Log.instance.AddText($"{this.name} is Angry.", logged);
                    yield return Stunned(1, logged + 1);
                }
            }
            else if (this.CurrentEmotion == Emotion.Happy)
            {
                if (!extraTurn && chosenAbility.mainType != AbilityType.Attack)
                {
                    Log.instance.AddText($"{this.name} is Happy.", logged);
                    yield return Extra(1, logged+1);
                }
            }
            else if (this.CurrentEmotion == Emotion.Sad)
            {
                Log.instance.AddText($"{this.name} is Sad.", logged);
                yield return (chosenAbility.mainType == AbilityType.Attack) ? GainHealth(2, logged + 1) : TakeDamage(2, logged + 1);
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
            newPosition.y = startingPosition + (25 * Mathf.Cos(Time.time * 3f));
            transform.localPosition = newPosition;
        }
        else
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (this is EnemyCharacter) ? 300 : -570;
            newPosition.y = startingPosition;
            transform.localPosition = newPosition;
        }
    }

    public void CharacterUI()
    {
        topText.text = (CurrentPosition == Position.Dead || CurrentEmotion == Emotion.Dead) ? "Dead" : KeywordTooltip.instance.EditText($"{CurrentEmotion}\n{CurrentPosition}");
        statusText.text = "";

        for (int i = 0; i < TurnsStunned; i++)
            statusText.text += "StunImage";
        for (int i = 0; i < TurnsProtected; i++)
            statusText.text += "ProtectedImage";
        for (int i = 0; i < ExtraTurns; i++)
            statusText.text += "ExtraImage";
        for (int i = 0; i < TurnsLocked; i++)
            statusText.text += "LockedImage";

        if (TurnManager.instance != null && (TurnManager.instance.targetedPlayer == this || TurnManager.instance.targetedEnemy == this))
        {
            for (int i = 0; i < TurnsTargeted; i++)
                statusText.text += "TargetedImage";
        }
        else
        {
            _turnsTargeted = 0;
        }
        statusText.text = KeywordTooltip.instance.EditText(statusText.text);
    }

    #endregion

}