using UnityEngine;
using System.Collections;

public class Narrator : MonoBehaviour
{
    AudioSource clipPlayer;

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

    public void PlayNextOverride(AudioClip clip)
    {
        AudioSource newPlayer = new AudioSource();

        newPlayer.clip = clip;
        newPlayer.Play();
    }
}
