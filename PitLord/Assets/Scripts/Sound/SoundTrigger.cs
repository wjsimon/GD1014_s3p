using UnityEngine;
using System.Collections;

public class SoundTrigger : MonoBehaviour {

    public GameObject SoundManager;
    public AudioClip clip;
    bool played;

	// Use this for initialization
	void Start () {
        SoundManager = GameObject.Find("Narrator");
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

        if(other.tag == "Player")
        {
            SoundManager.GetComponent<SoundManager>().PlayNextNew(clip);
            played = true;
        }
    }
}
