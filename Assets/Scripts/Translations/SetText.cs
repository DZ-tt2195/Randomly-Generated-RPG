using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SetText : MonoBehaviour
{
    [SerializeField] string key;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Translate();
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Translate()
    {
        GetComponent<TMP_Text>().text = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate(key));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex >= 1)
        {
            Debug.Log($"translate {key}");
            Translate();
        }
    }
}