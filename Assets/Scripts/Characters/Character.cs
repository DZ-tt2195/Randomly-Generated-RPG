using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using static Ability;
//using static UnityEngine.GraphicsBuffer;
//using UnityEditor.Playables;

public enum Position { Grounded, Airborne, Dead };
public enum Emotion { Dead, Neutral, Happy, Ecstatic, Angry, Enraged, Sad, Depressed };
public enum CharacterType { Player, Enemy }

[RequireComponent(typeof(Button))][RequireComponent(typeof(Image))]
public class Character : MonoBehaviour, IPointerClickHandler
{

#region Variables

    public static float borderColor;

    [Foldout("Player info", true)]
    protected Ability chosenAbility;
    [ReadOnly] public List<Ability> listOfAbilities = new List<Ability>();
    [ReadOnly] public CharacterType myType;
    [ReadOnly] public bool isHelper;
    protected string aiTargeting;
    protected string entersFight;
    [ReadOnly] public Weapon weapon;

    [Foldout("Stats", true)]
    protected int baseHealth;
    protected float baseAttack;
    protected float baseDefense;
    protected float baseSpeed;
    protected float baseLuck;
    protected float baseAccuracy;

    protected Position startingPosition;
    protected Emotion startingEmotion;

    protected int currentHealth;
    [ReadOnly] public Position currentPosition;
    [ReadOnly] public Emotion currentEmotion;
    protected float modifyAttack = 1f;
    protected float modifyDefense = 1f;
    protected float modifySpeed = 1f;
    protected float modifyLuck = 1f;
    protected float modifyAccuracy = 1f;

    public int turnsStunned { get; private set; }

    [Foldout("UI", true)]
    [ReadOnly] public Image border;
    [ReadOnly] public Button myButton;
    [ReadOnly] public Image myImage;
    [ReadOnly] public Image weaponImage;
    Button infoButton;
    TMP_Text statusText;
    TMP_Text healthText;
    [ReadOnly] public string description;

#endregion

#region Setup

    private void Awake()
    {
        myImage = GetComponent<Image>();
        infoButton = transform.Find("Info").GetComponent<Button>();
        infoButton.onClick.AddListener(RightClickInfo);
        myButton = GetComponent<Button>();
        border = transform.Find("border").GetComponent<Image>();
        statusText = transform.Find("Status Text").GetComponent<TMP_Text>();
        healthText = transform.Find("Health %").GetChild(0).GetComponent<TMP_Text>();
        weaponImage = transform.Find("Weapon Image").GetComponent<Image>();
    }

    public IEnumerator SetupCharacter(CharacterType type, CharacterData characterData, bool isHelper, WeaponData weaponData, float multiplier = 1f)
    {
        yield return null;
        myType = type;
        this.name = characterData.myName;
        this.description = KeywordTooltip.instance.EditText(characterData.description);
        baseHealth = (int)(characterData.baseHealth * multiplier); currentHealth = baseHealth;
        baseAttack = (int)(characterData.baseAttack * multiplier);
        baseDefense = (int)(characterData.baseDefense * multiplier);
        baseSpeed = (int)(characterData.baseSpeed * multiplier);
        baseLuck = characterData.baseLuck;
        baseAccuracy = characterData.baseAccuracy;
        StartCoroutine(ChangePosition(characterData.startingPosition, -1));
        startingEmotion = characterData.startingEmotion; StartCoroutine(ChangeEmotion(characterData.startingEmotion, -1));
        this.isHelper = isHelper;
        this.aiTargeting = characterData.aiTargeting;

        AddAbility(FileManager.instance.FindAbility("Skip Turn"));

        if (this.isHelper)
        {
            this.myImage.sprite = Resources.Load<Sprite>($"Helpers/{this.name}");
            AddAbility(FileManager.instance.FindAbility("Retreat"));
        }
        else if (myType == CharacterType.Player)
        {
            this.myImage.sprite = Resources.Load<Sprite>($"Teammates/{this.name}");
        }
        else if (myType == CharacterType.Enemy)
        {
            this.myImage.sprite = Resources.Load<Sprite>($"Enemies/{this.name}");
        }

        string[] divideSkillsIntoNumbers = characterData.skillNumbers.Split(',');
        List<string> putIntoList = new();
        foreach (string next in divideSkillsIntoNumbers)
            putIntoList.Add(next);
        putIntoList = putIntoList.Shuffle();

        for (int i = 0; listOfAbilities.Count < 5 && i < 10; i++)
        {
            try
            {
                string skillNumber = putIntoList[i];
                skillNumber.Trim();
                AddAbility(FileManager.instance.listOfAbilities[int.Parse(skillNumber)]);
            }
            catch (FormatException){continue;}
            catch (ArgumentOutOfRangeException){break;}
        }
        listOfAbilities = listOfAbilities.OrderBy(o => o.baseCooldown).ToList();

        if (weaponData == null)
        {
            weaponImage.gameObject.SetActive(false);
            this.weapon = null;
        }
        else
        {
            this.weapon = this.gameObject.AddComponent<Weapon>();
            this.weapon.SetupWeapon(weaponData);
            weaponImage.sprite = Resources.Load<Sprite>($"Weapons/{this.weapon.myName}");
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
            modifyAttack += this.weapon.startingAttack;
            modifyDefense += this.weapon.modifyDefense;
            modifySpeed += this.weapon.modifySpeed;
            modifyLuck += this.weapon.modifyLuck;
            modifyAccuracy += this.weapon.modifyAccuracy;
        }
    }

