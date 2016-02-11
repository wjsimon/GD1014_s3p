using UnityEngine;
using System.Collections;

public class CozyZone : MonoBehaviour {

    SoundManager manager;
	// Use this for initialization
	void Start () {
        if(manager == null)
        {
            manager = GameObject.Find("Narrator").GetComponent<SoundManager>();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            Debug.Log("Enter cozy");
            manager.EnterCozy();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            Debug.Log("Exit cozy");
            manager.ExitCozy();
        }
    }
}
