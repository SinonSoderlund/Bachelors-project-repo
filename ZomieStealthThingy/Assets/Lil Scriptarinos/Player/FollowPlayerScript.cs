using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerScript : MonoBehaviour
{
    //pplayer ref
    private Transform Player;

    // Update is called once per frame
    void Update()
    {//if palayer ref isnt null
        if(!(PlayerMovementScript.player == null))
        {//if internal player ref is null set it to player ref
            if (Player == null)
                Player = PlayerMovementScript.player.transform;
            else
            {//else follow player
                transform.SetXY(Player);
            }
        }
        
    }

}
