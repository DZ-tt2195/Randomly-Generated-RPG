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
        protected Ability chosenAbility;
        [ReadOnly] public CharacterData data;
        [ReadOnly] public List<Ability> listOfAbilities = new();
        CharacterType myType;
        [ReadOnly] public Weapon weapon;

    [Foldout("Stats", true)]
        protected int currentHealth;

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

    protected float modifyAttack = 1f;
        protected float modifyDefense = 1f;
        protected float modifySpeed = 1f;
        protected float modifyLuck = 1f;
        protected float modifyAccuracy = 1f;    
        [ReadOnly] public int turnsStunned { get; private set; }

    [Foldout("UI", true)]
        [ReadOnly] public Image border;
        [ReadOnly] public Button myButton;
        [ReadOnly] public Image myImage;
        [ReadOnly] public Image weaponImage;
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
        weaponImage = transform.Find("Weapon Image").GetComponent<Image>();
    }

    public void SetupCharacter(CharacterType type, CharacterData characterData, List<AbilityData> listOfAbilityData, Emotion startingEmotion, float multiplier = 1f, WeaponData weaponData = null)
    {
        data = characterData;
        myType = type;
        this.name = characterData.myName;
        data.description = KeywordTooltip.instance.EditText(data.description);

        data.baseHealth = (int)(data.baseHealth * multiplier); currentHealth = data.baseHealth;
        data.baseAttack = (int)(data.baseAttack * multiplier);
        data.baseDefense = (int)(data.baseDefense * multiplier);
        data.baseSpeed = (int)(data.baseSpeed * multiplier);
        data.baseLuck *= multiplier;
        data.baseAccuracy *= multiplier;

        AddAbility(FileManager.instance.FindAbility("Skip Turn"), false);
        this.myImage.sprite = Resources.Load<Sprite>($"Characters/{this.name}");

        StartCoroutine(ChangePosition(data.startingPosition, -1));
        StartCoroutine(ChangeEmotion(startingEmotion, -1));

        foreach (AbilityData data in listOfAbilityData)
            AddAbility(data, myType != CharacterType.Player);
        listOfAbilities = listOfAbilities.OrderBy(o => o.data.baseCooldown).ToList();

        if (weaponData == null)
        {
            weaponImage.gameObject.SetActive(false);
            this.weapon = null;
        }
        else
        {
            this.weapon = this.gameObject.AddComponent<Weapon>();
            this.weapon.SetupWeapon(weaponData);
            weaponImage.sprite = Resources.Load<Sprite>($"Weapons/{this.weapon.data.myName}");
        }
        WeaponStartingEffects();
    }

    void WeaponStartingEffects()
    {
        modifyAttack = 1f;
        modifyDefense = 1f;
        modifyAccuracy = 1f;
        modifyLuck = 1f;
        modifyAccuracy = 1f;

        if (this.weapon != null)
        {
            modifyAttack += this.weapon.data.startingAttack;
            modifyDefense += this.weapon.data.startingDefense;
            modifySpeed += this.weapon.data.startingSpeed;
            modifyLuck += this.weapon.data.startingLuck;
            modifyAccuracy += this.weapon.data.startingAccuracy;
        }
    }

    void AddAbility(AbilityData ability, bool startWithCooldown)
    {
        Ability newAbility = this.gameObject.AddComponent<Ability>();
        listOfAbilities.Add(newAbility);
        newAbility.SetupAbility(ability, startWithCooldown);
    }

#endregion

