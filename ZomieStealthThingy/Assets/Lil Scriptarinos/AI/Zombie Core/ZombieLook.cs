using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
using UnityEngine.U2D;

/// <summary>
/// Class for zombie vision and vision visulization, as well as acting as a mster controll script for hearing and activities
/// </summary>
public class ZombieLook : Shootable
{
    //Delegate for token return from actiities
    public delegate void ReturnActivityToken();
    public ReturnActivityToken TokenReturn;
    //Delegate for passing activity token to chosen activity
    public delegate void SetActivtyState(activityState AState, ref bool AToken);
    public SetActivtyState NewActivityState;
    //Dellegate for passing target to chosen script
    public delegate void TargetUpdate(activityState AState,Vector2 ATarget);
    public TargetUpdate NewTarget;
    //enum for the current activity state
    private activityState currentActivity = activityState.None;
    //enum for determening zombie starting activity
    private enum StartState {Stand, Patroll, Guard };
    [SerializeField] private StartState StartingState = StartState.Stand;
    //The varibles for the zombies FoV and vision ranage, as well as the number of segments that make up the vision cone vizulisation
    [SerializeField, Range(0,360)] float FoV = 90;
    [SerializeField, Range(0, 20)] float VisionRange = 10;
    [SerializeField, Range(1, 30)] int DegreesPerSegment = 10;
    [SerializeField, Range(0, 1)] private float AttackRange;
    //Variable for wait time, used to disable script temporarely when player is well outside of vision range
    float waitTime = 1f;
    //public float x;
    //controll bools for activity, ILRBCA current serves no purpose, may be removed in the future
    private bool IsLastRemainingBrainCellAwake = true, isHearing = false;
    //Refrence to hearing script
    private ZombieHear hearing;
    //hearing cutoff variable, used to enable and diable hearing script based on distance to player
    private float hearingCutoff;
    //Activity token, used to controll activity scripts
    private bool activityToken = true;
    private bool isEat = false;
    //Image used for Vision cone visulization
    private SpriteShapeController Cone;
    [SerializeField] private float soundplaytime;
    [SerializeField] AudioSource MunchSound;
    private Vector2 StartPos;
    public Vector2 SP { get => StartPos; }
    protected override void Start()
    {
        base.Start();
        StartPos = transform.position;
        //fetches script renfrences, calculates wait time and hearing cutoff variables based on the ammout of time it could take for them to
        //need to be activiated due to range entry
        Cone = GetComponentInChildren<SpriteShapeController>();
        waitTime = VisionRange / PlayerMovementScript.player.MoveSpeed;
        hearing = GetComponent<ZombieHear>();
        hearingCutoff = (1-hearing.hearingThreshold) * 50; //TEMP
        //render the vision cone "correctly" in accordance to releant vaiables
        RenderCone();
        Cone.enabled = ZombieVisionedDetctor.ZVD.isConeGide;
        //starts the main routine for the zombie look behavior
        StartCoroutine(ZombieLookRoutine());
        //Ensures that starting behavior is in accordance with set variable
        BehaviorStartUp();
    }

    private void OnValidate()
    {
        RenderCone();
    }


    protected override void OnDead()
    {
        StartingState = StartState.Stand;
        BehvaiorRetraction();
        coneActiveToggle(false);
        StopAllCoroutines();
        Destroy(Cone);
        gameObject.tag = "Untagged";
        GetComponent<ZombieMove>().enabled = false;
        hearing.enabled = false;
        this.enabled = false;

    }

    public void RetProt()
    {
        TokenReturn();
        if (StartingState == StartState.Patroll)
        {
            BehaviorStartUp();
        }
        else
            EnterReturn();

    }

    private void EnterReturn()
    {
        //Note to self, not setting activity state breaks process execution, should not be allowed outside of a activityState protocol really
        currentActivity = activityState.Returning;
        NewActivityState?.Invoke(activityState.Returning, ref activityToken);
    }

