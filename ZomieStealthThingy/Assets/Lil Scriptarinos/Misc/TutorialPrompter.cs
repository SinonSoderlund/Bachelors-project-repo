using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;


public class TutorialPrompter : InteractableBase
{
    //nool for controlling wether full oor only start prompt will be sent
    [SerializeField] private bool SentStartPromptOnly;
    //prompt start string
    [SerializeField] private string PromptStart;
    //the key of the action being tutorilized
    [SerializeField] private InputType ActionKey;
    //toggle for wether or not correct grammar will be injected regarding toggle states
    [SerializeField] private bool injectToggleState = true;
    //prompt end string
    [SerializeField] private string PromptEnd;
    


    protected override WaitUntil OnInteract()
    {
        return null;
    }

    protected override void InteractEndCall()
    {
        throw new System.NotImplementedException();
    }
    protected override void TopLevelStart()
    {
        if (IM.getInputKeyDisabled(ActionKey))
            LoopBreak();
    }

    protected override PromptRequest PromptConstructor()
    {
        string t;
        if (!SentStartPromptOnly)
        {
            if (!injectToggleState)
               t =(PromptStart + " " + IM.getInputKeyName(ActionKey) + " " + PromptEnd);
            else
                t =(PromptStart + IM.ToggleInjectStart(ActionKey) + IM.getInputKeyName(ActionKey) + IM.ToggleInjectEnd(ActionKey) + PromptEnd);
        }
        else
           t =(PromptStart);
        return new PromptRequest(transform, t);
    }


}
