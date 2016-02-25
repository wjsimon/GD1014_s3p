using UnityEngine;
using System.Collections;

public class BossBehaviourAOE : IBossBehaviour
{
    const float range = 5.0f;
    const float aoeRange = 10f;
    float aoeTimer;

    public BossBehaviourAOE( Boss boss )
    {
        this.boss = boss;
    }

    public override bool Execute()
    {
        if (attackTimer == 0)
        {
            if ((boss.transform.position - boss.target.position).magnitude > range)
            {
                MoveToTarget(boss.target);
                return true;
            }

            //aoeTimer = boss.GetAnimation(4).colStart;
            aoeTimer = 1.0f;
            StartAttack(4);
            return true;
        }

        if (aoeTimer > 0)
        {
            aoeTimer -= Time.deltaTime;
            if (aoeTimer <= 0)
            {
                DoAoe();
                aoeTimer = 0;
            }
        }

        attackTimer -= Time.deltaTime;

        return attackTimer > 0;
    }

    void DoAoe()
    {
        Transform weaponTarget = boss.transform.FindChildRecursive("WeaponTarget");
        if ((weaponTarget.position - boss.target.position).magnitude > aoeRange) { return; }

        boss.target.GetComponent<PlayerController>().ApplyDamage(5, 5, boss);
    }
}
