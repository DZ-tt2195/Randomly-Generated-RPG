using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;
using System.Linq;

public enum Position { Grounded, Airborne, Dead };
public enum Emotion { Dead, Neutral, Happy, Angry, Sad };
public enum CharacterType { Player, Enemy }

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
        [ReadOnly] public CharacterType myType { get; private set; }

    [Foldout("Base Stats", true)]
        protected int baseHealth;
        protected int baseSpeed;
        protected float baseLuck;
        protected float baseAccuracy;

    [Foldout("Current Stats", true)]
        private int _currentHealth;
        [ReadOnly] public int CurrentHealth
        {
            get { return _currentHealth; }
            private set { healthText.text = $"{value}/{baseHealth}"; _currentHealth = value; }
        }
        public int modifyPower { get; private set; }
        public int modifyDefense { get; private set; }
        public int modifySpeed { get; private set; }
        public float modifyLuck { get; private set; }
        public float modifyAccuracy { get; private set; }

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

    [Foldout("UI", true)]
        [ReadOnly] public Image border;
        [ReadOnly] public Button myButton;
        [ReadOnly] public Image myImage;
        Image statusImage;
        TMP_Text statusText;
        TMP_Text healthText;
        TMP_Text nameText;
        Sprite stunSprite;
        Sprite protectedSprite;

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
        statusImage = transform.Find("Status Image").GetComponent<Image>();
        stunSprite = Resources.Load<Sprite>("Art/Stun");
        protectedSprite = Resources.Load<Sprite>("Art/Protected");
    }

    public void SetupCharacter(CharacterType type, CharacterData characterData,
        List<AbilityData> listOfAbilityData, Emotion startingEmotion, bool abilitiesBeginWithCooldown)
    {
        data = characterData;
        myType = type;
        this.name = characterData.myName;
        editedDescription = KeywordTooltip.instance.EditText(data.description);

        this.baseHealth = data.baseHealth;
        CurrentHealth = this.baseHealth;
        this.baseSpeed = data.baseSpeed;
        this.baseLuck = (CarryVariables.instance.ActiveChallenge("No Luck") && myType == CharacterType.Enemy) ? 0 : data.baseLuck;
        this.baseAccuracy = data.baseAccuracy;

        AddAbility(FileManager.instance.FindEnemyAbility("Skip Turn"), true, false);
        AddAbility(FileManager.instance.FindEnemyAbility("Revive"), true, false);
        this.myImage.sprite = Resources.Load<Sprite>($"Characters/{this.name}");

        StartCoroutine(ChangePosition(data.startingPosition, -1));
        StartCoroutine(ChangeEmotion(startingEmotion, -1));
        nameText.text = this.name;

        foreach (AbilityData data in listOfAbilityData)
            AddAbility(data, false, abilitiesBeginWithCooldown);
        listOfRandomAbilities = listOfRandomAbilities.OrderBy(o => o.mainType).ToList();

        modifyPower = 0;
        modifyDefense = 0;
        modifySpeed = 0;
        modifyLuck = 0f;
        modifyAccuracy = 0f;
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

    public int CalculateHealth()
    {
        return CurrentHealth;
    }

    public float CalculateHealthPercent()
    {
        return (float)CurrentHealth / this.baseHealth;
    }

    public int CalculatePower()
    {
        return modifyPower + (CurrentEmotion == Emotion.Angry ? 2 : 0);
    }

    public int CalculateDefense()
    {
        return modifyDefense;
    }

    public int CalculateSpeed()
    {
        return this.baseSpeed + modifySpeed;
    }

    public float CalculateLuck()
    {
        return this.baseLuck + modifyLuck;
    }

    public float CalculateAccuracy()
    {
        return this.baseAccuracy + modifyAccuracy;
    }

    public float CalculateStatTotals()
    {
        return (float)(CalculatePower() + CalculateDefense() + CalculateSpeed() + CalculateLuck() + CalculateAccuracy());
    }

#endregion

#region Change Stats

    public IEnumerator MaxHealth(int health, int logged)
    {
        if (this == null) yield break;
        baseHealth += health;
        healthText.text = $"{CurrentHealth}/{baseHealth}";
        TurnManager.instance.CreateVisual($"+{health} MAX Health", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} has {health} more max Health.", logged);
    }

    public IEnumerator GainHealth(int health, int logged)
    {
        if (this == null) yield break;

        CurrentHealth = Mathf.Clamp(CurrentHealth += health, 0, this.baseHealth);
        TurnManager.instance.CreateVisual($"+{health} Health", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} regains {health} Health.", logged);
    }

    public IEnumerator TakeDamage(int damage, int logged, Character attacker = null)
    {
        if (attacker != null)
            lastToAttackThis = attacker;
        if (this == null || damage == 0) yield break;

        if (TurnsProtected > 0 && attacker != null)
        {
            TurnsProtected--;
            TurnManager.instance.CreateVisual($"BLOCKED", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} is Protected from {damage} damage.", logged);
        }
        else
        {
            CurrentHealth -= damage;
            TurnManager.instance.CreateVisual($"-{damage} Health", this.transform.localPosition);
            Log.instance.AddText($"{(this.name)} takes {damage} damage.", logged);

            if (CurrentHealth <= 0)
                yield return HasDied(logged);
        }
    }

    public IEnumerator HasDied(int logged)
    {
        if (this == null) yield break;

        CurrentHealth = 0;
        TurnsStunned = 0;
        TurnsProtected = 0;
        CurrentPosition = Position.Dead;
        CurrentEmotion = Emotion.Dead;

        Log.instance.AddText($"{this.name} has died.", logged);
        TurnManager.instance.speedQueue.Remove(this);

        if (this.myType == CharacterType.Player)
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

    public IEnumerator Revive(int health, int logged)
    {
        if (this == null || this.CalculateHealth() > 0) yield break;

        Log.instance.AddText($"{(this.name)} comes back to life.", logged);
        TurnManager.instance.listOfPlayers.Add(this);
        TurnManager.instance.listOfDead.Remove(this);

        yield return GainHealth(health, logged);
        yield return ChangePosition(data.startingPosition, -1);
        yield return ChangeEmotion((Emotion)UnityEngine.Random.Range(1, 5), -1);

        modifyPower = 0;
        modifyDefense = 0;
        modifySpeed = 0;
        modifyLuck = 0f;
        modifyAccuracy = 0f;
    }

    public IEnumerator ChangePower(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        modifyPower = Math.Clamp(modifyPower += effect, -3, 3);
        if (logged >= 0)
        {
            TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Power", this.transform.localPosition);
            Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Power.", logged);
        }
    }

    public IEnumerator ChangeDefense(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        modifyDefense = Math.Clamp(modifyDefense += effect, -6, 3);
        if (logged >= 0)
        {
            TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Defense", this.transform.localPosition);
            Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Defense.", logged);
        }
    }

    public IEnumerator ChangeSpeed(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        modifySpeed = Math.Clamp(modifySpeed += effect, -5, 5);
        if (logged >= 0)
        {
            TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Speed", this.transform.localPosition);
            Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{Math.Abs(effect)} Speed.", logged);
        }
    }

    public IEnumerator ChangeLuck(float effect, int logged)
    {
        if (this == null || effect == 0f) yield break;

        modifyLuck = Mathf.Clamp(modifyLuck += effect, -0.5f, 0.5f);
        if (logged >= 0)
        {
            TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100 * Math.Abs(effect)}% Luck", this.transform.localPosition);
            Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{100 * Math.Abs(effect)}% Luck.", logged);
        }
    }

    public IEnumerator ChangeAccuracy(float effect, int logged)
    {
        if (this == null || effect == 0f) yield break;

        modifyAccuracy = Mathf.Clamp(modifyAccuracy += effect, -0.3f, 0.6f);
        if (logged >= 0)
        {
            TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100 * Math.Abs(effect)}% Accuracy", this.transform.localPosition);
            Log.instance.AddText($"{this.name} gets {(effect > 0 ? '+' : '-')}{100 * Math.Abs(effect)}% Accuracy.", logged);
        }
    }

    #endregion

