using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System;

public class WeaponBox : MonoBehaviour
{
    public Weapon weapon { get; private set; }
    [SerializeField] Image image;
    [SerializeField] TMP_Text textName;
    [SerializeField] HoverImage hover;

    public void ReceiveWeapon(Weapon weapon)
    { 
        this.weapon = weapon;
        textName.text = weapon.data.myName;
        image.sprite = weapon.sprite;
        hover.NewDescription(weapon.editedDescription);
    }
}
