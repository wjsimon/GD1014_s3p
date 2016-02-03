using UnityEngine;
using System.Collections;

public class Enemy : Attributes
{
    public Animator animator;
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
    public bool alerted;

    public int BehaviourRandomize;


    //THIS TIMER IS FOR TESTING ONLY, USE SEPERATE TIMERS FOR NON-TEMPORARY STUFF
    float timer;

    protected State currentState;
    protected enum State
    {
        IDLE,
        APPROACH,
        BACKOFF,
        STRAFE,
        RETREAT,
        ATTACK,
    };

    protected int strafeDir;
    public float behavCooldown;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        Init();
    }

    public void Init()
    {
        behavCooldown = 0;
        currentState = State.IDLE;

        //Sets the spawnpoint by creating a new GameObject a playerpos
        StoreTransform temp = new StoreTransform(transform.position, transform.rotation, transform.localScale);
        spawnPoint = new GameObject(gameObject.name + "_Spawn");
        spawnPoint.transform.parent = gameObject.transform;
        spawnPoint.transform.position = temp.position;
        spawnPoint.transform.rotation = temp.rotation;
        spawnPoint.transform.localScale = temp.localScale;

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.destination = target.position;

        RegisterObject();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (deactivate)
        {
            return;
        }

        CombatUpdate();
        //Basically a state machine, gotta do all the randomizing and checking for which "state" the enemy should be in here <-- This is pretty much where enemies get coded, all the other stuff is the same
        //Debug.Log(currentState);
        Behaviour(currentState);
        /**/
    }

    protected void Behaviour( State state )
    {
        //Debug.Log("behaviour state " + state);

        switch (state)
        {
            case State.IDLE:
                Idle();
                break;
            case State.APPROACH:
                Approach();
                break;
            case State.RETREAT:
                Retreat();
                break;
            case State.BACKOFF:
                BackOff();
                break;
            case State.STRAFE:
                Mathf.Sign(Random.Range(-2, 1));
                Strafe(strafeDir);
                break;
            case State.ATTACK:
                Attack();
                break;
        }
    }

    protected void LookAtTarget()
    {
        transform.LookAt(target, Vector3.up);
    }

    protected virtual void Tracking()
    {

    }

    public void Idle() //0
    {
        SwitchNavMesh(false);

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
    }

    public void Approach()//1
    {
        alerted = true;

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 1);
        SwitchNavMesh(true);
        agent.SetDestination(target.position);
    }
    public void Retreat() //2
    {
        alerted = false;

        agent.speed = 6;
        target = spawnPoint.transform;
        agent.SetDestination(spawnPoint.transform.position);

        if (transform.position == spawnPoint.transform.position)
        {
            ChangeState(0);
        }
    }

    public void BackOff() //3
    {
        BackwardsMovement();
    }
    public void Strafe( int direction ) //4
    {
        SwitchNavMesh(false);

        /*
        if ((Vector3.Distance(target.position, transform.position) > detectionRange || (Vector3.Distance(target.position, transform.position) < combatRange)))
        {
            behavCooldown = 0;
            return;
        }
        /**/

        //Debug.LogWarning("strafing");

        animator.SetFloat("X", -direction);
        animator.SetFloat("Y", 0);

        //float step = agent.speed * Time.deltaTime;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up), Time.deltaTime * 180);
        //transform.RotateAround(target.position, Vector3.up, (Mathf.Sign(direction) * Mathf.Clamp((10 * Vector3.Distance(transform.position, target.position)), 15, 15) * Time.deltaTime));
        transform.RotateAround(target.position, Vector3.up, (Mathf.Sign(direction) * 8 * Time.deltaTime));
        transform.LookAt(target.transform);
        //GetComponent<CharacterController>().Move((Mathf.Sign(animator.GetFloat("X")) * transform.right) * Time.deltaTime * agent.speed / 3);
    }

    public void ForwardMovement()
    {
        if (Vector3.Angle((target.position - transform.position), transform.forward) > 30)
        {
            SwitchNavMesh(false);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up), Time.deltaTime * agent.angularSpeed);
            animator.SetFloat("X", 0);
            animator.SetFloat("Y", 1);
        }
        else
        {
            SwitchNavMesh(true);
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
    }

    public void BackwardsMovement()
    {
        SwitchNavMesh(false);

        //Debug.LogWarning("backwalking");

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", -1);

        //float step = agent.speed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized, Vector3.up), Time.deltaTime * agent.angularSpeed * 2);
        GetComponent<CharacterController>().Move(-transform.forward * Time.deltaTime * agent.speed / 3);
    }

    protected virtual void Attack()
    {
        SwitchNavMesh(false);
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

    protected State ChangeState( State newState )
    {
        if (newState == currentState)
        {
            return currentState;
        }
        //Debug.Log("change state to " + newState);

        /*
        if (behavCooldown >= 0 && (state == 3 || state == 4 || state == 5))
        {
            return state;
        }
        if (behavCooldown >= 0 && state == 1 && (newState != 5))
        {
            return state;
        }
        /**/

        if (currentState == State.RETREAT)
        {
            return currentState;
        }

        if (newState != currentState)
        {
            if (newState == State.STRAFE)
            {
                strafeDir = (int)Mathf.Sign(Random.Range(-2, 1));
            }
            /**/
        }

        currentState = newState;
        return newState;
    }

    protected void CombatUpdate()
    {
        if (inAttack())
        {
            attacking -= Time.deltaTime;
            attackingInv += Time.deltaTime;
            //transform.Rotate(new Vector3(0, 10, 0));
            if (attacking >= AnimationLibrary.Get().SearchByName(attackName).colStart)
            {
                SwitchNavMesh(false);
            }

            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart && attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colEnd)
            {
                weapon.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                weapon.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    protected override void DisableHitbox(float dur)
    {
        base.DisableHitbox(dur);
        GetComponent<Enemy>().weapon.GetComponent<BoxCollider>().enabled = false;
    }

    protected virtual void SwitchNavMesh(bool enable)
    {
        if(enable == agent.enabled)
        {
            return;
        }

        if(!enable)
        {
            agent.Stop();
            agent.enabled = false;
        }
        else if (enable)
        {
            agent.enabled = true;
            agent.Resume();
        }
    }

    protected override void RegisterObject()
    {
        base.RegisterObject();
        GameManager.instance.AddEnemy(gameObject);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.instance.AddEnemy(gameObject);
    }

    public void Alert()
    {
        alerted = true;
        ChangeState(State.APPROACH);
    }
}