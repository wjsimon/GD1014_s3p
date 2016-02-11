using UnityEngine;
using System.Collections;

public class SwordEnemy : Enemy
{
    //Use this for initialization

    public int combo;
    protected override void Start()
    {
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
        //Retreats rapidly to spawnPoint when player is out of leashing Range
        /*
        if (Vector3.Distance(spawnPoint.transform.position, transform.position) > leashingRange)
        {
            ChangeState(State.RETREAT); //Retreat
            return;
        }
        /**/

        //Within detection Range, enemy approaches the player
        if (Vector3.Distance(target.position, transform.position) > combatRange && (behavCooldown <= 0) && !inAttack())
        {
            BehaviourRandomize = Random.Range(0, 100);

            if (BehaviourRandomize >= 0 && BehaviourRandomize < 60)
            {
                ChangeState(State.APPROACH);
                behavCooldown = Random.Range(0, 4) + 1;
            }
            if(BehaviourRandomize >= 60 && BehaviourRandomize < 90)
            {
                if (!Physics.Raycast(transform.position, transform.right, 1) || !Physics.Raycast(transform.position, -transform.right, 1))
                {
                    ChangeState(State.STRAFE);
                    behavCooldown = Random.Range(0, 4) + 1;
                }
            }
            if(BehaviourRandomize >= 90 && BehaviourRandomize < 100)
            {
                ChangeState(State.BACKOFF);
                behavCooldown = Random.Range(0, 4) + 1;
            }
        }

        //Within Combat Range, the enemy decides to attack, backoff or strafe around player
        if (Vector3.Distance(target.position, transform.position) <= combatRange && !inAttack())
        {
            BehaviourRandomize = (int)Mathf.Sign(Random.Range(-2, 5));

            switch (BehaviourRandomize)
            {
                case 1:
                    ChangeState(State.ATTACK);
                    behavCooldown = Random.Range(0, 4) + 1;
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
        base.Attack();

        if(!inAttack() && combo <= 0)
        {
            ChangeState(State.IDLE);
        }
        //animationLock = true;
        if (inAttack())
        {
            return;
        }

        int attackIndex = Random.Range(0, 4);

        if(attackIndex < 3)
        {
            StartAttack("E_SwordCombo01");
        }
        if(attackIndex == 3)
        {
            StartAttack("E_SwordHeavy");
        }

        if(attackName == "E_SwordHeavy")
        {
            animator.SetInteger("AttackId", 1);
        }
        else
        {
            animator.SetInteger("AttackId", 0);
            combo = 2; //combo = hit count - 1
        }

        animator.SetTrigger("Attack");
    }

    protected override void CombatUpdate()
    {
        base.CombatUpdate();

        if(combo > 0 && !inAttack())
        {
            if (attackName == "E_SwordCombo01")
            {
                StartAttack("E_SwordCombo02");
                combo -= 1;
            }
            else if (attackName == "E_SwordCombo02")
            {
                StartAttack("E_SwordCombo03");
                combo -= 1;
            }
        }
    }

    protected override void CancelAttack()
    {
        base.CancelAttack();
        combo = 0;
    }
    protected override void Kill()
    {
        Debug.Log("SWORDENEMY KILL");
        base.Kill();
        combo = 0;
    }

    protected override void RegisterObject()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().AddEnemy(GetComponent<Enemy>());
    }
}
