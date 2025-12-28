using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyBox;

public class LoadScene : MonoBehaviour
{
    [Scene] [SerializeField] string scene;
    [SerializeField] GameMode gameMode;
    [SerializeField] LoadSceneMode loadMode;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(NextScene);
    }

    void NextScene()
    {
        ScreenOverlay.instance.UnloadObjects(SceneManager.GetActiveScene().name, scene, gameMode);
    }
}
