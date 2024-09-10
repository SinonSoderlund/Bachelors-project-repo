using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
using MyBox;

public abstract class InteractableBase : MonoBehaviour
{
    //sets wether object is a normal interactable or a parasite
    [SerializeField] public bool isInteractionParasite;
    //{
    //    get;
    //    [SerializeField] protected set;
    //}

    //Variables for narmal interactables, controlls how long the object is asleep for when player is out of range, as well as how far the interaction range of the objects is
    [ConditionalField(nameof(isInteractionParasite),true), SerializeField] private float SleepTime = 5;
    [ConditionalField(nameof(isInteractionParasite), true), SerializeField] private float MaxInteractionDistance = 4;
    protected float MID { get => MaxInteractionDistance; }
    //parasite fields, controlls when the interaction event is intercepted and from which interactable
    [ConditionalField(nameof(isInteractionParasite)), SerializeField] private InteractionEvent InteractionTriggerEvent;
    [ConditionalField(nameof(isInteractionParasite)), SerializeField] protected InteractableBase InteractionOrigin;
    //delegate base and the two instances that send the two types of interaction event timings
    protected delegate WaitUntil InteractionEventCaller();
    protected InteractionEventCaller OnInteractBegin;
    protected InteractionEventCaller OnInteractEnding;
    //input manager refrence
    protected InputManagement IM;
    //bool for checking wether or not a prompt has been sent
    private bool RangeEntry = false;
    //prompt controller refrence
    protected PromptController PCR;
    //internal refrence to prompt request, used for token checking and request retraction
    private PromptRequest PPR;
    //loop breaker and interactended flags
    private bool LoopBreaker = false, interactEnded = false;
    //ref to player transform for distance checking
    private Transform playerRef;
    //the two diffrent interaction event hook ins
    private enum InteractionEvent { InteractionStart, InteractionEnd};
    // Start is called before the first frame upd1ate
    private void Start()
    {
        //fetches external refrences
        IM = InputManagement.InputManager;
        PCR = PromptController.promptController;
        playerRef = PlayerMovementScript.player.transform;
        //Calls TLS
        TopLevelStart();
        //either starts interactable loop or the parasite setup fubction
        if (!isInteractionParasite)
            StartCoroutine(InteractableMainLoop());
        else ParasiteUp();
    }

    //adds the parasite to the correct timing event delegate
    private void ParasiteUp()
    {
        if (InteractionTriggerEvent == InteractionEvent.InteractionEnd)
            InteractionOrigin.OnInteractEnding += OnInteract;
        else InteractionOrigin.OnInteractBegin += OnInteract;
    }

    //function for stopping loop/script executiion 
    protected void LoopBreak()
    {//set loop breker flag to true, or unsubscribes from the interaction delegate
        LoopBreaker = true;
        if(isInteractionParasite)
            if (InteractionTriggerEvent == InteractionEvent.InteractionEnd)
                InteractionOrigin.OnInteractEnding -= OnInteract;
            else InteractionOrigin.OnInteractBegin -= OnInteract;
    }

    internal virtual void ExternEntrance()
    { }

    //same as loop breaker but it also retracts PPR from PCR
    protected void ScriptEnder()
    {
        LoopBreak();
        if (!isInteractionParasite)
            PCR.RetractPromptRequest(PPR);
        else InteractionOrigin.ScriptEnder();
    }
    /// <summary>
    /// Start function for inherenting scripts
    /// </summary>
    protected virtual void TopLevelStart()
    { }
    /// <summary>
    /// Rettracts current PPR and calls prompt constrcutor for a new one
    /// </summary>
    protected void RetractPPR()
    {
        PCR.RetractPromptRequest(PPR);
        PPR = PromptConstructor();
        PCR.AddPromptRequest(PPR);
    }
    //simpply makews sure that the object prompt is retracted if one was sent if the object is to be destroyed
    private void OnDestroy()
    {
        if (RangeEntry)
        {
            PCR.RetractPromptRequest(PPR);
        }
    }
    //The inetactable main loop
    private IEnumerator InteractableMainLoop()
    {//i floop breaker flag in not set
        while (!LoopBreaker)
        {//calculate bline and manahttan distance to player
            Vector3 Bline = (playerRef.position - transform.position);
            float BManDist = Mathf.Abs(Bline.x + Bline.y);
            //if player is suffienciently far away, pu tobject to sleep
            if (BManDist > 2 * SleepTime * 2)
                yield return new WaitForSeconds(SleepTime);
            else if (Bline.magnitude < MaxInteractionDistance)
            {//else if player is in range and prompt has not been sent to PCR, do so
                if (!RangeEntry)
                {
                    PPR = PromptConstructor();
                    PCR.AddPromptRequest(PPR);
                    RangeEntry = true;
                }//if player has interacted with this objects prompt, and prompt is the presented one, call early interact parasite delegate and oninteract, then block loop from proceeding until interact end is set true (prevents weird dialogue bug)
                if (RangeEntry && IM.GetInput(InputType.Interact) && PPR.TokU.hasToken())
                {
                    if (OnInteractBegin != null)
                        OnInteractBegin();
                    yield return OnInteract();
                    yield return new WaitUntil(InteractEnded);
                }
            }//else if player is out of range and the prompt hasnt been retarcted from PCR, do so
            else if (RangeEntry)
            {
                PCR.RetractPromptRequest(PPR);
                RangeEntry = false;
            }
            //wait for end of frame
            yield return new WaitForEndOfFrame();
        }//if loop is broken and prompt is not retracted, do so
        if (RangeEntry)
        {
            PCR.RetractPromptRequest(PPR);
            RangeEntry = false;
        }
    }
    //abstract functions to be implemented by inherenting classes
    protected abstract PromptRequest PromptConstructor();
    protected abstract WaitUntil OnInteract();
    protected abstract void InteractEndCall();
    //function for setting interactEnded flag, and for calling the interaction end parasite delelgate
    protected void OnInteractEnd()
    {
        interactEnded = true;
        if(OnInteractEnding != null)
            OnInteractEnding();
    }
    //function for checking if the interaction has yet ended, thereby allowing continueaction fo script execution
    private bool InteractEnded()
    {
        if (interactEnded)
        {
            interactEnded = !interactEnded;
            return !interactEnded;
        }
        else return interactEnded;
    }

}
