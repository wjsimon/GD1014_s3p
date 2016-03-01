using UnityEngine;
using System.Collections;

public class BossBehaviourCombo : IBossBehaviour
{
    const float range = 6.0f;
    const float cancelRange = 8.0f;
    int comboCounter;

    //0,1,2 im Animator - attackid

    public BossBehaviourCombo( Boss boss )
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

            StartAttack(comboCounter);
            return true;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            comboCounter++;
            if (comboCounter >= 3) { return false; }

            if (Random.Range(0, 100) <= 30) { return false; } //Test later. Maybe.
            if ((boss.transform.position - boss.target.position).magnitude > cancelRange) { return false; }

            StartAttack(comboCounter);
            return true;
        }

        return true;
    }

    /*
    public void StartAttack()
    {
        Debug.Log("startcombo " + comboCounter);
        boss.animator.SetFloat("Y", 0);
        boss.agent.Stop();

        //attackTimer = boss.GetAnimation(comboCounter).duration;
        attackTimer = 2.0f;
        boss.animator.SetInteger("AttackId", comboCounter);
        boss.animator.SetTrigger("Attack");
        return;
    }
    /**/
}
