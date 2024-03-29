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
    public Ability ability { get; private set; }
    [SerializeField] Image image;
    [SerializeField] TMP_Text textName;
    [SerializeField] TMP_Text textCountdown;
    [SerializeField] TMP_Text cantUse;
    [SerializeField] HoverImage hover;

    public void ReceiveAbility(bool disableOverlay, Ability ability)
    {
        this.ability = ability;
        try { button.interactable = disableOverlay; } catch (NullReferenceException) { /*do nothing*/ };

        if (ability == null)
        {
            textCountdown.transform.parent.gameObject.SetActive(false);
            cantUse.transform.parent.gameObject.SetActive(true);
            cantUse.text = "X";
            image.color = Color.red;
            hover.enabled = false;
        }

        else
        {
            textName.text = ability.data.myName;
            hover.enabled = true;
            hover.NewDescription(ability.editedDescription);
            image.color = (ability.data.typeOne == AbilityType.Attack || ability.data.typeTwo == AbilityType.Attack) ? Color.red : Color.blue;

            if (ability.data.baseCooldown > 0)
            {
                textCountdown.transform.parent.gameObject.SetActive(true);
                textCountdown.text = $"{ability.data.baseCooldown}";
            }
            else
            {
                textCountdown.transform.parent.gameObject.SetActive(false);
            }

            if (!disableOverlay && ability.currentCooldown > 0)
            {
                cantUse.transform.parent.gameObject.SetActive(true);
                cantUse.text = $"{ability.currentCooldown}";
            }
            else if (!disableOverlay && button != null && !button.interactable)
            {
                cantUse.transform.parent.gameObject.SetActive(true);
                cantUse.text = "X";
            }
            else
            {
                cantUse.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
