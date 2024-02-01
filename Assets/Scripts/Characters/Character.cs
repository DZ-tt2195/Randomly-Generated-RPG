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
    Vector3 originalSize = new Vector3(1, 1, 1);
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
        /*[ReadOnly]*/ public Position currentPosition;
        [ReadOnly] public Emotion currentEmotion;
        protected float modifyAttack = 1f;
        protected float modifyDefense = 1f;
        protected float modifySpeed = 1f;
        protected float modifyLuck = 1f;
        protected float modifyAccuracy = 1f;

    [Foldout("UI", true)]
        [ReadOnly] public Image border;
        [ReadOnly] public Button myButton;
        Image image;
        Button infoButton;
        TMP_Text emotionText;
        TMP_Text healthText;
        [ReadOnly] public string description;

#endregion

#region Setup

    private void Awake()
    {
        image = GetComponent<Image>();
        infoButton = transform.Find("Info").GetComponent<Button>();
        infoButton.onClick.AddListener(RightClickInfo);
        myButton = GetComponent<Button>();
        border = transform.Find("border").GetComponent<Image>();
        emotionText = transform.Find("Emotion Text").GetComponent<TMP_Text>();
        healthText = transform.Find("Health %").GetChild(0).GetComponent<TMP_Text>();
    }

    public IEnumerator SetupCharacter(CharacterType type, CharacterData data, bool isHelper)
    {
        yield return null;
        myType = type;
        this.name = data.name;
        this.description = data.description;
        baseHealth = data.baseHealth; currentHealth = baseHealth;
        baseAttack = data.baseAttack;
        baseDefense = data.baseDefense;
        baseSpeed = data.baseSpeed;
        baseLuck = data.baseLuck;
        baseAccuracy = data.baseAccuracy;
        StartCoroutine(ChangePosition(data.startingPosition, false));
        startingEmotion = data.startingEmotion; StartCoroutine(ChangeEmotion(data.startingEmotion, false));
        this.isHelper = isHelper;
        this.aiTargeting = data.aiTargeting;

        if (this.isHelper)
        {
            this.image.sprite = Resources.Load<Sprite>($"Helpers/{this.name}");
        }
        else if (myType == CharacterType.Teammate )
        {
            this.image.sprite = Resources.Load<Sprite>($"Teammates/{this.name}");
        }
        else if (myType == CharacterType.Enemy)
        {
            this.image.sprite = Resources.Load<Sprite>($"Enemies/{this.name}");
        }

        string[] divideSkillsIntoNumbers = data.skillNumbers.Split(',');
        foreach (string skill in divideSkillsIntoNumbers)
        {
            if (skill.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                listOfAbilities.Add(nextAbility);
                nextAbility.SetupAbility(TitleScreen.instance.listOfAbilities[int.Parse(skill)]);
            }
        }

        string[] divideEntersIntoNumbers = data.entersFight.Split(',');
        foreach (string enters in divideEntersIntoNumbers)
        {
            if (enters.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                nextAbility.SetupAbility(TitleScreen.instance.listOfEntersFight[int.Parse(enters)]);

                if (nextAbility.CanPlay(this))
                {
                    Log.instance.AddText(Log.Substitute(nextAbility, this));
                    yield return ChooseTarget(nextAbility);
                    yield return ResolveAbility(nextAbility);
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

        RightClick.instance.DisplayInfo(this, image.sprite, stats1, stats2);
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
            Emotion.Angry => 1.15f,
            Emotion.Enraged => 1.3f,
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
                emotionEffect = 1.15f;
        }
        if (this.currentEmotion == Emotion.Depressed)
        {
            if (attacker.currentEmotion != Emotion.Enraged && attacker.currentEmotion != Emotion.Angry)
                emotionEffect = 1.3f;
        }

        return baseDefense * modifyDefense * emotionEffect;
    }

    public float CalculateSpeed()
    {
        var emotionEffect = currentEmotion switch
        {
            Emotion.Happy => 1.15f,
            Emotion.Ecstatic => 1.3f,
            _ => 1f,
        };
        return baseSpeed * modifySpeed * emotionEffect;
    }

    public float CalculateLuck()
    {
        var emotionEffect = currentEmotion switch
        {
            Emotion.Happy => 1.15f,
            Emotion.Ecstatic => 1.3f,
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

    public IEnumerator GainHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > baseHealth)
            currentHealth = baseHealth;
        healthText.text = $"{100 * ((float)currentHealth / baseHealth):F0}%";
        Log.instance.AddText($"{(this.name)} regains {health} HP.");

        yield return null;
    }

    public IEnumerator TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthText.text = $"{100 * ((float)currentHealth / baseHealth):F0}%";
        Log.instance.AddText($"{(this.name)} takes {damage} damage.");

        if (currentHealth <= 0)
            yield return HasDied(true);
    }

    public IEnumerator HasDied(bool logged)
    {
        currentHealth = 0;
        yield return ChangeEmotion(Emotion.Dead, false);
        currentPosition = Position.Dead;
        healthText.text = $"0%";

        if (logged)
            Log.instance.AddText($"{(this.name)} has died.");

        if (this.myType == CharacterType.Teammate && !isHelper)
        {
            image.color = Color.gray;
        }
        else
        {
            TurnManager.instance.teammates.Remove(this);
            TurnManager.instance.enemies.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Revive(int health)
    {
        Log.instance.AddText($"{(this.name)} comes back to life.");
        yield return GainHealth(health);
        yield return ChangePosition(startingPosition, false);
        yield return ChangeEmotion(startingEmotion, false);
        image.color = Color.white;

        modifyAttack = 1f;
        modifyDefense = 1f;
        modifyAccuracy = 1f;
        modifyLuck = 1f;
        modifyAccuracy = 1f;
    }

    public IEnumerator ChangeAttack(float effect)
    {
        modifyAttack += effect;
        if (modifyAttack < 0.5f)
            modifyAttack = 0.5f;
        else if (modifyAttack > 1.5f)
            modifyAttack = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s attack is reduced.");
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s attack is increased.");

        yield return null;
    }

    public IEnumerator ChangeDefense(float effect)
    {
        modifyDefense += effect;
        if (modifyDefense < 0.5f)
            modifyDefense = 0.5f;
        else if (modifyDefense > 1.5f)
            modifyDefense = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s defense is reduced.");
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s defense is increased.");

        yield return null;
    }

    public IEnumerator ChangeSpeed(float effect)
    {
        modifySpeed += effect;
        if (modifySpeed < 0.5f)
            modifySpeed = 0.5f;
        else if (modifySpeed > 1.5f)
            modifySpeed = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s speed is reduced.");
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s speed is increased.");

        yield return null;
    }

    public IEnumerator ChangeLuck(float effect)
    {
        modifyLuck += effect;
        if (modifyLuck < 0.5f)
            modifyLuck = 0.5f;
        else if (modifyLuck > 1.5f)
            modifyLuck = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s luck is reduced.");
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s luck is increased.");

        yield return null;
    }

    public IEnumerator ChangeAccuracy(float effect)
    {
        modifyAccuracy += effect;
        if (modifyAccuracy < 0.5f)
            modifyAccuracy = 0.5f;
        else if (modifyAccuracy > 1.5f)
            modifyAccuracy = 1.5f;

        if (effect < 0)
            Log.instance.AddText($"{(this.name)}'s accuracy is reduced.");
        if (effect > 0)
            Log.instance.AddText($"{(this.name)}'s accuracy is increased.");

        yield return null;
    }

    public IEnumerator ChangePosition(Position newPosition, bool logged)
    {
        yield return null;

        if (newPosition != Position.Dead && currentPosition != newPosition)
        {
            currentPosition = newPosition;
            if (newPosition == Position.Grounded && logged)
                Log.instance.AddText($"{(this.name)} is now grounded.");
            if (newPosition == Position.Airborne && logged)
                Log.instance.AddText($"{(this.name)} is now airborne.");
        }
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, bool logged)
    {
        if (newEmotion == Emotion.Angry && currentEmotion == Emotion.Angry || newEmotion == Emotion.Angry && currentEmotion == Emotion.Enraged)
            currentEmotion = Emotion.Enraged;
        else if (newEmotion == Emotion.Sad && currentEmotion == Emotion.Sad || newEmotion == Emotion.Sad && currentEmotion == Emotion.Depressed)
            currentEmotion = Emotion.Depressed;
        else if (newEmotion == Emotion.Happy && currentEmotion == Emotion.Happy || newEmotion == Emotion.Happy && currentEmotion == Emotion.Ecstatic)
            currentEmotion = Emotion.Ecstatic;
        else
            currentEmotion = newEmotion;

        if (logged)
            Log.instance.AddText($"{(this.name)} is now {currentEmotion}.");

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

    public IEnumerator MyTurn()
    {
        yield return ChooseAbility();
        yield return ChooseTarget(thisTurnAbility);

        foreach (Ability ability in listOfAbilities)
        {
            if (ability.currentCooldown > 0)
                ability.currentCooldown--;
        }

        TurnManager.instance.instructions.text = "";
        Log.instance.AddText(Log.Substitute(thisTurnAbility, this));
        if (thisTurnAbility.myName == "Skip Turn")
        {
        }
        else
        { 
            int happinessPenalty = 0;
            switch (currentEmotion)
            {
                case Emotion.Happy:
                    happinessPenalty = 1;
                    break;
                case Emotion.Ecstatic:
                    happinessPenalty = 2;
                    break;
            }

            thisTurnAbility.currentCooldown = thisTurnAbility.baseCooldown + happinessPenalty;
            yield return ResolveAbility(thisTurnAbility);
        }

        if (this.currentEmotion == Emotion.Angry)
        {
            Log.instance.AddText($"{this.name} is Angry.");
            yield return TakeDamage((int)(baseHealth * 0.05f));
        }
        else if (this.currentEmotion == Emotion.Enraged)
        {
            Log.instance.AddText($"{this.name} is Enraged.");
            yield return TakeDamage((int)(baseHealth * 0.1f));
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

    protected IEnumerator ResolveAbility(Ability ability)
    {
        string divide = ability.instructions.Replace(" ", "");
        divide = divide.ToUpper();
        string[] methodsInStrings = divide.Split('/');

        foreach (string nextMethod in methodsInStrings)
        {
            if (nextMethod == "" || nextMethod == "NONE")
            {
                continue;
            }
            else
            {
                yield return ability.ResolveMethod(nextMethod);
            }
        }
    }

#endregion

}