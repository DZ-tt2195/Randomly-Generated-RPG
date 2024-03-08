using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyBox;
using static FileManager;

public class LoadScene : MonoBehaviour
{
    [Scene]
    [SerializeField] string scene;
    [SerializeField] GameMode mode;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(NextScene);
    }

    public void NextScene()
    {
        FileManager.instance.UnloadObjects(scene);
        FileManager.instance.mode = mode;
        SceneManager.LoadScene(scene);
    }
}
