using UnityEngine;
using System.Collections;

public class SpearEnemy : Enemy
{
    //Use this for initialization

    protected override void Start()
    {
        type = EnemyType.SPEARENEMY;
        base.Start();

        if (deactivate)
        {
            SwitchNavMesh(false);
            return;
        }

        ChangeState(0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isDead()) { return; }
        base.Update();

        if (deactivate)
        {
            return;
        }

        BehaviourSwitch();
        Tracking();
    }

    protected override void Tracking()
    {
        base.Tracking();

        if(inAttack())
        {
            if (attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colStart)
            //if (attackingInv <= 0.8f)
            {
                LookAtTarget();
            }
        }
    }

    protected override void LookAtTarget()
    {
        transform.forward = Vector3.Lerp(transform.forward, target.position - transform.position, Time.deltaTime * turnSpeed);
    }

    void BehaviourSwitch()
    {
        //ChangeState(4);

        //Idle until Detection of Player
        behavCooldown -= Time.deltaTime;

        //Not working?? - Slides around during attack animation a lot
        if (inAttack() || !alerted)
        {
            return;
        }
        //Retreats rapidly to spawnPoint when character is out of leashing Range
        /*
        if (Vector3.Distance(spawnPoint, transform.position) > leashingRange)
        {
            ChangeState(State.RETREAT); //Retreat
            return;
        }
        /**/

        //Within detection Range, enemy approaches the player
        if (Vector3.Distance(target.position, transform.position) > combatRange && (behavCooldown <= 0) && !inAttack())
        {
            BehaviourRandomize = Random.Range(0, 100);

            if (BehaviourRandomize >= 0 && BehaviourRandomize < 30)
            {
                ChangeState(State.APPROACH);
                behavCooldown = Random.Range(1, 5) + 1;
            }
            if(BehaviourRandomize >= 30 && BehaviourRandomize < 80)
            {
                if (!Physics.Raycast(transform.position, transform.right, 1) || !Physics.Raycast(transform.position, -transform.right, 1))
                {
                    ChangeState(State.STRAFE);
                    behavCooldown = Random.Range(1, 3) + 1;
                }
            }
            if(BehaviourRandomize >= 80 && BehaviourRandomize < 100)
            {
                ChangeState(State.BACKOFF);
                behavCooldown = Random.Range(1, 3);
            }
        }

        //Within Combat Range, the enemy decides to attack, backoff or strafe around player
        if (Vector3.Distance(target.position, transform.position) <= combatRange && !inAttack() && !blocking)
        {
            BehaviourRandomize = (int)Mathf.Sign(Random.Range(-2, 5));

            switch (BehaviourRandomize)
            {
                case 1:
                    ChangeState(State.ATTACK);
                    break;
                case -1:
                    ChangeState(State.BACKOFF);
                    behavCooldown = Random.Range(0, 4) + 1;
                    break;
            }

            //Approach, BackOff, or Strafe. <- Need to implement random here
        }

        /**/
        Behaviour(currentState);
    }

    protected override void Attack()
    {
        //BLOCKING CONDITIONS
        base.Attack();

        if(!inAttack())
        {
            ChangeState(State.IDLE);
        }

        if (inAttack())
        {
            return;
        }
        
        int attackIndex = Random.Range(0,3);

        if(attackIndex <= 0)
        {
            StartAttack("E_SpearLight01");
            animator.SetInteger("AttackId", 0);
        }
        if(attackIndex == 1)
        {
            StartAttack("E_SpearLight02");
            animator.SetInteger("AttackId", 1);
        }
        if(attackIndex >= 2)
        {
            StartAttack("E_SpearHeavy01");
            animator.SetInteger("AttackId", 2);
        }
        /**/
        //Debug.Log(attackName);
        behavCooldown = AnimationLibrary.Get().SearchByName(attackName).duration;
        animator.SetTrigger("Attack");
    }

    protected override void CombatUpdate()
    {
        base.CombatUpdate();
        /* 
         * Need?
        /**/
    }

    protected override void CancelAttack()
    {
        base.CancelAttack();
        /* 
         * Need?
        /**/
    }

    protected override void Kill()
    {
        base.Kill();
        /* 
         * Need?
        /**/
    }

    protected override void Blocking()
    {
        base.Blocking();

        if (blockCooldown <= 0)
        {
            int rng = Random.Range(0, 2);
            //Debug.Log(rng);
            if (rng != 0)
            {
                if (!inAttack() || !blocking)
                {
                    blockDuration = Random.Range(2, 5);
                    blockCooldown = 3.0f;
                }
            }
        }
    }
}
