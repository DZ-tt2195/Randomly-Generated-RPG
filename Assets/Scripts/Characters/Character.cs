using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))] [RequireComponent(typeof(Image))]
public class Character : MonoBehaviour, IPointerClickHandler
{

#region Variables

    public static float borderColor;
    public enum Position { Grounded, Airborne, Dead };
    public enum Emotion { Dead, Neutral, Happy, Ecstatic, Angry, Enraged, Sad, Depressed };
    public enum CharacterType { Teammate, Enemy }

    [Foldout("Player info", true)]
        protected Ability thisTurnAbility;
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

    [Foldout("UI", true)]
        [ReadOnly] public Image border;
        [ReadOnly] public Button myButton;
        Image myImage;
        Image weaponImage;
        Button infoButton;
        TMP_Text emotionText;
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
        emotionText = transform.Find("Emotion Text").GetComponent<TMP_Text>();
        healthText = transform.Find("Health %").GetChild(0).GetComponent<TMP_Text>();
        weaponImage = transform.Find("Weapon Image").GetComponent<Image>();
    }

    public IEnumerator SetupCharacter(CharacterType type, CharacterData characterData, bool isHelper, WeaponData weaponData, float multiplier = 1f)
    {
        yield return null;
        myType = type;
        this.name = characterData.name;
        this.description = characterData.description;
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

        if (this.isHelper)
        {
            this.myImage.sprite = Resources.Load<Sprite>($"Helpers/{this.name}");
        }
        else if (myType == CharacterType.Teammate )
        {
            this.myImage.sprite = Resources.Load<Sprite>($"Teammates/{this.name}");
        }
        else if (myType == CharacterType.Enemy)
        {
            this.myImage.sprite = Resources.Load<Sprite>($"Enemies/{this.name}");
        }

        if (weapon == null)
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

        string[] divideSkillsIntoNumbers = characterData.skillNumbers.Split(',');
        foreach (string skill in divideSkillsIntoNumbers)
        {
            if (skill.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                listOfAbilities.Add(nextAbility);
                nextAbility.SetupAbility(FileManager.instance.listOfAbilities[int.Parse(skill)]);
            }
        }

        string[] divideEntersIntoNumbers = characterData.entersFight.Split(',');
        foreach (string enters in divideEntersIntoNumbers)
        {
            if (enters.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                nextAbility.SetupAbility(FileManager.instance.listOfOtherAbilities[int.Parse(enters)]);

                if (nextAbility.CanPlay(this))
                {
                    Log.instance.AddText(Log.Substitute(nextAbility, this), 1);
                    yield return ChooseTarget(nextAbility);
                    yield return ResolveAbility(nextAbility, 2);
                }
            }
        }
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
        stats1 += $"Attack: {baseAttack} {ConvertStatPercentage(modifyAttack)}\n";
        stats1 += $"Defense: {baseDefense} {ConvertStatPercentage(modifyDefense)}\n";

        stats2 += $"Speed: {baseSpeed} {ConvertStatPercentage(modifySpeed)}\n";
        stats2 += $"Luck: {100 * baseLuck:F0}% {ConvertStatPercentage(modifyLuck)}\n";
        stats2 += $"Accuracy: {100 * baseAccuracy:F0}% {ConvertStatPercentage(modifyAccuracy)}\n";

        RightClick.instance.DisplayInfo(this, myImage.sprite, null, stats1, stats2);
    }

    string ConvertStatPercentage(float stat)
    {
        if (stat == 1f)
            return "";
        else if (stat > 1f)
            return $"+ {100 * (stat - 1):F0}%";
        else
            return $"- {100 * (stat - 1) * -1:F0}%";
    }

    #endregion

#region Stats

    public int CalculateHealth()
    {
        return currentHealth;
    }

    public float CalculateAttack()
    {
        var emotionEffect = currentEmotion switch
        {
            Emotion.Angry => 1.25f,
            Emotion.Enraged => 1.5f,
            _ => 1f,
        };
        return baseAttack * modifyAttack * emotionEffect;
    }

    public float CalculateDefense(Character attacker)
    {
        float emotionEffect = 1f;

        if (this.currentEmotion == Emotion.Sad )
        {
            if (attacker.currentEmotion != Emotion.Enraged && attacker.currentEmotion != Emotion.Angry)
                emotionEffect = 1.25f;
        }
        if (this.currentEmotion == Emotion.Depressed)
        {
            if (attacker.currentEmotion != Emotion.Enraged && attacker.currentEmotion != Emotion.Angry)
                emotionEffect = 1.5f;
        }

        return baseDefense * modifyDefense * emotionEffect;
    }

    public float CalculateSpeed()
    {
        var emotionEffect = currentEmotion switch
        {
            Emotion.Happy => 1.25f,
            Emotion.Ecstatic => 1.5f,
            _ => 1f,
        };
        return baseSpeed * modifySpeed * emotionEffect;
    }

    public float CalculateLuck()
    {
        var emotionEffect = currentEmotion switch
        {
            Emotion.Happy => 1.25f,
            Emotion.Ecstatic => 1.5f,
            _ => 1f,
        };
        return baseLuck * modifyLuck * emotionEffect;
    }

    public float CalculateAccuracy()
    {
        var emotionEffect = currentEmotion switch
        {
            Emotion.Sad => 0.9f,
            Emotion.Depressed => 0.8f,
            _ => 1f,
        };
        return baseAccuracy * modifyAccuracy * emotionEffect;
    }

    #endregion

