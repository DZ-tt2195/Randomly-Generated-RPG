using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnloadScene : MonoBehaviour
{
    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(CloseScene);
    }

    public void CloseScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
    }

}
