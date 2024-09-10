using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
using MyBox;

public class NoiseMaker : InteractableBase
{
    //ref to audio coupler
    private AudioCoupler AC;
    //variables controlling the current state as well as functioning of this instance
    [SerializeField] private bool IsOn = false;
    [SerializeField] private bool PeriodicEvents = false;
    [SerializeField] private bool SingleUse = false;
    [SerializeField] private float SoundStrenght = 10;
    [ConditionalField(nameof(PeriodicEvents), false), SerializeField] private float PeriodicEventDelay = 0.5f;
    [SerializeField] private float InteractionCooldownTime = 0.5f;
    [ConditionalField(nameof(isInteractionParasite),false), SerializeField, TextArea] private string endPrompt;
    [ConditionalField(nameof(SingleUse),true,true), TextArea, SerializeField] private string endPromptWhenOn;
    private string ActionText;
    private Timer InteractionDelay;
    //just sets the audio coupler refrence and inits a timer
    protected override void TopLevelStart()
    {
        AC = AudioCoupler.audioCoupler;
        InteractionDelay = new Timer(InteractionCooldownTime);
    }
    //A fix to make sure that the periodic noise couroutine runs when it should, important for checkpoint restoration and if the unit is set to start on
    private void OnEnable()
    {
        if (AC == null)
            AC = AudioCoupler.audioCoupler;

        if (IsOn)
            StartCoroutine(NoiseGenerator());
    }
    //On interact implementation
    protected override WaitUntil OnInteract()
    {//if the interqaction delay timer is cleared
        if (InteractionDelay)
        {//and its not a single use object
            if (!SingleUse)
            {//and events are to be periodic
                if (PeriodicEvents)
                {//switch onstate, and if ison, start the noise geenrator coroutine, also retract PPR
                    IsOn = !IsOn;
                    RetractPPR();
                    if (IsOn)
                        StartCoroutine(NoiseGenerator());
                }//else send a single sound event
                else
                {
                    AC.IncommingAudioEvent(SoundStrenght, transform.position);
                }
                InteractionDelay.Reset();
            }
            else
            {//else if events are to be periodic
                if (PeriodicEvents)
                {//switch is on and if ison start the noise genrator couroutine, also retracts PPR
                    IsOn = !IsOn;
                    RetractPPR();
                    if (IsOn)
                        StartCoroutine(NoiseGenerator());
                }
                else
                {//else send single sound event
                    AC.IncommingAudioEvent(SoundStrenght, transform.position);
                }//and break the loop
                LoopBreak();
            }
        }//End call and return
        InteractEndCall();
        return null;
    }
    //flags interaction as being over
    protected override void InteractEndCall()
    {
        OnInteractEnd();
    }
    //constructs the promot text
    protected override PromptRequest PromptConstructor()
    {
        if (IsOn)
            ActionText = "Press " + IM.getInputKeyName(InputType.Interact) + " " + endPromptWhenOn;
        else
            ActionText = "Press " + IM.getInputKeyName(InputType.Interact)+ " " + endPrompt;
        return new PromptRequest(transform, ActionText);
    }


    
    //generates periodic noise events while running
    IEnumerator NoiseGenerator()
    {
        while(IsOn)
        {
            AC.IncommingAudioEvent(SoundStrenght, transform.position);
            yield return new WaitForSeconds(PeriodicEventDelay);
        }
    }

}
