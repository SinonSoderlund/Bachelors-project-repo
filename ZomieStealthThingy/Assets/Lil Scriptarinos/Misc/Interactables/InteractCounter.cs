using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
public class InteractCounter : InteractableBase
{

    [SerializeField] int counter;
    [SerializeField] int countertarget;
    internal override void ExternEntrance()
    {
        counter++;
        if (counter >= countertarget)
            InteractEndCall();
    }
    protected override WaitUntil OnInteract()
    {
        throw new System.NotImplementedException();
    }
    protected override void InteractEndCall()
    {
        OnInteractEnd();
    }
    protected override PromptRequest PromptConstructor()
    {
        throw new System.NotImplementedException();
    }
}
