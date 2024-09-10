using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

/// <summary>
/// Script for handling zombie patrolling
/// </summary>
public class ZombiePatroll : ZombieActivityBase
{
    //Contolls wether or not to vizulise patroll route
    [SerializeField] private bool toggleVisulization = true;
    //Controlls weher to ping pong from end to end of patrol list or to loop through it
    private enum PatrollMode { EndToEnd, Cycle};
    [SerializeField] private PatrollMode patrollMode = PatrollMode.Cycle;
    //Holds patroll Data and vizulization
    [SerializeField] private ColorVisulizer[] PatrollPointList = new ColorVisulizer[0];
    //Index and index direction variables used to loop through the patrol points list
    int index = 0, indexerDir = 1;
    //the distance from the currect target at which it goes to the next index value
    [SerializeField] float TargetSwitchDistance = 0.2f;
    protected override void Awake()
    {
        SetActivityType(activityState.Patroll);
        base.Awake();
    }

    private void Update()
    {
        if(hasActivityToken)
        {//checks if index should be updated, then passes target value to the zombie move script
            index = indexControll();
            zombieMove.SetNewTarget(PatrollPointList[index].Coordinate);
        }
    }

    /// <summary>
    /// Function for checking if distance to targer is less than TSD, then updates index value
    /// </summary>
    /// <returns></returns>
    private int indexControll()
    {
        //distcance test
        if (Vector2.Distance(PatrollPointList[index].Coordinate, transform.position) < TargetSwitchDistance)
        {
            //fetches PPL lenght
            int plenght = PatrollPointList.Length;
            //if cycle mode, simply add indDir to ind, modulate by plenght
            if (patrollMode == PatrollMode.Cycle)
            {
                return (index + indexerDir).mod(plenght);
            }
            else
            {//else check if adding indDir to ind is oob, if so invert indDir, add together and ret results
                if(index + indexerDir == plenght || index + indexerDir == -1)
                {
                    indexerDir /= -1;
                    return index + indexerDir;
                }
                else
                    return index + indexerDir;

            }
        }//out of range of target, return unmodified index
        return index;
    }

    private void OnDrawGizmosSelected()
    { //function for vizulizing the patroll route
        if(toggleVisulization && PatrollPointList.Length != 0)
        {
            Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
            int plenght = PatrollPointList.Length;
            for (int i = 0; i < plenght; i++)
            {
                
                Gizmos.color = PatrollPointList[i].PointColor;
                Gizmos.DrawCube(PatrollPointList[i].Coordinate, size);
                if (patrollMode == PatrollMode.Cycle)
                    Gizmos.DrawLine(PatrollPointList[i].Coordinate, PatrollPointList[(i + 1).mod(plenght)].Coordinate);
                else if((i + 1).mod(plenght) != 0)
                    Gizmos.DrawLine(PatrollPointList[i].Coordinate, PatrollPointList[i + 1].Coordinate);
            }
        }
    }

}
