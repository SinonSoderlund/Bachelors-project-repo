using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

public class PlayerHideObject : InteractableBase
{
    //timer for getting in an out of box + timer lenght
    Timer hideToggle;
    [SerializeField] private float TimeToGetInOtOut = 1;
    //internal ref to wether or not player is hiding
    private bool  isHide = false;
    private string ActionText;

    //inits timer
    protected override void TopLevelStart()
    {
        hideToggle = new Timer(TimeToGetInOtOut);
    }
    //creates prompt with differing text depending on wether player is hiding or not
    protected override PromptRequest PromptConstructor()
    {
        if (isHide)
            ActionText = "Press " + IM.getInputKeyName(InputType.Interact) + " to come out.";
        else
            ActionText = "Press " + IM.getInputKeyName(InputType.Interact) + " to hide.";
        return new PromptRequest(transform, ActionText);
    }
    
    protected override void InteractEndCall()
    {
        OnInteractEnd();
    }
    //changes internal hide toggle, resets timer, calls player to hide, switches prompt text
    protected override WaitUntil OnInteract()
    {
        if (hideToggle)
        {
            hideToggle.Reset();
            PlayerMovementScript.player.PlayerHideToggle(transform);
            isHide = !isHide;
            RetractPPR();
        }
        InteractEndCall();
        return null;
    }

}
