using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class SoundEnabler : MonoBehaviour
{
    static bool sneakSwap = false;

    private void Start()
    {
        if(!sneakSwap)
        AudioCoupler.audioCoupler.turnoff();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == PlayerMovementScript.player.gameObject && !sneakSwap)
        {
            AudioCoupler.audioCoupler.turnon();
            sneakSwap = !sneakSwap;
            CheckPointLoader.CPL.couply();
        }
    }

}
