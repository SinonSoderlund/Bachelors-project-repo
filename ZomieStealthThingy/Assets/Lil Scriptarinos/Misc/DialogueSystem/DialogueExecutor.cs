using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilitySpace;


public class DialogueExecutor : InteractableBase
{
    //is the dialogue repetable or not
    [SerializeField] bool isReapetable = true;
    //refrence to the dialogue object this script is to play
    [SerializeField] DialogueObject Dialogue;
    private string ActionText;
    int ND;
    // sets the correct action text for the prompt generation
    protected override void TopLevelStart()
    {
        ActionText = "Press " + IM.getInputKeyName(InputType.Interact) + " to talk.";
    }
    //Constructs the PR
    protected override PromptRequest PromptConstructor()
    {
        return new PromptRequest(transform, ActionText);
    }

    //Starts the dialogue coroutine and either loop breaks or returns a waituntil thatll last until the dialogue is finished
    protected override WaitUntil OnInteract()
    {
        StartCoroutine(Dialogueroutine());
        if (!isReapetable)
        {
            LoopBreak();
            return null;
        }
        else return new WaitUntil(PCR.IsDialogue);
    }
    //signals the end of the interaction
    protected override void InteractEndCall()
    {
        OnInteractEnd();
    }
    private bool DPT()
    { int t = PCR.NextDialogue();
        if (t == 0)
            return false;
        else
            ND = t;
        return true;
    }

    //loops through every eelemnt in the dialogue object and sends it to the promot controller, them ends the dialogue and signal interaction end
    private IEnumerator Dialogueroutine()
    {
        int i = 0;
        while (i < Dialogue.FullDialogue.Length)
        {
            PCR.SendDialogue(Dialogue.FullDialogue[i]);
            yield return new WaitUntil(DPT);
            i = Mathf.Clamp(i + ND, 0, Dialogue.FullDialogue.Length);
        }
        PCR.EndDialogue();
        InteractEndCall();
    }
}