    public void BehvaiorRetraction()
    {
        TokenReturn();
        BehaviorStartUp();
    }

    private void BehaviorStartUp()
    {
        if (StartingState == StartState.Patroll)
            StartPatroll();
        else if (StartingState == StartState.Guard)
            StartGuard();
        else
            currentActivity = activityState.None;
    }


    /// <summary>
    /// Function for returning activity token to the look script
    /// </summary>
    /// <param name="b">Acitivty token refrence</param>
    public void ReturnToken(ref bool b)
    {
        if (b)
        {
            activityToken = b;
            b = false;
        }
    }
    /// <summary>
    /// Function is fired in the case of a sound being heard by the zombie hearing script
    /// </summary>
    /// <param name="coords">the origin coordinates for the sound</param>
    public void SoundHeard(Vector2 coords)
    {
        //if the zombie is chasing it sould not investigate, return to ordinary program flow
        if (currentActivity == activityState.Chasing)
            return;
        else if (currentActivity == activityState.Investigating && NewTarget != null) //if investigation is ongoing, set new target
            NewTarget(activityState.Investigating, coords);
            else 
        {//else, if activity token is not available, demand return, then hand token to investigation script, hand it a new target, set 
            //activity state to investigating
            if (!activityToken)
                TokenReturn();
            if(NewActivityState != null)
            NewActivityState(activityState.Investigating, ref activityToken);
            if (NewTarget != null)
                NewTarget(activityState.Investigating, coords);
            currentActivity = activityState.Investigating;
        }

    }
    /// <summary>
    /// Function for entering patroll mode
    /// </summary>
    public void StartPatroll()
    {
        if (currentActivity == activityState.Patroll)
            return;
        else
        {
            if (!activityToken)
                TokenReturn();
            if (NewActivityState != null)
                NewActivityState(activityState.Patroll, ref activityToken);
            currentActivity = activityState.Patroll;
        }
    }
    /// <summary>
    /// Function for entering guard mode
    /// </summary>
    public void StartGuard()
    {
        if (currentActivity == activityState.Guard)
            return;
        else
        {
            if (!activityToken)
                TokenReturn();
            if (NewActivityState != null)
                NewActivityState(activityState.Guard, ref activityToken);
            currentActivity = activityState.Guard;
        }
    }
    /// <summary>
    /// Function for activitating the chase state
    /// </summary>
    public void EnterChase()
    {//if current activiity is chasing, return to prgram flow
        if (currentActivity == activityState.Chasing)
            return;
        else
        {//chase doesnt have any override behaviors, so go directly to demanding token back, then setting program state for current entity
            //to chase mode
            if (!activityToken)
                TokenReturn();
            if (NewActivityState != null)
                NewActivityState(activityState.Chasing, ref activityToken);
            currentActivity = activityState.Chasing;
        }
    }
    /// <summary>
    /// Coroutine check to see wether the look script is currently disabled or not
    /// </summary>
    /// <returns></returns>
    public bool BraintTime()
    {
        return IsLastRemainingBrainCellAwake;
    }
    /// <summary>
    /// Runs the main look routine for a zombie
    /// </summary>
    /// <returns></returns>
    private IEnumerator ZombieLookRoutine()
    {
        //run forever basically
        while(true)
        {
            //Calculates the diagonal from zombie to player, as well as the manhattan distance between the two
            Vector3 Bline = (PlayerMovementScript.player.transform.position - transform.position);
            float BManDist = Mathf.Abs(Bline.x + Bline.y);
            //print(BManDist);
            //cheap check to make sure that a vision check is only made if its possible for the zombie to perceive the player based on distance
            if (BManDist < VisionRange * 2)
            {
                if(!PlayerMovementScript.player.isHiding && Bline.magnitude < AttackRange && !isEat)
                {
                    isEat = true;
                    if (MunchSound != null)
                        StartCoroutine(eats());
                    else
                        CheckPointLoader.CPL.DeathTime();
                }
                //Bline is normalized and then an angle check is made to see weather or not the player is within the field of veiw of the zombie
                Bline = Bline.normalized;
                if (Vector3.Angle(Bline, transform.TransformDirection(Vector3.up)) < FoV / 2)
                {
                    //If the zombie could possibly see the player, a raycast is thrown to see if there is a clear line of sight
                    int layermask = 0b1;
                    RaycastHit2D[] f = Physics2D.RaycastAll(transform.position, Bline, VisionRange, layermask);
                    if(f.Length > 1)
                        if (f[1].collider != null && f[1].collider.gameObject == PlayerMovementScript.player.gameObject)
                        {
                            EnterChase();
                            //print("???");
                        }
                }

            } 
            else if(BManDist > VisionRange*4)
            {//if the player is more than two Vision Range manhattan distances outside of the vision range, 
             //tell zombie to "deactivate" for the ammount of time it takes the player to traverse said distance
             //Debug.Log("but now youre gone, gone, gone");
             //if not currently hearing and within cutoff range, activiate the hearing script, if opposite, deactivate it

                IsLastRemainingBrainCellAwake = !IsLastRemainingBrainCellAwake;
                yield return new WaitForSeconds(waitTime);
                IsLastRemainingBrainCellAwake = !IsLastRemainingBrainCellAwake;
            }
            //RenderCone();
            if (!isHearing && BManDist < hearingCutoff)
                hearing.enabled = isHearing = true;
            else if (isHearing && BManDist > hearingCutoff)
                hearing.enabled = isHearing = false;
            yield return new WaitForEndOfFrame();

        }
    }
    private IEnumerator eats()
    {
        PlayerMovementScript.player.onEats();
        MunchSound.Play();
        yield return new WaitForSeconds(soundplaytime);
        CheckPointLoader.CPL.DeathTime();
    }


