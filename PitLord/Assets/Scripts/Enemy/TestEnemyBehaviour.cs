using UnityEngine;
using System.Collections;

public class TestEnemyBehaviour : Enemy
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
        if (deactivate)
        {
            return;
        }

        base.Update();

        StaminaRegen();
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
        //Debug.Log("behavCooldown" + (int)behavCooldown);

        //Idle until Detection of Player
        //*
        behavCooldown -= Time.deltaTime;
        /*
        if (behavCooldown < 0)
        {
            behavCooldown = 0;
        }
        /**/

        //Not working?? - Slides around during attack animation a lot
        if (inAttack())
        {
            return;
        }

        if (Vector3.Distance(target.position, transform.position) > detectionRange)
        {
            //If you move outside detection range while in combat, always approaches after finishing current action
            if (Vector3.Distance(target.position, transform.position) < leashingRange && !inAttack())
            {
                ChangeState(State.APPROACH);
                return;
            }

            ChangeState(State.IDLE); //Idle
            return;
        }

        //Retreats rapidly to spawnPoint when player is out of leashing Range
        if (Vector3.Distance(spawnPoint.transform.position, transform.position) > leashingRange)
        {
            ChangeState(State.RETREAT); //Retreat
            return;
        }

        //Within detection Range, enemy approaches the player
        if (Vector3.Distance(target.position, transform.position) > combatRange && (behavCooldown <= 0) && !inAttack())
        {
            behavCooldown = Random.Range(0, 4) + 1;
            BehaviourRandomize = Random.Range(0, 3);

            //Debug.LogWarning("RANDOM MOVEMENT INT " + BehaviourRandomize);

            switch (BehaviourRandomize)
            {
                case 0:
                    ChangeState(State.APPROACH);
                    break;
                case 1:
                    ChangeState(State.BACKOFF);
                    break;
                case 2:
                    ChangeState(State.STRAFE);
                    break;
            }
        }

        //Within Combat Range, the enemy decides to attack, backoff or strafe around player
        if (Vector3.Distance(target.position, transform.position) < combatRange && !inAttack())
        {
            BehaviourRandomize = Random.Range(0, 2);
            //Debug.LogWarning("RANDOM MOVEMENT INT " + BehaviourRandomize);

            switch (BehaviourRandomize)
            {
                case 0:
                    ChangeState(State.ATTACK);
                    break;
                case 1:
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
        GameObject.Find("GameManager").GetComponent<GameManager>().AddEnemy(GetComponent<Enemy>());
    }
}
