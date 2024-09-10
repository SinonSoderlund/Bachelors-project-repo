using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
public class ZombieReturn : ZombieActivityBase
{
    Vector2 target;
    Timer t;
    protected override void Awake()
    {
        SetActivityType(activityState.Returning);
        base.Awake();
    }

    protected override void OnToken()
    {
        target = zombieLook.SP;
        zombieMove.SetNewTarget(target);
        t = new Timer(1);
    }
    // Update is called once per frame
    void Update()
    {
        if(hasActivityToken)
        {
            if(t)
            {
                t.Reset();
                if(Vector2.Distance(transform.position,target) < 0.5f)
                {
                    zombieLook.BehvaiorRetraction();
                }
            }
        }
    }
}
