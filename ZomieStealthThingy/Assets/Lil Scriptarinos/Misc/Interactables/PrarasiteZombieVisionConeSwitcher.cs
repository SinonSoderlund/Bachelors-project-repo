using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
public class PrarasiteZombieVisionConeSwitcher : InteractableBase
{
    [SerializeField] ZombieLook Zombie;
    protected override PromptRequest PromptConstructor()
    {
        return null;
    }
    protected override void InteractEndCall()
    {
        throw new System.NotImplementedException();
    }
    protected override WaitUntil OnInteract()
    {
        Zombie.coneActiveToggle(true);
        return null;
    }
}
