using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : Attributes
{
    public NavMeshAgent agent;
    public Animator animator;
    public Transform target;
    public BossTurret turret;

    public BossShockwave shockwave;

    public int phase;

    public float speed;
    public IBossBehaviour currentBehaviour;
    public bool active;

    public string[] attackNames = new string[]
    {
        "Combo01",
        "Combo02",
        "Combo03",
        "Heavy",
        "Aoe",
        "Projectile",
    };

    void Start()
    {
        base.Start();
        Init();
    }

    void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameObject.Find("Player").transform;

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);

        phase = 1;
    }

    void Update()
    {
        if (!active) { return; }

        base.Update();
        Behaviour();
    }

    void Behaviour()
    {
        if (currentBehaviour != null)
        {
            if (!currentBehaviour.Execute())
            {
                currentBehaviour.Finish();
                currentBehaviour = null;
            }
        }

        if (currentBehaviour == null)
        {
            float rng = Random.Range(0.0f, 4.0f);
            //Debug.Log(rng);

            //rng = 1.5f; //DEBUG to Force Behaviours

            if (rng <= 0)
            {
                SetBehaviour(new BossBehaviourIdle(this));
            }
            else if (rng <= 1)
            {
                SetBehaviour(new BossBehaviourSpawnTurret(this));
            }
            else if (rng <= 2)
            {
                SetBehaviour(new BossBehaviourHeavy(this));
            }
            else if (rng <= 3)
            {
                SetBehaviour(new BossBehaviourCombo(this));
            }
            else if (rng <= 4)
            {
                SetBehaviour(new BossBehaviourAOE(this));
            }
        }
    }

    public void SetBehaviour( IBossBehaviour newBehaviour )
    {
        currentBehaviour = newBehaviour;
        currentBehaviour.Init();
        Debug.Log("setBehaviour " + currentBehaviour.ToString());
    }
    public IBossBehaviour GetBehaviour()
    {
        return currentBehaviour;
    }

    public void Activate()
    {
        active = true;
    }

    public AnimationWrapper GetAnimation( int index )
    {
        string name = "B_" + attackNames[index] + "_" + phase;
        return AnimationLibrary.Get().SearchByName(name);
    }

    public override bool ApplyDamage( int healthDmg, int staminaDmg, Attributes source )
    {
        if (!base.ApplyDamage(healthDmg, staminaDmg, source)) { return false; }

        currentHealth -= healthDmg;

        if (currentHealth <= (maxHealth * 0.66f))
        {
            phase = 2;
        }
        else if (currentHealth <= (maxHealth * 0.33f))
        {
            phase = 3;
        }

        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            Kill();
        }

        return true;
    }
}
