using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MageEnemy : Enemy
{
    public List<Transform> portLocations;
    Transform lastPort;
    public float warping;
    public float healing;

    public int healthDmg = 9;
    public int staminaDmg = 9;

    bool aoe;

    // Use this for initialization
    protected override void Start()
    {
        type = EnemyType.MAGENEMY;
        base.Start();

        if (deactivate)
        {
            SwitchNavMesh(false);
            return;
        }

        portLocations.Add(transform);
        lastPort = transform;

        if (combatRange == 0)
        {
            combatRange = 20;
        }

        ChangeState(State.IDLE);
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
    }

    void BehaviourSwitch()
    {
        behavCooldown -= Time.deltaTime;

        if (inAttack() || inWarp() || inHeal() || !alerted)
        {
            return;
        }

        if(Vector3.Distance(transform.position, target.position) <= 5)
        {
            StartDamageSpell();
        }

        if(behavCooldown <= 0 && !inAttack())
        {
            BehaviourRandomize = Random.Range(0, 2);

            if(BehaviourRandomize == 0)
            {
                StartWarp();
                behavCooldown = 15;
            }
            if(BehaviourRandomize == 1)
            {
                StartHeal();
                behavCooldown = 15;
            }
        }

        //StartWarp();
        //StartDamageSpell();
        //StartHeal();
    }

    protected override void CombatUpdate()
    {
        if (!alerted) { return; }

        if (inAttack())
        {
            attacking -= Time.deltaTime;
            attackingInv += Time.deltaTime;

            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart)
            {
                AreaOfEffectDamage();
            }
        }

        if (inWarp())
        {
            warping -= Time.deltaTime;

            if (warping <= 0)
            {
                warping = 0;
                Warp();
            }
        }

        if (inHeal())
        {
            healing -= Time.deltaTime;

            if (healing <= 0)
            {
                healing = 0;
                HealingEffect();
            }
        }
    }

    protected override void Blocking()
    {
        //Bow Enemy can't Block!
        blocking = false;
    }

    void StartDamageSpell()
    {
        if (inAttack() || inHeal())
        {
            return;
        }

        StartAttack("E_MageSpell01");
        animator.SetInteger("AttackId", 0);
        animator.SetTrigger("Attack");

        aoe = true;
    }

    void AreaOfEffectDamage()
    {
        if (!aoe) { return; }
        Debug.Log("AOE");

        if(Vector3.Distance(transform.position, target.position) <= 5)
        {
            GameManager.instance.player.ApplyDamage(healthDmg, staminaDmg, this);
            aoe = false;
        }
    }

    void StartHeal()
    {
        if (inWarp() || inAttack()) { return; }
        healing = 2.0f;
    }

    void HealingEffect()
    {
        for(int i = 0; i < GameManager.instance.enemyList.Count; i++)
        {
            GameManager.instance.enemyList[i].currentHealth = GameManager.instance.enemyList[i].maxHealth;
        }
    }

    public void StartWarp()
    {
        if (inHeal() || inAttack()) { return; }
        warping = 2.0f;
    }

    public void Warp()
    {
        List<Transform> locations = new List<Transform>();

        for (int i = 0; i < portLocations.Count; i++)
        {
            if(portLocations[i].position == transform.position)
            {
                continue;
            }

            else if(Vector3.Distance(portLocations[i].position, target.position) <= 5)
            {
                continue;
            }

            else
            {
                locations.Add(portLocations[i]);
            }
        }


        int rng = Random.Range(0, locations.Count);
        agent.Warp(locations[rng].position);
        lastPort = locations[rng];

        animator.SetTrigger("Warp");
    }

    public bool inWarp()
    {
        return warping > 0;
    }

    public bool inHeal()
    {
        return healing > 0;
    }

    void Cancel()
    {
        warping = 0;
        healing = 0;
        CancelAttack();

        animator.SetTrigger("Reset");
    }

    public override void OnHit()
    {
        base.OnHit();
        Cancel();
        Warp();
    }

    public override void Alert()
    {
        alerted = true;
    }

    /*
    protected override void Attack()
    {
        //base.Attack();
        //Base.Attack portion, needed to modify

        if (inAttack())
        {
            return;
        }

        RaycastHit rayInfo;
        Vector3 rayCastTarget = (rayTarget.transform.position - projectileSource.transform.position).normalized;

        Debug.DrawRay(projectileSource.transform.position, rayCastTarget * 20, Color.red, 20, false);
        if (Physics.Raycast(projectileSource.transform.position, rayCastTarget, out rayInfo, 100)) //transform.position needs to be projectileSource
        {
            //Debug.Log(rayInfo.collider.gameObject.name + " " + rayInfo.collider.gameObject.tag);
            if(rayInfo.collider.gameObject.tag == "Player")
            {
                animator.SetTrigger("Attack");
                attackName = "LightAttack1";
                attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                attackingInv = 0;

                LaunchProjectile();
            }
        }
    }
    /**/
}
