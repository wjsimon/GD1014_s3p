using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Narrator : MonoBehaviour
{
    public AudioSource clipPlayer;
    public AudioSource parallellPlayer;

    //Need uniques + Environmental stuff in Narrator, all trigger based into SoundTrigger?
    public List<AudioClip> uniques_player;
    public List<AudioClip> uniques_enemies;

    public AudioClip introduction;

    public List<AudioClip> onIdle;
    public int onIdleIndex;

    public List<AudioClip> onReset;

    public List<AudioClip> inCombat;
    public List<AudioClip> onCombatWin;
    public List<AudioClip> inCombatBoss;

    public List<AudioClip> alpacaKill;

    bool playIntroduction;
    // Use this for initialization
    void Start()
    {
        //Debug.Log(onIdle[0].name);

        onIdleIndex = PlayerPrefs.GetInt("Narrator/OnIdleIndex", 0);
        if (onIdleIndex >= onIdle.Count) { onIdle.Shuffle(); }

        if (clipPlayer == null)
        {
            clipPlayer = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayNextNew( AudioClip clip )
    {
        if (clip == null) { return; }
        Debug.Log(clip.name);

        clipPlayer.Stop();
        //Cuts current audio
        clipPlayer.clip = clip;
        clipPlayer.Play();
    }

    public void PlayNextParallel(AudioClip clip)
    {
        //Second Player instead of newing one
        if (parallellPlayer == null) { return; }

        AudioSource current = GameObject.Instantiate<AudioSource>(parallellPlayer);
        //parallellPlayer.transform.position = Camera.main.transform.position;
        Debug.Log(clip.name);
        current.clip = clip;
        current.Play();
    }

    public AudioClip GetNextClip(List<AudioClip> list, ref int index, string name)
    {
        while (list.Count > 0)
        {
            if(index != 0 && index%list.Count == 0)
            {
                list.Shuffle();
            }

            AudioClip entry = list[index % list.Count];
            index++;

            if(entry.name.StartsWith("seq_") && index >= list.Count)
            {
                continue;
            }

            PlayerPrefs.SetInt("Narrator/" + name, index);
            PlayerPrefs.Save();
            return entry;
        }

        return null;
    }

    public void PlayIdle()
    {
        Debug.Log("Playing Idle Line...");

        PlayNextNew(GetNextClip(onIdle, ref onIdleIndex, "OnIdleIndex"));
    }
    public void PlayDeath() { Debug.Log("Playing Revival Line..."); }
    public void PlayInCombat() { Debug.Log("Playing inCombat Line..."); }
    public void PlayOnCombatWin() { Debug.Log("Playing CombatWin Line..."); }
    public void PlayUniqueEncounter( Enemy.EnemyType type ) { Debug.Log("Playing Unique Encounter Line for..." + type.ToString()); }
    public void PlayIntroduction() { }
}
