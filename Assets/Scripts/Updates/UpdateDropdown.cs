using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UpdateDropdown : MonoBehaviour
{
    TMP_Dropdown dropdown;
    TMP_Text updateText;
    UpdateNotes[] listOfUpdates;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        updateText = this.transform.GetChild(0).GetComponent<TMP_Text>();

        listOfUpdates = Resources.LoadAll<UpdateNotes>("Update Notes");
        dropdown.onValueChanged.AddListener(ChangeDropdown);
        for (int i = 0; i < listOfUpdates.Length; i++)
        {
            UpdateNotes nextLog = listOfUpdates[i];
            dropdown.AddOptions(new List<string>() { nextLog.display });
            //Debug.Log(nextLog.display);
        }
        dropdown.value = listOfUpdates.Length - 1;
        void ChangeDropdown(int n)
        {
            updateText.text = KeywordTooltip.instance.EditText(listOfUpdates[dropdown.value].update);
        }
        this.gameObject.SetActive(dropdown.options.Count >= 2);
    }
}
