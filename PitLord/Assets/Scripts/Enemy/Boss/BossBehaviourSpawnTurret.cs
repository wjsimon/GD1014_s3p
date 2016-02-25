using UnityEngine;
using System.Collections;

public class BossBehaviourSpawnTurret : IBossBehaviour
{
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

        currentState = State.START;
        stateTimer = AnimationLibrary.Get().SearchByName("B_ProjectileSpawn").colStart;

        boss.animator.SetInteger("AttackId", 5);
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
}
