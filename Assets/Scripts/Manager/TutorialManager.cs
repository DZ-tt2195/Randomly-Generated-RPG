using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class ForcedPlayerAbilities
{
    public string name;
    public Emotion forcedEmotion;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public Character currentCharacter;
    int currentStep;

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
            currentStep = 1;
            StartCoroutine(NextStep());
        }
    }

    void GeneratePlayers()
    {
        List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
        for (int i = 0; i < playerData.Count; i++)
        {
            PlayerCharacter nextCharacter = Instantiate(characterPrefab).AddComponent<PlayerCharacter>();
            List<AbilityData> characterAbilities = new();

            nextCharacter.SetupCharacter(CharacterType.Player, playerData[i], characterAbilities, forcedPlayerInfo[i].forcedEmotion);
            listOfPlayers.Add(nextCharacter);
        }
    }

    public IEnumerator NextStep()
    {
        switch (currentStep)
        {
            case 1: //introduce the concept of the game
                TextCollector collector1 = TurnManager.instance.MakeTextCollector(
                    "This is a turn-based RPG where you're given random abilities against random enemies.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector1.WaitForChoice();
                Destroy(collector1.gameObject);

                currentStep = 2;
                StartCoroutine(NextStep());
                break;

            case 2: //right click on knight
                TurnManager.instance.AddPlayer(listOfPlayers[0]); //add knight
                listOfPlayers[0].AddAbility(FileManager.instance.FindAbility("Stab"), false);

                TextCollector collector2 = TurnManager.instance.MakeTextCollector(
                    "You have 3 party members each game. Right click on the Knight to read what they do.",
                    Vector3.zero);
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;
                Destroy(collector2.gameObject);

                currentStep = 3;
                StartCoroutine(NextStep());
                break;

            case 3: //first time attacking
                yield return TurnManager.instance.NewWave();
                TurnManager.instance.CreateEnemy(FileManager.instance.FindEnemy("Page"), Emotion.Neutral, 0);

                TextCollector collector3 = TurnManager.instance.MakeTextCollector(
                    "Here's your first enemy. Use the first Knight's ability against it.",
                    Vector3.zero, new List<string>() { "Fight" });
                yield return collector3.WaitForChoice();
                Destroy(collector3.gameObject);

                yield return TurnManager.instance.NewRound();
                currentStep = 4;
                StartCoroutine(NextStep());
                break;

            case 4: //introduce cooldowns
                listOfPlayers[0].AddAbility(FileManager.instance.FindAbility("Joust"), false);
                TextCollector collector4 = TurnManager.instance.MakeTextCollector(
                    "The ability you used has a cooldown, so you can't use it again this round. Instead use the other ability.",
                    Vector3.zero, new List<string>() { "Fight" });
                yield return collector4.WaitForChoice();
                Destroy(collector4.gameObject);

                yield return TurnManager.instance.NewRound();
                currentStep = 5;
                StartCoroutine(NextStep());
                break;

            case 5: //introduce angry
                TextCollector collector5 = TurnManager.instance.MakeTextCollector(
                    "You killed your first enemy! Now it's time to explain Emotions.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector5.WaitForChoice();

                collector5.textbox.text = "Everyone begins with random emotions. Emotions are super-effective against other emotions, and have their own effects.";
                yield return collector5.WaitForChoice();

                collector5.textbox.text = "Happy beats Angry, which beats Sad, which beats Happy. Neutral is neutral against everything else.";
                yield return collector5.WaitForChoice();

                collector5.textbox.text = "This game, your Knight started off Angry, which means they have a higher Attack, but they get Stunned each time they kill an enemy.";
                yield return collector5.WaitForChoice();

                collector5.textbox.text = "Your Knight being Stunned means they're forced skip their next turn.";
                yield return collector5.WaitForChoice();
                Destroy(collector5.gameObject);

                currentStep = 6;
                StartCoroutine(NextStep());
                break;

            case 6: //introduce angel
                TurnManager.instance.AddPlayer(listOfPlayers[1]); //add angel
                listOfPlayers[1].AddAbility(FileManager.instance.FindAbility("Share Healing"), false);

                TextCollector collector6 = TurnManager.instance.MakeTextCollector(
                    "Here's your 2nd party member. Right click on the Angel to read what they do.",
                    Vector3.zero);
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;
                Destroy(collector6.gameObject);

                currentStep = 7;
                StartCoroutine(NextStep());
                break;

            case 7: //introduce angel's healing
                Log.instance.AddText("");
                yield return TurnManager.instance.NewWave();
                TurnManager.instance.CreateEnemy(FileManager.instance.FindEnemy("Page"), Emotion.Happy, 0);

                TextCollector collector7 = TurnManager.instance.MakeTextCollector(
                    "This Page is Happy. Happy is super effective against Angry, which puts your Knight at a disadvantage.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector7.WaitForChoice();

                collector7.textbox.text = "Normally, your Knight would take their turn first, because they have the higher Speed. But they’re Stunned right now.";
                yield return collector7.WaitForChoice();

                collector7.textbox.text = "So instead it's your Angel's turn. Use the Angel’s ability to heal the Knight.";
                yield return collector7.WaitForChoice();
                Destroy(collector7.gameObject);

                currentCharacter = listOfPlayers[1]; //wait for angel's next turn
                currentStep = 8;

                while (TurnManager.instance.enemies.Count>0)
                    yield return TurnManager.instance.NewRound();

                currentStep = 9;
                StartCoroutine(NextStep());
                break;

            case 8: //introduce happy
                listOfPlayers[1].AddAbility(FileManager.instance.FindAbility("Induce Sadness"), false);
                TextCollector collector8 = TurnManager.instance.MakeTextCollector(
                    "Your Angel is Happy, which means it can use an extra ability when it doesn't attack, but all its abilities have longer cooldowns.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector8.WaitForChoice();

                collector8.textbox.text = "Since your Angel didn’t attack, it can use an extra ability. Try changing the Page’s Emotion so your Knight will gain the advantage.";
                yield return collector8.WaitForChoice();
                Destroy(collector8.gameObject);
                break;

            case 9: //introduce grounded and airborne
                Log.instance.AddText("");
                yield return TurnManager.instance.NewWave();
                TurnManager.instance.CreateEnemy(FileManager.instance.FindEnemy("Page"), Emotion.Neutral, 0);
                TurnManager.instance.CreateEnemy(FileManager.instance.FindEnemy("Crow"), Emotion.Neutral, 0);
                TurnManager.instance.CreateEnemy(FileManager.instance.FindEnemy("Crow"), Emotion.Neutral, 0);

                TextCollector collector9 = TurnManager.instance.MakeTextCollector(
                    "Characters are either Grounded or Airborne. Your Knight is incapable of hitting Airborne enemies, so they’ll have to attack the Page for now.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector9.WaitForChoice();
                Destroy(collector9.gameObject);
                yield return TurnManager.instance.NewRound();

                currentStep = 10;
                StartCoroutine(NextStep());
                break;

            case 10: //introduce wizard
                TurnManager.instance.AddPlayer(listOfPlayers[2]); //add angel
                listOfPlayers[2].AddAbility(FileManager.instance.FindAbility("Falling Rocks"), false);

                TextCollector collector10 = TurnManager.instance.MakeTextCollector(
                    "Finally, your 3rd party member is the Wizard. Right click on them to read what they do.",
                    Vector3.zero);
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;
                Destroy(collector10.gameObject);

                currentStep = 11;
                StartCoroutine(NextStep());
                break;

            case 11: //introduce wizard abilities
                TextCollector collector11 = TurnManager.instance.MakeTextCollector(
                    "The Wizard has an ability that can force all airborne enemies to be Grounded. When it’s their turn, use Falling Rocks against the 2 Airborne enemies.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector11.WaitForChoice();
                Destroy(collector11.gameObject);

                currentCharacter = listOfPlayers[2]; //wait for wizard's next turn
                currentStep = 12;

                while (TurnManager.instance.enemies.Count > 0)
                    yield return TurnManager.instance.NewRound();

                currentStep = 15;
                StartCoroutine(NextStep());
                break;

            case 12: //introduce sad
                TextCollector collector12 = TurnManager.instance.MakeTextCollector(
                    "Your Wizard is Sad, which means everytime it attacks, it loses some health. But when it uses a non-attacking ability, it regains health.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector12.WaitForChoice();

                collector12.textbox.text = "Attacking abilities are colored red, and non-attacking ones are colored blue.";
                yield return collector12.WaitForChoice();

                Destroy(collector12.gameObject);

                currentStep = 13;
                yield return (NextStep());
                break;

            case 13: //emotions guide
                TextCollector collector13 = TurnManager.instance.MakeTextCollector(
                    "You can always remind yourself of the Emotions by opening the Emotion Guide in the bottom right.",
                    Vector3.zero);
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Emotion)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;
                Destroy(collector13.gameObject);

                currentStep = 14;
                yield return NextStep();
                break;

            case 14: //finish off the enemies
                listOfPlayers[2].AddAbility(FileManager.instance.FindAbility("Blaze"), false);
                listOfPlayers[2].AddAbility(FileManager.instance.FindAbility("Fog"), false);
                TextCollector collector14 = TurnManager.instance.MakeTextCollector(
                    "Anyways, now that the enemies are Grounded, your Knight is capable of hitting them, so finish off those enemies.",
                    Vector3.zero, new List<string>() { "Next" });
                yield return collector14.WaitForChoice();
                Destroy(collector14.gameObject);
                break;

            case 15: //tutorial over
                TurnManager.instance.GameFinished("Tutorial finished.", "");
                TextCollector collector15 = TurnManager.instance.MakeTextCollector(
                    "You’ve completed the tutorial. In the actual game, each character has 5 random abilities, and they also have random Weapons. Good luck!",
                    Vector3.zero);
                break;
        }
    }
}