    /// <summary>
    /// Function for ensuring the visulized vision range is "accurately" configured according to the given variables
    /// </summary>
    private void RenderCone()
    {
        //if there is no cone ref, get it
        if(Cone == null)
            Cone = GetComponentInChildren<SpriteShapeController>();

        //Clear out the rough, preexisting spline shape
        Cone.spline.Clear();
        //inserts connecting corner
        Cone.spline.InsertPointAt(0, Vector3.zero);
        //nr of segments, starting angle
        int seg = 1 + (int)(FoV / DegreesPerSegment);
        float angle = -FoV / 2;
        //create remaining points
        Vector2 next;
        next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * VisionRange, Mathf.Cos(angle * Mathf.Deg2Rad) * VisionRange);
        for (int i = 1; i <seg+1; i++)
        {
            //create new splines
            Cone.spline.InsertPointAt(i, next);
            angle += (FoV / (seg - 1));
            next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * VisionRange, Mathf.Cos(angle * Mathf.Deg2Rad) * VisionRange);
        }
        

    }
    /// <summary>
    /// Simple function for hiding and showing the vision cone
    /// </summary>
    /// <param name="state"></param>
    public void coneActiveToggle(bool state)
    {
        Cone.enabled = state;
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Vector2 o = new Vector2(Mathf.Sin(0) * VisionRange, Mathf.Cos(0) * VisionRange);
    //    Vector2 l = new Vector2(Mathf.Sin(0 - FoV / 2 * Mathf.Deg2Rad) * VisionRange, Mathf.Cos(0 - FoV / 2 * Mathf.Deg2Rad) * VisionRange);
    //    Vector2 r = new Vector2(Mathf.Sin(0 + FoV / 2 * Mathf.Deg2Rad) * VisionRange, Mathf.Cos(0 + FoV / 2 * Mathf.Deg2Rad) * VisionRange);
    //    Gizmos.DrawLine(Vector3.zero, o); Gizmos.DrawLine(Vector3.zero, l); Gizmos.DrawLine(Vector3.zero, r);
    //    RenderCone();
    //}
}
