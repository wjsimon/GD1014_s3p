using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Narrator : MonoBehaviour
{
    public AudioSource clipPlayer;

    //Need uniques + Environmental stuff in Narrator, all trigger based into SoundTrigger?
    List<AudioClip> uniques_player;
    List<AudioClip> uniques_enemies;

    List<AudioClip> introduction;
    List<AudioClip> onIdle;
    List<AudioClip> onReset;

    List<AudioClip> inCombat;
    List<AudioClip> onCombatWin;
    List<AudioClip> inCombatBoss;

    List<AudioClip> alpacaKill;

    bool playIntroduction;
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
    
    public void PlayIdle() { Debug.Log("Playing Idle Line..."); }
    public void PlayDeath() { Debug.Log("Playing Revival Line..."); }
    public void PlayInCombat() { Debug.Log("Playing inCombat Line..."); }
    public void PlayOnCombatWin() { Debug.Log("Playing CombatWin Line..."); }
    public void PlayUniqueEncounter(Enemy.EnemyType type) { Debug.Log("Playing Unique Encounter Line for..." + type.ToString()); }
    public void PlayIntroduction() { }
}
