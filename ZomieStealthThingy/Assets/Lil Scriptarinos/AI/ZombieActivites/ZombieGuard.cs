using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

/// <summary>
/// Script for zombie "guard" activity
/// </summary>
public class ZombieGuard : ZombieActivityBase
{
    //Contains the rotation data for the guard behavior
    [SerializeField] RotaTime Configure = new RotaTime();
    Timer RotTimer;
    // Start is called before the first frame update
    protected override void Awake()
    {//bla bla bla setup
        SetActivityType(activityState.Guard);
        RotTimer = new Timer(Configure.Delay);
        base.Awake();
    }

    //rests turn timer when token is received
    protected override void OnToken()
    {
        RotTimer.Reset();
    }
    // Update is called once per frame
    void Update()
    {//if rot timer, send rot target and then reset rot timer
        if (hasActivityToken)
        {
            if (RotTimer)
            {
                zombieMove.SetNewRotTarget(Configure.Degrees);
                RotTimer.Reset();
            }
        }
    }
}