#region Other Stats

    public IEnumerator ChangePosition(Position newPosition, int logged)
    {
        if (this == null || newPosition == Position.Dead || newPosition == CurrentPosition) yield break;

        CurrentPosition = newPosition;
        if (Log.instance != null && logged >= 0)
        {
            Log.instance.AddText($"{(this.name)} is now {newPosition}.", logged);
            TurnManager.instance.CreateVisual($"{newPosition.ToString().ToUpper()}", this.transform.localPosition);
        }
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, int logged)
    {
        if (this == null || newEmotion == Emotion.Dead || newEmotion == CurrentEmotion) yield break;

        CurrentEmotion = newEmotion;
        if (newEmotion == Emotion.Neutral)
        {
            this.myImage.color = Color.white;
        }
        else
        {
            Color newColor = KeywordTooltip.instance.SearchForKeyword(CurrentEmotion.ToString()).color;
            this.myImage.color = new Color(newColor.r, newColor.g, newColor.b);
        }
        if (Log.instance != null && logged >= 0)
        {
            Log.instance.AddText($"{(this.name)} is now {CurrentEmotion}.", logged);
            TurnManager.instance.CreateVisual($"{CurrentEmotion.ToString().ToUpper()}", this.transform.localPosition);
        }
    }

    public IEnumerator Stun(int amount, int logged)
    {
        if (this == null) yield break;

        TurnsStunned += amount;
        TurnManager.instance.CreateVisual($"STUNNED", this.transform.localPosition);
        Log.instance.AddText($"{this.name} is Stunned for {TurnsStunned} turn{(TurnsStunned == 1 ? "" : "s")}.", logged);
    }

    public IEnumerator NoStun(int logged)
    {
        if (this == null || TurnsStunned == 0) yield break;

        TurnsStunned = 0;
        Log.instance.AddText($"{this.name} is no longer Stunned.", logged);
    }

    public IEnumerator Protected(int amount, int logged)
    {
        if (this == null) yield break;

        TurnsProtected += amount;
        TurnManager.instance.CreateVisual($"PROTECTED", this.transform.localPosition);
        Log.instance.AddText($"{this.name} is Protected for {TurnsProtected} turn{(TurnsProtected == 1 ? "" : "s")}.", logged);
    }

    #endregion

