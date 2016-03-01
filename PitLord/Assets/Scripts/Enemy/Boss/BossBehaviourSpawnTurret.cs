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

    public Transform[] spawns = new Transform[8];

    public BossBehaviourSpawnTurret( Boss boss )
    {
        this.boss = boss;

        currentState = State.START;
        stateTimer = AnimationLibrary.Get().SearchByName("B_Projectile").colStart;

        for(int i = 0; i < spawns.Length; i++)
        {
            spawns[i] = GameObject.Find("Spawn" + i + 1).transform;
        }

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

            //Vector3 pos = boss.transform.GetRayCastTarget().position;
            if(boss.phase == 1)
            {
                Vector3 pos = spawns[Random.Range(0, spawns.Length)].position;
                GameObject.Instantiate(boss.turret, pos, boss.transform.localRotation);
            }
            if(boss.phase == 2)
            {
                Vector3 pos = spawns[Random.Range(0, spawns.Length)].position;
                GameObject.Instantiate(boss.turret, pos, boss.transform.localRotation);
            }
            if(boss.phase == 3)
            {
                Vector3 pos = spawns[Random.Range(0, spawns.Length)].position;
                GameObject.Instantiate(boss.turret, pos, boss.transform.localRotation);
            }

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
