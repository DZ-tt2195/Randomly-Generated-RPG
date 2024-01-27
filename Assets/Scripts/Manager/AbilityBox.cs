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
    public TMP_Text textName;
    public TMP_Text textDescription;
    public TMP_Text textCountdown;

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
    }
}
