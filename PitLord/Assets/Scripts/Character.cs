using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Attributes
{
    [HideInInspector]
    public CharacterController cc;
    public Animator animator;

    protected float gravity = 9.81f;
    protected float fallSpeed = 0;
    protected bool falling;
    protected float fallHeight = 0;

    public float currentStamina;
    public float maxStamina = 10;

    public float staminaTick = 3.0f;
    protected float regenCounter = 0;
    protected float tickRate = 0.01f;

    protected Vector3 offMeshPos;

    public bool targettable;
    public bool deactivate;

    public int weaponIndex;
    public List<EnemyProjectileScript> projectiles;
    public float applyKnockback;

    //Animation Controls
    public string attackName = "default";
    public float attacking;
    public float attackingInv;
    public bool blocking;
    public bool running;
    public float stunned;

    public bool colliderSwitch;

    public WeaponScript shortSword;
    public WeaponScript shield;
    public WeaponScript greatSword;
    public WeaponMode currentWeaponMode;
    public WeaponMode newWeaponMode;
    public enum WeaponMode
    {
        ONEHANDED,
        TWOHANDED,
        COUNT,
    }

    //Animation Control - RoMo
    public float romoStartTime;
    public float romoDuration;
    public float romoDirection;

    public float iFrames;
    public List<DamageBuffer> buffer = new List<DamageBuffer>();

    // Use this for initialization
    protected override void Start()
    {
        //SetAnimTrigger("Death");
        base.Start();

        cc = GetComponent<CharacterController>();
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        WeaponColliderUpdate();
        //RomoUpdate();
        StaminaRegen();
        iFrames -= Time.deltaTime;
        stunned -= Time.deltaTime;
        //DamageUpdate();
    }


    public override bool ApplyDamage(int healthDmg, int staminaDmg, Attributes source)
    {
        if (!base.ApplyDamage(healthDmg, staminaDmg, source)) { return false; }

        if (iFrames > 0)
        {
            Debug.Log("INVINCIBLE");
            return false;
        }

        Vector3 dir = (source.transform.position - transform.position);
        dir.y = 0;
        dir.Normalize();

        transform.forward = dir;
        //int facing = (int)Mathf.Clamp01(Mathf.Sign(Vector3.Dot(transform.forward, dir)));
        int facing = Vector3.Dot(transform.forward, dir) >= 0 ? 1 : 0;

        if (blocking)
        {
            currentStamina -= staminaDmg * facing;
            currentHealth -= healthDmg * (1 - facing);

            if (GetComponent<PlayerController>() != null)
            {
                if(GetComponent<PlayerController>().currentWeaponMode == WeaponMode.TWOHANDED)
                {
                    currentHealth -= Mathf.RoundToInt(healthDmg * 0.2f);
                }
            }


            if (currentStamina <= 0)
            {
                blocking = false;
                GetComponent<Animator>().SetInteger("HitInt", 2);
                SetAnimTrigger("Hit");

                stunned = 1.5f;
                regenCounter = -5.0f;
                iFrames = 0.0f;
                currentStamina = 0;
            }
            else
            {
                GetComponent<Animator>().SetInteger("HitInt", 1);
                SetAnimTrigger("Hit");
            }
        }
        else
        {
            currentHealth -= healthDmg;

            if (currentHealth > 0)
            {
                //OnHit();
                GetComponent<Animator>().SetInteger("HitInt", 0);
                SetAnimTrigger("Hit");
                stunned = 0.9f;

                CancelAttack();
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Kill();
                return true;
            }
        }

        return true;
    }

    protected override void Kill()
    {
        base.Kill();

        if (cc != null) { cc.enabled = false; }
        blocking = false;
        targettable = false;
        //SetAnimTrigger("Death");
        SetAnimTrigger("Death");
        CancelAttack();
        //Destroy(gameObject, 10.0f);
    }

    protected virtual void RomoUpdate()
    {
        if (romoStartTime <= 0)
        {
            return;
        }

        Vector3 dir = (transform.forward * romoDirection);
        dir.y = 0;
        cc.Move(dir * Time.deltaTime);

        if (Time.time >= romoStartTime + romoDuration)
        {
            romoStartTime = 0;
            return;
        }
    }

    protected virtual void SetRomo(float duration, float length)
    {
        if (length == 0)
            return;

        romoStartTime = Time.time;
        romoDuration = duration;
        romoDirection = length;
    }

    protected virtual void StartAttack(string name)
    {
        attackName = name;
        float duration = AnimationLibrary.Get().SearchByName(attackName).duration;
        attacking = duration;
        attackingInv = 0;
        colliderSwitch = true;

        animator.SetFloat("X", 0);
        animator.SetFloat("Y", 0);


        SetRomo(duration, AnimationLibrary.Get().SearchByName(attackName).romoLength);
    }

    protected virtual void CancelAttack()
    {
        if (attacking == 0) { return; }
        attackName = "default";
        attacking = 0;
        attackingInv = 0;
        colliderSwitch = false;

        if (shortSword != null) { shortSword.GetComponent<BoxCollider>().enabled = false; }
        if (greatSword != null) { greatSword.GetComponent<BoxCollider>().enabled = false; }
        if (shield != null) { shield.GetComponent<BoxCollider>().enabled = false; }
    }

    protected virtual bool KnockBack(Character source)
    {
        float duration = AnimationLibrary.Get().SearchByName(source.attackName).koboDuration;
        if (duration <= 0) { return false; }

        Vector3 dir = (transform.position - source.transform.position);
        dir.y = 0;
        dir.Normalize();
        transform.forward = -dir;

        Debug.Log(duration + " " + AnimationLibrary.Get().SearchByName(source.attackName).koboLength);
        SetRomo(duration, -AnimationLibrary.Get().SearchByName(source.attackName).koboLength);
        stunned = duration;
        GetComponent<Animator>().SetInteger("HitInt", 3);
        SetAnimTrigger("Hit");
        return true;
    }

    protected virtual void SetInvincibility(float dur)
    {
        iFrames = dur;
    }

    public override bool inAttack()
    {
        return attacking > 0;
    }
    public virtual bool inStun()
    {
        return stunned > 0;
    }

    public virtual void OnHit()
    {
        CancelAttack();
    }

    public virtual void LaunchProjectile()
    {
        //can be changed to random, can be overridden in specific enemies;
        EnemyProjectileScript projectile = projectiles[0];
        Transform projectileSource = gameObject.transform.FindChild("ProjectileSource");

        projectile.source = this;
        GameObject.Instantiate(projectile, projectileSource.position, transform.localRotation);
    }

    public override void SoftReset()
    {
        base.SoftReset();
        //Alternative; Spawn prefab of enemy instead of reusing same - can't forget resetting values that way, may prevent bugs ?
        currentStamina = maxStamina;
        blocking = false;
        targettable = true;
        if (cc != null) { cc.enabled = true; }
    }

    protected virtual bool StaminaCost(GameObject source, string action)
    {
        bool pass = false;

        if (source.tag == "Enemy")
        {
            if (action == "Block")
            {

            }
        }

        return pass;
    }

    protected virtual void StaminaRegen()
    {
        if (currentStamina < maxStamina)
        {
            regenCounter += Time.deltaTime;

            if (regenCounter >= tickRate)
            {
                regenCounter = 0;
                currentStamina += staminaTick * tickRate;
            }
        }

        //current can overflow by small amounts, this is just for cleanup
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }
    }

    protected virtual void WeaponColliderUpdate()
    {
        if (shortSword == null && greatSword == null && shield == null)
        {
            return;
        }

        if (GetComponent<BowEnemy>() == true)
        {
            return;
        }

        if (inAttack())
        {
            WeaponScript weapon = shortSword;
            if (attackName == "P_ShortHeavy")
            {
                weapon = shield;
            }

            if (currentWeaponMode == WeaponMode.TWOHANDED)
            {
                weapon = greatSword;
            }

            if ((attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart) && (attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colEnd))
            {
                EnableWeaponCollider(weapon);
            }
            else
            {
                DisableWeaponCollider(weapon);
            }
        }
        else
        {
            if (shortSword != null)
                shortSword.GetComponent<BoxCollider>().enabled = false;
            if (shield != null)
                shield.GetComponent<BoxCollider>().enabled = false;
            if (greatSword != null)
                greatSword.GetComponent<BoxCollider>().enabled = false;
        }
    }

    protected virtual void EnableWeaponCollider(WeaponScript weapon)
    {
        if (colliderSwitch)
        {
            weapon.GetComponent<BoxCollider>().enabled = true;
            colliderSwitch = false;
        }
    }
    protected virtual void DisableWeaponCollider(WeaponScript weapon)
    {
        weapon.GetComponent<BoxCollider>().enabled = false;
    }
}