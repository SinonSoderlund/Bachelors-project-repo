using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

public class EventDelayer : InteractableBase
{
    [SerializeField] float WaitTime;
    bool countdownStarted = false;
    bool eventSent = false;
    [SerializeField] private InteractableBase EventTarget;
    [SerializeField] private bool destroyHost = false;
    protected override WaitUntil OnInteract()
    {
        if (!countdownStarted)
            StartCoroutine(DelayRun());
        else
            StopAllCoroutines();
        return null;
    }
    protected override PromptRequest PromptConstructor()
    {
        return null;
    }
    protected override void InteractEndCall()
    {
        if (!eventSent)
        {
            EventTarget.ExternEntrance();
            OnInteractEnd();
            eventSent = true;
        }
    }
    
    IEnumerator DelayRun()
    {
        InteractEndCall();
        yield return new WaitForSeconds(WaitTime);
        if (destroyHost)
            Destroy(InteractionOrigin);
    }
}
