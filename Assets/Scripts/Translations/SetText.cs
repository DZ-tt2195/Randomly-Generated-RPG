using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SetText : MonoBehaviour
{
    [SerializeField] string key;

    private void Start()
    {
        if (CarryVariables.instance == null)
            SceneManager.LoadScene(0);
        else
            GetComponent<TMP_Text>().text = KeywordTooltip.instance.EditText(CarryVariables.instance.Translate(key));
    }
}
