using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UpdateDropdown : MonoBehaviour
{
    TMP_Dropdown dropdown;
    [SerializeField] TMP_Text updateText;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(ChangeDropdown);

        int nextUpdate = 0;
        while (CarryVariables.instance.TranslationExists($"Update {nextUpdate}") && CarryVariables.instance.TranslationExists($"Update {nextUpdate} Text"))
        {
            dropdown.AddOptions(new List<string>() { CarryVariables.instance.Translate($"Update {nextUpdate}") });
            nextUpdate++;
        }
        dropdown.value = dropdown.options.Count-1;

        void ChangeDropdown(int n)
        {
            updateText.text = CarryVariables.instance.Translate($"Update {dropdown.value} Text");
        }
        this.gameObject.SetActive(dropdown.options.Count >= 1);
    }
}
