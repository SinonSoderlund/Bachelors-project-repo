using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
public class AreaEnterEventer : InteractableBase
{
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerMovementScript.player.gameObject)
            InteractEndCall();
    }

}
