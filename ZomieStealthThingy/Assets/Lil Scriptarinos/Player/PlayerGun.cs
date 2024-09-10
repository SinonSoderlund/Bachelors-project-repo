using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
using UnityEngine.U2D;


/// <summary>
/// Pleyr gun
/// </summary>
public class PlayerGun : MonoBehaviour
{
    //Cooldown time between shots, and the tiemr
    [SerializeField, Range(0, 5)] private float gunShotDelay = 1;
    //gun shot set up variables
    [SerializeField, Range(4, 20)] private float gunshotNoiseStenght;
    [SerializeField, Range(0, 360)] float ShotSpread = 90;
    [SerializeField, Range(0, 20)] float ShotRange = 10;
    [SerializeField, Range(1, 30)] int DegreesPerHitscan = 10;
    [SerializeField] private bool VisulizeGunspread = false;
    [SerializeField] private AudioSource GunBANG;
    //ref to player vision cone transform, which is the official facing of the player
    Transform rotated;
    Timer GunTimer;
    private AudioCoupler AC;
    InputManagement IM;
    PromptController DC;
    private SpriteShapeController Cone;
    [SerializeField] private bool gunViz;
    [SerializeField] private float ConeTime = 0.1f;
    Timer coneTimer;
    // Start is called before the first frame update
    void Start()
    {
        //inits timer
        GunTimer = new Timer(gunShotDelay);
        //fetch audio coupler ref
        AC = AudioCoupler.audioCoupler;
        //fetch facing transform ref
        rotated = GetComponent<PlayerLookScript>().TurnObject.transform;
        IM = InputManagement.InputManager;
        DC = PromptController.promptController;
        Cone = GetComponentInChildren<SpriteShapeController>();
        if (!gunViz || IM.getInputKeyDisabled(InputType.FireGun))
            Cone.enabled = false;
        else
            RenderCone();
        Cone.enabled = false;
        coneTimer = new Timer(ConeTime);
    }

    private void OnValidate()
    {
        RenderCone();
    }

    // Update is called once per frame
    void Update()
    {//if lmb is pressed and timer is passed cooldown, fire gun, create noise event
            if (coneTimer == true && Cone.enabled)
                Cone.enabled = false;

            if (IM.GetInput(InputType.FireGun)&& GunTimer == true &&!DC.IsDialogue())
            {
                ShootDaGun();
                AC.IncommingAudioEvent(gunshotNoiseStenght, transform.position);
                Cone.enabled = true;
                GunTimer.Reset();
                coneTimer.Reset();
            }
        }    
    private void ShootDaGun()
    {
        if (GunBANG != null)
            GunBANG.Play();
        //calculates the needed number of raycasts (this is just copied over form zombie look)
    int seg = 1 + (int)(ShotSpread / DegreesPerHitscan);
        //calculate the iteration angle
    float angle = -ShotSpread / 2;
    //calculate start angle
    Vector2 next;
        next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * ShotRange, Mathf.Cos(angle * Mathf.Deg2Rad) * ShotRange);
        //transform angle according to player facing
        next = rotated.TransformDirection(next);
        for (int i = 1; i < seg + 1; i++)
    {
            //interate through each raycast, check if zombie was hit, if so destroy them
            int layermask = 0b1;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, next.normalized, ShotRange,layermask);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag == "Zombie")
                {
                    hit.collider.gameObject.GetComponent<ZombieLook>().OnGetShot(transform.position, Vector2.Distance(transform.position, hit.transform.position) / ShotRange);
                }
                //print(hit.collider.name);
                //print(transform.position.ToVector2() + (next.normalized * ShotRange));
            }

            //calculate next angle
            angle += (ShotSpread / (seg - 1));
            next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * ShotRange, Mathf.Cos(angle * Mathf.Deg2Rad) * ShotRange).normalized;
            next = rotated.TransformDirection(next);
        }
    }


    private void OnDrawGizmosSelected()
    {//basically the same as shoot da gun but for gizmo visulization    
        if (VisulizeGunspread)
        {
            if(rotated == null)
                rotated = GetComponent<PlayerLookScript>().TurnObject.transform;
            Gizmos.color = Color.red;
            int seg = 1 + (int)(ShotSpread / DegreesPerHitscan);
            float angle = -ShotSpread / 2;
            //create remaining points
            Vector2 next;
            next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * ShotRange, Mathf.Cos(angle * Mathf.Deg2Rad) * ShotRange).normalized;
            next = rotated.TransformDirection(next);
            next = transform.position.ToVector2() + (next * ShotRange);
            for (int i = 1; i < seg + 1; i++)
            {
                //create new splines
                Gizmos.DrawLine(transform.position, next);

                angle += (ShotSpread / (seg - 1));
                next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * ShotRange, Mathf.Cos(angle * Mathf.Deg2Rad) * ShotRange).normalized;
                next = rotated.TransformDirection(next);
                next = transform.position.ToVector2() + (next * ShotRange);

            }
        }
    }
    private void RenderCone()
    {
        //if there is no cone ref, get it
        if (Cone == null)
            Cone = GetComponentInChildren<SpriteShapeController>();

        //Clear out the rough, preexisting spline shape
        Cone.spline.Clear();
        //inserts connecting corner
        Cone.spline.InsertPointAt(0, Vector3.zero);
        //nr of segments, starting angle
        int seg = 1 + (int)(ShotSpread / DegreesPerHitscan);
        float angle = -ShotSpread / 2;
        //create remaining points
        Vector2 next;
        next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * ShotRange, Mathf.Cos(angle * Mathf.Deg2Rad) * ShotRange);
        for (int i = 1; i < seg + 1; i++)
        {
            //create new splines
            Cone.spline.InsertPointAt(i, next);
            angle += (ShotSpread / (seg - 1));
            next = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * ShotRange, Mathf.Cos(angle * Mathf.Deg2Rad) * ShotRange);
        }


    }
}

