using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatTrigger : MonoBehaviour {

    public List<Enemy> enemies;
    public bool triggered;
    public int active;

	// Use this for initialization
	void Start () {
        GameManager.instance.AddCombatTrigger(this);

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].combatTrigger = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(triggered)
        {
            if (active == 0 && enemies.Count != 0)
            {
                GameManager.instance.narrator.PlayOnCombatWin();
                GameManager.instance.ExitCombat();
                active = -1;
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(triggered)
        {
            return;
        }

        if(enemies.Count <= 0) { return; }

        if(other.gameObject.tag == "Player")
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Alert();
            }

            GameManager.instance.EnterCombat();
        }

        triggered = true;
    }

    public void SoftReset()
    {
        triggered = false;
        active = enemies.Count;
    }
}
