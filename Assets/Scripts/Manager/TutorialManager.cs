using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[Serializable]
class ForcedPlayerAbilities
{
    public string name;
    public Emotion forcedEmotion;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [ReadOnly] public Character currentCharacter;
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
        if (CarryVariables.instance.mode != CarryVariables.GameMode.Tutorial)
        {
            Destroy(this.gameObject);
        }
        else
        {
            ScreenOverlay.instance.SetAnimationSpeed(0.4f);
            ScreenOverlay.instance.SetUndo(true);

            List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data", 0);
            for (int i = 0; i < playerData.Count; i++)
            {
                PlayerCharacter nextCharacter = Instantiate(characterPrefab).AddComponent<PlayerCharacter>();
                nextCharacter.SetupCharacter(CharacterType.Player, playerData[i], new List<AbilityData>(), forcedPlayerInfo[i].forcedEmotion, false);
                listOfPlayers.Add(nextCharacter);
            }

            currentStep = 1;
            StartCoroutine(NextStep());
        }
    }

    IEnumerator ClickThroughDialogue(List<string> allDialogue)
    {
        TextCollector collector = TurnManager.instance.MakeTextCollector("", Vector3.zero, new List<string>() { "Next" });
        foreach (string nextString in allDialogue)
        {
            collector.textbox.text = KeywordTooltip.instance.EditText(nextString);
            yield return collector.WaitForChoice();
        }
        Destroy(collector.gameObject);
    }

    public IEnumerator NextStep()
    {
        switch (currentStep)
        {
            case 1: //introduce the concept of the game
                yield return ClickThroughDialogue(new List<string>()
                { "This is a turn-based RPG where you're given random Abilities to fight against random Enemies." });

                currentStep = 2;
                StartCoroutine(NextStep());
                break;

            case 2: //right click on knight
                TurnManager.instance.AddPlayer(listOfPlayers[0]); //add knight
                listOfPlayers[0].AddAbility(FileManager.instance.FindPlayerAbility("Stab"), false, false);

                TextCollector collector2 = TurnManager.instance.MakeTextCollector(
                    "You always have the same 3 party members. Right click on the Knight to see what they do.",
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
                TurnManager.instance.CreateEnemy(FileManager.instance.FindBonusEnemy("Page"), Emotion.Neutral, 0);

                yield return ClickThroughDialogue(new List<string>()
                { "Here's your first Enemy. Use the Knight's Ability against them." });

                yield return TurnManager.instance.NewRound();
                currentStep = 4;
                StartCoroutine(NextStep());
                break;

            case 4: //introduce Cooldowns
                listOfPlayers[0].AddAbility(FileManager.instance.FindPlayerAbility("Joust"), false, false);
                yield return ClickThroughDialogue(new List<string>()
                { "The Ability you used has a cooldown (Cooldown), so you can't use it again this round. Instead use the other Ability." });

                yield return TurnManager.instance.NewRound();
                currentStep = 5;
                StartCoroutine(NextStep());
                break;

            case 5: //introduce angry
                yield return ClickThroughDialogue(new List<string>()
                { "You killed your first Enemy! Now you may have noticed your Knight is \"Angry\".",
                "Everyone begins with a random Emotion. Emotions are effective against other Emotions, and have their own side effects.",
                "Happy beats Angry, which beats Sad, which beats Happy. Neutral is neutral against everything else.",
                "This game, your Knight started off Angry, which means they get +2 Power (attacks and healing abilities are stronger).",
                "However, if they kill an Enemy (like they just did right now), or heal someone to full Health, they get Stunned and miss their next turn."});

                currentStep = 6;
                StartCoroutine(NextStep());
                break;

            case 6: //introduce angel
                TurnManager.instance.AddPlayer(listOfPlayers[1]); //add angel
                listOfPlayers[1].AddAbility(FileManager.instance.FindPlayerAbility("Invigorate"), false, false);

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
                TurnManager.instance.CreateEnemy(FileManager.instance.FindBonusEnemy("Page"), Emotion.Happy, 0);

                yield return ClickThroughDialogue(new List<string>()
                { "This Page is Happy. Happy is super effective against Angry, which puts your Knight at a disadvantage.",
                "Your Knight has the higher Speed, so normally they'd take their turn before the Angel. But they're Stunned right now.",
                "So instead, use the Angel’s Ability to heal the Knight."});

                currentCharacter = listOfPlayers[1]; //wait for angel's next turn
                currentStep = 8;

                while (TurnManager.instance.listOfEnemies.Count>0)
                    yield return TurnManager.instance.NewRound();

                currentStep = 9;
                StartCoroutine(NextStep());
                break;

            case 8: //introduce happy
                listOfPlayers[1].AddAbility(FileManager.instance.FindPlayerAbility("Induce Sadness"), false, false);

                yield return ClickThroughDialogue(new List<string>()
                { "Your Angel is Happy, which means they get an extra turn when they don't attack, but all their Abilities have 1 more turn of Cooldown.",
                "Emotions also apply to Enemies. If you right click the Page, you'll see that their attack Ability got placed on Cooldown.",
                "Your Angel didn’t attack, so they now get an extra turn. However, Invigorate is on Cooldown.",
                "Instead, you can change the Page’s Emotion to Sad. That way, your Angry Knight will gain the advantage."});
                break;

            case 9: //introduce grounded and airborne
                Log.instance.AddText("");
                yield return TurnManager.instance.NewWave();

                listOfPlayers[0].AddAbility(FileManager.instance.FindPlayerAbility("Shun"), false, false);
                listOfPlayers[0].AddAbility(FileManager.instance.FindPlayerAbility("Meal"), false, false);

                listOfPlayers[1].AddAbility(FileManager.instance.FindPlayerAbility("Share Healing"), false, false);
                listOfPlayers[1].AddAbility(FileManager.instance.FindPlayerAbility("Delegate"), false, false);

                TurnManager.instance.CreateEnemy(FileManager.instance.FindBonusEnemy("Page"), Emotion.Neutral, 0);
                TurnManager.instance.CreateEnemy(FileManager.instance.FindBonusEnemy("Crow"), Emotion.Neutral, 0);
                TurnManager.instance.CreateEnemy(FileManager.instance.FindBonusEnemy("Crow"), Emotion.Neutral, 0);

                yield return ClickThroughDialogue(new List<string>()
                { "Everyone is either Grounded or Airborne. Your Knight always starts Grounded, and Angel always starts Airborne.",
                  "One of the Knight's big weaknesses is that they have no way to attack Airborne Enemies." });

                currentStep = 10;
                StartCoroutine(NextStep());
                break;

            case 10: //introduce wizard
                TurnManager.instance.AddPlayer(listOfPlayers[2]); //add wizard
                listOfPlayers[2].AddAbility(FileManager.instance.FindPlayerAbility("Stalactites"), false, false);

                TextCollector collector10 = TurnManager.instance.MakeTextCollector(
                    "And now, your 3rd party member. Right click on the Wizard to read what they do.",
                    Vector3.zero);
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;
                Destroy(collector10.gameObject);

                currentStep = 11;
                StartCoroutine(NextStep());
                break;

            case 11: //introduce wizard Abilities
                yield return ClickThroughDialogue(new List<string>()
                { "Your Wizard has an Ability that forces all Airborne Enemies to be Grounded. Use Stalactites against the 2 Crows." });

                currentCharacter = listOfPlayers[2]; //wait for wizard's next turn
                currentStep = 12;

                while (TurnManager.instance.listOfEnemies.Count > 0)
                    yield return TurnManager.instance.NewRound();

                currentStep = 15;
                StartCoroutine(NextStep());
                break;

            case 12: //introduce sad
                yield return ClickThroughDialogue(new List<string>()
                { "Your Wizard is Sad, which means everytime they attack, they gain 2 Health. But if they don't attack, they lose 2 Health.",
                "For your convenience, attacking Abilities are colored red, healing Abilities are colored green, the others are colored blue."});

                currentStep = 13;
                yield return (NextStep());
                break;

            case 13: //emotions guide
                TextCollector collector13 = TurnManager.instance.MakeTextCollector(
                    "You can always remind yourself of what each Emotion does by opening the Emotion Guide in the bottom right corner.",
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
                listOfPlayers[2].AddAbility(FileManager.instance.FindPlayerAbility("Blaze"), false, false);
                listOfPlayers[2].AddAbility(FileManager.instance.FindPlayerAbility("Ice Blast"), false, false);
                listOfPlayers[2].AddAbility(FileManager.instance.FindPlayerAbility("Intimidation"), false, false);

                yield return ClickThroughDialogue(new List<string>()
                { "Anyways, now that all the Enemies are Grounded, your Knight is capable of hitting them, so finish them off."});
                break;

            case 15: //tutorial over
                TurnManager.instance.GameFinished("Tutorial finished.", "");
                TextCollector collector15 = TurnManager.instance.MakeTextCollector(
                    "You’ve completed the tutorial. In the main game, each character gets 6 random Abilities to fight 5 waves of Enemies. Glhf.",
                    Vector3.zero);
                break;
        }
    }
}
