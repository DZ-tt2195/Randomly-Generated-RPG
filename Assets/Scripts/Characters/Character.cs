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

    [Foldout("Player info", true)]
        [ReadOnly] public string editedDescription;
        protected Ability chosenAbility;
        [ReadOnly] public CharacterData data;
        [ReadOnly] public List<Ability> listOfAutoAbilities = new();
        [ReadOnly] public List<Ability> listOfRandomAbilities = new();
        CharacterType myType;

    [Foldout("Base Stats", true)]
        protected int baseHealth;
        protected int baseSpeed;
        protected float baseLuck;
        protected float baseAccuracy;

    [Foldout("Current Stats", true)]
        private int _currentHealth;
        [ReadOnly] public int currentHealth
        {
            get { return _currentHealth; }
            private set { _currentHealth = value; healthText.text = value.ToString();}
        }
        private Position _currentPosition;
        [ReadOnly] public Position currentPosition
        {
            get { return _currentPosition; }
            private set {
            statusText.text = KeywordTooltip.instance.EditText($"{currentEmotion} {data.myName}\n{value}"); _currentPosition = value; }
        }
        private Emotion _currentEmotion;
        [ReadOnly] public Emotion currentEmotion
        {
            get { return _currentEmotion; }
            private set{
            statusText.text = KeywordTooltip.instance.EditText($"{value} {data.myName}\n{currentPosition}"); _currentEmotion = value;}
        }
        protected int modifyAttack;
        protected int modifyDefense;
        protected int modifySpeed;
        protected float modifyLuck;
        protected float modifyAccuracy;    
        [ReadOnly] public int turnsStunned { get; private set; }

    [Foldout("UI", true)]
        [ReadOnly] public Image border;
        [ReadOnly] public Button myButton;
        [ReadOnly] public Image myImage;
        TMP_Text statusText;
        TMP_Text healthText;

#endregion