    void AddAbility(AbilityData ability)
    {
        Ability newAbility = this.gameObject.AddComponent<Ability>();
        listOfAbilities.Add(newAbility);
        newAbility.SetupAbility(ability);
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
            int startingPosition = (myType == CharacterType.Enemy) ? 425 : -425;
            newPosition.y = startingPosition + 45 * Mathf.Cos(Time.time * 2.5f);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClickInfo();
        }
    }

    void RightClickInfo()
    {
        string stats1 = "";
        string stats2 = "";

        stats1 += $"Health: {currentHealth} / {baseHealth}\n";
        stats1 += $"Attack: {CalculateAttack():F1}\n";
        stats1 += $"Defense: {CalculateDefense(null)}\n";

        stats2 += $"Speed: {CalculateSpeed():F1}\n";
        stats2 += $"Luck: {(CalculateLuck() * 100):F1}%\n";
        stats2 += $"Accuracy: {(CalculateAccuracy() * 100):F1}%\n";

        RightClick.instance.DisplayInfo(this, KeywordTooltip.instance.EditText(stats1), KeywordTooltip.instance.EditText(stats2));
    }

    #endregion

#region Stats

    public int CalculateHealth()
    {
        return currentHealth;
    }

    public float CalculateAttack()
    {
        float emotionEffect = currentEmotion switch
        {
            Emotion.Angry => 1.25f,
            Emotion.Enraged => 1.5f,
            _ => 1f,
        };

        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.modifyAttack;

        return baseAttack * modifyAttack * emotionEffect * weaponEffect;
    }

    public float CalculateDefense(Character attacker)
    {
        float emotionEffect = 1f;

        if (this.currentEmotion == Emotion.Sad)
        {
            if (attacker != null && attacker.currentEmotion != Emotion.Enraged && attacker.currentEmotion != Emotion.Angry)
                emotionEffect = 1.25f;
        }
        if (this.currentEmotion == Emotion.Depressed)
        {
            if (attacker != null && attacker.currentEmotion != Emotion.Enraged && attacker.currentEmotion != Emotion.Angry)
                emotionEffect = 1.5f;
        }

        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.modifyDefense;

        return baseDefense * modifyDefense * emotionEffect * weaponEffect;
    }

    public float CalculateSpeed()
    {
        float emotionEffect = currentEmotion switch
        {
            Emotion.Happy => 1.25f,
            Emotion.Ecstatic => 1.5f,
            _ => 1f,
        };

        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.modifySpeed;

        return baseSpeed * modifySpeed * emotionEffect * weaponEffect;
    }

    public float CalculateLuck()
    {
        float emotionEffect = currentEmotion switch
        {
            Emotion.Happy => 1.25f,
            Emotion.Ecstatic => 1.5f,
            _ => 1f,
        };

        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.modifyLuck;

        return baseLuck * modifyLuck * emotionEffect * weaponEffect;
    }

    public float CalculateAccuracy()
    {
        float emotionEffect = currentEmotion switch
        {
            Emotion.Sad => 0.9f,
            Emotion.Depressed => 0.8f,
            _ => 1f,
        };

        float weaponEffect = 1;
        if (weapon != null && weapon.StatCalculation())
            weaponEffect += weapon.modifyAccuracy;

        return baseAccuracy * modifyAccuracy * emotionEffect * weaponEffect;
    }

    #endregion

