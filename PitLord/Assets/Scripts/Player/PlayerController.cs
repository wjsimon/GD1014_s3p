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
    float gravity = 9.81f;

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

        if (inRoll())
        {
            return;
        }

        if (inAttack())
        {
            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart)
            {
                return;
            }
            else
            {
                //math magics
                float y = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;

                //Cus' inputs can be 0;
                if (y == 0)
                {
                    y = transform.localEulerAngles.y;
                }

                //thanks, unity answers
                //transform.eulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);

                //transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * 90);
                //transform.Rotate(0, Input.GetAxis("Horizontal") * 180 * Time.deltaTime, 0);
                return;      
            }
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
                SprintSwitch();
            }
        }

        else if (Input.GetAxis("Sprint") <= 0)
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
                SprintSwitch();

                attackName = "LightAttack1";
                attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                attackingInv = 0;

                animator.SetTrigger("Attack");
            }
            else if (inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel)
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

                attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                attackingInv = 0;

                animator.SetTrigger("Attack");
            }

            Debug.Log(attackName);
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
            regenCounter = .5f;
        }

        return pass;
    }

    protected override void DisableHitbox()
    {
        base.DisableHitbox();
        GetComponent<PlayerController>().weapon.GetComponent<BoxCollider>().enabled = false;
    }
}
