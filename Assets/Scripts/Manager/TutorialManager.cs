using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class ForcedPlayerAbilities
{
    public string name;
    public Emotion forcedEmotion;
    public List<string> listOfAbilities = new();
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    [SerializeField] List<ForcedPlayerAbilities> forcedPlayerInfo = new(); 
    List<Character> listOfPlayers = new();
    [SerializeField] GameObject characterPrefab;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (FileManager.instance.mode == FileManager.GameMode.Main)
        {
            Destroy(this.gameObject);
        }
        else
        {
            GeneratePlayers();
            StartCoroutine(TutorialList());
        }
    }

    void GeneratePlayers()
    {
        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(characterPrefab).AddComponent<PlayerCharacter>();
            List<AbilityData> characterAbilities = new();

            foreach (string abilityName in forcedPlayerInfo[i].listOfAbilities)
                characterAbilities.Add(FileManager.instance.FindAbility(abilityName));

            nextCharacter.SetupCharacter(CharacterType.Player, playerData[i], characterAbilities, forcedPlayerInfo[i].forcedEmotion);
            //TurnManager.instance.AddPlayer(nextCharacter);
            listOfPlayers.Add(nextCharacter);
        }
    }

    IEnumerator TutorialList()
    {
        TextCollector collector1 = TurnManager.instance.MakeTextCollector(
            "Welcome to Randomly Generated RPG, a turn-based RPG where you're given random abilities against random enemies.",
            Vector3.zero, new List<string>() { "Next" });
        yield return collector1.WaitForChoice();
        Destroy(collector1.gameObject);

        TurnManager.instance.AddPlayer(listOfPlayers[0]); //add knight
        TextCollector collector2 = TurnManager.instance.MakeTextCollector(
            "You have 3 party members each game. Right click on the Knight to read what they do.",
            Vector3.zero);
        while (ScreenOverlay.instance.displayedScreen != ScreenOverlay.CurrentScreen.Character)
            yield return null;
        while (ScreenOverlay.instance.displayedScreen != ScreenOverlay.CurrentScreen.None)
            yield return null;
        Destroy(collector2.gameObject);

        Debug.Log("tutorial finished");
    }

}
