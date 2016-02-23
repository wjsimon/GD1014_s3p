using UnityEngine;
using System.Collections;

public class Enemy : Character
{
    bool blending = true;
    float animationBlend;

    [HideInInspector]
    public CombatTrigger combatTrigger;
    float encounterUpdate;

    public static bool[] encountered = new bool[(int)EnemyType.COUNT];
    public enum EnemyType
    {
        SWORDENEMY,
        SPEARENEMY,
        BOWENEMY,
        MAGENEMY,
        COUNT
    }

    [HideInInspector]
    public EnemyType type;

    /*
    public string currentAnimation;
    public float animationCooldown;
    public float animationDuration;
    /**/

    [HideInInspector]
    protected NavMeshAgent agent;
    public Transform target;
    public WeaponScript weapon;
    public bool alerted;

    public int BehaviourRandomize;
    public float blockCooldown = 5.0f;
    public float blockDuration = 0.0f;
    public float turnSpeed = 360;

    public float leashingRange;
    public float combatRange;

    public Vector3 curNavPos;
    public float navMeshTimer;
    //THIS TIMER IS FOR TESTING ONLY, USE SEPERATE TIMERS FOR NON-TEMPORARY STUFF
    float timer;

    public State currentState;
    public enum State
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
        targettable = true;

        encountered[(int)type] = PlayerPrefs.GetInt(type.ToString() + "/encountered/") > 0;
        encounterUpdate = 0;

        agent = gameObject.GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        
        agent.destination = target.position;

