using UnityEngine;
using System.Collections;
public class IBossBehaviour{
    
    protected Boss boss;
    protected float navMeshUpdate = 0.1f;

    protected float attackTimer;
    protected float attackTimerInv;
    public virtual void Init() { }
    public virtual bool Execute() { return false; }
    public virtual void Finish()
    { 
        boss.animator.SetFloat("Y", 0); 
    }

    protected void MoveToTarget( Transform target )
    {
        if (navMeshUpdate <= 0) { return; }

        navMeshUpdate -= Time.deltaTime;

        if (navMeshUpdate <= 0)
        {
            navMeshUpdate = 0.5f;
            boss.agent.SetDestination(target.position);
            boss.agent.Resume();

            boss.animator.SetFloat("Y", 1);

            Debug.DrawRay(boss.transform.position, target.position - boss.transform.position, Color.red, 1.2f);
        }
    }

    protected void StartAttack(int index)
    {
        boss.transform.LookAt(boss.target);

        Debug.Log("startattack " + index);
        boss.animator.SetFloat("Y", 0);
        boss.agent.Stop();

        //attackTimer = boss.GetAnimation(index).duration;
        attackTimer = 2.0f;
        attackTimerInv = 0;
        boss.animator.SetInteger("AttackId", index);
        boss.animator.SetTrigger("Attack");
        return;
    }
}
