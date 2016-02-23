using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : Attributes {

    public NavMeshAgent agent;
    public Animator animator;
    public Transform target;

    public float speed;

    IBossBehaviour currentBehaviour;

    public bool active;

	void Start () {
        base.Start();
        Init();
        SetBehaviour(new BossBehaviourApproach(this));
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
        currentBehaviour.Execute();
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
