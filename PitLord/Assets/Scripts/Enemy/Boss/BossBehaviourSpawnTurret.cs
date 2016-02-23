using UnityEngine;
using System.Collections;

public class BossBehaviourSpawnTurret : IBossBehaviour
{

    Boss boss;

    public float navMeshUpdate;
    public float animationDuration = 2.5f;

    public enum State
    {
        START,
        END,
        COUNT,
    }

    public State currentState;
    public float stateTimer;

    public BossBehaviourSpawnTurret( Boss boss )
    {
        this.boss = boss;

        navMeshUpdate = 0.5f;

        currentState = State.START;
        stateTimer = AnimationLibrary.Get().SearchByName("B_ProjectileSpawn").colStart;

        boss.animator.SetInteger("AttackId", 4);
        boss.animator.SetTrigger("Attack");
    }

    public override bool Execute()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer > 0) { return true; }
        if (currentState == State.END) { return false; }
        if (currentState == State.START)
        {
            currentState = State.END;
            stateTimer = AnimationLibrary.Get().SearchByName("B_ProjectileSpawn").GetAnimationEnd();

            Vector3 pos = boss.transform.GetRayCastTarget().position;
            GameObject.Instantiate(boss.turret, pos, boss.transform.localRotation);

            return true;
        }

        /*
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
        /**/

        return false;
    }

    void MoveToTarget( Transform target )
    {
        if (navMeshUpdate <= 0) { return; }

        navMeshUpdate -= Time.deltaTime;

        if (navMeshUpdate <= 0)
        {
            navMeshUpdate = 0.5f;
            boss.agent.SetDestination(target.position);
            boss.agent.Resume();

            Debug.DrawRay(boss.transform.position, target.position - boss.transform.position, Color.red, 1.2f);
        }
    }
}
