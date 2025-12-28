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
        if (ScreenOverlay.instance.mode != GameMode.Tutorial)
        {
            Destroy(this.gameObject);
        }
        else
        {
            ScreenOverlay.instance.SetAnimationSpeed(0.4f);
            ScreenOverlay.instance.SetUndo(true);
            ScreenOverlay.instance.SetTooltip(true);

            for (int i = 0; i < GameFiles.inst.listOfPlayers.Count; i++)
            {
                PlayerCharacter nextCharacter = Instantiate(characterPrefab).AddComponent<PlayerCharacter>();
                Emotion forcedEmotion = Emotion.Neutral;
                switch (GameFiles.inst.listOfPlayers[i].characterName)
                {
                    case ToTranslate.Knight:
                        knight = nextCharacter;
                        forcedEmotion = Emotion.Angry;
                        break;
                    case ToTranslate.Angel:
                        angel = nextCharacter;
                        forcedEmotion = Emotion.Happy;
                        break;
                    case ToTranslate.Wizard:
                        wizard = nextCharacter;
                        forcedEmotion = Emotion.Sad;
                        break;
                }
                nextCharacter.SetupCharacter(GameFiles.inst.listOfPlayers[i], new List<AbilityData>(), forcedEmotion, false);
            }

            currentStep = 1;
            StartCoroutine(NextStep());
        }
    }

    IEnumerator ClickThroughDialogue(List<ToTranslate> allDialogue)
    {
        TextCollector collector = TurnManager.instance.MakeTextCollector("", Vector3.zero, new List<string>() { "Next" });
        foreach (ToTranslate nextString in allDialogue)
        {
            collector.textbox.text = KeywordTooltip.instance.EditText(AutoTranslate.DoEnum(nextString));
            yield return collector.WaitForChoice();
        }
        Destroy(collector.gameObject);
    }

    public IEnumerator NextStep()
    {
        switch (currentStep)
        {
            case 1: //introduce the concept of the game
                Log.instance.AddText(AutoTranslate.Defeat_Waves("3"));
                Log.instance.AddText("");
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_10 });

                currentStep = 2;
                StartCoroutine(NextStep());
                break;

            case 2: //right click on knight
                TurnManager.instance.AddPlayer(knight); //add knight
                knight.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Strike), false, false);

                string text2 = KeywordTooltip.instance.EditText(AutoTranslate.DoEnum(ToTranslate.Tutorial_20));
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
                TurnManager.instance.CreateEnemy(GameFiles.inst.FindBonusEnemy(ToTranslate.Page), Emotion.Neutral, 0);
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_30 });

                yield return TurnManager.instance.NewRound(false);
                currentStep = 4;
                StartCoroutine(NextStep());
                break;

            case 4: //introduce Cooldowns
                knight.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Joust), false, false);
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_40 });

                yield return TurnManager.instance.NewRound(true);
                while (TurnManager.instance.listOfEnemies.Count > 0)
                    yield return TurnManager.instance.NewRound(true);

                currentStep = 5;
                StartCoroutine(NextStep());
                break;

            case 5: //introduce angry
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_50, ToTranslate.Tutorial_51, ToTranslate.Tutorial_52, ToTranslate.Tutorial_53, ToTranslate.Tutorial_54 });
                currentStep = 6;
                StartCoroutine(NextStep());
                break;

            case 6: //introduce angel
                TurnManager.instance.AddPlayer(angel); //add angel
                angel.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Assist), false, false);

                string text6 = KeywordTooltip.instance.EditText(AutoTranslate.DoEnum(ToTranslate.Tutorial_60));
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
                TurnManager.instance.CreateEnemy(GameFiles.inst.FindBonusEnemy(ToTranslate.Page), Emotion.Happy, 0);

                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_70, ToTranslate.Tutorial_71, ToTranslate.Tutorial_72});
                currentCharacter = angel; //wait for angel's next turn
                currentStep = 8;

                yield return TurnManager.instance.NewRound(false);
                while (TurnManager.instance.listOfEnemies.Count>0)
                    yield return TurnManager.instance.NewRound(true);

                currentStep = 9;
                StartCoroutine(NextStep());
                break;

            case 8: //introduce happy
                angel.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Exhaust), false, false);
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_80, ToTranslate.Tutorial_81, ToTranslate.Tutorial_82, ToTranslate.Tutorial_83 });
                break;

            case 9: //introduce grounded and Elevated
                Log.instance.AddText("");
                yield return TurnManager.instance.NewWave();

                knight.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Embarass), false, false);
                knight.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Cheer), false, false);

                angel.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Team_Up), false, false);
                angel.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Motivate), false, false);

                TurnManager.instance.CreateEnemy(GameFiles.inst.FindBonusEnemy(ToTranslate.Page), Emotion.Neutral, 0);
                for (int i = 0; i<2; i++)
                    TurnManager.instance.CreateEnemy(GameFiles.inst.FindBonusEnemy(ToTranslate.Crow), Emotion.Neutral, 0);

                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_90, ToTranslate.Tutorial_91 });
                currentStep = 10;
                StartCoroutine(NextStep());
                break;

            case 10: //introduce wizard
                TurnManager.instance.AddPlayer(wizard); //add wizard
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Stalactites), false, false);

                string text10 = KeywordTooltip.instance.EditText(AutoTranslate.DoEnum(ToTranslate.Tutorial_100));
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
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_110 });
                currentCharacter = wizard; //wait for wizard's next turn
                currentStep = 12;

                yield return TurnManager.instance.NewRound(false);
                while (TurnManager.instance.listOfEnemies.Count > 0)
                    yield return TurnManager.instance.NewRound(true);

                currentStep = 15;
                StartCoroutine(NextStep());
                break;

            case 12: //introduce sad
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_120, ToTranslate.Tutorial_121 });
                currentStep = 13;
                yield return (NextStep());
                break;

            case 13: //emotions guide
                GameObject emotionButton = GameObject.Find("Emotion Button");
                Vector3 originalPos = emotionButton.transform.localPosition;
                emotionButton.transform.localPosition = new Vector2(0, -150);

                string text13 = KeywordTooltip.instance.EditText(AutoTranslate.DoEnum(ToTranslate.Tutorial_130));
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
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Readjust), false, false);
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Flood), false, false);
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(ToTranslate.Shockwave), false, false);

                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_140 });
                break;

            case 15: //tutorial over
                yield return ClickThroughDialogue(new List<ToTranslate>() { ToTranslate.Tutorial_150, ToTranslate.Tutorial_151 });
                TurnManager.instance.GameFinished(ToTranslate.Tutorial_Finished, true);
                break;
        }
    }
}
