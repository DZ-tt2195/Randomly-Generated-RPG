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

    public enum Position { Grounded, Airborne};
    protected Position startingPosition;
    public Position currentPosition;

    public enum Emotion { Neutral, Happy, Ecstatic, Angry, Enraged, Sad, Depressed};
    protected Emotion startingEmotion;
    public Emotion currentEmotion;

    [ReadOnly] public List<Ability> listOfAbilities = new List<Ability>();

    [HideInInspector] public Image image;
    [HideInInspector] public Button button;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick.instance.DisplayInfo(this);
        }
    }

    public void SetupCharacter(CharacterData data)
    {
        this.name = data.name;
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
