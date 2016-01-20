using UnityEngine;
using System.Collections;

public class TestEnemyBehaviour : Enemy {

    float attack1Start;
    float attack1End;

    // Use this for initialization
    void Start () {
        Init();

        attack1Start = AnimationLibrary.Get().SearchByName("LightAttack1").start;
        attack1End = AnimationLibrary.Get().SearchByName("LightAttack1").end;

        state = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (Vector3.Distance(target.position, transform.position) > 10)
        {
            ChangeState(0); //Idle
        }
        if (Vector3.Distance(spawnPoint.transform.position, transform.position) > 20 && state != 0)
        {
            ChangeState(2); //Retreat
        }
        if (Vector3.Distance(target.position, transform.position) < 10)
        {
            ChangeState(1); //Approach or BackOff, or Strafe. <- Need to implement random here
        }
        if (Vector3.Distance(target.position, transform.position) <= agent.stoppingDistance)
        {
            ChangeState(5); //Attack
        }

        //Im a pro.
        if (state != 5)
        {
            Behaviour(state);
        }
        else if (state == 5)
        {
            Attack();
        }

        animStateLayer1 = animator.GetCurrentAnimatorStateInfo(0);
        animTransition1 = animator.GetAnimatorTransitionInfo(0);
        isAttacking = animStateLayer1.IsTag("Attack");
	}

    void Attack()
    {
        //animationLock = true;
        if (!isAttacking)
        {
            animator.SetTrigger("Attack");
        }

        if (isAttacking)
        {
            agent.Stop();
            if (animStateLayer1.IsName("LightAttack1") == true)
            {
                if (animStateLayer1.normalizedTime >= attack1Start && animStateLayer1.normalizedTime <= attack1End)
                {
                    weapon.GetComponent<BoxCollider>().enabled = true;
                }
                else
                {
                    weapon.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }
}
