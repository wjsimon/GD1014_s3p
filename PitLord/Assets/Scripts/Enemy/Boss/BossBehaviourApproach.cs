using UnityEngine;
using System.Collections;

public class BossBehaviourApproach : IBossBehaviour {

    bool init;

    Boss boss;

    public float navMeshUpdate;

    public BossBehaviourApproach(Boss boss)
    {
        init = true;
        this.boss = boss;
    }

    public void Execute()
    {
        //throw new System.NotImplementedException();
        if (init) { Init(); }

        NavMeshAgent agent = boss.agent;
        Animator anim = boss.animator;
        Transform target = boss.target;

        Debug.Log(Vector3.Distance(boss.transform.position, target.position));

        if(Vector3.Distance(boss.transform.position, target.position) > 5)
        {
            MoveToTarget(target);
        }
        else
        {
            agent.Stop(); //Geht besser?

            //StartAttack()
        }

        anim.SetFloat("X", (agent.velocity.magnitude > 0 ? 1 : 0));
        anim.SetFloat("Y", (agent.velocity.magnitude > 0 ? 1 : 0));
    }

    void Init()
    {
        if (!init) { return; }

        init = false;
        navMeshUpdate = 0.5f;
    }

    void MoveToTarget(Transform target)
    {
        if (navMeshUpdate <= 0) { return; }

        navMeshUpdate -= Time.deltaTime;

        if(navMeshUpdate <= 0)
        {
            navMeshUpdate = 0.5f;
            boss.agent.SetDestination(target.position);
            boss.agent.Resume();

            Debug.DrawRay(boss.transform.position, target.position - boss.transform.position, Color.red, 1.2f);
        }
    }
}
