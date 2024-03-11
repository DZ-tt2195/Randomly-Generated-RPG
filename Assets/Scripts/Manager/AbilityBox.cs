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
    [SerializeField] Image image;
    [SerializeField] TMP_Text textName;
    [SerializeField] TMP_Text textCountdown;
    [SerializeField] TMP_Text cantUse;
    [SerializeField] HoverImage hover;

    public void ReceiveAbility(Ability ability, Character user)
    {
        try {button.interactable = ability.CanPlay(user); } catch (NullReferenceException) { /*do nothing*/ };
        textName.text = ability.data.myName;
        hover.NewAbility(ability.data.description);

        if (ability.data.baseCooldown > 0)
        {
            textCountdown.transform.parent.gameObject.SetActive(true);
            textCountdown.text = $"{ability.data.baseCooldown}";
        }
        else
        {
            textCountdown.transform.parent.gameObject.SetActive(false);
        }

        if (ability.currentCooldown > 0)
        {
            cantUse.transform.parent.gameObject.SetActive(true);
            cantUse.text = $"{ability.currentCooldown}";
        }
        else if (button != null && !button.interactable)
        {
            cantUse.transform.parent.gameObject.SetActive(true);
            cantUse.text = "X";
        }
        else
        {
            cantUse.transform.parent.gameObject.SetActive(false);
        }

        image.color = (ability.data.typeOne == AbilityType.Attack || ability.data.typeTwo == AbilityType.Attack) ? Color.red : Color.blue;
    }
}
