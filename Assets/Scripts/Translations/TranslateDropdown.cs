using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TranslateDropdown : MonoBehaviour
{
    TMP_Dropdown dropdown;

    void Awake()
    {
        if (CarryVariables.instance == null)
            SceneManager.LoadScene(0);

        dropdown = GetComponent<TMP_Dropdown>();
        for (int i = 0; i < dropdown.options.Count; i++)
            dropdown.options[i].text = CarryVariables.instance.Translate(dropdown.options[i].text);
    }
}
