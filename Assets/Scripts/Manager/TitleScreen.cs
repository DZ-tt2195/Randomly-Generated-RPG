using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] string playerFile;
    [SerializeField] string enemyFile;
    [SerializeField] string abilityFile;

    [SerializeField] PlayerCharacter playerPrefab;

    [ReadOnly] public List<PlayerData> players;
    [ReadOnly] public List<EnemyData> enemies;
    [ReadOnly] public List<AbilityData> abilities;

    [Tooltip("0 = Ranger, 1 = Angel, 2 = Knight, 3 = Wizard")] [SerializeField] List<Sprite> playerSprites;

    void Start()
    {
        players = DataLoader.ReadPlayerData(playerFile);
        //enemies = DataLoader.ReadEnemyData(enemyFile);
        //abilities = DataLoader.ReadAbilityData(abilityFile);

        for (int i = 0; i < players.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(playerPrefab);
            nextCharacter.SetupCharacter(players[i]);
            nextCharacter.image.sprite = playerSprites[i];
        }

    }

}
