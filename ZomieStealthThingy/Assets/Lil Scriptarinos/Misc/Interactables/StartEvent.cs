using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
using UnityEngine.SceneManagement;

public class StartEvent : InteractableBase
{
    [SerializeField] private int sceneTarget;
    [SerializeField] bool isGun = false;
    protected override void TopLevelStart()
    {
        if (isGun && !IM.getInputKeyDisabled(InputType.FireGun))
            OnInteractBegin?.Invoke();
        else if (!isGun && IM.getInputKeyDisabled(InputType.FireGun))
            OnInteractBegin?.Invoke();
        else LoopBreak();

    }


    protected override void InteractEndCall()
    {
        throw new System.NotImplementedException();
    }
    protected override WaitUntil OnInteract()
    {
        SceneManager.LoadScene(sceneTarget);
        return null;
    }
    protected override PromptRequest PromptConstructor()
    {
        throw new System.NotImplementedException();
    }
}
