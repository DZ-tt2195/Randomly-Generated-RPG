using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class AbilityBox : MonoBehaviour
{
    public Button button;
    public TMP_Text textName;
    public TMP_Text textDescription;
    public TMP_Text textCountdown;
    public Image groundedOff;
    public Image airborneOff;

    public void ReceiveAbility(Ability ability)
    {
        textName.text = ability.myName;
        textDescription.text = ability.description;
        textCountdown.text = $"{ability.countdown}";
        //groundedOff.gameObject.SetActive(ability.positionTarget == Ability.PositionTarget.All || ability.positionTarget == Ability.PositionTarget.OnlyAirborne);
        //airborneOff.gameObject.SetActive(ability.positionTarget == Ability.PositionTarget.All || ability.positionTarget == Ability.PositionTarget.OnlyGrounded);
    }
}