#region Change Stats

    public IEnumerator Stun(int amount, int logged)
    {
        if (this == null) yield break;

        turnsStunned += amount;
        TurnManager.instance.CreateVisual($"STUNNED", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} is stunned for {turnsStunned} turns.", logged);
    }

    public IEnumerator GainHealth(float health, int logged)
    {
        if (this == null) yield break;

        currentHealth += (int)health;
        if (currentHealth > baseHealth)
            currentHealth = baseHealth;
        healthText.text = $"{100 * ((float)currentHealth / baseHealth):F0}%";
        TurnManager.instance.CreateVisual($"+{(int)health}", this.transform.localPosition);
        Log.instance.AddText($"{(this.name)} regains {health} HP.", logged);
    }

    public IEnumerator TakeDamage(int damage, int logged)
    {
        if (this == null) yield break;

        currentHealth -= damage;
        healthText.text = $"{100 * ((float)currentHealth / baseHealth):F0}%";
        TurnManager.instance.CreateVisual($"-{(int)damage}", this.transform.localPosition);
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
            yield return weapon.OnDeath(logged + 1);

        yield return ChangeEmotion(Emotion.Dead, logged);

        if (this.myType == CharacterType.Player && !isHelper)
        {
            myImage.color = Color.gray;
        }
        else
        {
            TurnManager.instance.players.Remove(this);
            TurnManager.instance.enemies.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Revive(float health, int logged)
    {
        if (this == null) yield break;

        Log.instance.AddText($"{(this.name)} comes back to life.", logged);
        yield return GainHealth(health, logged);
        yield return ChangePosition(startingPosition, -1);
        yield return ChangeEmotion(startingEmotion, -1);
        myImage.color = Color.white;
        WeaponStartingEffects();
    }

    public IEnumerator ChangeAttack(float effect, int logged)
    {
        if (this == null) yield break;

        modifyAttack += effect;
        if (modifyAttack < 0.5f)
            modifyAttack = 0.5f;
        else if (modifyAttack > 1.5f)
            modifyAttack = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s attack is reduced.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s attack is increased.", logged);
    }

    public IEnumerator ChangeDefense(float effect, int logged)
    {
        if (this == null) yield break;

        modifyDefense += effect;
        if (modifyDefense < 0.5f)
            modifyDefense = 0.5f;
        else if (modifyDefense > 1.5f)
            modifyDefense = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s defense is reduced.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s defense is increased.", logged);
    }

    public IEnumerator ChangeSpeed(float effect, int logged)
    {
        if (this == null) yield break;

        modifySpeed += effect;
        if (modifySpeed < 0.5f)
            modifySpeed = 0.5f;
        else if (modifySpeed > 1.5f)
            modifySpeed = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s speed is reduced.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s speed is increased.", logged);
    }

    public IEnumerator ChangeLuck(float effect, int logged)
    {
        if (this == null) yield break;

        modifyLuck += effect;
        if (modifyLuck < 0.5f)
            modifyLuck = 0.5f;
        else if (modifyLuck > 1.5f)
            modifyLuck = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s luck is reduced.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s luck is increased.", logged);
    }

    public IEnumerator ChangeAccuracy(float effect, int logged)
    {
        if (this == null) yield break;

        modifyAccuracy += effect;
        if (modifyAccuracy < 0.5f)
            modifyAccuracy = 0.5f;
        else if (modifyAccuracy > 1.5f)
            modifyAccuracy = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s accuracy is reduced.", logged);
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s accuracy is increased.", logged);
    }

    public IEnumerator ChangePosition(Position newPosition, int logged)
    {
        if (this == null) yield break;

        if (newPosition != Position.Dead && currentPosition != newPosition)
        {
            currentPosition = newPosition;
            try
            {
                if (newPosition == Position.Grounded)
                    Log.instance.AddText($"{(this.name)} is now grounded.", logged);
                else if (newPosition == Position.Airborne)
                    Log.instance.AddText($"{(this.name)} is now airborne.", logged);
            }
            catch { };
        }
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, int logged)
    {
        if (this == null) yield break;

        if (newEmotion == Emotion.Angry && currentEmotion == Emotion.Angry || newEmotion == Emotion.Angry && currentEmotion == Emotion.Enraged)
            currentEmotion = Emotion.Enraged;
        else if (newEmotion == Emotion.Sad && currentEmotion == Emotion.Sad || newEmotion == Emotion.Sad && currentEmotion == Emotion.Depressed)
            currentEmotion = Emotion.Depressed;
        else if (newEmotion == Emotion.Happy && currentEmotion == Emotion.Happy || newEmotion == Emotion.Happy && currentEmotion == Emotion.Ecstatic)
            currentEmotion = Emotion.Ecstatic;
        else
            currentEmotion = newEmotion;

        statusText.text = KeywordTooltip.instance.EditText($"{currentEmotion}\n{currentPosition}");
        if (Log.instance != null && currentEmotion != Emotion.Dead)
            Log.instance.AddText($"{(this.name)} is now {currentEmotion}.", logged);
    }

    #endregion

#region Abilities and Turns

    public IEnumerator MyTurn(int logged)
    {
        yield return StartOfTurn(logged);
        if (turnsStunned > 0)
        {
            yield return TurnManager.instance.WaitTime();
            turnsStunned--;
            Log.instance.AddText($"{this.name} is stunned.", 0);
        }
        else
        {
            yield return ChooseTurn(logged);
            yield return ResolveTurn(logged);
        }
        yield return EndOfTurn(logged);
    }

    IEnumerator StartOfTurn(int logged)
    {
        if (this.weapon != null)
            yield return weapon.StartOfTurn(logged);
    }

    IEnumerator ChooseTurn(int logged)
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
            if (ability.currentCooldown > 0)
                ability.currentCooldown--;
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

        string part1 = $"{this.name}: Use {chosenAbility.myName}";
        string part2 = (chosenAbility.singleTarget.Contains(chosenAbility.teamTarget)) ? $" on {chosenAbility.listOfTargets[0].name}?" : "?";

        TextCollector confirmDecision = TurnManager.instance.MakeTextCollector(part1 + part2, new Vector2(0, 0), new List<string>() { "Confirm", "Rechoose" });

        yield return confirmDecision.WaitForChoice();
        int decision = confirmDecision.chosenButton;
        Destroy(confirmDecision.gameObject);

        if (decision == 1)
        {
            chosenAbility = null;
            yield break;
        }
    }

    IEnumerator ResolveTurn(int logged)
    {
        if (chosenAbility.myName == "Skip Turn")
        {
            Log.instance.AddText(Log.Substitute(chosenAbility, this), 0);
        }
        else
        {
            int happinessPenalty = currentEmotion switch
            {
                Emotion.Happy => 1,
                Emotion.Ecstatic => 2,
                _ => 0,
            };
            chosenAbility.currentCooldown = chosenAbility.baseCooldown + happinessPenalty;

            yield return TurnManager.instance.WaitTime();
            yield return ResolveAbility(chosenAbility, logged + 1);
        }
    }

    protected IEnumerator ResolveAbility(Ability ability, int logged)
    {
        yield return ability.ResolveInstructions(TurnManager.SpliceString(ability.instructions), logged);
    }

    IEnumerator EndOfTurn(int logged)
    {
        if (this.weapon != null)
            yield return weapon.EndOfTurn(logged);

        if (this.currentEmotion == Emotion.Angry)
        {
            Log.instance.AddText($"{this.name} is Angry.", logged);
            yield return TakeDamage((int)(baseHealth * 0.1f), logged + 1);
        }
        else if (this.currentEmotion == Emotion.Enraged)
        {
            Log.instance.AddText($"{this.name} is Enraged.", logged);
            yield return TakeDamage((int)(baseHealth * 0.2f), logged + 1);
        }
    }

#endregion

}