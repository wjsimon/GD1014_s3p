using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundTrigger : MonoBehaviour {

    public static bool allAlpacas;
    public bool unique;
    public Narrator narrator;
    public AudioClip uniqueClip;
    public List<AudioClip> randomClips;
    bool played;

	// Use this for initialization
	void Start () {
        narrator = GameObject.Find("Narrator").GetComponent<Narrator>();
        played = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (played)
        {
            return;
        }

        if(other.GetComponent<PlayerController>() != null)
        {
            //Trigger Sound
        }
    }

    public void TriggerSound()
    {
        if(played) { return; }
        if(uniqueClip == null && randomClips.Count == 0) { return; }

        played = true;

        if(allAlpacas)
        {
            Debug.Log("Playing All Alpacas Dead Line");
            narrator.clipPlayer.PlayOneShot(new AudioClip());
            enabled = false;
            return;
        }

        if(unique && uniqueClip != null)
        {
            Debug.Log("Playing unique/first time Line");
            narrator.clipPlayer.PlayOneShot(uniqueClip);
            unique = false;
            return;
        }

        Debug.Log("Playing Random Trigger Line");
        int rng = Random.Range(0, randomClips.Count);
        narrator.clipPlayer.PlayOneShot(randomClips[rng]);
        randomClips.Remove(randomClips[rng]);
    }
}
