using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TranslateDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown { get; private set; }
    public List<(string, string)> translatedOptions = new();

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            string original = dropdown.options[i].text;
            string translated = Translator.inst.Translate(original);
            translatedOptions.Add((original, translated));
            dropdown.options[i].text = translated;
        }
    }

    public string GetOriginal()
    {
        return translatedOptions[dropdown.value].Item1;
    }
}
