using UnityEngine;
using System.Collections;

public class PlayerController : Attributes
{
    CharacterController cc;
    [HideInInspector]
    public Transform lockOnTarget;
    public Animator animator;

    public float walkSpeed = 4;
    public float runSpeed = 8;

    float fallSpeed = 0;
    float gravity = 20;

    public GameObject weapon;

    bool triggerPressed;

    //ROLLING STUFF
    float rollStorage;
    public float rollDuration;
    public float rollCancel;
    public float rollDelay;
    public float rollSpeed;
    public Vector3 rollAxis;

    // Use this for initialization
    protected override void Start()
    {
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

        if (Input.GetButtonDown("Heal"))
        {
            if (heals > 0 && !(inAttack() || inRoll()))
            {
                animator.SetTrigger("Heal");
                //UseHeal(); - Triggered in Animation
            }
        }
    }


    void MovementUpdate()
    {
        //No idea how to get local direction
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);

        if (inAttack() || inRoll())
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 move = forward * v + right * h;
        move *= running ? runSpeed : walkSpeed;

        if (Input.GetAxis("Sprint") > 0)
        {
            if (!running && currentStamina > 0)
            {
                running = true;
                staminaTick *= -1;
            }
        }

        else if (Input.GetAxis("Sprint") <= 0)
        {
            if (running)
            {
                running = false;
                staminaTick *= -1;
            }
        }

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
            fallSpeed = 0;
        }
        else
        {
            fallSpeed += gravity * Time.deltaTime;
            move.y -= fallSpeed;
        }

        if (move.magnitude > 0.01)
        {
            cc.Move(move * Time.deltaTime);
        }
    }

    void CombatUpdate()
    {
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
                blocking = false;

                attackName = "LightAttack1";
                attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                attackingInv = 0;

                animator.SetTrigger("Attack");
            }
            else if (inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel)
            {
                blocking = false;

                if (attackName == "LightAttack1")
                {
                    attackName = "LightAttack2";
                }
                else if (attackName == "LightAttack2")
                {
                    attackName = "LightAttack1";
                }

                attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                attackingInv = 0;

                animator.SetTrigger("Attack");
            }

            Debug.Log(attackName);
        }

        //HeavyAttack - KEYBOARD
        if (Input.GetButtonDown("HeavyAttack") && !(inAttack() || inRoll()))
        {
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
                    attackName = "HeavyAttack1";
                    attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                    attackingInv = 0;

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
                    rollDuration = rollStorage;
                    attacking = 0;

                    //---
                    float v = Input.GetAxis("Vertical");
                    float h = Input.GetAxis("Horizontal");

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


        //does collider for weapon regardless of Input
        if (inAttack())
        {
            if ((attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart) && (attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colEnd))
            {
                weapon.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                weapon.GetComponent<BoxCollider>().enabled = false;
            }
        }

        if (!inAttack())
        {
            attackName = "";
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

    public void UseHeal()
    {
        heals -= 1;
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    protected override void DisableHitbox()
    {
        base.DisableHitbox();
        GetComponent<PlayerController>().weapon.GetComponent<BoxCollider>().enabled = false;
    }
}
