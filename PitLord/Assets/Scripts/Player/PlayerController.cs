using UnityEngine;
using System.Collections;

public class PlayerController : Character
{
    [HideInInspector]
    public Transform lockOnTarget;
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
        //Still a problem with the speed getting affected by camera angle??
        if (inRoll() || inHeal())
        {
            animator.SetBool("Block", blocking);
            return;
        }

        if (inAttack())
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
            transform.LookAt(lockOnTarget.position);
            animator.SetFloat("X", Input.GetAxis("Horizontal"));
            animator.SetFloat("Y", Input.GetAxis("Vertical"));
        }

        if (cc.isGrounded)
        {
            fallSpeed = 10;
        }
        else
        {
            fallSpeed += gravity * Time.deltaTime;
            move.y -= fallSpeed;
        }

        if (move.magnitude > 0.01)
        {
            cc.Move(move * Time.deltaTime);

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

            else if (inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel)
            {
                if (StaminaCost(gameObject, "LightAttack"))
                {
                    blocking = false;
                    SprintSwitch();

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
        if (Input.GetButtonDown("Block"))
        {
            if (!inAttack() && !inRoll() && currentStamina > 0)
            {
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

    public void SetInteraction(Interaction t)
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

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healed = true;
    }
    protected override void Kill()
    {
        base.Kill();
        GameObject.Find("GameManager").GetComponent<GameManager>().GameOver();
    }
    protected void SprintSwitch()
    {
        running = !running;
    }

    protected override bool StaminaCost(GameObject source, string action)
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
