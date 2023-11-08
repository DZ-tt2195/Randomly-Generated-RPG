using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    Weapon weapon;

    public void SetupCharacter(PlayerData data)
    {
        this.name = data.name;
        baseHealth = data.baseHealth; currentHealth = baseHealth;
        baseAttack = data.baseAttack;
        baseDefense = data.baseDefense;
        baseSpeed = data.baseSpeed;
        baseLuck = data.baseLuck;
        baseAccuracy = data.baseAccuracy;
        startingEmotion = data.startingEmotion; currentEmotion = startingEmotion;
        startingPosition = data.startingPosition; currentPosition = startingPosition;
    }
}
