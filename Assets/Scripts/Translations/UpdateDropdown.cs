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
        while (Translator.inst.TranslationExists($"Update_{nextUpdate}") && Translator.inst.TranslationExists($"Update_{nextUpdate}_Text"))
        {
            dropdown.AddOptions(new List<string>() { Translator.inst.Translate($"Update_{nextUpdate}") });
            nextUpdate++;
        }
        dropdown.value = dropdown.options.Count-1;
        ChangeDropdown(dropdown.options.Count-1);

        void ChangeDropdown(int n)
        {
            updateText.text = Translator.inst.Translate($"Update_{dropdown.value}_Text");
        }
        this.gameObject.SetActive(dropdown.options.Count >= 1);
    }
}
