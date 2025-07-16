using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SetText : MonoBehaviour
{
    [SerializeField] string key;
    TMP_Text textBox;

    private void OnEnable()
    {
        textBox = GetComponent<TMP_Text>();
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.sceneLoaded += OnSceneLoaded;
        else
            Translate();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Translate()
    {
        textBox.text = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate(key));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex >= 1)
            Translate();
    }
}