#region Turns

    internal IEnumerator MyTurn(int logged, bool extraTurn)
    {
        chosenAbility = null;
        chosenTarget = null;

        if (TurnsStunned > 0)
        {
            yield return TurnManager.instance.WaitTime();
            TurnsStunned--;
            Log.instance.AddText($"{this.name} is Stunned.", 0);
        }
        else
        {
            yield return ResolveTurn(logged, extraTurn);
        }
        yield return EmotionEffect(logged, extraTurn);
    }

    IEnumerator Timer()
    {
        timer = 10f;
        if (this.myType == CharacterType.Player && CarryVariables.instance.ActiveChallenge("Player Timer"))
        {
            TurnManager.instance.timerText.gameObject.SetActive(true);
            while (timer > 0f)
            {
                yield return null;
                timer -= Time.deltaTime;
                TurnManager.instance.timerText.text = $"Timer: {timer:F1}";
            }

            TextCollector[] allCollectors = FindObjectsOfType<TextCollector>();
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

            if (this.myType == CharacterType.Player)
            {
                string part1 = $"{this.name}: Use {chosenAbility.data.myName}";
                string part2 = chosenTarget != null ? $" on {chosenTarget.data.myName}?" : "?";

                yield return TurnManager.instance.ConfirmUndo(part1 + part2, Vector3.zero);
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

            chosenAbility.currentCooldown = chosenAbility.data.baseCooldown +
                (CurrentEmotion == Emotion.Happy ? 1 : 0)
                + (CarryVariables.instance.ActiveCheat("Faster Cooldowns") && this.myType == CharacterType.Player ? -1 : 0);

            if (CarryVariables.instance.mode == CarryVariables.GameMode.Tutorial && TutorialManager.instance.currentCharacter == this)
            {
                TutorialManager.instance.currentCharacter = null;
                yield return TutorialManager.instance.NextStep();
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
                    yield return Stun(1, logged + 1);
                }
            }
            else if (this.CurrentEmotion == Emotion.Happy)
            {
                if (!extraTurn && chosenAbility.mainType != AbilityType.Attack)
                {
                    Log.instance.AddText($"{this.name} is Happy.", logged);
                    yield return ResolveTurn(logged + 1, true);
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
        if (CurrentPosition == Position.Airborne)
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (myType == CharacterType.Enemy) ? 375 : -500;
            newPosition.y = startingPosition + (25 * Mathf.Cos(Time.time * 3f));
            transform.localPosition = newPosition;
        }
        else
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (myType == CharacterType.Enemy) ? 300 : -570;
            newPosition.y = startingPosition;
            transform.localPosition = newPosition;
        }
    }

    void CharacterUI()
    {
        statusText.text = (CurrentPosition == Position.Dead || CurrentEmotion == Emotion.Dead) ? "Dead" : KeywordTooltip.instance.EditText($"{CurrentEmotion}\n{CurrentPosition}");

        statusImage.gameObject.SetActive(true);
        if (TurnsStunned > 0)
        {
            statusImage.sprite = stunSprite;
        }
        else if (TurnsProtected > 0)
        {
            statusImage.sprite = protectedSprite;
        }
        else
        {
            statusImage.gameObject.SetActive(false);
        }
    }

    #endregion

}