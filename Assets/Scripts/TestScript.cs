using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TestScript : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    private void Start()
    {
        inputField.onValueChanged.AddListener(SearchAbility);
        inputField.gameObject.SetActive(false);
    }

    void SearchAbility(string newValue)
    {
        IEnumerable<AbilityData> searchName = FileManager.instance.listOfAbilities.Where
            (ability => ability.myName.Contains(newValue, StringComparison.OrdinalIgnoreCase));

        IEnumerable<AbilityData> searchDescription = FileManager.instance.listOfAbilities.Where
            (ability => ability.description.Contains(newValue, StringComparison.OrdinalIgnoreCase));

        Debug.Log($"{newValue}: Names: {searchName.Count()} Descriptions: {searchDescription.Count()}");
    }
}
