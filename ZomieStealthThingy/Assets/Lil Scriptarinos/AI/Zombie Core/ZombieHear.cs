using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for zombie hearing
/// </summary>
public class ZombieHear : MonoBehaviour
{
    //The threashold at which zombies can hear sound, if below threshold sound wont be heard
    [SerializeField, Range(0, 1)] public float hearingThreshold = 0.001f;
    //Refreces to the decay curve and the look script
    private AnimationCurve audioCurve;
    private ZombieLook look;
    //the speed at which sound traveks
    private float SpeedOfSound;
    private void OnEnable()
    {
        //on enable, fetch up to date out of script and variable refrences, add AudioAnalyse to audio event delegate
        AudioCoupler.audioCoupler.AudioListener += AudioAnalyse;
        audioCurve = AudioCoupler.audioCoupler.SoundDecay;
        look = GetComponent<ZombieLook>();
        SpeedOfSound = AudioCoupler.audioCoupler.SpeedOfSound;
    }
    private void OnDisable()
    {//remove AA from audioevent
        AudioCoupler.audioCoupler.AudioListener -= AudioAnalyse;

    }
    /// <summary>
    /// Function that evaluates wether or not  given sound can be heard
    /// </summary>
    /// <param name="AudioStrenght">The audio streght value</param>
    /// <param name="Origin">The origin coordinates for the sound</param>
    private void AudioAnalyse(float AudioStrenght, Vector2 Origin)
    {
        //get distance from origin
        float dist = Vector2.Distance(Origin, transform.position.ToVector2());
        //device dist by audio strnght, subtract result from 1, evalute the resulting audio value at that positiong on the audion curve
        //if value on curve is greater then hearing threshold then register sound as heard, which results in a call to the audio coupler to create a Delayed Audio Pass,
        //Which sends an audio event to the given look script after delay seconds
        if (audioCurve.Evaluate(1 - (dist / AudioStrenght)) > hearingThreshold)
            AudioCoupler.audioCoupler.CreateDAP((dist / (SpeedOfSound */* its the most, magical number, of the year (no serious i dont understand why its a 3) */ 3)), look, Origin);
    }
}
