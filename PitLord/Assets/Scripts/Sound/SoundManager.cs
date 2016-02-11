using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    public AudioMixerSnapshot lowlevel;
    public AudioMixerSnapshot cozy;

    public AudioSource[] AudioPlayers;

    AudioSource clipPlayer = new AudioSource();

    [HideInInspector]
    public AudioClip current;

    public float transitionIn = 2.5f;
    public float transitionOut = 5;

    public void EnterCozy()
    {
        cozy.TransitionTo(transitionIn);
        //cozy.TransitionTo(0);
    }
    public void ExitCozy()
    {
        lowlevel.TransitionTo(transitionOut);
        //lowlevel.TransitionTo(0);
    }

    public void Start()
    {
        for(int i = 0; i < AudioPlayers.Length; i++)
        {
            AudioPlayers[i].volume = 0;
        }
    }
    //For testing stuff
    public void Update()
    {
        for (int i = 0; i < AudioPlayers.Length; i++)
        {
            if (AudioPlayers[i].volume < 1)
            {
                AudioPlayers[i].volume += 0.1f * Time.deltaTime;
                //AudioPlayers[i].volume = Mathf.Lerp(AudioPlayers[i].volume, 1, Time.deltaTime);
            }
        }

    }

    public void PlayNextNew(AudioClip clip)
    {
        clipPlayer.Stop();
        //Cuts current audio
        clipPlayer.clip = clip;
        clipPlayer.Play();
    }

    public void PlayNextOverride(AudioClip clip)
    {
        AudioSource newPlayer = new AudioSource();

        newPlayer.clip = clip;
        newPlayer.Play();
    }
}
