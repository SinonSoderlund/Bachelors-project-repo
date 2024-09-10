using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
public class ParaSoundPlayer : InteractableBase
{

    [SerializeField] AudioSource sounplay;
    private enum playstat { PlayOnce,TogglePlay };
    [SerializeField] playstat PlayMode;
    Timer Cooldown = new Timer(0.2);
    protected override WaitUntil OnInteract()
    {
        if (Cooldown)
        {
            Cooldown.Reset();
            if (PlayMode == playstat.PlayOnce)
                sounplay.Play();
            else
            {
                if (sounplay.isPlaying)
                    sounplay.Stop();
                else
                    sounplay.Play();

            }
        }
        return null;
    }

    protected override void InteractEndCall()
    {
        throw new System.NotImplementedException();

    }
    protected override PromptRequest PromptConstructor()
    {
        throw new System.NotImplementedException();
    }
}
