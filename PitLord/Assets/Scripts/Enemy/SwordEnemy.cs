using UnityEngine;
using System.Collections;

public class SwordEnemy : Enemy {

    float attack1Start;
    float attack1End;
    float blockRate = 3.0f;
    float blockDuration = 5.0f;

    // Use this for initialization
    void Start () {
        Init();

        attack1Start = AnimationLibrary.Get().SearchByName("LightAttack1").colStart;
        attack1End = AnimationLibrary.Get().SearchByName("LightAttack1").colEnd;
        
        if (deactivate)
        {
            agent.Stop();
            return;
        }

        ChangeState(0);
	}
	
	// Update is called once per frame
	void Update () {

        if (deactivate)
        {
            return;
        }

        Debug.Log("Current State" + state);

        animStateLayer1 = animator.GetCurrentAnimatorStateInfo(0);
        animTransition1 = animator.GetAnimatorTransitionInfo(0);
        isAttacking = animStateLayer1.IsTag("Attack") || animTransition1.IsUserName("Attack");

        behavCooldown -= Time.deltaTime;
        BehaviourSwitch();
	}

    void BehaviourSwitch()
    {
        //ChangeState(4);
        Debug.Log("behavCooldown" + (int)behavCooldown);
        
        //Idle until Detection of Player
        //*
        if (Vector3.Distance(target.position, transform.position) > detectionRange)
        {
            ChangeState(0); //Idle
        }

        //Retreats rapidly to spawnPoint when player is out of leashing Range
        if (Vector3.Distance(spawnPoint.transform.position, transform.position) > leashingRange && state != 0)
        {
            ChangeState(2); //Retreat
        }

        //Within detection Range, enemy approaches the player
        if ((Vector3.Distance(target.position, transform.position) < detectionRange || alerted) && Vector3.Distance(target.position, transform.position) > combatRange)
        {
            BehaviourRandomize = Random.Range(0, 3);

            //Debug.LogWarning("RANDOM MOVEMENT INT " + BehaviourRandomize);

            switch (BehaviourRandomize)
            {
                case 0:          
                    ChangeState(1);
                    break;
                case 1:
                    ChangeState(3);
                    break;
                case 2:
                    ChangeState(4);
                    break;
            }
        }

        //If you move outside detection range while in combat, always approaches after finishing current action
        if (((Vector3.Distance(target.position, transform.position) > detectionRange && alerted) && Vector3.Distance(target.position, transform.position) < leashingRange))
        {
            ChangeState(1);
        }

        if ((Vector3.Distance(target.position, transform.position) < detectionRange))
        {
            blockRate -= Time.deltaTime;

            if(blockRate <= 0)
            {
                blockRate = 0;
            }
        }

        //Within Combat Range, the enemy decides to attack, backoff or strafe around player
        if (Vector3.Distance(target.position, transform.position) < combatRange && !isAttacking)
        {
            BehaviourRandomize = Random.Range(0,2);
            //Debug.LogWarning("RANDOM MOVEMENT INT " + BehaviourRandomize);

            switch (BehaviourRandomize)
            {
                case 0:
                    ChangeState(5);
                    break;
                case 1:
                    ChangeState(3);
                    break;
            }

            //Approach or BackOff, or Strafe. <- Need to implement random here
        }

        /**/
        //Im a pro.
        if (state != 5)
        {
            Behaviour(state);
        }
        if (state == 5)
        {
            Attack();
        }
        /**/
    }

    void Attack()
    {
        if (state != 5)
        {
            return;
        }
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
