using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    protected string abilityInString;

    public void SetupCharacter(EnemyData data)
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
        abilityInString = data.listOfAbilities;
    }
}
