using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    public AudioMixerSnapshot lowlevel;
    public AudioMixerSnapshot cozy;

    public AudioSource AudioPlayer = new AudioSource();

    [HideInInspector]
    public AudioClip current;

    private float transitionIn = 1;
    private float transitionOut = 5;

    float timer;

    public void EnterCombat()
    {
        cozy.TransitionTo(transitionIn);
    }
    public void ExitCombat()
    {
        lowlevel.TransitionTo(transitionOut);
    }

    //For testing stuff
    public void Update()
    {
        
        timer += Time.deltaTime;

        if (timer >= 10)
        {
            EnterCombat();
        }
        if (timer >= 20)
        {
            ExitCombat();
            timer = 0;
        }
        /**/
    }

    public void PlayNextNew(AudioClip clip)
    {
        //Cuts current audio
        AudioPlayer.Pause();

        //Assigns & plays new audio
        AudioPlayer.clip = clip;
        AudioPlayer.Play();
    }

    public void PlayNextOverride(AudioClip clip)
    {
        AudioSource newPlayer = new AudioSource();
        newPlayer.clip = clip;

        newPlayer.Play();
    }
}
