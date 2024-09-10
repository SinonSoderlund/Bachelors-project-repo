using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
public class ParasiteObjectEnables : InteractableBase
{
    [SerializeField] GameObject Enabler;
    protected override PromptRequest PromptConstructor()
    {
        return null;
    }
    protected override void InteractEndCall()
    {
        throw new System.NotImplementedException();
    }
    protected override WaitUntil OnInteract()
    {
        Enabler.SetActive(true);
        return null;
    }
}