#region Setup

    private void Awake()
    {
        myImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        border = transform.Find("border").GetComponent<Image>();
        statusText = transform.Find("Status Text").GetComponent<TMP_Text>();
        healthText = transform.Find("Health %").GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetupCharacter(CharacterType type, CharacterData characterData,
        List<AbilityData> listOfAbilityData, Emotion startingEmotion, bool abilitiesBeginWithCooldown)
    {
        data = characterData;
        myType = type;
        this.name = characterData.myName;
        editedDescription = KeywordTooltip.instance.EditText(data.description);

        this.baseHealth = data.baseHealth;
        currentHealth = this.baseHealth;
        this.baseSpeed = data.baseSpeed;
        this.baseLuck = data.baseLuck;
        this.baseAccuracy = data.baseAccuracy;

        AddAbility(FileManager.instance.FindEnemyAbility("Skip Turn"), true, false);
        AddAbility(FileManager.instance.FindEnemyAbility("Resurrect"), true, false);
        this.myImage.sprite = Resources.Load<Sprite>($"Characters/{this.name}");

        StartCoroutine(ChangePosition(data.startingPosition, -1));
        StartCoroutine(ChangeEmotion(startingEmotion, -1));

        foreach (AbilityData data in listOfAbilityData)
            AddAbility(data, false, abilitiesBeginWithCooldown);
        listOfRandomAbilities = listOfRandomAbilities.OrderBy(o => o.mainType).ToList();

        modifyAttack = 0;
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

    #endregion

#region Stats

    public int CalculateHealth()
    {
        return currentHealth;
    }

    public float CalculateHealthPercent()
    {
        return (float)currentHealth / this.baseHealth;
    }

    public int CalculateAttack()
    {
        int emotionEffect = currentEmotion switch
        {
            Emotion.Angry => 2,
            _ => 0,
        };

        return modifyAttack + emotionEffect;
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

    #endregion

#region Change Stats

    public IEnumerator Stun(int amount, int logged)
    {
        if (this == null) yield break;

        turnsStunned += amount;
        TurnManager.instance.CreateVisual($"STUNNED", this.transform.localPosition);
        Log.instance.AddText($"{this.name} is Stunned for {turnsStunned} turn{(turnsStunned == 1 ? "" : "s")}.", logged);
    }

    public IEnumerator GainHealth(float health, int logged)
    {
        if (this == null) yield break;

        currentHealth = Mathf.Clamp(currentHealth += (int)health, 0, this.baseHealth);
        TurnManager.instance.CreateVisual($"+{(int)health} HP", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} regains {health} HP.", logged);
    }

    public IEnumerator TakeDamage(int damage, int logged)
    {
        if (this == null) yield break;

        currentHealth -= damage;
        TurnManager.instance.CreateVisual($"-{(int)damage} HP", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} takes {damage} damage.", logged);

        if (currentHealth <= 0)
            yield return HasDied(logged);
    }

    public IEnumerator HasDied(int logged)
    {
        if (this == null) yield break;

        currentHealth = 0;
        currentPosition = Position.Dead;

        if (this.currentHealth == 0)
        {
            Log.instance.AddText($"{(this.name)} has died.", logged);
            TurnManager.instance.speedQueue.Remove(this);
            if (this.myType == CharacterType.Player)
            {
                TurnManager.instance.listOfPlayers.Remove(this);
                myImage.color = Color.gray;
            }
            else
            {
                TurnManager.instance.listOfEnemies.Remove(this);
                Destroy(this.gameObject);
            }
        }
    }

    public IEnumerator Revive(float health, int logged)
    {
        if (this == null || this.CalculateHealth() > 0) yield break;

        Log.instance.AddText($"{(this.name)} comes back to life.", logged);
        TurnManager.instance.listOfPlayers.Add(this);

        yield return GainHealth(health, logged);
        yield return ChangePosition(data.startingPosition, -1);
        yield return ChangeEmotion((Emotion)UnityEngine.Random.Range(1, 5), -1);

        modifyAttack = 0;
        modifyDefense = 0;
        modifySpeed = 0;
        modifyLuck = 0f;
        modifyAccuracy = 0f;
    }

    public IEnumerator ChangeAttack(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        modifyAttack = Math.Clamp(modifyAttack += effect, -3, 3);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} ATTACK", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Attack is reduced by {effect}.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Attack is increased by {effect}.", logged);
    }

    public IEnumerator ChangeDefense(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        modifyDefense = Math.Clamp(modifyDefense += effect, -3, 3);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} DEFENSE", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Defense is reduced by {Math.Abs(effect)}.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Defense is increased by {Math.Abs(effect)}.", logged);
    }

    public IEnumerator ChangeSpeed(int effect, int logged)
    {
        if (this == null || effect == 0) yield break;

        modifySpeed = Math.Clamp(modifySpeed += effect, -3, 3);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{Math.Abs(effect)} SPEED", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Speed is reduced by {Math.Abs(effect)}.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Speed is increased by {Math.Abs(effect)}.", logged);
    }

    public IEnumerator ChangeLuck(float effect, int logged)
    {
        if (this == null || effect == 0f) yield break;

        modifyLuck = Mathf.Clamp(modifyLuck += effect, 0.25f, 1.75f);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100*Math.Abs(effect)}% LUCK", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Luck is reduced by {100*Math.Abs(effect)}%.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Luck is increased by {100*Math.Abs(effect)}%.", logged);
    }

    public IEnumerator ChangeAccuracy(float effect, int logged)
    {
        if (this == null || effect == 0f) yield break;

        modifyAccuracy = Mathf.Clamp(modifyAccuracy += effect, 0.25f, 1.75f);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100*Math.Abs(effect)}% ACCURACY", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Accuracy is reduced by {100*Math.Abs(effect)}%.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Accuracy is increased by {100*Math.Abs(effect)}%.", logged);
    }

    public IEnumerator ChangePosition(Position newPosition, int logged)
    {
        if (this == null) yield break;

        if (newPosition != Position.Dead && currentPosition != newPosition)
        {
            currentPosition = newPosition;

            if (Log.instance != null && logged >= 0)
            {
                if (newPosition == Position.Grounded)
                {
                    Log.instance.AddText($"{(this.name)} is now Grounded.", logged);
                    TurnManager.instance.CreateVisual($"GROUNDED", this.transform.localPosition);
                }
                else if (newPosition == Position.Airborne)
                {
                    Log.instance.AddText($"{(this.name)} is now Airborne.", logged);
                    TurnManager.instance.CreateVisual($"AIRBORNE", this.transform.localPosition);
                }
            }
        }
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, int logged)
    {
        if (this == null || newEmotion == Emotion.Dead || newEmotion == currentEmotion) yield break;

        currentEmotion = newEmotion;

        Color newColor = KeywordTooltip.instance.SearchForKeyword(currentEmotion.ToString()).color;
        this.myImage.color = new Color(newColor.r, newColor.g, newColor.b);

        if (Log.instance != null && logged >= 0)
        {
            Log.instance.AddText($"{(this.name)} is now {currentEmotion}.", logged);
            TurnManager.instance.CreateVisual($"{currentEmotion}", this.transform.localPosition);
        }
    }

#endregion

