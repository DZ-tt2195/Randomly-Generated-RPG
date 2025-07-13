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
        this.transform.localScale = Vector3.one;
        this.ability = ability;
        if (button != null)
            button.interactable = disableOverlay;

        if (ability == null)
        {
            textName.text = "";
            textCountdown.transform.parent.gameObject.SetActive(false);
            cantUse.transform.parent.gameObject.SetActive(true);
            cantUse.text = "";
            image.color = Color.gray;
            hover.enabled = false;
            hover.background.gameObject.SetActive(false);
        }

        else
        {
            textName.text = CarryVariables.instance.GetText(ability.data.myName);
            hover.enabled = true;
            hover.NewDescription(ability.editedDescription);

            image.color = ability.mainType switch
            {
                AbilityType.Attack => Color.red,
                AbilityType.Healing => new Color(0, 0.7f, 0),
                _ => Color.blue,
            };

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
