using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

/// <summary>
/// Class for the zombie chase activity
/// </summary>
public class ZombieChase : ZombieActivityBase
{
    //Refrence to the player gameobject
    private Transform playeref;
    private Timer DeltaTimer;
    private float lastDistanceDelta;
    private int deltaCounter;
    [SerializeField, Tooltip("Nr of seconds between every distance check update")] private float TimerDelta = 0.5f;
    [SerializeField, Tooltip("Number of consequtive distance delta increases before zombie disengages chase behavior")] private int DisengagementDelta = 5;
    [SerializeField] private float GruntBaseTime;
    [SerializeField] private float GrundRandomRange;
    [SerializeField] private AudioSource GruuntSOUNd;
    Timer grunttimer;
    protected override void Awake()
    {
        SetActivityType(activityState.Chasing);
        //Calls base start function and then fetches playerref
        base.Awake();
    }

    private void Start()
    {
        playeref = PlayerMovementScript.player.transform;

    }

    protected override void OnToken()
    {
        DeltaTimer = new Timer(TimerDelta);
        deltaCounter = 0;
        lastDistanceDelta = 0;
        grunttimer = new Timer(GruntBaseTime + Random.Range(-GrundRandomRange, GrundRandomRange));
        PlayerMovementScript.player.PlayerHidden += zombieLook.RetProt;
    }
    protected override void OnTokenLoss()
    {
        PlayerMovementScript.player.PlayerHidden -= zombieLook.RetProt;

    }

    private void Update()
    {
        //if token is aquired, moev towards player
        if(hasActivityToken)
        {
            zombieMove.SetNewTarget(playeref.position);
            if (DeltaTimer)
            {
                DeltaTimer.Reset();
                float t = Vector2.Distance(transform.position, playeref.position);
                if (t > lastDistanceDelta)
                    deltaCounter++;
                else
                    deltaCounter = 0;

                lastDistanceDelta = t;
                if (deltaCounter > DisengagementDelta)
                {
                    zombieMove.UnsetTarget();
                    zombieLook.RetProt();
                }

            }
            if(grunttimer)
            {
                grunttimer = new Timer(GruntBaseTime + Random.Range(-GrundRandomRange, GrundRandomRange));
                if (GruuntSOUNd != null)
                GruuntSOUNd.Play();
            }
        }



    }

}