#region UI

    private void FixedUpdate()
    {
        this.border.SetAlpha(borderColor);
        ScreenPosition();
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

#region Stats

    public int CalculateHealth()
    {
        return currentHealth;
    }

    public float CalculateHealthPercent()
    {
        return (float)currentHealth / data.baseHealth;
    }

    public float CalculateAttack()
    {
        float emotionEffect = currentEmotion switch
        {
            Emotion.Angry => 1.2f,
            _ => 1f,
        };

        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.data.modifyAttack;

        return data.baseAttack * modifyAttack * emotionEffect * weaponEffect;
    }

    public float CalculateDefense()
    {
        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.data.modifyDefense;

        return data.baseDefense * modifyDefense * weaponEffect;
    }

    public float CalculateSpeed()
    {
        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.data.modifySpeed;

        return data.baseSpeed * modifySpeed * weaponEffect;
    }

    public float CalculateLuck()
    {
        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.data.modifyLuck;

        return data.baseLuck * modifyLuck * weaponEffect;
    }

    public float CalculateAccuracy()
    {
        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.data.modifyAccuracy;

        return data.baseAccuracy * modifyAccuracy * weaponEffect;
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
        if (this == null || CalculateHealthPercent() >= 1f) yield break;

        currentHealth = Mathf.Clamp(currentHealth += (int)health, 0, data.baseHealth);
        healthText.text = $"{100 * CalculateHealthPercent():F0}%";
        TurnManager.instance.CreateVisual($"+{(int)health} HP", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} regains {health} HP.", logged);
    }

    public IEnumerator TakeDamage(int damage, int logged)
    {
        if (this == null) yield break;

        currentHealth -= damage;
        healthText.text = $"{100 * CalculateHealthPercent():F0}%";
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
        healthText.text = $"0%";

        Log.instance.AddText($"{(this.name)} has died.", logged);
        if (this.weapon != null)
            yield return this.weapon.WeaponEffect(TurnManager.SpliceString(this.weapon.data.onDeath), logged+1);

        TurnManager.instance.speedQueue.Remove(this);
        if (this.myType == CharacterType.Player)
        {
            TurnManager.instance.players.Remove(this);
            myImage.color = Color.gray;
        }
        else
        {
            TurnManager.instance.enemies.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Revive(float health, int logged)
    {
        if (this == null || this.CalculateHealth() > 0) yield break;

        Log.instance.AddText($"{(this.name)} comes back to life.", logged);
        TurnManager.instance.players.Add(this);

        yield return GainHealth(health, logged);
        yield return ChangePosition(data.startingPosition, -1);
        yield return ChangeEmotion((Emotion)UnityEngine.Random.Range(1, 5), -1);

        WeaponStartingEffects();
    }

    public IEnumerator ChangeAttack(float effect, int logged)
    {
        if (this == null) yield break;

        modifyAttack = Mathf.Clamp(modifyAttack += effect, 0.5f, 1.5f);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100*Math.Abs(effect)}% ATTACK", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Attack is reduced by {100*Math.Abs(effect)}%.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Attack is increased by {100*Math.Abs(effect)}%.", logged);
    }

    public IEnumerator ChangeDefense(float effect, int logged)
    {
        if (this == null) yield break;

        modifyDefense = Mathf.Clamp(modifyDefense += effect, 0.5f, 1.5f);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100*Math.Abs(effect)}% DEFENSE", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Defense is reduced by {100*Math.Abs(effect)}%.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Defense is increased by {100*Math.Abs(effect)}%.", logged);
    }

    public IEnumerator ChangeSpeed(float effect, int logged)
    {
        if (this == null) yield break;

        modifySpeed = Mathf.Clamp(modifySpeed += effect, 0.5f, 1.5f);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100*Math.Abs(effect)}% SPEED", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Speed is reduced by {100*Math.Abs(effect)}%.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Speed is increased by {100*Math.Abs(effect)}%.", logged);
    }

    public IEnumerator ChangeLuck(float effect, int logged)
    {
        if (this == null) yield break;

        modifyLuck = Mathf.Clamp(modifyLuck += effect, 0.5f, 1.5f);
        TurnManager.instance.CreateVisual($"{(effect > 0 ? '+' : '-')}{100*Math.Abs(effect)}% LUCK", this.transform.localPosition);

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s Luck is reduced by {100*Math.Abs(effect)}%.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s Luck is increased by {100*Math.Abs(effect)}%.", logged);
    }

    public IEnumerator ChangeAccuracy(float effect, int logged)
    {
        if (this == null) yield break;

        modifyAccuracy = Mathf.Clamp(modifyAccuracy += effect, 0.5f, 1.5f);
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

    public IEnumerator MyTurn(int logged)
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
            yield return ResolveTurn(logged, false);
        }
        yield return EndOfTurn(logged);
    }

    IEnumerator StartOfTurn(int logged)
    {
        if (weapon != null)
            yield return weapon.WeaponEffect(TurnManager.SpliceString(weapon.data.startOfTurn), logged);
    }

    IEnumerator ResolveTurn(int logged, bool extraAbility)
    {
        chosenAbility = null;
        while (chosenAbility == null)
        {
            yield return ChooseAbility(logged);
            yield return ChooseTarget(chosenAbility);
            if (this.myType != CharacterType.Enemy && (PlayerPrefs.GetInt("Confirm Choices") == 1))
                yield return ConfirmDecisions();
        }

        foreach (Ability ability in listOfAbilities)
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

            if (chosenAbility.killed && this.weapon != null)
                yield return this.weapon.WeaponEffect(TurnManager.SpliceString(weapon.data.onKill), logged+1);
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

    IEnumerator EndOfTurn(int logged)
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
                if (chosenAbility.data.typeOne != AbilityType.Attack && chosenAbility.data.typeTwo != AbilityType.Attack)
                {
                    Log.instance.AddText($"{this.name} is Happy.", logged);
                    yield return ResolveTurn(logged + 1, true);
                }
            }
            else if (this.currentEmotion == Emotion.Sad)
            {
                Log.instance.AddText($"{this.name} is Sad.", logged);
                if (chosenAbility.data.typeOne != AbilityType.Attack && chosenAbility.data.typeTwo != AbilityType.Attack)
                    yield return GainHealth((int)(data.baseHealth * 0.2f), logged + 1);
                else
                    yield return TakeDamage((int)(data.baseHealth * 0.2f), logged + 1);
            }
        }

        if (this.weapon != null)
            yield return this.weapon.WeaponEffect(TurnManager.SpliceString(weapon.data.endOfTurn), logged);
    }

#endregion

}