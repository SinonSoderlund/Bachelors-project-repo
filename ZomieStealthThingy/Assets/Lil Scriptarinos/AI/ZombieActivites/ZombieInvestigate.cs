using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

public class ZombieInvestigate : ZombieActivityBase
{
    //the point being investigated currently
    private Vector2 investigationPoint;
    [SerializeField] float DUpdateTimer = 1;
    [SerializeField] float InvestigationDistanceBreak = 0.7f;
    [SerializeField] int nrOfStillUpdateBeforeBehaviorREturn = 3;
    int SUBCounter;
    Timer SUBTimer;
    bool searching = true;
    // Start is called before the first frame update
    protected override void Awake()
    {
        SetActivityType(activityState.Investigating);
        base.Awake();
        zombieLook.NewTarget += SetNewInvestigate;
    }

    protected override void OnToken()
    {
        SUBCounter = 0;
        SUBTimer = new Timer(DUpdateTimer);
    }

    /// <summary>
    /// Function for setting new point to investigate
    /// </summary>
    /// <param name="coords">the investigation position</param>
    public void SetNewInvestigate(activityState Astate, Vector2 coords)
    {
        if (Astate == activityState.Investigating)
        {
            //Temp implimentation, want to implement as "swaying behavior" in movement, especilly when investigating, for which this woud be useful
            investigationPoint = coords;
            //tells movescript to move to position
            zombieMove.SetNewTarget(coords);
            SUBCounter = 0;
            SUBTimer = new Timer(DUpdateTimer);
            searching = true;
        }
    }


    private void Update()
    {
        if (hasActivityToken)
        {
            if (SUBTimer)
            {
                if (searching)
                {
                    if (Vector2.Distance(investigationPoint, transform.position) < InvestigationDistanceBreak)
                    {
                        searching = false;
                        zombieMove.UnsetTarget();
                    }
                }
                else
                {
                    SUBCounter++;
                    print(SUBCounter);
                    if (SUBCounter > nrOfStillUpdateBeforeBehaviorREturn)
                        zombieLook.RetProt();
                }
                SUBTimer.Reset();
            }
        }
    }
}
