using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
public class PlayerShhoter : InteractableBase
{
    [SerializeField] bool showShootRange = false;
    protected override WaitUntil OnInteract()
    {
        throw new System.NotImplementedException();
    }
    protected override void InteractEndCall()
    {
        throw new System.NotImplementedException();
    }
    protected override PromptRequest PromptConstructor()
    {
        Destroy(PCR.gameObject);
        OnInteractBegin?.Invoke();
        OnInteractEnding?.Invoke();
        PlayerMovementScript.player.OnGetShot(transform.position,0);
        Destroy(this.gameObject);
        return new PromptRequest(PlayerMovementScript.player.transform, "");
    }

    private void OnDrawGizmosSelected()
    {
        if (showShootRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, MID);
        }
    }
}
