using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [ReadOnly] public Character currentCharacter;
    int currentStep;

    [SerializeField] GameObject characterPrefab;
    Character knight;
    Character angel;
    Character wizard;

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
            ScreenOverlay.instance.SetTooltip(true);

            List<CharacterData> playerData = DataLoader.ReadCharacterData("Player Data");
            for (int i = 0; i < playerData.Count; i++)
            {
                PlayerCharacter nextCharacter = Instantiate(characterPrefab).AddComponent<PlayerCharacter>();
                Emotion forcedEmotion = Emotion.Neutral;
                switch (playerData[i].myName)
                {
                    case "Knight":
                        knight = nextCharacter;
                        forcedEmotion = Emotion.Angry;
                        break;
                    case "Angel":
                        angel = nextCharacter;
                        forcedEmotion = Emotion.Happy;
                        break;
                    case "Wizard":
                        wizard = nextCharacter;
                        forcedEmotion = Emotion.Sad;
                        break;
                }
                nextCharacter.SetupCharacter(playerData[i], new List<AbilityData>(), forcedEmotion, false);
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
            collector.textbox.text = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate($"Tutorial {nextString}"));
            yield return collector.WaitForChoice();
        }
        Destroy(collector.gameObject);
    }

    public IEnumerator NextStep()
    {
        switch (currentStep)
        {
            case 1: //introduce the concept of the game
                Log.instance.AddText(CarryVariables.instance.Translate("Defeat Waves", new() { ("Num", "3") }));
                Log.instance.AddText("");
                yield return ClickThroughDialogue(new List<string>() { "1.0" });

                currentStep = 2;
                StartCoroutine(NextStep());
                break;

            case 2: //right click on knight
                TurnManager.instance.AddPlayer(knight); //add knight
                knight.AddAbility(CarryVariables.instance.FindPlayerAbility("Strike"), false, false);

                string text2 = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate("Tutorial 2.0"));
                TextCollector collector2 = TurnManager.instance.MakeTextCollector(text2, Vector3.zero);

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
                TurnManager.instance.CreateEnemy(CarryVariables.instance.FindBonusEnemy("Page"), Emotion.Neutral, 0);
                yield return ClickThroughDialogue(new List<string>() { "3.0" });

                yield return TurnManager.instance.NewRound(false);
                currentStep = 4;
                StartCoroutine(NextStep());
                break;

            case 4: //introduce Cooldowns
                knight.AddAbility(CarryVariables.instance.FindPlayerAbility("Joust"), false, false);
                yield return ClickThroughDialogue(new List<string>() { "4.0" });

                yield return TurnManager.instance.NewRound(true);
                while (TurnManager.instance.listOfEnemies.Count > 0)
                    yield return TurnManager.instance.NewRound(true);

                currentStep = 5;
                StartCoroutine(NextStep());
                break;

            case 5: //introduce angry
                yield return ClickThroughDialogue(new List<string>() { "5.0", "5.1", "5.2", "5.3", "5.4" });
                currentStep = 6;
                StartCoroutine(NextStep());
                break;

            case 6: //introduce angel
                TurnManager.instance.AddPlayer(angel); //add angel
                angel.AddAbility(CarryVariables.instance.FindPlayerAbility("Assist"), false, false);

                string text6 = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate("Tutorial 6.0"));
                TextCollector collector6 = TurnManager.instance.MakeTextCollector(text6, Vector3.zero);

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
                TurnManager.instance.CreateEnemy(CarryVariables.instance.FindBonusEnemy("Page"), Emotion.Happy, 0);

                yield return ClickThroughDialogue(new List<string>() { "7.0", "7.1", "7.2" });
                currentCharacter = angel; //wait for angel's next turn
                currentStep = 8;

                yield return TurnManager.instance.NewRound(false);
                while (TurnManager.instance.listOfEnemies.Count>0)
                    yield return TurnManager.instance.NewRound(true);

                currentStep = 9;
                StartCoroutine(NextStep());
                break;

            case 8: //introduce happy
                angel.AddAbility(CarryVariables.instance.FindPlayerAbility("Exhaust"), false, false);
                yield return ClickThroughDialogue(new List<string>() { "8.0", "8.1", "8.2", "8.3" });
                break;

            case 9: //introduce grounded and Elevated
                Log.instance.AddText("");
                yield return TurnManager.instance.NewWave();

                knight.AddAbility(CarryVariables.instance.FindPlayerAbility("Embarass"), false, false);
                knight.AddAbility(CarryVariables.instance.FindPlayerAbility("Cheer"), false, false);

                angel.AddAbility(CarryVariables.instance.FindPlayerAbility("Team Up"), false, false);
                angel.AddAbility(CarryVariables.instance.FindPlayerAbility("Motivate"), false, false);

                TurnManager.instance.CreateEnemy(CarryVariables.instance.FindBonusEnemy("Page"), Emotion.Neutral, 0);
                TurnManager.instance.CreateEnemy(CarryVariables.instance.FindBonusEnemy("Crow"), Emotion.Neutral, 0);
                TurnManager.instance.CreateEnemy(CarryVariables.instance.FindBonusEnemy("Crow"), Emotion.Neutral, 0);

                yield return ClickThroughDialogue(new List<string>() { "9.0", "9.1" });
                currentStep = 10;
                StartCoroutine(NextStep());
                break;

            case 10: //introduce wizard
                TurnManager.instance.AddPlayer(wizard); //add wizard
                wizard.AddAbility(CarryVariables.instance.FindPlayerAbility("Stalactites"), false, false);

                string text10 = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate("Tutorial 10.0"));
                TextCollector collector10 = TurnManager.instance.MakeTextCollector(text10, Vector3.zero);

                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;
                Destroy(collector10.gameObject);

                currentStep = 11;
                StartCoroutine(NextStep());
                break;

            case 11: //introduce wizard Abilities
                yield return ClickThroughDialogue(new List<string>() { "11.0" });
                currentCharacter = wizard; //wait for wizard's next turn
                currentStep = 12;

                yield return TurnManager.instance.NewRound(false);
                while (TurnManager.instance.listOfEnemies.Count > 0)
                    yield return TurnManager.instance.NewRound(true);

                currentStep = 15;
                StartCoroutine(NextStep());
                break;

            case 12: //introduce sad
                yield return ClickThroughDialogue(new List<string>() { "12.0", "12.1" });
                currentStep = 13;
                yield return (NextStep());
                break;

            case 13: //emotions guide
                GameObject emotionButton = GameObject.Find("Emotion Button");
                Vector3 originalPos = emotionButton.transform.localPosition;
                emotionButton.transform.localPosition = new Vector2(0, -150);

                string text13 = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate("Tutorial 13.0"));
                TextCollector collector13 = TurnManager.instance.MakeTextCollector(text13, Vector3.zero);

                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Emotion)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;
                Destroy(collector13.gameObject);

                emotionButton.transform.localPosition = originalPos;
                currentStep = 14;
                yield return NextStep();
                break;

            case 14: //finish off the enemies
                wizard.AddAbility(CarryVariables.instance.FindPlayerAbility("Readjust"), false, false);
                wizard.AddAbility(CarryVariables.instance.FindPlayerAbility("Flood"), false, false);
                wizard.AddAbility(CarryVariables.instance.FindPlayerAbility("Shockwave"), false, false);

                yield return ClickThroughDialogue(new List<string>() { "14.0" });
                break;

            case 15: //tutorial over
                yield return ClickThroughDialogue(new List<string>() { "15.0", "15.1" });
                TurnManager.instance.GameFinished("Tutorial Finished", true);
                break;
        }
    }
}
