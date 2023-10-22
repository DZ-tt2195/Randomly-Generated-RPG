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
        baseEnergy = data.baseEnergy; currentEnergy = baseEnergy;
        baseAttack = data.baseAttack;
        baseDefense = data.baseDefense;
        baseSpeed = data.baseSpeed;
        baseLuck = data.baseLuck;
        baseAccuracy = data.baseAccuracy;
        currentPosition = data.startingPosition;
    }
}
