using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerClasses;
using UtilitySpace;


/// <summary>
/// Class for handling player movement and noise generation
/// </summary>
public class PlayerMovementScript : Shootable
{
    //static player refrence
    public static PlayerMovementScript player;
    //variable affecting movement speed and noise generation
    [SerializeField] private float SneakSpeedMod = 1f;
    [SerializeField] private float SneakSoundAmmount = 2;
    [SerializeField] private float MoveSpeedMod = 2f;
    [SerializeField] private float MoveSoundAmmount = 4;
    [SerializeField] private float FootstepsPerUnit = 1;
    [SerializeField] private float TimeFromBeingShotToRespawnMenu = 5;
    [SerializeField] private AudioSource SnakeNoise;
    [SerializeField] private AudioSource WalkNoise;
    private bool isSneaking = true;
    //delegate for sending audio events to the audioCoupler
    public AudioCoupler.AudioEvent PlayerOutgoing;
    //timer that controll how often audio event will be sent out while the player is continiously walking
    private Timer SneakSwitch = new Timer(0.5);
    private InputManagement InpMan;
    private CircleCollider2D Colli; private SpriteRenderer Sprite;
    bool isHide = false;
    public delegate void OnPlayerHide();
    public OnPlayerHide PlayerHidden;
    //public acessor for player hide state
    public bool isHiding
    {
        get => isHide;
    }
    /// <summary>
    /// external accesser for the movespeed value
    /// </summary>
    public float MoveSpeed
    {
        get { return MoveSpeedMod; }
    }
    //oldRef to movement controller, used to minimize number of animation updates
    private MovementManagerObject oldMMO = new MovementManagerObject();

    private void Awake()
    {//sets static refrence to this
        if (player == null)
            player = this;
    }

    protected override void Start()
    {//fetches refs
        InpMan = InputManagement.InputManager;
        Colli = GetComponent<CircleCollider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        base.Start();
    }

    protected override void OnDead()
    {
        GetComponent<PlayerGun>().enabled = false;
        GetComponent<PlayerLookScript>().enabled = false;
        this.enabled = false;
        StartCoroutine(Death());
    }
    public void onEats()
    {
        GetComponent<PlayerGun>().enabled = false;
        GetComponent<PlayerLookScript>().enabled = false;
        this.enabled = false;
        SR.color = DEAD;
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(TimeFromBeingShotToRespawnMenu);
        CheckPointLoader.CPL.DeathTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHide)
        {
            //creates new MMO,but copies over distance variable
            MovementManagerObject MMO = new MovementManagerObject(oldMMO);
            //checks inputs, fills MMO with relevant axis input and animation data
            if (InpMan.GetInput(InputType.Forward))
            {
                MMO[2] += 1;
                MMO[3L] += "a";
            }
            else if (InpMan.GetInput(InputType.Back))
            {
                MMO[2] -= 1;
                MMO[3L] += "c";
            }
            if (InpMan.GetInput(InputType.Right))
            {
                MMO[1] += 1;
                MMO[3L] += "b";
            }
            else if (InpMan.GetInput(InputType.Left))
            {
                MMO[1] -= 1;
                MMO[3L] += "d";
            }//if sneakswitch isnt blocking (to prevent rapid switching, and leftcontrol is pressed down, then switch sneak toggle
            if (InpMan.GetInput(InputType.Sneak))
            {
                isSneaking = true;

            }
            else isSneaking = false;

            //applies movemnt to positon, checks if noise should be generated and if yes does so
            if (MMO.MoveCheck())
            {
                if (isSneaking)
                {
                    transform.position += MMO.VectorAssembly(SneakSpeedMod);
                }
                else
                {
                    transform.position += MMO.VectorAssembly(MoveSpeedMod);
                }
                if (MMO.EvaluatedMovedDist(1 / FootstepsPerUnit) && PlayerOutgoing != null)
                {
                    if (isSneaking)
                    {
                        if (SnakeNoise != null)
                            SnakeNoise.Play();
                        PlayerOutgoing(SneakSoundAmmount, transform.position);
                    }
                    else
                    {
                        if(WalkNoise != null)
                        WalkNoise.Play();
                        PlayerOutgoing(MoveSoundAmmount, transform.position);
                    }
                }

            }
            //if animation state is diffent from last frame, update it
            if (MMO[3L] != oldMMO[3L])
            {
                //call to animation state update function
                SetNewOrientation(MMO[3L]);
            }
            //sets current mmo to oldmmo
            oldMMO = MMO;
        }
    }
    //switches player hide state
    public void PlayerHideToggle(Transform T)
    {
        isHide = !isHide;
        Colli.enabled = !isHide;
        Sprite.enabled = !isHide;
        transform.SetXY(T);
        PlayerHidden?.Invoke();
    }

    #region Animation Stuff
    /// <summary>
    /// Function for chaning character sprite dependng on movement direction
    /// </summary>
    /// <param name="MAV">Movement Assembly Value</param>
    void SetNewOrientation(string MAV)
    {
        switch (MAV)
        {
            case "a":
                break;
            case "b":
                break;
            case "c":
                break;
            case "d":
                break;
            case "ab":
                break;
            case "ad":
                break;
            case "cb":
                break;
            case "cd":
                break;
        }
    }
    #endregion
}