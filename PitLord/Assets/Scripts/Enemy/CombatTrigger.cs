using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatTrigger : MonoBehaviour
{

    public List<Enemy> enemies;
    public bool triggered;
    public bool finished;

    [HideInInspector]
    public int active; //DEPRECATED

    // Use this for initialization
    void Start()
    {
        GameManager.instance.AddCombatTrigger(this);

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].combatTrigger = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (finished) { return; }

        if (triggered)
        {
            if (AllEnemiesDead())
            {
                GameManager.instance.ExitCombat();
                finished = true;
            }
        }
    }

    void OnTriggerEnter( Collider other )
    {
        if (triggered)
        {
            return;
        }

        if (enemies.Count <= 0) { return; }
        if (AllEnemiesDead()) { triggered = true; return; }

        if (other.GetComponent<PlayerController>() != null)
        {
            if (other.GetComponent<PlayerController>().isDead()) { return; }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].isDead()) { enemies[i].Alert(); }
            }

            triggered = true;
            GameManager.instance.EnterCombat();
        }
    }

    bool AllEnemiesDead()
    {
        int count = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].isDead()) { count++; }
        }

        return count <= 0;
    }

    public void SoftReset()
    {
        triggered = false;
        finished = false;
        active = enemies.Count;
    }
}