#region Change Stats

    public IEnumerator GainHealth(int health, int logged)
    {
        if (this == null) yield break;

        currentHealth += health;
        if (currentHealth > baseHealth)
            currentHealth = baseHealth;
        healthText.text = $"{100 * ((float)currentHealth / baseHealth):F0}%";
        Log.instance.AddText($"{(this.name)} regains {health} HP.", logged);

        yield return null;
    }

    public IEnumerator TakeDamage(int damage, int logged)
    {
        if (this == null) yield break;

        currentHealth -= damage;
        healthText.text = $"{100 * ((float)currentHealth / baseHealth):F0}%";
        Log.instance.AddText($"{(this.name)} takes {damage} damage.", logged);

        if (currentHealth <= 0)
            yield return HasDied(logged);
    }

    public IEnumerator HasDied(int logged)
    {
        if (this == null) yield break;

        currentHealth = 0;
        yield return ChangeEmotion(Emotion.Dead, logged);
        currentPosition = Position.Dead;
        healthText.text = $"0%";

        Log.instance.AddText($"{(this.name)} has died.", logged);
    
        if (this.myType == CharacterType.Teammate && !isHelper)
        {
            myImage.color = Color.gray;
        }
        else
        {
            TurnManager.instance.teammates.Remove(this);
            TurnManager.instance.enemies.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Revive(int health, int logged)
    {
        if (this == null) yield break;

        Log.instance.AddText($"{(this.name)} comes back to life.", logged);
        yield return GainHealth(health, logged);
        yield return ChangePosition(startingPosition, -1);
        yield return ChangeEmotion(startingEmotion, -1);
        myImage.color = Color.white;

        modifyAttack = 1f;
        modifyDefense = 1f;
        modifyAccuracy = 1f;
        modifyLuck = 1f;
        modifyAccuracy = 1f;
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

        yield return null;
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

        yield return null;
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

        yield return null;
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

        yield return null;
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

        yield return null;
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
                if (newPosition == Position.Airborne)
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

        if (Log.instance != null && currentEmotion != Emotion.Dead)
            Log.instance.AddText($"{(this.name)} is now {currentEmotion}.", logged);

        switch (currentEmotion)
        {
            case Emotion.Neutral:
                emotionText.text = "NEUTRAL";
                emotionText.color = Color.white;
                border.color = Color.white;
                break;
            case Emotion.Happy:
                emotionText.text = "HAPPY";
                emotionText.color = Color.yellow;
                border.color = Color.yellow;
                break;
            case Emotion.Ecstatic:
                emotionText.text = "ECSTATIC";
                emotionText.color = Color.yellow;
                border.color = Color.yellow;
                break;
            case Emotion.Angry:
                emotionText.text = "ANGRY";
                emotionText.color = Color.red;
                border.color = Color.red;
                break;
            case Emotion.Enraged:
                emotionText.text = "ENRAGED";
                emotionText.color = Color.red;
                border.color = Color.red;
                break;
            case Emotion.Sad:
                emotionText.text = "SAD";
                emotionText.color = Color.blue;
                border.color = Color.blue;
                break;
            case Emotion.Depressed:
                emotionText.text = "DEPRESSED";
                emotionText.color = Color.blue;
                border.color = Color.blue;
                break;
            case Emotion.Dead:
                emotionText.text = "DEAD";
                emotionText.color = Color.gray;
                border.color = Color.gray;
                break;
        }

        yield return null;
    }

#endregion

#region Abilities

    public IEnumerator MyTurn(int logged)
    {
        yield return ChooseAbility();
        yield return ChooseTarget(thisTurnAbility);

        foreach (Ability ability in listOfAbilities)
        {
            if (ability.currentCooldown > 0)
                ability.currentCooldown--;
        }

        TurnManager.instance.instructions.text = "";
        TurnManager.instance.DisableCharacterButtons();
        Log.instance.AddText(Log.Substitute(thisTurnAbility, this), logged);

        if (thisTurnAbility.myName != "Skip Turn")
        {
            int happinessPenalty = currentEmotion switch
            {
                Emotion.Happy => 1,
                Emotion.Ecstatic => 2,
                _ => 0,
            };
            thisTurnAbility.currentCooldown = thisTurnAbility.baseCooldown + happinessPenalty;

            yield return TurnManager.instance.WaitTime;
            yield return ResolveAbility(thisTurnAbility, logged + 1);

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
    }

    protected virtual IEnumerator ChooseAbility()
    {
        yield return null;
    }

    protected virtual IEnumerator ChooseTarget(Ability ability)
    {
        yield return null;
    }

    protected IEnumerator ResolveAbility(Ability ability, int logged)
    {
        string divide = ability.instructions.Replace(" ", "");
        divide = divide.ToUpper();
        string[] methodsInStrings = divide.Split('/');
        yield return ability.ResolveInstructions(methodsInStrings, logged);
    }

#endregion

}