using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;

public class AbilityBox : MonoBehaviour
{
    public Button button;
    [SerializeField] TMP_Text textName;
    [SerializeField] TMP_Text textDescription;
    [SerializeField] TMP_Text textCountdown;
    [SerializeField] TMP_Text onCooldown;

    public void ReceiveAbility(Ability ability)
    {
        try {button.interactable = ability.CanPlay(); } catch (NullReferenceException) { /*do nothing*/ };
        textName.text = ability.myName;
        textDescription.text = ability.description;

        if (ability.baseCooldown > 0)
        {
            textCountdown.transform.parent.gameObject.SetActive(true);
            textCountdown.text = $"{ability.baseCooldown}";
        }
        else
        {
            textCountdown.transform.parent.gameObject.SetActive(false);
        }

        if (ability.currentCooldown > 0)
        {
            onCooldown.transform.parent.gameObject.SetActive(true);
            onCooldown.text = $"{ability.currentCooldown}";
        }
        else
        {
            onCooldown.transform.parent.gameObject.SetActive(false);
        }
    }
}
