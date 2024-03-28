using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyBox;

public class LoadScene : MonoBehaviour
{
    [Scene]
    [SerializeField] string scene;
    [SerializeField] FileManager.GameMode gameMode;
    [SerializeField] LoadSceneMode loadMode;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(NextScene);
    }

    public void NextScene()
    {
        FileManager.instance.UnloadObjects(SceneManager.GetActiveScene().name);
        FileManager.instance.mode = gameMode;
        SceneManager.LoadScene(scene, loadMode);
    }
}
