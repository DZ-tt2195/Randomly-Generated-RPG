using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using System;

public class EncyclopediaManager : MonoBehaviour
{
    [Foldout("Player Abilities", true)]
    List<AbilityBox> listOfBoxes = new();
    [SerializeField] TMP_InputField abilitySearch;
    [SerializeField] AbilityBox abilityBoxPrefab;
    [SerializeField] Transform storeAbilityBoxes;

    private void Start()
    {
        abilitySearch.onValueChanged.AddListener(SearchAbility);
        foreach (AbilityData data in FileManager.instance.listOfPlayerAbilities)
        {
            Ability nextAbility = this.gameObject.AddComponent<Ability>();
            nextAbility.SetupAbility(data, false, false);
            AbilityBox nextBox = Instantiate(abilityBoxPrefab, null);
            nextBox.ReceiveAbility(true, nextAbility);
            listOfBoxes.Add(nextBox);
        }
        SearchAbility("");
    }

    void SearchAbility(string newValue)
    {
        foreach (AbilityBox box in listOfBoxes)
        {
            if (newValue.Equals("") ||
                (box.ability.data.myName.IndexOf(newValue, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (box.ability.data.description.IndexOf(newValue, StringComparison.OrdinalIgnoreCase) >= 0))
                box.transform.SetParent(storeAbilityBoxes);
            else
                box.transform.SetParent(null);
        }
    }
}
