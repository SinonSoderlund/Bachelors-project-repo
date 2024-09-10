using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

public class DissapearInteractable : InteractableBase
{
    //the prompt end text
    [TextArea, SerializeField] private string PromptEnd;

    //we all know what this does by niw
    protected override void InteractEndCall()
    {
        OnInteractEnd();
    }
    //and this one
    protected override PromptRequest PromptConstructor()
    {
        return new PromptRequest(transform, "Press " + IM.getInputKeyName(InputType.Interact) + " " + PromptEnd);
    }
    //sends interact end call, then asks for a script end and deactivates the object
    protected override WaitUntil OnInteract()
    {
        InteractEndCall();
        ScriptEnder();
        gameObject.SetActive(false);


        return null;
    }
}
