using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundTrigger : MonoBehaviour {

    public bool uniquePlayed;
    public bool alpacaPlayed;

    public Narrator narrator;
    public AudioClip uniqueClip;
    public List<AudioClip> randomClips;

    bool randomPlayed;

	// Use this for initialization
	void Start () {
        narrator = GameObject.Find("Narrator").GetComponent<Narrator>();

        alpacaPlayed = PlayerPrefs.GetInt("SoundTrigger/alpacaPlayed") != 0;
        uniquePlayed = PlayerPrefs.GetInt("SoundTrigger/uniquePlayed") != 0;

        randomPlayed = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (randomPlayed)
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
        if (alpacaPlayed) { return; }

        if(uniqueClip == null && randomClips.Count == 0) { return; }

        if(GameManager.instance.AllAlpacasDead())
        {
            Debug.Log("Playing All Alpacas Dead Line");
            narrator.clipPlayer.PlayOneShot(new AudioClip());
            alpacaPlayed = true;
            PlayerPrefs.SetInt("SoundTrigger/alpacaPlayed", 1);
            PlayerPrefs.Save();
            return;
        }

        if(!uniquePlayed && uniqueClip != null)
        {
            Debug.Log("Playing unique/first time Line");
            narrator.clipPlayer.PlayOneShot(uniqueClip);
            uniquePlayed = true;
            PlayerPrefs.SetInt("SoundTrigger/uniquePlayed", 1);
            PlayerPrefs.Save();
            return;
        }


        if (randomPlayed) { return; }
        Debug.Log("Playing Random Trigger Line");
        randomPlayed = true;
        int rng = Random.Range(0, randomClips.Count);
        narrator.clipPlayer.PlayOneShot(randomClips[rng]);
        randomClips.Remove(randomClips[rng]);
    }
}
