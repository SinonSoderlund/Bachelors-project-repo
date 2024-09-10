using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisulizer : MonoBehaviour
{
    //Variable for holding the decay curve for audio, ref to object sprite renderer, various floats for proper function, bool that stops script execution prior to setup
    private AnimationCurve DecayCurve;
    SpriteRenderer thisSprite;
    internal float MoveSpeed, dist, AudioStrenght;
    private bool setUp = false;
    
    /// <summary>
    /// Sets the object upw ith the correct variables, ensuting its redy to get going
    /// </summary>
    /// <param name="AC">The audio decay curve</param>
    /// <param name="speed">The speed of sound value</param>
    /// <param name="strenght">the strenght of the given audio event</param>
    public void UnitSetup(AnimationCurve AC, float speed, float strenght)
    {
        DecayCurve = AC;
        MoveSpeed = speed;
        AudioStrenght = strenght;
        //dist is initiated to the current x scale of the object
        dist = transform.localScale.x;
        //gets ref to SR and unlocks update routine
        thisSprite = GetComponent<SpriteRenderer>();
        setUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (setUp)
        {//expands the scale of the object by speed
            dist += (MoveSpeed) * Time.deltaTime;
            //if 0 posiiton of decay curve has been surpassed then sound has 0 remaining streght, delete gameobject
            if (DecayCurve.Evaluate(1 - ((dist) / (AudioStrenght*0.5f))) < 0)
                Destroy(gameObject);
            else
            {//apply scale change, fetch SR alpha and set it in accordance with decay curve value for current posiiton
                transform.localScale = dist.ToVector3();
                Color s = thisSprite.color;
                s.a = DecayCurve.Evaluate(1 - (dist) / (AudioStrenght * 0.5f));
                thisSprite.color = s;
            }
        }
    }
}
