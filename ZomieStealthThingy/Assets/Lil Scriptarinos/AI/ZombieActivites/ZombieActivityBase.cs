using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

/// <summary>
/// Base class for the Zombie Activity Scripts, impliments a cmpletely hidden activity token management system
/// </summary>
public class ZombieActivityBase : MonoBehaviour
{
    //The token bool, as well as refrences to the look and move scripts, which, reasonably speaking any activity script would need access to
    private bool activityToken = false;
    protected ZombieLook zombieLook;
    protected ZombieMove zombieMove;
    //for listening to activity update events to see if this script should take the token
    private activityState thisActivity;
    /// <summary>
    /// protected get for activity token to see wether given activity has the right to execute
    /// </summary>
    protected bool hasActivityToken
    {
        get => activityToken;
    }
    // Fectches script refrences, adds giveToekn function to the token recall delegate
    protected virtual void Awake()
    {
        zombieLook = GetComponent<ZombieLook>();
        zombieLook.TokenReturn += giveToken;
        zombieMove = GetComponent<ZombieMove>();
        zombieLook.NewActivityState += getToken;
    }


    /// <summary>
    /// Function for setting the given scripts activity type, is used for intercepting the token pass calls
    /// </summary>
    /// <param name="AState">The activity state to llok for</param>
    protected void SetActivityType(activityState AState)
    {
        thisActivity = AState;
    }


    /// <summary>
    /// Private function for returning the ativity token to zombie look if the current script has it
    /// </summary>
    private void giveToken()
    {
        if (activityToken)
        {
            zombieLook.ReturnToken(ref activityToken);
            OnTokenLoss();
        }
        zombieMove.UnsetTarget();
    }

    protected virtual void OnTokenLoss()
    {

    }

    //function for executing code when a token has been passed to the script
    protected virtual void OnToken()
    {

    }

    /// <summary>
    /// Public function for handing activity token to this script
    /// </summary>
    /// <param name="Astate">The selected activity to be activated</param>
    /// <param name="b">Zombie look activity token</param>
    private void getToken(activityState Astate, ref bool b)
    {
        if (Astate == thisActivity && b)
        {
            activityToken = b;
            b = false;
            OnToken();
        }
    }

}
