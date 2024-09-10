using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookScript : MonoBehaviour
{
    //turn speed modifiers, object being turned, camera refrence and rotation cutoff that keeps turn object from violently vibrate
    [SerializeField, Range (0,2)] private float TurnSpeedMod = 1;
    [SerializeField] internal GameObject TurnObject;
    private Camera cam;
    [SerializeField, Range(1, 360)] private float RotationCutoff = 5;
    //internal rotation value refrence
    Vector3 tRot;
    // Start is called before the first frame update
    void Start()
    {//gets value for trot
        tRot = TurnObject.transform.rotation.eulerAngles;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;
        Vector3 dir = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
        //dir.y *= -1;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg)-90;
        TurnObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //fecteches current mouse position and translates it from ascreen ppoint to a world point
        //Vector2 mPos = Input.mousePosition;
        //mPos = cam.ScreenToWorldPoint(mPos);
        ////mPos = TurnObject.transform.worldToLocalMatrix.MultiplyPoint3x4(mPos);
        ////gets the normal from the player to the mouse position
        //// mPos = mPos.normalized; //new Vector2(mPos.x - transform.position.x, mPos.y - transform.position.y).normalized;
        ////invert y to get it to behave, for some ungodly reason
        ////mPos.y *= -1;
        ////calculate the signed angle from the forwa<rs normal of the turn object and the mouse pos normal
        //TurnObject.transform.LookAt(mPos.ToVector3(), Vector3.back);
        //float turn = Vector2.SignedAngle(TurnObject.transform.InverseTransformDirection(Vector2.up), mPos);
        ////if th e absolute value of the angle is less than cutoff, abort
        //if (Mathf.Abs(turn) < RotationCutoff)
        //{
        //    //tRot.z -= turn * Time.deltaTime * TurnSpeedMod;
        //    //TurnObject.transform.rotation = Quaternion.Euler(tRot);
        //    //if turn is less than 0, add 360
        //    if (turn < 0)
        //        turn += 360;
        //    //move current facing towards target facing by turnspeed
        //    float f = Mathf.MoveTowardsAngle(tRot.z, tRot.z + turn, 360 * TurnSpeedMod * Time.deltaTime);
        //    //update trot value, the conver to quart and apply ro turnobject transform
        //    tRot.z = f;
        //    TurnObject.transform.rotation = Quaternion.Euler(tRot);

        //}

    }
}
