using System;
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
        protected string description;

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

    public void SetupCharacter(CharacterType type, CharacterData data, bool isHelper)
    {
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

        string[] divideIntoNumbers = data.skillNumbers.Split(',');
        foreach (string x in divideIntoNumbers)
        {
            if (x.Trim() != "")
            {
                Ability nextAbility = this.gameObject.AddComponent<Ability>();
                listOfAbilities.Add(nextAbility);
                nextAbility.SetupAbility(TitleScreen.instance.listOfAbilities[int.Parse(x)]);
            }
        }
    }

#endregion

#region UI

    private void FixedUpdate()
    {
        this.transform.localScale = originalSize;
        this.border.SetAlpha(borderColor);
        ScreenPosition();
    }

    void ScreenPosition()
    {
        if (currentPosition == Position.Airborne)
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (myType == CharacterType.Enemy) ? 425 : -425;
            newPosition.y = startingPosition + 50 * Mathf.Cos(Time.time * 2.5f);
            transform.localPosition = newPosition;
        }
        else
        {
            Vector3 newPosition = transform.localPosition;
            int startingPosition = (myType == CharacterType.Enemy) ? 300 : -550;
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
        RightClick.instance.DisplayInfo(this, image.sprite);
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
        {
            yield return HasDied();
        }
    }

    public IEnumerator HasDied()
    {
        currentHealth = -1;
        yield return ChangeEmotion(Emotion.Dead, false);
        currentPosition = Position.Dead;
        healthText.text = $"0%";
        Log.instance.AddText($"{(this.name)} has died.");

        if (this.myType == CharacterType.Teammate)
        {

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
        currentPosition = newPosition;
        if (newPosition == Position.Grounded && logged)
            Log.instance.AddText($"{(this.name)} is now grounded.");
        if (newPosition == Position.Airborne && logged)
            Log.instance.AddText($"{(this.name)} is now airborne.");
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion, bool logged)
    {
        if (newEmotion == Emotion.Angry && currentEmotion == Emotion.Angry)
            currentEmotion = Emotion.Enraged;
        else if (newEmotion == Emotion.Sad && currentEmotion == Emotion.Sad)
            currentEmotion = Emotion.Depressed;
        else if (newEmotion == Emotion.Happy && currentEmotion == Emotion.Happy)
            currentEmotion = Emotion.Ecstatic;
        else
            currentEmotion = newEmotion;

        if (logged)
            Log.instance.AddText($"{(this.name)} is now {currentEmotion}.");

        switch (currentEmotion)
        {
            case Emotion.Neutral:
                emotionText.text = "NEUTRAL";
                border.color = Color.white;
                break;
            case Emotion.Happy:
                emotionText.text = "HAPPY";
                border.color = Color.yellow;
                break;
            case Emotion.Ecstatic:
                emotionText.text = "ECSTATIC";
                border.color = Color.yellow;
                break;
            case Emotion.Angry:
                emotionText.text = "ANGRY";
                border.color = Color.red;
                break;
            case Emotion.Enraged:
                emotionText.text = "ENRAGED";
                border.color = Color.red;
                break;
            case Emotion.Sad:
                emotionText.text = "SAD";
                border.color = Color.blue;
                break;
            case Emotion.Depressed:
                emotionText.text = "DEPRESSED";
                border.color = Color.blue;
                break;
            case Emotion.Dead:
                emotionText.text = "DEAD";
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

        Log.instance.AddText(Log.Substitute(thisTurnAbility, this));
        if (thisTurnAbility.myName == "Do Nothing")
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