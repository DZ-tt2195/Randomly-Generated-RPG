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

    Vector3 originalSize = new Vector3(1, 1, 1);

    protected int baseHealth;
    protected int currentHealth;
    [SerializeField] TMP_Text healthText;

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
    [SerializeField] TMP_Text emotionText;

    [ReadOnly] public List<Ability> listOfAbilities = new List<Ability>();
    protected Ability thisTurnAbility;

    public enum CharacterType { Teammate, Enemy, Helper}
    [ReadOnly] public CharacterType myType;

    public static float borderColor;
    public Image image;
    public Image border;
    public Button myButton;
    [SerializeField] Button infoButton;

#endregion

#region Setup

    private void Awake()
    {
        infoButton.onClick.AddListener(RightClickInfo);
    }

    public void SetupCharacter(CharacterType type, CharacterData data)
    {
        myType = type;
        this.name = data.name;
        baseHealth = data.baseHealth; currentHealth = baseHealth;
        baseAttack = data.baseAttack;
        baseDefense = data.baseDefense;
        baseSpeed = data.baseSpeed;
        baseLuck = data.baseLuck;
        baseAccuracy = data.baseAccuracy;
        StartCoroutine(ChangePosition(data.startingPosition));
        startingEmotion = data.startingEmotion; StartCoroutine(ChangeEmotion(data.startingEmotion));

        switch (myType)
        {
            case CharacterType.Teammate:
                this.image.sprite = Resources.Load<Sprite>($"Teammates/{this.name}");
                break;
            case CharacterType.Enemy:
                this.image.sprite = Resources.Load<Sprite>($"Enemies/{this.name}");
                break;
            case CharacterType.Helper:
                this.image.sprite = Resources.Load<Sprite>($"Helpers/{this.name}");
                break;
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
        RightClick.instance.DisplayInfo(this);
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

        yield return null;
    }

    public IEnumerator TakeDamage(int damage)
    {
        Debug.Log(damage);
        currentHealth -= damage;
        healthText.text = $"{100 * ((float)currentHealth / baseHealth):F0}%";
        if (currentHealth <= 0)
        {
            yield return HasDied();
        }
    }

    public IEnumerator HasDied()
    {
        currentHealth = -1;
        yield return ChangeEmotion(Emotion.Dead);
        currentPosition = Position.Dead;
        healthText.text = $"0%";

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
        yield return GainHealth(health);
        yield return ChangePosition(startingPosition);
        yield return ChangeEmotion(startingEmotion);

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

        if (thisTurnAbility.myName != "Do Nothing")
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