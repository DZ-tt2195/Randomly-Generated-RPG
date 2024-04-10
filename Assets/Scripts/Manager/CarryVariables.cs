using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarryVariables : MonoBehaviour
{
    public enum GameMode { Main, Tutorial, Other };
    public static CarryVariables instance;

    [Foldout("Cheats and Challenges", true)]
        [ReadOnly] public List<string> listOfCheats = new();
        [ReadOnly] public List<string> listOfChallenges = new();

    [Foldout("Misc info", true)]
        public GameMode mode;
        [ReadOnly] public Transform sceneCanvas;
        [SerializeField] Canvas permanentCanvas;

    [Foldout("Scene transition", true)]
        [SerializeField] Image transitionImage;
        [SerializeField] float transitionTime;

    private void Awake()
    {
        sceneCanvas = GameObject.Find("Canvas").transform;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        permanentCanvas.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator UnloadObjects(string originalScene, string nextScene, GameMode mode)
    {
        yield return SceneTransitionEffect(0);

        FileManager.instance.listOfPlayers.RemoveAll(item => item == null);
        if (!originalScene.Equals("1. Battle"))
        {
            foreach (Character player in FileManager.instance.listOfPlayers)
            {
                player.gameObject.transform.SetParent(null);
                DontDestroyOnLoad(player.gameObject);
            }
        }

        this.mode = mode;
        SceneManager.LoadScene(nextScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneCanvas = GameObject.Find("Canvas").transform;
        StartCoroutine(BringBackObjects());
    }

    IEnumerator BringBackObjects()
    {
        yield return SceneTransitionEffect(1);
        transitionImage.gameObject.SetActive(false);
    }

    IEnumerator SceneTransitionEffect(float begin)
    {
        transitionImage.gameObject.SetActive(true);
        transitionImage.SetAlpha(begin);

        float waitTime = 0f;
        while (waitTime < transitionTime)
        {
            transitionImage.SetAlpha(Mathf.Abs(begin - (waitTime / transitionTime)));
            waitTime += Time.deltaTime;
            yield return null;
        }

        transitionImage.SetAlpha(Mathf.Abs(begin - 1));
        transitionImage.gameObject.SetActive(true);
    }

    public bool ActiveCheat(string cheat)
    {
        return (mode == GameMode.Main && listOfCheats.Contains(cheat));
    }

    public bool ActiveChallenge(string challenge)
    {
        return (mode == GameMode.Main && listOfChallenges.Contains(challenge));
    }

}
