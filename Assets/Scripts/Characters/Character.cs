using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class Character : MonoBehaviour
{
    protected int baseHealth;
    protected int baseEnergy;
    protected int currentHealth;
    protected int currentEnergy;

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

    public enum Position { Grounded, Airborne};
    protected Position startingPosition;
    public Position currentPosition;

    public enum Emotion { Neutral, Happy, Ecstatic, Angry, Enraged, Sad, Depressed};
    protected Emotion startingEmotion;
    public Emotion currentEmotion;

    protected List<Ability> listOfAbilities = new List<Ability>();

    public Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public float CalculateAttack()
    {
        return baseAttack * modifyAttack;
    }

    public float CalculateDefense()
    {
        return baseDefense * modifyDefense;
    }

    public float CalculateSpeed()
    {
        return baseSpeed * modifySpeed;
    }

    public float CalculateLuck()
    {
        return baseLuck * modifyLuck;
    }

    public float CalculateAccuracy()
    {
        return baseAccuracy * modifyAccuracy;
    }
}
