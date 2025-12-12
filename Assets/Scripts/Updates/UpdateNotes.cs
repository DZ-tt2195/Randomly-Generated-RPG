using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UpdateNotes", menuName = "ScriptableObjects/UpdateNotes")]
public class UpdateNotes : ScriptableObject
{
    public string display;
    [TextArea(25, 0)] public string update;
}