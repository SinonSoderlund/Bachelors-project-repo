using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/DialogueObject", order = 1)]



public class DialogueObject : ScriptableObject
{
    //array of Dialogue entries
    [SerializeField] public DialogueEntry[] FullDialogue;
}
