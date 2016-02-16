using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : Attributes
{
    public CharacterController cc;

    public float currentStamina;
    public float maxStamina = 10;

    public float staminaTick = 3.0f;
    protected float regenCounter = 0;
    protected float tickRate = 0.01f;

    protected Vector3 offMeshPos;

    public bool targettable;
    public bool deactivate;

    public int weaponIndex;
    public List<ProjectileScript> projectiles;
    public float applyKnockback;

    //Animation Controls
    public string attackName = "default";
    public float attacking;
    public float attackingInv;
    public bool blocking;
    public bool running;
    public float stunned;

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
        base.Start();

        cc = GetComponent<CharacterController>();
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        WeaponColliderUpdate();
        RomoUpdate();
        StaminaRegen();
        iFrames -= Time.deltaTime;
        stunned -= Time.deltaTime;
        //DamageUpdate();
    }


    public override bool ApplyDamage(int healthDmg, int staminaDmg, Character source)
    {
        if (!base.ApplyDamage(healthDmg,staminaDmg, source)) { return false; }

        if (iFrames > 0)
        {
            Debug.Log("INVINCIBLE");
            return false;
        }

        Vector3 dir = (source.transform.position - transform.position).normalized;
        int facing = (int)Mathf.Clamp01(Mathf.Sign(Vector3.Dot(transform.forward, dir)));

        if (blocking)
        {
            currentStamina -= staminaDmg * facing;
            currentHealth -= healthDmg * (1 - facing);
        }
        else
        {
            currentHealth -= healthDmg;
            stunned = 0.2f;

            if(currentHealth > 0)
            {
                OnHit();
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Kill();
        }

        else
        {
            if (source != this)
            {
                if (blocking == false)
                {
                    CancelAttack();
                    //Set BlockHit Int for hit
                    if (!KnockBack(source))
                    {
                        GetComponent<Animator>().SetInteger("HitInt", 0);
                        SetAnimTrigger("Hit");
                        SetInvincibility(0.5f);
                    }
                }

                if (blocking == true && currentStamina > 0)
                {
                    //Set BlockHit Int for blockhit
                    GetComponent<Animator>().SetInteger("HitInt", 1);
                    SetAnimTrigger("Hit");
                    SetInvincibility(0.1f);
                }
            }
        }

        if (currentStamina <= 0)
        {
            //BLOCK BREAK
            blocking = false;
            GetComponent<Animator>().SetInteger("HitInt", 2);
            SetAnimTrigger("Hit");

            stunned = 1.5f;
            regenCounter = -2.5f;
            currentStamina = 0;
        }

        return true;
    }

    protected override void Kill()
    {
        base.Kill();
        SetInvincibility(0.5f);
        CancelAttack();

        cc.enabled = false;
        blocking = false;
        targettable = false;
        SetAnimTrigger("Death");
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
        if (length == 0) return;

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

        SetRomo(duration, AnimationLibrary.Get().SearchByName(attackName).romoLength);
    }

    protected virtual void CancelAttack()
    {
        if (attacking == 0) { return; }
        attackName = "default";
        attacking = 0;
        attackingInv = 0;

        romoDuration = 0;
        romoStartTime = 0;

        if (shortSword != null) { shortSword.GetComponent<BoxCollider>().enabled = false; }
        if (greatSword != null) { greatSword.GetComponent<BoxCollider>().enabled = false; }
        if (shield != null) { shield.GetComponent<BoxCollider>().enabled = false; }
    }

    protected virtual bool KnockBack(Character source)
    {
        float duration = AnimationLibrary.Get().SearchByName(source.attackName).koboDuration;
        if(duration <= 0) { return false; }

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

    protected virtual void SetInvincibility( float dur )
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

    }

    public virtual void LaunchProjectile()
    {
        //can be changed to random, can be overridden in specific enemies;
        ProjectileScript projectile = projectiles[0];
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
        cc.enabled = true;
    }

    protected virtual bool StaminaCost( GameObject source, string action )
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
        if(shortSword == null && greatSword == null && shield == null)
        {
            return;
        }

        if (inAttack())
        {
            WeaponScript weapon = shortSword;
            if(attackName == "P_ShortHeavy")
            {
                weapon = shield;
            }

            if(currentWeaponMode == WeaponMode.TWOHANDED)
            {
                weapon = greatSword;
            }

            if ((attackingInv >= AnimationLibrary.Get().SearchByName(attackName).colStart) && (attackingInv <= AnimationLibrary.Get().SearchByName(attackName).colEnd))
            {
                weapon.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                weapon.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            if (shortSword != null)shortSword.GetComponent<BoxCollider>().enabled = false;
            if (shield != null)shield.GetComponent<BoxCollider>().enabled = false;
            if (greatSword != null)greatSword.GetComponent<BoxCollider>().enabled = false;
        }
    }
}