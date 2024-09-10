using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;

/// <summary>
/// Class that acts as a centralzized audio event distibutor
/// </summary>
public class AudioCoupler : MonoBehaviour
{
    //outgoing audio event delegate
    public delegate void AudioEvent(float Strnght, Vector2 Coords);
    public AudioEvent AudioListener;
    //static refrence to audio coupler instance
    public static AudioCoupler audioCoupler;
    //animation curve that modles the sound decay over a distance, as well as varuable for controlling how quickly sound travels
    public AnimationCurve SoundDecay;
    public float SpeedOfSound;
    //Prefab for audio visulization, as well as a somehwhat unforunately accronymed list for hold DAPs
    [SerializeField] private GameObject AudiosVisulizerPrefab;
    private List<DelayedAudioPass> DAPL = new List<DelayedAudioPass>();
    public bool block = false;
    public bool bloky { get => block; }
    private void Awake()
    {//sets static class instance to this
        if (audioCoupler == null)
            audioCoupler = this;
    }

public void turnoff()
    {
        block = true;

    }
    public void turnon()
    {
        block = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //adds listener to player sound generation delegate
        PlayerMovementScript.player.PlayerOutgoing += IncommingAudioEvent;
    }

    private void Update()
    {//loops through every dap, unless there are none
        if (DAPL.Count != 0)
        {
            List<DelayedAudioPass> Cull = new List<DelayedAudioPass>();
            for (int i = 0; i < DAPL.Count; i++)
            {//if the DAP is done, add it to the cull list
                if (DAPL[i].Evaluate())
                    Cull.Add(DAPL[i]);
            }//remove all entries in the cull list from DAPL
            for (int i = 0; i < Cull.Count; i++)
                DAPL.Remove(Cull[i]);
        }
    }
    //externally acessable function for adding DAPs
    public void CreateDAP(float delay, ZombieLook objRef, Vector2 Coords)
    {
        DAPL.Add(new DelayedAudioPass(delay, objRef, Coords));
    }

    private void OnDisable()
    {//unsubscribe from delegate
        PlayerMovementScript.player.PlayerOutgoing -= IncommingAudioEvent;
    }
    /// <summary>
    /// Function for receving an external audio event
    /// </summary>
    /// <param name="AudioStrenght">The strenght of the audio event</param>
    /// <param name="OriginCoords">Originting position fo event</param>
    public void IncommingAudioEvent(float AudioStrenght, Vector2 OriginCoords)
    {
        if (!block)
        {
            //send out delegate call
            if (AudioListener != null)
                AudioListener(AudioStrenght, OriginCoords);
            Instantiate(AudiosVisulizerPrefab, OriginCoords, Quaternion.identity).GetComponent<AudioVisulizer>().UnitSetup(SoundDecay, SpeedOfSound, AudioStrenght);
        }
    }
    /// <summary>
    /// Class for adding delayed passings of audio events to looks cripts
    /// </summary>
    private class DelayedAudioPass
    {
        //the variables necessary for proper function
        Timer T;
        ZombieLook PassRef;
        Vector2 Coord;
        /// <summary>
        /// Creates a new DAP wth the following paramaterns
        /// </summary>
        /// <param name="Delay">The ammount of time in seconds until event should be passed</param>
        /// <param name="objRef">The look script that the event should be passed to</param>
        /// <param name="Origin">The origin coordinates of the auidio event</param>
        public DelayedAudioPass(float Delay, ZombieLook objRef, Vector2 Origin)
        {
            T = new Timer(Delay);
            PassRef = objRef;
            Coord = Origin;
        }
        /// <summary>
        /// Function that evalues wether or not the timer is passed its target time or not, if yes send event to target script and return true
        /// </summary>
        /// <returns></returns>
        public bool Evaluate()
        {
            if(T)
            {
                PassRef.SoundHeard(Coord);
                return true;
            }
            return false;
        }
    }
}
