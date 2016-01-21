using UnityEngine;
using System.Collections;

public class Enemy : Attributes
{

    //[HideInInspector]
    public GameObject spawnPoint;

    public Animator animator;
    protected AnimatorStateInfo animStateLayer1;
    protected AnimatorStateInfo animStateLayer2;
    protected AnimatorTransitionInfo animTransition1;
    protected AnimatorTransitionInfo animTransition2;
    bool blending = true;
    float animationBlend;

    /*
    public string currentAnimation;
    public float animationCooldown;
    public float animationDuration;
    /**/

    [HideInInspector]
    protected NavMeshAgent agent;
    public Transform target;
    public GameObject weapon;

    public int BehaviourRandomize;


    //TIMER IS FOR TESTING ONLY, USE SEPERATE TIMERS FOR NON-TEMPORARY STUFF
    float timer = 0;

    protected int state;
    protected int strafeDir;
    protected bool isAttacking;

    protected bool alerted;

    public float behavCooldown;
    float cooldownStorage;
    // Use this for initialization
    void Start()
    {
        Init();
    }

    public void Init()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        cooldownStorage = behavCooldown;

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
    void Update()
    {
        //Basically a state machine, gotta do all the randomizing and checking for which "state" the enemy should be in here <-- This is pretty much where enemies get coded, all the other stuff is the same
        Behaviour(state);
        /**/
    }

    public void Behaviour(int state)
    {
        Debug.Log("behaviour state " + state);

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
            case 3:
                BackOff();
                break;
            case 4:
                Random rng = new Random();
                Mathf.Sign(Random.Range(-2, 1));
                Strafe(strafeDir);
                break;
        }
    }

    public void Idle() //0
    {
        agent.Stop();

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
    }

    public float Approach()//1
    {
        alerted = true;

        agent.SetDestination(target.position);

        return ForwardMovement();
    }

    public void BackOff() //3
    {
        BackwardsMovement();
    }
    public void Strafe(int direction) //4
    {
        agent.Stop();

        if((Vector3.Distance(target.position, transform.position) > detectionRange || (Vector3.Distance(target.position, transform.position) < combatRange)))
        {
            behavCooldown = 0;
            return;
        }

        Debug.LogWarning("strafing");

        animator.SetFloat("X", -direction);
        animator.SetFloat("Y", 0);

        float step = agent.speed * Time.deltaTime;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up), Time.deltaTime * 180);
        transform.RotateAround(target.position, Vector3.up, (Mathf.Sign(direction) * Mathf.Clamp((10 * Vector3.Distance(transform.position, target.position)), 15, 15) * Time.deltaTime));
        transform.LookAt(target.transform);
        //GetComponent<CharacterController>().Move((Mathf.Sign(animator.GetFloat("X")) * transform.right) * Time.deltaTime * agent.speed / 3);
    }
    public float Retreat() //2
    {
        alerted = false;

        agent.speed = 6;
        target = spawnPoint.transform;
        agent.SetDestination(spawnPoint.transform.position);

        return ForwardMovement();
    }

    public float ForwardMovement()
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

    public float BackwardsMovement()
    {
        agent.Stop();
        Debug.LogWarning("backwalking");

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", -1);

        float step = agent.speed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up), Time.deltaTime * agent.angularSpeed * 2);
        GetComponent<CharacterController>().Move(-transform.forward * Time.deltaTime * agent.speed / 3);
        return agent.remainingDistance;
    }

    //BETA TESTING----------------DONT USE FOR BLENDING MORE THAN ONE VALUE----------------------
    public void ResetAnimationBlend( float value, float from, float to )
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

    public int ChangeState(int newState)
    {
        Debug.Log("change state to " + newState);

        if (behavCooldown >= 0 && (state == 3 || state == 4 || state == 5))
        {
            return state;
        }
        if (behavCooldown >= 0 && state == 1 && (newState != 5))
        {
            return state;
        }
        /**/

        if (state == 2)
        {
            return state;
        }

        if (newState != state)
        {
            if (newState == 4)
            {
                strafeDir = (int)Mathf.Sign(Random.Range(-2, 1));
            }
            
            if (behavCooldown < 0)
            {
                behavCooldown = cooldownStorage;//+ Random.Range(-2, 2);
            }
            /**/
        }

        state = newState;
        return newState;
    }
}
