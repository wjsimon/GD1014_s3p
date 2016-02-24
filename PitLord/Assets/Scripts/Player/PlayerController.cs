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

    public float walkSpeed = 4;
    public float runSpeed = 8;

    public int heals;
    public int maxHeals = 5;
    public int healAmount = 10;
    public bool healed;
    public float healTrigger;
    public float healDuration;

    public float healthRegenCounter = 0;
    public float healthTick = 1.0f;

    //ROLLING STUFF
    float rollStorage;
    public float rollDuration;
    public float rollCancel;
    public float rollDelay;
    public float rollSpeed;
    public Vector3 rollAxis;

    float switchWeaponDuration = 1.5f; //same length as Animation
    public float switchWeaponTime = 0.8f; //point in Animation where weapon models are swapped

    public Interaction interaction;
    bool triggerPressed;

    enum TargetCycle
    {
        ANY,
        LEFT,
        RIGHT,
    }

    // Use this for initialization
    protected override void Start()
    {
        //Debug
        int mode = PlayerPrefs.GetInt("WeaponMode");
        newWeaponMode = mode == 0 ? WeaponMode.ONEHANDED : WeaponMode.TWOHANDED;
        switchWeaponDuration = 0.01f;
        animator.SetFloat("MovesetId", mode);

        base.Start();

        rollDelay = -rollDelay;
        rollStorage = rollDuration;
        rollDuration = rollDelay;
        heals = maxHeals;

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
        if (Input.GetButtonDown("Heal") && !inputBlock())
        {
            if (heals > 0)
            {
                animator.SetTrigger("Heal");
                StartHeal(); //- Triggered in Animation
            }
        }

        if (Input.GetButtonDown("Switch") && !inputBlock())
        {
            SwitchWeaponMode();
        }

        UpdateWeaponMode();
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

        if (inRoll()) { return; }

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
                    ApplyDamage((int)Mathf.Round(Mathf.Abs(fallHeight)) - 4, 0, this);
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
            fallSpeed = 1;
        }
        else
        {
            fallSpeed += gravity * Time.deltaTime;
            cc.Move(fallSpeed * Vector3.down * 0.05f);
        }

        //Still a problem with the speed getting affected by camera angle??
        if (inRoll() || inHeal() || isDead() || inStun())
        {
            return;
        }

        if (inAttack())
        {
            //!(inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel)
            if (attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colStart && lockOnTarget == null)
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
            if (attackingInv >= AnimationLibrary.Get().SearchByName(attackName).cancel && (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.7 || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.7))
            {
                animator.SetBool("MoveTransition", true);
                CancelAttack();
            }
            else
            {
                animator.SetBool("MoveTransition", false);
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
        animator.SetBool("Block", blocking);

        if (inHeal() || inWeaponSwitch() || inStun() || isDead())
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
                if (currentWeaponMode == WeaponMode.ONEHANDED)
                {
                    if (StaminaCost(gameObject, "LightAttack"))
                    {
                        blocking = false;
                        SprintSwitch();

                        StartAttack("P_ShortLight01");

                        animator.SetTrigger("Attack");
                    }
                }
                else if (currentWeaponMode == WeaponMode.TWOHANDED)
                {
                    if (StaminaCost(gameObject, "LightAttack"))
                    {
                        blocking = false;
                        SprintSwitch();

                        StartAttack("P_GreatLight01");

                        animator.SetTrigger("Attack");
                    }
                }

            }
            else if (inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colEnd)
            {
                if (StaminaCost(gameObject, "LightAttack"))
                {
                    blocking = false;
                    SprintSwitch();
                    //CancelAttack();
                    if (currentWeaponMode == WeaponMode.ONEHANDED)
                    {
                        if (attackName == "P_ShortLight01")
                        {
                            attackName = "P_ShortLight02";
                        }
                        else if (attackName == "P_ShortLight02")
                        {
                            attackName = "P_ShortLight01";
                        }
                    }

                    StartAttack(attackName);

                    animator.SetTrigger("Attack");
                }
            }
            //Debug.Log(attackName);
        }

        /*
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
        /**/

        //HeavyAttack - XBOX CONTROLLER
        if (Input.GetAxis("HeavyAttack") > 0 && triggerPressed == false && !(inAttack() || inRoll()))
        {
            triggerPressed = true;
            if (!inAttack())
            {
                if (currentWeaponMode == WeaponMode.ONEHANDED)
                {
                    if (StaminaCost(gameObject, "HeavyAttack"))
                    {
                        blocking = false;
                        SprintSwitch();

                        StartAttack("P_ShortHeavy");

                        animator.SetTrigger("HeavyAttack");

                        if (inRoll())
                        {
                            rollDuration = rollStorage;
                        }
                    }
                }
                else if (currentWeaponMode == WeaponMode.TWOHANDED)
                {
                    if (StaminaCost(gameObject, "LightAttack"))
                    {
                        blocking = false;
                        SprintSwitch();

                        StartAttack("P_GreatHeavy01");

                        animator.SetTrigger("HeavyAttack");
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
            if ((rollDuration <= rollDelay) && (!inAttack() || inAttack() && attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colEnd))
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

                    //v = v <= 0 ? -1 : 1;

                    Vector3 forward = Camera.main.transform.forward;
                    Vector3 right = Camera.main.transform.right;

                    forward.y = 0;
                    right.y = 0;

                    rollAxis = forward * v + right * h;
                    if (rollAxis.magnitude <= Mathf.Epsilon)
                    {
                        rollAxis = -transform.forward;
                    }

                    rollAxis.Normalize();
                    //For looking in roll direction and stuff
                    transform.forward = rollAxis;
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
        if (inputBlock()) { return; }
        if (interaction == null)
        {
            return;
        }

        else if (Input.GetButtonDown("Interaction"))
        {
            interaction.Execute();
        }
    }

    void TargettingUpdate()
    {
        if (lockOnTarget != null && !lockOnTarget.GetComponent<Character>().targettable)
        {
            lockOnTarget = null;
            return;
        }

        if (Input.GetButtonDown("LockOn"))
        {
            if (lockOnTarget == null)
            {
                LockOnTarget(TargetCycle.ANY);
            }

            else if (lockOnTarget != null)
            {
                lockOnTarget = null;
            }
        }

        if (Input.GetButtonDown("LockOnSwitchLeft"))
        {
            LockOnTarget(TargetCycle.LEFT);
        }
        if (Input.GetButtonDown("LockOnSwitchRight"))
        {
            LockOnTarget(TargetCycle.RIGHT);
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

    void SwitchWeaponMode()
    {
        newWeaponMode = (WeaponMode)(((int)currentWeaponMode + 1) % (int)WeaponMode.COUNT);

        //Play Animation
        switchWeaponDuration = 1.5f;
        animator.SetTrigger("WeaponSwitch");

        if (newWeaponMode == WeaponMode.ONEHANDED)
        {
            animator.SetFloat("MovesetId", 0);
            PlayerPrefs.SetInt("WeaponMode", 0);
            PlayerPrefs.Save();
        }
        else if (newWeaponMode == WeaponMode.TWOHANDED)
        {
            animator.SetFloat("MovesetId", 1);
            PlayerPrefs.SetInt("WeaponMode", 1);
            PlayerPrefs.Save();
        }
    }

    void UpdateWeaponMode()
    {
        if (switchWeaponDuration <= 0) { return; }

        switchWeaponDuration -= Time.deltaTime;

        if (switchWeaponDuration <= switchWeaponTime)
        {
            currentWeaponMode = newWeaponMode;

            shortSword.gameObject.SetActive(currentWeaponMode == WeaponMode.ONEHANDED);
            shield.gameObject.SetActive(currentWeaponMode == WeaponMode.ONEHANDED);
            greatSword.gameObject.SetActive(currentWeaponMode == WeaponMode.TWOHANDED);
        }

        if (switchWeaponDuration <= 0)
        {
            switchWeaponDuration = 0;
        }
    }

    void LockOnTarget(TargetCycle dir)
    {
        CreateLockTargetList();

        Vector3 currentDir = Camera.main.transform.forward;
        if (lockOnTarget != null)
        {
            currentDir = (lockOnTarget.transform.FindChild("RayCastTarget").transform.position - Camera.main.transform.position).normalized;
        }

        Quaternion quat = new Quaternion();
        quat.SetLookRotation(currentDir, Vector3.up);
        quat = Quaternion.Inverse(quat);

        if (targetList.Count > 0)
        {
            //lockOnTarget = targetList[0].transform;
            float bestDot = -1;
            Transform bestTarget = null;
            for (int i = 0; i < targetList.Count; i++)
            {
                Vector3 yA = Camera.main.transform.forward;
                Vector3 yB = targetList[i].transform.position - Camera.main.transform.position;

                yA.y = 0;
                yB.y = 0;

                yA.Normalize();
                yB.Normalize();

                float dot = Vector3.Dot(yA, yB);

                //Vector3 localPos = quat * targetList[i].transform.position;
                Vector3 localPos = Camera.main.transform.InverseTransformPoint(targetList[i].transform.position);
                TargetCycle currentCycle = localPos.x <= 0.0 ? TargetCycle.LEFT : TargetCycle.RIGHT;
                bool match = dir == TargetCycle.ANY;

                if (!match)
                {
                    match = currentCycle == dir;
                }

                Debug.Log("best:" + bestDot + " - " + dot + " " + targetList[i].transform.gameObject.name + " " + localPos);
                if (dot > bestDot && targetList[i].transform != lockOnTarget && match)
                {
                    bestDot = dot;
                    bestTarget = targetList[i].transform;
                }
                /*
                if(Vector3.Distance(transform.position, lockOnTarget.transform.position) > Vector3.Distance(transform.position, targetList[i].transform.position))
                {
                    lockOnTarget = targetList[i].transform;
                }
                /**/
            }
            if (bestTarget != null) { lockOnTarget = bestTarget; }
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
                        if (enemy.targettable)
                        {
                            targetList.Add(enemy);
                        }
                    }
                }
            }
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
            staminaRegenCounter = -0.1f;
        }
    }

    void HealthRegen()
    {
        if (!GameManager.instance.inventory.upgrades.Contains("healthregen")) { return; }

        float ceiling = maxHealth / heals;

        if (currentHealth < ceiling)
        {
            healthRegenCounter += Time.deltaTime;

            if (healthRegenCounter >= tickRate)
            {
                healthRegenCounter = 0;
                currentHealth += healthTick * tickRate;
            }
        }

        //current can overflow by small amounts, this is just for cleanup
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
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
        healTrigger = 1.0f;
        healDuration = 2.1f;
        healed = false;

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);
    }

    public void UseHeal()
    {
        if (healed)
        {
            return;
        }

        heals -= 1;
        currentHealth += healAmount;
        //GameObject.Find("HealItemDisplay").GetComponent<HealItemDisplay>().UpdateDisplay();

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healed = true;
    }
    protected override void Kill()
    {
        PlayerPrefs.SetInt("GameManager/newSession", 0);
        PlayerPrefs.Save();

        base.Kill();
        lockOnTarget = null;
    }

    public override void SoftReset()
    {
        transform.position = spawnPoint;
        transform.rotation = Quaternion.LookRotation(Vector3.forward);

        GameManager.instance.player.heals = GameManager.instance.player.maxHeals; //PotionRefill
        base.SoftReset();
    }

    public void ApplyUpgrade(string t)
    {
        if (t == "maxhealth")
        {
            maxHealth += 5;
            currentHealth = maxHealth;
        }
        if (t == "maxstamina")
        {
            maxStamina += 10;
            currentStamina = maxStamina;
        }
        if (t == "staminaregen")
        {
            staminaTick *= 2;
            currentStamina = maxStamina;
        }
        if (t == "death")
        {
            ApplyDamage(10, 10, null);
        }
    }
    public void RemoveUpgrade(string t)
    {
        if (t == "maxhealth")
        {
            maxHealth -= 5;
            currentHealth = maxHealth;
        }
        if (t == "maxstamina")
        {
            maxStamina -= 10;
            currentStamina = maxStamina;
        }
        if (t == "staminaregen")
        {
            staminaTick /= 2;
            currentStamina = maxStamina;
        }
    }

    protected void SprintSwitch()
    {
        running = !running;
    }

    protected override bool StaminaCost(GameObject source, string action)
    {
        if(currentStamina <= 0.5f)
        {
            return HealthCost(action);
        }

        if (currentStamina <= 2)
        {
            currentStamina = 0;
            staminaRegenCounter = -1.0f;
            return false;
        }

        if (action == "LightAttack")
        {
            currentStamina -= 2;
        }
        else if (action == "HeavyAttack")
        {
            currentStamina -= 4;
        }
        else if (action == "Roll")
        {
            currentStamina -= 3;
        }
        else if (action == "Sprint")
        {
            currentStamina -= (staminaTick * tickRate) * 0.5f;
        }

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            staminaRegenCounter = 1.0f;
        }

        return true;
    }

    bool HealthCost(string action)
    {
        //Actions on less than 2 stamina cost half their cost in health;
        if(!GameManager.instance.inventory.upgrades.Contains("healthcost")) { return false; }

        currentStamina = 0;
        staminaRegenCounter = -1.0f;

        if (action == "LightAttack")
        {
            currentHealth -= 1;
        }
        else if (action == "HeavyAttack")
        {
            currentHealth -= 2;
        }
        else if (action == "Roll")
        {
            currentHealth -= 1.5f;
        }
        else if (action == "Sprint")
        {
            currentHealth -= (staminaTick * tickRate) * 0.25f;
        }

        return true;
    }

    bool inHeal()
    {
        return healDuration > 0;
    }
    bool inWeaponSwitch()
    {
        return switchWeaponDuration > 0;
    }

    public bool inputBlock()
    {
        return inHeal() || inRoll() || inAttack() || blocking || inWeaponSwitch();
    }
}
