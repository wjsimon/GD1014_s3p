using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : Attributes {

    public NavMeshAgent agent;
    public Animator animator;
    public Transform target;
    public BossTurret turret;


    public float speed;

    IBossBehaviour currentBehaviour;

    public bool active;

	void Start () {

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
    }
	
	void Update () {
        if (!active) { return; }

        base.Update();
        Behaviour();
	}

    void Behaviour()
    {
        if (currentBehaviour != null)
        {
            if(!currentBehaviour.Execute())
            {
                //currentBehaviour.Finish();
                currentBehaviour = null;
            }
        }

        if(currentBehaviour == null)
        {
            float rng = Random.Range(0, 5);

            if(rng <= 1)
            {
                //
            }
            else if(rng <= 3)
            {
                //
            }

            SetBehaviour(new BossBehaviourSpawnTurret(this));
        }
    }

    public void SetBehaviour(IBossBehaviour newBehaviour)
    {
        currentBehaviour = newBehaviour;
    }
    public IBossBehaviour GetBehaviour()
    {
        return currentBehaviour;
    }

    public void Activate()
    {
        active = true;
    }
}
