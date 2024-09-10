using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointRegister : MonoBehaviour
{
    //Ref to see wether or not checkpoint has been triggered yet, has to be public in order to carry over to new world
    public bool isCheckpoointTriggered = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == PlayerMovementScript.player.gameObject)
        if (!isCheckpoointTriggered)
        {//if checkpoint is triggered for first time, tell CPL to set new world copy
            isCheckpoointTriggered = true;
            CheckPointLoader.CPL.OnCheckPoint();
        }

    }


}
