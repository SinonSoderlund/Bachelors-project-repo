using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
using MyBox;
public class GateScript : InteractableBase
{
    [SerializeField] bool moveInX = true;
    [SerializeField] float moveByUnits = 5;
    [SerializeField] float moveTime= 3;
    [SerializeField] bool MoveOnce = true;
    Vector2 TargetPos, StartPos;
    float TCounter;
    bool hasMove = false;
    [ButtonMethod]
    private void Swapsies()
    {
        Vector2 v = transform.position;
        transform.position = TargetPos;
        TargetPos = v;
    }
    private void OnValidate()
    {
        TargetPos = transform.position;
        if (moveInX)
            TargetPos.x += moveByUnits;
        else TargetPos.y += moveByUnits;
    }
    protected override void InteractEndCall()
    {
        OnInteractEnd();
    }
    protected override WaitUntil OnInteract()
    {
        if (!hasMove || !MoveOnce)
        {
            StartPos = transform.position;
            OnValidate();
            StartCoroutine(Mover());
        }
        return null;

    }
    protected override PromptRequest PromptConstructor()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Mover()
    {
        while(true)
        {
            hasMove = true;
            TCounter += Time.deltaTime;
            transform.position = Vector2.Lerp(StartPos, TargetPos, TCounter / moveTime);
            if (TCounter / moveTime > 1)
                break;
            yield return new WaitForEndOfFrame();
        }
    }
}
