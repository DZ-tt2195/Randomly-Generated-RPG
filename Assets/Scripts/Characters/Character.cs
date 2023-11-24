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

    protected int baseHealth;
    protected int currentHealth;

    protected float baseAttack;
    protected float baseDefense;
    protected float baseSpeed;
    protected float baseLuck;
    protected float baseAccuracy;

    protected float modifyAttack = 1f;
    protected float modifyDefense = 1f;
    protected float modifySpeed = 1f;
    protected float modifyLuck = 1f;
    protected float modifyAccuracy = 1f;

    public enum Position { Grounded, Airborne, Dead};
    protected Position startingPosition;
    [ReadOnly] public Position currentPosition;

    public enum Emotion { Dead, Neutral, Happy, Ecstatic, Angry, Enraged, Sad, Depressed};
    protected Emotion startingEmotion;
    [ReadOnly] public Emotion currentEmotion;

    [ReadOnly] public List<Ability> listOfAbilities = new List<Ability>();
    public enum CharacterType { Teammate, Enemy, Helper}
    [ReadOnly] public CharacterType myType;

    [ReadOnly] public Image image;
    [ReadOnly] public Image border;
    public static float borderColor;
    [ReadOnly] public Button button;

    #endregion

#region Setup

    private void Awake()
    {
        border = this.transform.GetChild(0).GetComponent<Image>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void SetupCharacter(CharacterType type, CharacterData data)
    {
        this.transform.localScale = new Vector3(1, 1, 1);
        myType = type;
        this.name = data.name;
        this.image.sprite = Resources.Load<Sprite>($"Teammates/{this.name}");
        baseHealth = data.baseHealth; currentHealth = baseHealth;
        baseAttack = data.baseAttack;
        baseDefense = data.baseDefense;
        baseSpeed = data.baseSpeed;
        baseLuck = data.baseLuck;
        baseAccuracy = data.baseAccuracy;
        currentPosition = data.startingPosition;
        startingEmotion = data.startingEmotion; currentEmotion = startingEmotion;

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
        BorderControl();
        ScreenPosition();
    }

    void BorderControl()
    {
        Color newColor = this.border.color;
        newColor.a = borderColor;
        this.border.color = newColor;
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
            RightClick.instance.DisplayInfo(this);
        }
    }

    #endregion

#region Stats

    public int CalculateHealth()
    {
        return currentHealth;
    }

    public float CalculateAttack()
    {
        float emotionEffect;
        switch (currentEmotion)
        {
            case Emotion.Angry:
                emotionEffect = 1.15f;
                break;
            case Emotion.Enraged:
                emotionEffect = 1.3f;
                break;
            default:
                emotionEffect = 1f;
                break;
        }

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
        float emotionEffect;
        switch (currentEmotion)
        {
            case Emotion.Happy:
                emotionEffect = 1.15f;
                break;
            case Emotion.Ecstatic:
                emotionEffect = 1.3f;
                break;
            default:
                emotionEffect = 1f;
                break;
        }

        return baseSpeed * modifySpeed * emotionEffect;
    }

    public float CalculateLuck()
    {
        float emotionEffect;
        switch (currentEmotion)
        {
            case Emotion.Happy:
                emotionEffect = 1.15f;
                break;
            case Emotion.Ecstatic:
                emotionEffect = 1.3f;
                break;
            default:
                emotionEffect = 1f;
                break;
        }
        return baseLuck * modifyLuck * emotionEffect;
    }

    public float CalculateAccuracy()
    {
        float emotionEffect;
        switch (currentEmotion)
        {
            case Emotion.Sad:
                emotionEffect = 0.9f;
                break;
            case Emotion.Depressed:
                emotionEffect = 0.8f;
                break;
            default:
                emotionEffect = 1f;
                break;
        }
        return baseAccuracy * modifyAccuracy * emotionEffect;
    }

    #endregion

#region Change Stats

    public IEnumerator GainHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > baseHealth)
            currentHealth = baseHealth;
        yield return null;
    }

    public IEnumerator TakeDamage(int damage)
    {
        yield return null;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            yield return HasDied();
        }
    }

    public IEnumerator HasDied()
    {
        currentHealth = -1;
        currentEmotion = Emotion.Dead;
        currentPosition = Position.Dead;

        if (this.myType == CharacterType.Teammate)
        {

        }
        else
        {
            TurnManager.instance.teammates.Remove(this);
            TurnManager.instance.enemies.Remove(this);
            Destroy(this.gameObject);
        }
        yield return null;
    }

    public IEnumerator Revive(int health)
    {
        currentHealth = health;
        currentEmotion = startingEmotion;
        currentPosition = startingPosition;

        modifyAttack = 1f;
        modifyDefense = 1f;
        modifyAccuracy = 1f;
        modifyLuck = 1f;
        modifyAccuracy = 1f;
        yield return null;
    }

    public IEnumerator ChangeAttack(float effect)
    {
        modifyAttack += effect;
        if (modifyAttack < 0.5f)
            modifyAttack = 0.5f;
        else if (modifyAttack > 1.5f)
            modifyAttack = 1.5f;

        yield return null;
    }

    public IEnumerator ChangeDefense(float effect)
    {
        modifyDefense += effect;
        if (modifyDefense < 0.5f)
            modifyDefense = 0.5f;
        else if (modifyDefense > 1.5f)
            modifyDefense = 1.5f;

        yield return null;
    }

    public IEnumerator ChangeSpeed(float effect)
    {
        modifySpeed += effect;
        if (modifySpeed < 0.5f)
            modifySpeed = 0.5f;
        else if (modifySpeed > 1.5f)
            modifySpeed = 1.5f;

        yield return null;
    }

    public IEnumerator ChangeLuck(float effect)
    {
        modifyLuck += effect;
        if (modifyLuck < 0.5f)
            modifyLuck = 0.5f;
        else if (modifyLuck > 1.5f)
            modifyLuck = 1.5f;

        yield return null;
    }

    public IEnumerator ChangeAccuracy(float effect)
    {
        modifyAccuracy += effect;
        if (modifyAccuracy < 0.5f)
            modifyAccuracy = 0.5f;
        else if (modifyAccuracy > 1.5f)
            modifyAccuracy = 1.5f;

        yield return null;
    }

    public IEnumerator ChangePosition(Position newPosition)
    {
        yield return null;
        currentPosition = newPosition;
    }

    public IEnumerator ChangeEmotion(Emotion newEmotion)
    {
        if (newEmotion == Emotion.Angry && currentEmotion == Emotion.Angry)
            currentEmotion = Emotion.Enraged;
        else if (newEmotion == Emotion.Sad && currentEmotion == Emotion.Sad)
            currentEmotion = Emotion.Depressed;
        else if (newEmotion == Emotion.Happy && currentEmotion == Emotion.Happy)
            currentEmotion = Emotion.Ecstatic;
        else
            currentEmotion = newEmotion;

        if (newEmotion == Emotion.Angry || newEmotion == Emotion.Enraged)
            border.color = Color.red;
        if (newEmotion == Emotion.Happy || newEmotion == Emotion.Ecstatic)
            border.color = Color.yellow;
        if (newEmotion == Emotion.Sad || newEmotion == Emotion.Depressed)
            border.color = Color.blue;
        if (newEmotion == Emotion.Neutral)
            border.color = Color.white;

        yield return null;
    }

    #endregion

#region Abilities

    public virtual IEnumerator MyTurn()
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