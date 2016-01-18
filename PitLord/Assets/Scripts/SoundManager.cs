using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    private static SoundManager instance = new SoundManager();
   
    public AudioMixerSnapshot lowlevel;
    public AudioMixerSnapshot combat;

    private float transitionIn = 100;
    private float transitionOut = 500;

    float timer;

    public void EnterCombat()
    {
        combat.TransitionTo(transitionIn);
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
    }

    public static SoundManager Get()
    {
        return instance;
    }
}
