using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class Character : MonoBehaviour
{
    int baseHealth;
    int baseEnergy;
    int currentHealth;
    int currentEnergy;

    float baseAttack;
    float baseDefense;
    float baseSpeed;
    float baseLuck;
    float baseAccuracy;

    float modifyAttack = 1f;
    float modifyDefense = 1f;
    float modifySpeed = 1f;
    float modifyLuck = 1f;
    float modifyAccuracy = 1f;

    public enum Position { Grounded, Airborne};
    Position currentPosition;

    public enum Emotion { Neutral, Happy, Ecstatic, Angry, Enraged, Sad, Depressed};
    Emotion currentEmotion = Emotion.Neutral;

    List<Ability> listOfAbilities = new List<Ability>();

    public virtual void SetupCharacter(CharacterData data)
    {
        this.name = data.name;
        baseHealth = data.baseHealth; currentHealth = baseHealth;
        baseEnergy = data.baseEnergy; currentEnergy = baseEnergy;
        baseAttack = data.baseAttack;
        baseDefense = data.baseDefense;
        baseSpeed = data.baseSpeed;
        baseLuck = data.baseLuck;
        baseAccuracy = data.baseAccuracy;
        currentPosition = data.startingPosition;
    }
}
