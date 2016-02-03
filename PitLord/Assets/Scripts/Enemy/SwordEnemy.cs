using UnityEngine;
using System.Collections;

public class SwordEnemy : Enemy
{
    //Animationen per Hand; Bool raus - Cooldown rein
    //Use this for initialization
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
        base.Update();

        StaminaRegen();

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
            //if(attacking >= AnimationLibrary.Get().SearchByName(attackName).colStart);
            if (attackingInv <= 0.8f)
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
            behavCooldown = Random.Range(0, 4) + 1;
            BehaviourRandomize = Random.Range(0, 100);

            if (BehaviourRandomize >= 0 && BehaviourRandomize < 60)
            {
                ChangeState(State.APPROACH);
            }
            if(BehaviourRandomize >= 60 && BehaviourRandomize < 90)
            {
                ChangeState(State.STRAFE);
            }
            if(BehaviourRandomize >= 90 && BehaviourRandomize < 100)
            {
                ChangeState(State.BACKOFF);
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
                    break;
                case -1:
                    ChangeState(State.BACKOFF);
                    break;
            }

            //Approach or BackOff, or Strafe. <- Need to implement random here
        }

        /**/
        Behaviour(currentState);
    }

    protected override void Attack()
    {
        base.Attack();

        if(!inAttack())
        {
            ChangeState(State.IDLE);
        }
        //animationLock = true;
        if (inAttack())
        {
            return;
        }

        animator.SetTrigger("Attack");
        attackName = "LightAttack1";
        attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
        attackingInv = 0;
    }

    protected override void RegisterObject()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().AddEnemy(gameObject);
    }
}
