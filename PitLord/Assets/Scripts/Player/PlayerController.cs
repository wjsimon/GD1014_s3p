using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : Character
{
    [HideInInspector]
    public Transform lockOnTarget;
    public List<Enemy> targetList;
    public LayerMask targetLayer;
    public Animator animator;

    public float walkSpeed = 4;
    public float runSpeed = 8;

    public int heals;
    public int maxHeals = 5;
    public int healAmount = 10;
    public bool healed;
    public float healTrigger;
    public float healDuration;

    //ROLLING STUFF
    float rollStorage;
    public float rollDuration;
    public float rollCancel;
    public float rollDelay;
    public float rollSpeed;
    public Vector3 rollAxis;

    public Interaction interaction;
    bool triggerPressed;

    // Use this for initialization
    protected override void Start()
    {
        //Debug
        GameManager.instance.inventory.AddKey(new Key("test"));

        base.Start();

        rollDelay = -rollDelay;
        rollStorage = rollDuration;
        rollDuration = rollDelay;
        heals = maxHeals;

        //lockOnTarget = GameObject.Find("TestEnemy").transform;

        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        MovementUpdate();
        CombatUpdate();
        InteractionUpdate();
        FallUpdate();
        TargettingUpdate();

        //Input Update()?
        if (Input.GetButtonDown("Heal"))
        {
            if (heals > 0 && !(inAttack() || inRoll()))
            {
                if (!inHeal())
                {
                    animator.SetTrigger("Heal");
                    StartHeal(); //- Triggered in Animation
                }
            }
        }

        if (inHeal())
        {
            //Can control via AnimLibrary? - Set Name in StartHeal(), Set healTrigger = Library.start in StartHeal();
            healTrigger -= Time.deltaTime;
            healDuration -= Time.deltaTime;
            if (healTrigger <= 0)
            {
                UseHeal();
            }
        }
    }

    void FallUpdate()
    {
        if (!cc.isGrounded && !falling)
        {
            offMeshPos = transform.position;
        }

        Debug.DrawRay((transform.position + Vector3.up * 0.9f), -transform.up, Color.red);
        fallHeight = Mathf.Abs(offMeshPos.y - transform.position.y);

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), -transform.up, out hitInfo))
        {
            //Debug.Log(fallHeight);
            float mag = (hitInfo.point - (transform.position + (Vector3.up * 0.5f))).magnitude;
            if (mag >= 1.5f)
            {
                falling = true;
                animator.SetBool("Falling", falling);
            }

            else if (mag < 1.5f)
            {
                if(!falling)
                {
                    Vector3 pos = transform.position;
                    float z = hitInfo.point.z;
                    pos.z = z;
                    transform.position = pos;
                }
                if (falling)
                {
                    Debug.Log("Play Land Animation");
                }

                if (fallHeight > 5 && falling)
                {
                    ApplyDamage((int)Mathf.Round(Mathf.Abs(fallHeight)) - 4, this);
                }

                falling = false;
                animator.SetBool("Falling", falling);
                return;
            }
        }

        /*
        fallHeight = Mathf.Abs(transform.position.y + (Vector3.up * 0.5f).y - offMeshPos.y);

        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), -transform.up, out hitInfo, 20f))
        {
            if ((hitInfo.point - transform.position).magnitude <= 1.5f)
            {
                if (fallHeight >= 5 && fallHeight <= 8)
                {
                    Debug.Log("FALLANIMATION 1");
                    animator.SetBool("Falling", false);
                }
                else if (fallHeight >= 8)
                {
                    Debug.Log("FALLANIMATION 2");
                    animator.SetBool("Falling", false);
                }
            }
        }

        if (fallHeight > 1.0f)
        {
            Debug.Log(fallHeight);
            animator.SetBool("Falling", true);
        }

        if (falling)
        {
            falling = false;
            if (fallHeight > 5)
            {
                ApplyDamage((int)Mathf.Round(Mathf.Abs(fallHeight)) - 4, gameObject);
            }
        }
        /**/
    }

    void MovementUpdate()
    {
        if (cc.isGrounded)
        {
            fallSpeed = 10;
        }
        else
        {
            fallSpeed += gravity * Time.deltaTime;
            cc.Move(fallSpeed * Vector3.down);
        }

        //Still a problem with the speed getting affected by camera angle??
        if (inRoll() || inHeal())
        {
            animator.SetBool("Block", blocking);
            return;
        }

        if (inAttack() && !(inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel + 0.1f))
        {
            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart)
            {
                //return;
            }
            else
            {
                Vector3 traceDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                if (traceDir.magnitude > 0.1f)
                {
                    traceDir = Camera.main.transform.TransformDirection(traceDir);
                    traceDir.y = 0;

                    traceDir.Normalize();
                    transform.forward = Vector3.Lerp(transform.forward, traceDir, 0.07f);
                }
            }

            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //Debug.Log(h + " " + v); - Controller dead zones bugs movement

        Transform camera = Camera.main.GetComponent<CameraController>().CameraSmooth;
        Vector3 forward = camera.transform.forward.normalized;
        Vector3 right = camera.transform.right.normalized;

        forward.y = 0;
        right.y = 0;

        Vector3 move = forward * v + right * h;
        move *= running ? runSpeed : walkSpeed;

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", move.magnitude);

        if (move.magnitude > 0.01)
        {
            transform.LookAt(transform.position + move);
            //Debug.Log(move.magnitude);
        }
        if (lockOnTarget != null)
        {
            transform.LookAt(new Vector3(lockOnTarget.position.x, transform.position.y, lockOnTarget.position.z), Vector3.up);
            //transform.Rotate(new Vector3(0, 1, 0));
            animator.SetFloat("X", Input.GetAxis("Horizontal"));
            animator.SetFloat("Y", Input.GetAxis("Vertical"));
        }

        if (move.magnitude > 0.01)
        {
            cc.Move(move * Time.deltaTime);
            if (inAttack() && (v > 0.7 || h > 0.7))
            {
                animator.SetBool("MoveTransition", true);
                CancelAttack();
            }
            else
            {
                animator.SetBool("MoveTransition", false);
            }


            if (Input.GetAxis("Sprint") > 0)
            {
                if (!running && currentStamina > 0)
                {
                    SprintSwitch();
                }
            }
            if (Input.GetAxis("Sprint") <= 0)
            {
                if (running)
                {
                    SprintSwitch();
                }
            }

            if (running)
            {
                StaminaCost(gameObject, "Sprint");
                if (currentStamina <= 0)
                {
                    SprintSwitch();
                }
            }
        }
        else
        {
            if (running)
            {
                SprintSwitch();
            }
            //animator.SetBool("MoveTransition", false);
        }
    }

    void CombatUpdate()
    {
        if (inHeal())
        {
            return;
        }

        if (inAttack())
        {
            attacking -= Time.deltaTime;
            attackingInv += Time.deltaTime;
        }

        rollDuration -= Time.deltaTime;

        //LightAttack
        if (Input.GetButtonDown("LightAttack"))
        {
            if (!(inAttack() || inRoll()))
            {
                if (StaminaCost(gameObject, "LightAttack"))
                {
                    blocking = false;
                    SprintSwitch();

                    StartAttack("LightAttack1");

                    animator.SetTrigger("Attack");
                }
            }

            else if (inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart)
            {
                if (StaminaCost(gameObject, "LightAttack"))
                {
                    blocking = false;
                    SprintSwitch();
                    //CancelAttack();

                    if (attackName == "LightAttack1")
                    {
                        attackName = "LightAttack2";
                    }
                    else if (attackName == "LightAttack2")
                    {
                        attackName = "LightAttack1";
                    }

                    StartAttack(attackName);

                    animator.SetTrigger("Attack");
                }
            }
            //Debug.Log(attackName);
        }

        //HeavyAttack - KEYBOARD ---- THIS IS MISSING STAMING COST YO
        if (Input.GetButtonDown("HeavyAttack") && !(inAttack() || inRoll()))
        {
            blocking = false;
            SprintSwitch();

            attackName = "HeavyAttack1";
            attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
            attackingInv = 0;

            animator.SetTrigger("HeavyAttack");
        }

        //HeavyAttack - XBOX CONTROLLER
        if (Input.GetAxis("HeavyAttack") > 0 && triggerPressed == false && !(inAttack() || inRoll()))
        {
            triggerPressed = true;
            if (!inAttack())
            {
                if (StaminaCost(gameObject, "HeavyAttack"))
                {
                    blocking = false;
                    SprintSwitch();

                    StartAttack("HeavyAttack1");

                    animator.SetTrigger("HeavyAttack");

                    if (inRoll())
                    {
                        rollDuration = rollStorage;
                    }
                }
            }
        }
        if (Input.GetAxis("HeavyAttack") <= 0 && !inAttack())
        {
            triggerPressed = false;
        }

        //Blocking
        if (Input.GetButton("Block"))
        {
            if ((!inAttack() || inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel) && !inRoll() && currentStamina > 0)
            {
                CancelAttack();
                blocking = true;
            }
        }
        if (Input.GetButtonUp("Block"))
        {
            blocking = false;
        }

        //Rolling
        if (Input.GetButtonDown("Roll"))
        {
            if ((rollDuration <= rollDelay) && (!inAttack() || inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel))
            {
                if (StaminaCost(gameObject, "Roll"))
                {
                    blocking = false;
                    SprintSwitch();
                    CancelAttack();

                    rollDuration = rollStorage;
                    iFrames = rollDuration;
                    attacking = 0;

                    //---
                    float v = Input.GetAxis("Vertical");
                    float h = Input.GetAxis("Horizontal");

                    v = v <= 0 ? -1 : 1;

                    Vector3 forward = Camera.main.transform.forward;
                    Vector3 right = Camera.main.transform.right;

                    forward.y = 0;
                    right.y = 0;

                    rollAxis = forward * v + right * h;
                    //---
                }
            }
        }

        if (inRoll())
        {
            /*
            if (lockOn)
            {
                GetComponent<CharacterController>().Move((transform.TransformDirection(new Vector3(h, 0, v)) * speed) * Time.deltaTime * speed * rollAccel);
            }
            */

            GetComponent<CharacterController>().Move(rollAxis * Time.deltaTime * rollSpeed);
        }

        animator.SetBool("Block", blocking);
        animator.SetBool("Roll", inRoll());

        if (!inAttack())
        {
            attackName = "";
        }
    }
    void InteractionUpdate()
    {
        if (interaction == null)
        {
            return;
        }

        else if (Input.GetButtonDown("Interaction"))
        {
            interaction.Execute();
            interaction = null;
        }
    }

    void TargettingUpdate()
    {
        if (Input.GetButtonDown("LockOn"))
        {
            if (lockOnTarget == null)
            {
                LockOnTarget(false);
            }

            else if (lockOnTarget != null)
            {
                lockOnTarget = null;
            }
        }

        if (Input.GetButtonDown("LockOnSwitchLeft"))
        {
            LockOnTarget(false);
        }
        if (Input.GetButtonDown("LockOnSwitchRight"))
        {
            LockOnTarget(true);
            //lockOnTarget = targetList[]
        }


        //Debug
        RaycastHit hitInfo;
        /*
        for(int i = 0; i < GameManager.instance.enemyList.Count; i++)
        {
            Debug.DrawRay(Camera.main.transform.position, (GameManager.instance.enemyList[i].transform.FindChild("RayCastTarget").transform.position - Camera.main.transform.position), Color.cyan);
            
            if (Physics.Raycast(Camera.main.transform.position, (GameManager.instance.enemyList[i].transform.FindChild("RayCastTarget").transform.position - Camera.main.transform.position), out hitInfo, targetLayer))
            {
                //Debug.Log(hitInfo.transform.name);
            }
        }

        Vector3 yA = Camera.main.transform.forward;
        Vector3 yB = GameManager.instance.enemyList[1].transform.position - Camera.main.transform.position;

        yA.y = 0;
        yB.y = 0;

        Debug.DrawRay(Camera.main.transform.position, yA, Color.green);
        Debug.DrawRay(Camera.main.transform.position, yB, Color.green);
        /**/
        /*
        Debug.DrawRay(Camera.main.transform.position, (GameManager.instance.enemyList[1].transform.FindChild("RayCastTarget").transform.position - Camera.main.transform.position), Color.cyan);

       
        /**/
    }

    void LockOnTarget(bool right)
    {
        CreateLockTargetList();

        Vector3 currentDir = Camera.main.transform.forward;
        if(lockOnTarget != null)
        {
            currentDir = (lockOnTarget.transform.FindChild("RayCastTarget").transform.position - Camera.main.transform.position).normalized;
        }

        if (targetList.Count > 0)
        {
            //lockOnTarget = targetList[0].transform;
            float bestDot = -1;
            for (int i = 0; i < targetList.Count; i++)
            {
                Vector3 yA = Camera.main.transform.forward;
                Vector3 yB = targetList[i].transform.position - Camera.main.transform.position;

                yA.y = 0;
                yB.y = 0;

                yA.Normalize();
                yB.Normalize();

                float dot = Vector3.Dot(yA, yB);

                Debug.Log("best:" + bestDot + " - "+ dot + " " + targetList[i].transform.gameObject.name);
                if (dot > bestDot && targetList[i].transform != lockOnTarget)
                {
                    bestDot = dot;
                    lockOnTarget = targetList[i].transform;
                }
                /*
                if(Vector3.Distance(transform.position, lockOnTarget.transform.position) > Vector3.Distance(transform.position, targetList[i].transform.position))
                {
                    lockOnTarget = targetList[i].transform;
                }
                /**/
            }
            /*
            {
                if (Vector3.Dot(Camera.main.transform.forward, lockOnTarget.transform.position - Camera.main.transform.position) > Vector3.Dot(Camera.main.transform.forward, targetList[i].transform.position  - Camera.main.transform.position))
                {
                    lockOnTarget = targetList[i].transform;
                }
            }
            /**/
        }
    }

    private void CreateLockTargetList()
    {
        List<Enemy> enemies = GameManager.instance.enemyList;
        RaycastHit hitInfo;
        targetList.Clear();

        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];
            //Debug.Log((enemies[i].transform.position - transform.position).magnitude);
            if (Mathf.Abs(enemy.transform.position.y - transform.position.y) > 6)
            {
                continue;
            }

            if (Vector3.Distance(transform.position, enemy.transform.position) > 20)
            {
                continue;
            }

            if (Physics.Raycast(Camera.main.transform.position, (enemy.transform.FindChild("RayCastTarget").transform.position - Camera.main.transform.position).normalized, out hitInfo, targetLayer))
            {
                if (hitInfo.transform.GetComponent<Enemy>() != null)
                {
                    Vector3 yA = Camera.main.transform.forward;
                    Vector3 yB = enemy.transform.position - Camera.main.transform.position;

                    yA.y = 0;
                    yB.y = 0;

                    yA.Normalize();
                    yB.Normalize();

                    float dot = Vector3.Dot(yA, yB);
                    Debug.Log(dot + " " + hitInfo.transform.gameObject.name);

                    if (dot > 0.3f)
                    {
                        targetList.Add(enemy);
                    }
                }
            }
        }
    }

    public void GetTargetRight()
    {
        //NOT WORKING
        if (lockOnTarget == null) { return; }
        Transform targetStorage = lockOnTarget;
        Vector3 relativePoint;

        for(int i = 0; i < targetList.Count; i++)
        {
            relativePoint = lockOnTarget.InverseTransformPoint(targetList[i].transform.position);
            Debug.Log(relativePoint);

            if(relativePoint.x > 0.0)
            {
                continue;
            }
            else if (relativePoint.x < 0.0)
            {
                if(lockOnTarget != targetStorage)
                {
                    if (Vector3.Distance(targetStorage.position, targetList[i].transform.position) <= Vector3.Distance(targetStorage.position, lockOnTarget.position))
                    {
                        lockOnTarget = targetList[i].transform;
                    }
                }
                else
                {
                    lockOnTarget = targetList[i].transform;

                }
                return;
            }
            else
            {
                continue;
            }
        }
    }

    public void GetTargetLeft()
    {
        if (lockOnTarget == null) { return; }
    }

    public void SetInteraction( Interaction t )
    {
        interaction = t;
    }

    protected override void StaminaRegen()
    {
        base.StaminaRegen();
        if (inAttack() || inRoll() || blocking || running)
        {
            regenCounter = -0.1f;
        }
    }

    public bool inRoll()
    {
        if (rollDuration > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartHeal()
    {
        blocking = false;
        //AnimationLibrary / Variables?, prolly not worth though bud
        healTrigger = 1.5f;
        healDuration = 2.5f;
        healed = false;
    }

    public void UseHeal()
    {
        if (healed)
        {
            return;
        }

        heals -= 1;
        currentHealth += healAmount;
        GameObject.Find("HealItemDisplay").GetComponent<HealItemDisplay>().UpdateDisplay();

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healed = true;
    }
    protected override void Kill()
    {
        base.Kill();
        GameManager.instance.GameOver();
    }
    protected void SprintSwitch()
    {
        running = !running;
    }

    protected override bool StaminaCost( GameObject source, string action )
    {
        bool pass = false;

        if (action == "LightAttack")
        {
            if (currentStamina >= 2)
            {
                currentStamina -= 2;
                pass = true;
            }
            else
            {
                pass = false;
            }
        }
        if (action == "HeavyAttack")
        {
            if (currentStamina >= 4)
            {
                currentStamina -= 4;
                pass = true;
            }
            else
            {
                pass = false;
            }
        }
        if (action == "Roll")
        {
            if (currentStamina >= 3)
            {
                currentStamina -= 3;
                pass = true;
            }
            else
            {
                pass = false;
            }
        }
        if (action == "Sprint")
        {
            if (currentStamina >= 0)
            {
                currentStamina -= (staminaTick * tickRate) * 0.5f;
                pass = true;
            }
            else
            {
                pass = false;
            }
        }

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            regenCounter = -.5f;
        }

        return pass;
    }

    bool inHeal()
    {
        if (healDuration > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
