using UnityEngine;
using System.Collections;

public class SoundTrigger : MonoBehaviour {

    public AudioClip clip;
    bool played;

	// Use this for initialization
	void Start () {
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
            other.gameObject.transform.FindChild("Narrator").gameObject.GetComponent<SoundManager>().PlayNextNew(clip);
            played = true;
        }
    }
}