        RegisterObject();
    }

    // Update is called once per frame
    protected override void Update()
    {
        animator.SetBool("Block", blocking);
        base.Update();

        Debug.DrawLine(transform.position, agent.destination, Color.red);

        if (deactivate) { return; }

        CombatUpdate();
        navMeshTimer -= Time.deltaTime;

        Behaviour(currentState);
        FirstEncounterCheck();
        /**/
    }

    protected void Behaviour( State state )
    {
        //Debug.Log("behaviour state " + state);
        if (inStun()) { return; }

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

    protected virtual void LookAtTarget()
    {
        if(!alerted) { return; }

        Vector3 fo = Vector3.Lerp(transform.forward, target.position - transform.position, Time.deltaTime * turnSpeed);
        fo.y = 0;
        fo.Normalize();
        transform.forward = fo;
    }
    protected virtual void Tracking()
    {
        if (!alerted) { return; }
    }

    public void Idle() //0
    {
        SwitchNavMesh(false);

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
    }

    public void Approach()//1
    {
        if (isDead()) { return; }
        SetNavPosition(target.position);

        alerted = true;

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", Mathf.Clamp01(agent.velocity.magnitude));
        SwitchNavMesh(true);
    }

    protected void SetNavPosition( Vector3 pos )
    {
        if ((pos - curNavPos).magnitude > 2 || navMeshTimer <= 0)
        {
            agent.SetDestination(pos);
            agent.Resume();
            curNavPos = pos;
            navMeshTimer = 1.0f;
        }
    }

    public void Retreat() //2
    {
        alerted = false;

        agent.speed = 6;
        SetNavPosition(spawnPoint);

        if ((transform.position - spawnPoint).magnitude < 0.1f)
        {
            ChangeState(State.IDLE);
        }
    }

    public void BackOff() //3
    {
        RaycastHit hitInfo;
        Vector3 origin = transform.FindChild("RayCastTarget").position;
        Debug.DrawRay(origin, -transform.forward, Color.green);
        if (Physics.Raycast(origin, -transform.forward, out hitInfo, 1.0f, (1<<LayerMask.NameToLayer("Geometry"))))
        {
            Debug.Log(hitInfo.collider.gameObject.name + " Backoff blocked by Geometry");
            ChangeState(State.APPROACH);
            behavCooldown = 3.0f;
            return;
        }

        BackwardsMovement();
    }
    public void Strafe( int direction ) //4
    {
        RaycastHit hitInfo;
        Vector3 origin = transform.FindChild("RayCastTarget").position;
        Debug.DrawRay(origin, (transform.right * -direction), Color.green);
        //Debug.Log((1 << LayerMask.NameToLayer("Geometry")));

        if (Physics.Raycast(origin, transform.right * -direction, out hitInfo, 1.0f, (1 << LayerMask.NameToLayer("Geometry"))))
        {
            Debug.Log(hitInfo.collider.gameObject.name + " Strafe blocked by Geometry");
            ChangeState(State.APPROACH);
            behavCooldown = 3.0f;
            return;
        }

        SwitchNavMesh(false);

        //Debug.LogWarning("strafing");

        animator.SetFloat("X", -direction);
        animator.SetFloat("Y", 0);

        Vector3 lookAt = target.transform.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);

        agent.Move(-transform.right * direction * Time.deltaTime);
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

        transform.forward = Vector3.Lerp(transform.forward, target.position - transform.position, Time.deltaTime * turnSpeed);
        agent.Move(-transform.forward * Time.deltaTime);
    }

    protected virtual void Attack()
    {
        blockCooldown = 5f;
        blockDuration = 0.0f;
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
                /*
                if(Physics.Raycast(transform.position, -transform.right * strafeDir, 3))
                {
                    return currentState;
                }
                /**/
            }
        }

        currentState = newState;
        SetNavPosition(target.position);
        return newState;
    }

    protected virtual void CombatUpdate()
    {
        if (!alerted) { return; }
        if (inStun()) { return; }

        SetNavPosition(target.position);

        if (inAttack())
        {
            attacking -= Time.deltaTime;
            attackingInv += Time.deltaTime;
            //transform.Rotate(new Vector3(0, 10, 0));
            if (attacking >= AnimationLibrary.Get().SearchByName(attackName).colStart)
            {
                SwitchNavMesh(false);
            }

            /*
            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart && attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colEnd)
            {
                Debug.Log("collider true");
                weapon.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                weapon.GetComponent<BoxCollider>().enabled = false;
            }
            /**/
        }

        Blocking();
    }

    protected virtual void Blocking()
    {
        //Works, needs tuning big time
        blocking = blockDuration > 0;
        blockCooldown -= Time.deltaTime;
        animator.SetBool("Block", blocking);

        if(blocking)
        {
            blockDuration -= Time.deltaTime;
            turnSpeed = 90;
        }
        else
        {
            turnSpeed = 360;
        }

    }

    protected override void StaminaRegen()
    {
        base.StaminaRegen();
        if (inAttack() || blocking)
        {
            regenCounter = -1.5f;
        }
    }

    protected override void SetInvincibility( float dur )
    {
        base.SetInvincibility(dur);

        if(weapon != null)
        {
            GetComponent<Enemy>().weapon.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public override void SoftReset()
    {
        base.SoftReset();

        agent.Stop();
        ChangeState(State.IDLE);
        agent.Warp(spawnPoint);

        if (weapon != null && weapon.GetComponent<BoxCollider>() != null)
        {
            weapon.GetComponent<BoxCollider>().enabled = false;
        }

        alerted = false;
    }
    protected override void Kill()
    {
        base.Kill();

        if(combatTrigger != null)
        {
            combatTrigger.active -= 1;
        }

        GetComponent<CapsuleCollider>().enabled = false;
    }

    protected override void CancelAttack()
    {
        base.CancelAttack();
    }

    protected virtual void SwitchNavMesh( bool enable )
    {
        if (!enable)
        {
            //Debug.Log("stop");
            agent.Stop();
            //agent.enabled = false;
        }
        else if (enable)
        {
            //agent.enabled = true;
            //agent.Resume();
        }
    }


    protected override void RegisterObject()
    {
        base.RegisterObject();
        GameManager.instance.AddEnemy(GetComponent<Enemy>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.instance.AddEnemy(GetComponent<Enemy>());
    }

    public virtual void Alert()
    {
        alerted = true;
        combatTrigger.active += 1;
        ChangeState(State.APPROACH);
    }

    protected virtual void FirstEncounterCheck()
    {
        if (encountered[(int)type]) { return; }

        encounterUpdate += Time.deltaTime;

        if (encounterUpdate >= 1.0f)
        {
            encounterUpdate = 0;

            if (Vector3.Distance(target.position, transform.position) >= 20f)
            {
                return;
            }

            Vector3 yA = Camera.main.transform.forward;
            Vector3 yB = transform.position - Camera.main.transform.position;

            yA.y = 0;
            yB.y = 0;

            yA.Normalize();
            yB.Normalize();

            float dot = Vector3.Dot(yA, yB);
            //Debug.Log(name + " " + dot);
            if (dot < 0.7f) { return; }

            RaycastHit hitInfo;
            Transform origin = transform.FindChild("RayCastTarget");
            Transform rayTarget = target.FindChild("RayCastTarget");

            //Physics.DefaultRaycastLayers

            if (Physics.Raycast(origin.position, rayTarget.position - origin.position, out hitInfo, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Trigger"))))
            {
                if (hitInfo.transform.GetComponent<PlayerController>() != null)
                {
                    FirstEncounterSoundTrigger();
                }
            }
        }
    }

    public void FirstEncounterSoundTrigger()
    {
        if (encountered[(int)type]) { return; }


        Debug.Log(type.ToString() + " " + (int)type);
        encountered[(int)type] = true;
        PlayerPrefs.SetInt(type.ToString() +"/encountered/", 1);
        PlayerPrefs.Save();
        GameManager.instance.narrator.PlayUniqueEncounter(type);
    }

    public bool CheckLineOfSight()
    {
        RaycastHit hitInfo;

        Transform losOrigin = transform.FindChild("RayCastTarget");
        Transform losTarget = target.transform.FindChild("RayCastTarget");
        Vector3 rayCastDirection = (losTarget.position - losOrigin.position);

        
        Debug.DrawRay(losOrigin.position, rayCastDirection, Color.magenta);
        if (Physics.Raycast(losOrigin.position, rayCastDirection, out hitInfo, Mathf.Infinity, ~(1<<LayerMask.NameToLayer("Trigger"))))
        {
            if (hitInfo.collider.gameObject.GetComponent<PlayerController>())
            {
                return true;
            }
        }

        return false;
    }
}