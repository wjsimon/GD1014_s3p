using UnityEngine;
using System.Collections;

public class BossTrigger : MonoBehaviour {

    Boss boss;

	// Use this for initialization
	void Start () {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            boss.Activate();
            enabled = false;
        }
    }
}
