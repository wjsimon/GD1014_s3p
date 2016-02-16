using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Narrator : MonoBehaviour
{
    AudioSource clipPlayer;

    //Need uniques + Environmental stuff in Narrator, all trigger based into SoundTrigger?
    List<AudioClip> uniques_player;
    List<AudioClip> uniques_enemies;

    List<AudioClip> introduction;
    List<AudioClip> onIdle;
    List<AudioClip> onReset;
    List<AudioClip> onStairs;

    List<AudioClip> inCombat;
    List<AudioClip> onCombatWin;
    List<AudioClip> inCombatBoss;

    bool playIntroduction;
    bool doorOpened;
    // Use this for initialization
    void Start()
    {
        if (clipPlayer == null)
        {
            clipPlayer = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayNextNew(AudioClip clip)
    {
        clipPlayer.Stop();
        //Cuts current audio
        clipPlayer.clip = clip;
        clipPlayer.Play();
    }

    public void PlayNextParallel(AudioClip clip)
    {
        //Second Player instead of newing one
        AudioSource newPlayer = new AudioSource();

        newPlayer.clip = clip;
        newPlayer.Play();
    }
}
