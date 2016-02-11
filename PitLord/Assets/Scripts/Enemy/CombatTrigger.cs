using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatTrigger : MonoBehaviour {

    public List<Enemy> enemies;
    public bool triggered;

	// Use this for initialization
	void Start () {
        GameManager.instance.AddCombatTrigger(this);
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider other)
    {
        if(triggered)
        {
            return;
        }

        if(other.gameObject.tag == "Player")
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Alert();
            }
        }

        triggered = true;
    }
}