#region Turns

    internal IEnumerator MyTurn(int logged, bool extraTurn)
    {
        chosenAbility = null;
        yield return StartOfTurn(logged);
        if (turnsStunned > 0)
        {
            yield return TurnManager.instance.WaitTime();
            turnsStunned--;
            Log.instance.AddText($"{this.name} is stunned.", 0);
        }
        else
        {
            yield return ResolveTurn(logged, extraTurn);
        }
        yield return EndOfTurn(logged, extraTurn);
    }

    IEnumerator StartOfTurn(int logged)
    {
        yield return null;
    }

    IEnumerator ResolveTurn(int logged, bool extraAbility)
    {
        chosenAbility = null;
        while (chosenAbility == null)
        {
            yield return ChooseAbility(logged);
            yield return ChooseTarget(chosenAbility);
            if (this.myType == CharacterType.Player && (PlayerPrefs.GetInt("Confirm Choices") == 1))
                yield return ConfirmDecisions();
        }

        foreach (Ability ability in listOfAutoAbilities)
        {
            if (!extraAbility && ability.currentCooldown > 0)
                ability.currentCooldown--;
        }

        foreach (Ability ability in listOfRandomAbilities)
        {
            if (!extraAbility && ability.currentCooldown > 0)
                ability.currentCooldown--;
        }

        if (chosenAbility.data.myName == "Skip Turn")
        {
            Log.instance.AddText(Log.Substitute(chosenAbility, this), 0);
            chosenAbility = null;
        }
        else
        {
            yield return TurnManager.instance.WaitTime();
            yield return chosenAbility.ResolveInstructions(TurnManager.SpliceString(chosenAbility.data.instructions), logged + 1);

            int happinessPenalty = currentEmotion switch
            {
                Emotion.Happy => 1,
                _ => 0,
            };
            chosenAbility.currentCooldown = chosenAbility.data.baseCooldown + happinessPenalty;

            if (FileManager.instance.mode == FileManager.GameMode.Tutorial)
            {
                if (TutorialManager.instance.currentCharacter == this)
                {
                    TutorialManager.instance.currentCharacter = null;
                    yield return TutorialManager.instance.NextStep();
                }
            }
        }
    }

    protected virtual IEnumerator ChooseAbility(int logged)
    {
        yield return null;
    }

    protected virtual IEnumerator ChooseTarget(Ability ability)
    {
        yield return null;
    }

    IEnumerator ConfirmDecisions()
    {
        TurnManager.instance.instructions.text = "";
        TurnManager.instance.DisableCharacterButtons();

        string part1 = $"{this.name}: Use {chosenAbility.data.myName}";
        string part2 = (chosenAbility.singleTarget.Contains(chosenAbility.data.teamTarget)) ? $" on {chosenAbility.listOfTargets[0].name}?" : "?";

        TextCollector confirmDecision = TurnManager.instance.MakeTextCollector
            (part1 + part2, new Vector2(0, 0),
            new List<string>() { "Confirm", "Rechoose" });

        yield return confirmDecision.WaitForChoice();
        int decision = confirmDecision.chosenButton;
        Destroy(confirmDecision.gameObject);

        if (decision == 1)
        {
            chosenAbility = null;
            yield break;
        }
    }

    IEnumerator EndOfTurn(int logged, bool extraTurn)
    {
        if (chosenAbility != null && chosenAbility.data.myName != "Skip Turn")
        {
            if (this.currentEmotion == Emotion.Angry)
            {
                if (chosenAbility.killed)
                {
                    Log.instance.AddText($"{this.name} is Angry.", logged);
                    yield return Stun(1, logged + 1);
                }
            }
            else if (this.currentEmotion == Emotion.Happy)
            {
                if (!extraTurn && chosenAbility.mainType != AbilityType.Attack)
                {
                    Log.instance.AddText($"{this.name} is Happy.", logged);
                    yield return ResolveTurn(logged + 1, true);
                }
            }
            else if (this.currentEmotion == Emotion.Sad)
            {
                Log.instance.AddText($"{this.name} is Sad.", logged);
                if (chosenAbility.mainType == AbilityType.Attack)
                    yield return GainHealth((int)(this.baseHealth * 0.15f), logged + 1);
                else
                    yield return TakeDamage((int)(this.baseHealth * 0.15f), logged + 1);
            }
        }
    }

    #endregion

#region UI

    private void FixedUpdate()
    {
        if (FileManager.instance.mode != FileManager.GameMode.Other)
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
        if (currentPosition == Position.Airborne)
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (myType == CharacterType.Enemy) ? 375 : -500;
            newPosition.y = startingPosition + (25 * Mathf.Cos(Time.time * 3f));
            transform.localPosition = newPosition;
        }
        else
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (myType == CharacterType.Enemy) ? 300 : -575;
            newPosition.y = startingPosition;
            transform.localPosition = newPosition;
        }
    }

    #endregion

}