using UnityEngine;
using System.Collections;

public class BossBehaviourHeavy : IBossBehaviour {

    float range = 6.0f;
    bool shockwaveSpawned;
    
    public BossBehaviourHeavy( Boss boss )
    {
        this.boss = boss;
    }

    public override bool Execute()
    {
        if(attackTimer == 0)
        {
            if ((boss.transform.position - boss.target.position).magnitude > range)
            {
                MoveToTarget(boss.target);
                return true;
            }

            StartAttack(3);
            shockwaveSpawned = false;
            return true;
        }

        attackTimer -= Time.deltaTime;
        attackTimerInv += Time.deltaTime;

        if(attackTimerInv >= 1.0f && !shockwaveSpawned) //Library;
        {
            GameObject.Instantiate(boss.shockwave);
            shockwaveSpawned = true;
        }

        return attackTimer > 0;
    }
}
