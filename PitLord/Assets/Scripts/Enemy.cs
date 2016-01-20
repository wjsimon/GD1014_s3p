using UnityEngine;
using System.Collections;

public class Enemy : Attributes {

    //[HideInInspector]
    public GameObject spawnPoint;

    public Animator animator;
    bool blending = true;
    float animationBlend;

    NavMeshAgent agent;
    public Transform target;


    //TIMER IS FOR TESTING ONLY, USE SEPERATE TIMERS FOR NON-TEMPORARY STUFF
    float timer = 0;
    int state = 1;

	// Use this for initialization
    void Start()
    {
        Init();
    }

    public void Init()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        //Sets the spawnpoint by creating a new GameObject a playerpos
        StoreTransform temp = new StoreTransform(transform.position, transform.rotation, transform.localScale);
        spawnPoint = new GameObject(gameObject.name + "_Spawn");
        spawnPoint.transform.position = temp.position;
        spawnPoint.transform.rotation = temp.rotation;
        spawnPoint.transform.localScale = temp.localScale;

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.destination = target.position;
    }
	
	// Update is called once per frame
	void Update () 
    {
        Debug.LogWarning(state);

        timer += Time.deltaTime;

        if (timer > 5)
        {
            state = 2;
        }
        //Basically a state machine, gotta do all the randomizing and checking for which "state" the enemy should be in here <-- This is pretty much where enemies get coded, all the other stuff is the same
        Behaviour(state);
        /**/
	}

    public void Behaviour(int state)
    {
        switch (state)
        {
            case 0:
                Idle();
                break;
            case 1:
                Approach();
                break;
            case 2:
                Retreat();
                break;
        }
    }

    public void Idle()
    {
        agent.Stop();

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
    }

    public float Approach()
    {
        Debug.LogWarning("ENTERED APPROACH");
        agent.SetDestination(target.position);

        return Movement();        
    }

    public void BackOff()
    {

    }
    public void Strafe()
    {

    }
    public float Retreat()
    {
        agent.speed = 6;
        target = spawnPoint.transform;
        agent.SetDestination(spawnPoint.transform.position);

        return Movement();
    }

    public float Movement()
    {
        if (Vector3.Angle((target.position - transform.position), transform.forward) > 30)
        {
            agent.Stop();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up), Time.deltaTime * agent.angularSpeed);
            animator.SetFloat("X", 0);
            animator.SetFloat("Y", 1);
        }
        else
        {
            agent.Resume();
        }

        if (Vector3.Distance(Vector3.zero, agent.velocity) > 0 && Vector3.Distance(Vector3.zero, agent.velocity) < 1)
        {
            animator.SetFloat("X", 0);
            animator.SetFloat("Y", 1);
        }
        if (Vector3.Distance(Vector3.zero, agent.velocity) >= 1)
        {
            float tempBlend = 2;
            if (blending)
            {
                animationBlend += Time.deltaTime;
                tempBlend = Mathf.Clamp(Mathf.Lerp(1, 2, animationBlend), 0, 2);
            }

            ResetAnimationBlend(animator.GetFloat("Y"), 2, 1.9f);

            animator.SetFloat("X", 0);
            animator.SetFloat("Y", tempBlend);
        }

        if (Vector3.Distance(target.position, transform.position) <= agent.stoppingDistance)
        {
            animationBlend += Time.deltaTime;
            animator.SetFloat("X", Mathf.Lerp(animator.GetFloat("X"), 0, animationBlend));
            animator.SetFloat("Y", Mathf.Lerp(animator.GetFloat("Y"), 0, animationBlend));
        }

        return agent.remainingDistance;
    }

    //BETA TESTING----------------DONT USE FOR BLENDING MORE THAN ONE VALUE----------------------
    public void ResetAnimationBlend( float value, float from, float to)
    {
        if (value >= to)
        {
            blending = false;
            animationBlend = 0;
        }
        else if (value >= from && value < to)
        {
            blending = true;
        }
    }
}
