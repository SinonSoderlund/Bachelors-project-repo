using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manages the actual locomotion of the zombie
/// </summary>
public class ZombieMove : MonoBehaviour
{
    //bool to make zombie does not move unless it has a target
    private bool hasTarget = false, hasRotTarget = false;
    //target position and internal rotation tracking
    Vector3 Target;
    Vector3 rot;
    float rotTarget;
    //variables for controlling movement and turn speeds
    [Range(0, 8), SerializeField] private float TurnSpeedModifier = 5;
    [Range(0, 8), SerializeField] private float MoveSpeedModifier = 1;

    /// <summary>
    /// Sets new target for the zombie move script,set has target to ture
    /// </summary>
    /// <param name="coord">the target coordinates</param>
    public void SetNewTarget(Vector2 coord)
    {
        Target = coord;
        hasTarget = true;
        hasRotTarget = false;
    }
    /// <summary>
    /// Sets a rotation target
    /// </summary>
    /// <param name="degrees">the target rotatio in degrees</param>
    public void SetNewRotTarget(float degrees)
    {
        degrees *= -1;
        hasRotTarget = true;
        rotTarget = rot.z + degrees;
        if (degrees < 0)
            rotTarget += 360;
    }

    /// <summary>
    /// Function for unsetting script hastarget bool
    /// </summary>
    public void UnsetTarget()
    {
        hasTarget = false;
    }

    private void Start()
    {//fetches start rotation
        rot = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        //pnly move when there is a target
        if (hasTarget)
        {
            //calculate direction normal between zombie and target, calculate drection of forward vector in world space
            Vector3 Bline = (Target - transform.position).normalized;
            Vector3 Upity = transform.TransformDirection(Vector3.up);
            //calculate angular diffrence between forward vector and the target direction normal, apply rotation mutiplied by speed variable
            rot.z += Vector2.SignedAngle(Upity.ToVector2(), Bline.ToVector2()) * Time.deltaTime * TurnSpeedModifier;
            //set new rotation
            transform.rotation = Quaternion.Euler(rot);
            //apply forwards movement to zzombie
            transform.position += Upity * Time.deltaTime * MoveSpeedModifier;
        }
        if(hasRotTarget)
        {//if has rot target, rrotate towrds target, if at target, stop rotate
            float f =  Mathf.MoveTowardsAngle(rot.z, rotTarget, 360 * TurnSpeedModifier * Time.deltaTime);
            rot.z = f;
            transform.rotation = Quaternion.Euler(rot);
            if (rot.z.Equals(rotTarget))
                hasRotTarget = false;
        }
    }
}
