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

            int counter = 0;
            foreach (var KVP in GameFiles.inst.listOfPlayers)
            {
                PlayerCharacter nextCharacter = Instantiate(characterPrefab).AddComponent<PlayerCharacter>();
                Mood forcedMood = Mood.Lively;

                if (KVP.Key.Equals(nameof(AutoTranslate.Knight)))
                {
                    knight = nextCharacter;
                    forcedMood = Mood.Focused;
                }
                else if (KVP.Key.Equals(nameof(AutoTranslate.Angel)))
                {
                    angel = nextCharacter;
                    forcedMood = Mood.Lively;
                }
                else if (KVP.Key.Equals(nameof(AutoTranslate.Wizard)))
                {
                    wizard = nextCharacter;
                    forcedMood = Mood.Tired;
                }
                nextCharacter.SetupCharacter(KVP.Value, new List<AbilityData>(), forcedMood, counter, false);  
                counter++;             
            }

            currentStep = 1;
            StartCoroutine(NextStep());
        }
    }

    IEnumerator ClickThroughDialogue(List<string> allDialogue)
    {
        foreach (string nextString in allDialogue)
        {
            bool waiting = true;
            MakeDecision.inst.SetTextButtons(nextString, new() {new(AutoTranslate.Next(), Done)});
            void Done()
            {
                waiting = false;
            }
            while (waiting)
                yield return null;
        }
    }

    public IEnumerator NextStep()
    {
        switch (currentStep)
        {
            case 1: //introduce the concept of the game
                Log.instance.AddText(AutoTranslate.Defeat_Waves("3"));
                Log.instance.AddText(AutoTranslate.Blank());
                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_10() });

                currentStep = 2;
                StartCoroutine(NextStep());
                break;

            case 2: //right click on knight
                TurnManager.inst.AddPlayer(knight); //add knight
                knight.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Knock_Down)), false, false);

                MakeDecision.inst.SetInstruction(AutoTranslate.Tutorial_20());
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;

                currentStep = 3;
                StartCoroutine(NextStep());
                break;

            case 3: //first time attacking
                yield return TurnManager.inst.NewWave();
                yield return TurnManager.inst.CreateEnemy(GameFiles.inst.FindSpecificEnemy(nameof(AutoTranslate.Page), 0), Mood.Focused, 0);
                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_30() });

                yield return TurnManager.inst.NewRound(false);
                currentStep = 4;
                StartCoroutine(NextStep());
                break;

            case 4: //introduce Cooldowns
                knight.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Joust)), false, false);
                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_40() });

                yield return TurnManager.inst.NewRound(true);
                while (TurnManager.inst.listOfEnemies.Count > 0)
                    yield return TurnManager.inst.NewRound(true);

                currentStep = 5;
                StartCoroutine(NextStep());
                break;

            case 5: //introduce angry
                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_50(), AutoTranslate.Tutorial_51() });

                MakeDecision.inst.SetInstruction(AutoTranslate.Tutorial_52());

                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Emotion)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;

                currentStep = 6;
                StartCoroutine(NextStep());
                break;

            case 6: //introduce angel
                TurnManager.inst.AddPlayer(angel); //add angel
                angel.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Assist)), false, false);
                angel.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Exhaust)), false, false);

                MakeDecision.inst.SetInstruction(AutoTranslate.Tutorial_60());
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;

                currentStep = 7;
                StartCoroutine(NextStep());
                break;

            case 7: //introduce angel abilities
                Log.instance.AddText(AutoTranslate.Blank());
                yield return TurnManager.inst.NewWave();
                yield return TurnManager.inst.CreateEnemy(GameFiles.inst.FindSpecificEnemy(nameof(AutoTranslate.Page), 0), Mood.Lively, 0);

                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_70(), AutoTranslate.Tutorial_71()});
                yield return TurnManager.inst.NewRound(false);
                while (TurnManager.inst.listOfEnemies.Count>0)
                    yield return TurnManager.inst.NewRound(true);
                currentStep = 9;

                StartCoroutine(NextStep());
                break;

            case 9: //introduce grounded and Elevated
                Log.instance.AddText(AutoTranslate.Blank());
                yield return TurnManager.inst.NewWave();

                knight.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Overwhelm)), false, false);
                knight.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Cheer)), false, false);

                angel.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Team_Up)), false, false);
                angel.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Tailwinds)), false, false);

                yield return TurnManager.inst.CreateEnemy(GameFiles.inst.FindSpecificEnemy(nameof(AutoTranslate.Page), 0), Mood.Tired, 0);
                yield return TurnManager.inst.CreateEnemy(GameFiles.inst.FindSpecificEnemy(nameof(AutoTranslate.Crow), 0), Mood.Focused, 0);
                yield return TurnManager.inst.CreateEnemy(GameFiles.inst.FindSpecificEnemy(nameof(AutoTranslate.Crow), 0), Mood.Lively, 0);

                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_90(), AutoTranslate.Tutorial_91() });
                currentStep = 10;
                StartCoroutine(NextStep());
                break;

            case 10: //introduce wizard
                TurnManager.inst.AddPlayer(wizard); //add wizard
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Stalactites)), false, false);

                MakeDecision.inst.SetInstruction(AutoTranslate.Tutorial_100());
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.Character)
                    yield return null;
                while (ScreenOverlay.instance.displayedScreen != CurrentScreen.None)
                    yield return null;

                currentStep = 11;
                StartCoroutine(NextStep());
                break;

            case 11: //introduce wizard Abilities
                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_110() });
                currentCharacter = wizard; //wait for wizard's next turn
                currentStep = 12;

                yield return TurnManager.inst.NewRound(false);
                while (TurnManager.inst.listOfEnemies.Count > 0)
                    yield return TurnManager.inst.NewRound(true);

                currentStep = 15;
                StartCoroutine(NextStep());
                break;

            case 12: //finish off enemies
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Readjust)), false, false);
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Flood)), false, false);
                wizard.AddAbility(GameFiles.inst.FindPlayerAbility(nameof(AutoTranslate.Shockwave)), false, false);

                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_120(), AutoTranslate.Tutorial_121() });

                yield return TurnManager.inst.NewRound(false);
                while (TurnManager.inst.listOfEnemies.Count > 0)
                    yield return TurnManager.inst.NewRound(true);
                currentStep = 13;
                StartCoroutine(NextStep());
                break;

            case 13: //tutorial over
                yield return ClickThroughDialogue(new List<string>() { AutoTranslate.Tutorial_130(), AutoTranslate.Tutorial_131() });
                TurnManager.inst.GameFinished(AutoTranslate.Tutorial_Finished(), true);
                break;
        }
    }
}
