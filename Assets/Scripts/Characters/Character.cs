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

    public enum Position { Grounded, Airborne, Dead};
    protected Position startingPosition;
    public Position currentPosition;

    public enum Emotion { Dead, Neutral, Happy, Ecstatic, Angry, Enraged, Sad, Depressed};
    protected Emotion startingEmotion;
    public Emotion currentEmotion;

    [ReadOnly] public List<Ability> listOfAbilities = new List<Ability>();

    [ReadOnly] public Image image;
    [ReadOnly] public Image border;
    public static float borderColor;
    [ReadOnly] public Button button;

    #region Setup

    private void Awake()
    {
        border = this.transform.GetChild(0).GetComponent<Image>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
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

    #endregion

    #region UI

    private void FixedUpdate()
    {
        Color newColor = this.border.color;
        newColor.a = (button.interactable) ? borderColor : 0;
        this.border.color = newColor;
    }

    private void Update()
    {
        if (currentPosition == Position.Airborne)
        {
            Vector3 newPosition = transform.localPosition;
            newPosition.y = -425 + 50 * Mathf.Cos(Time.time * 2.5f);
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

    public int GetHealth()
    {
        return currentHealth;
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

    #endregion

    #region

    public virtual IEnumerator ChooseAbility()
    {
        yield return null;
        for (int i = 0; i < TurnManager.instance.listOfBoxes.Count; i++)
        {
            try
            {
                TurnManager.instance.listOfBoxes[i].ReceiveAbility(listOfAbilities[i]);
                TurnManager.instance.listOfBoxes[i].gameObject.SetActive(true);
            }
            catch (ArgumentOutOfRangeException) { TurnManager.instance.listOfBoxes[i].gameObject.SetActive(false); }
        }
    }
    #endregion
}
