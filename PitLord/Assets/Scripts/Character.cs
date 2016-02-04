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

    //Animation Control - RoMo
    public float romoStartTime;
    public float romoDuration;
    public Vector3 romoDirection;

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

        iFrames -= Time.deltaTime;
        StaminaRegen();
        //DamageUpdate();
    }

    public override bool ApplyDamage(int damage, Character source)
    {
        if (iFrames > 0)
        {
            Debug.Log("INVINCIBLE");
            return false;
        }

        Vector3 dir = (source.transform.position - transform.position).normalized;
        int facing = (int)Mathf.Clamp01(Mathf.Sign(Vector3.Dot(transform.forward, dir)));

        if (blocking)
        {
            currentStamina -= damage * facing;
            currentHealth -= damage * (1 - facing);
        }
        else
        {
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Kill();
        }
        else
        {
            if (!(source.tag == "Player" && gameObject.tag == "Player"))
            {
                if (blocking == false)
                {
                    //Set BlockHit Int for hit
                    SetAnimTrigger("Hit");
                    DisableHitbox(0.5f);
                }
                if (blocking == true && currentStamina > 0)
                {
                    //Set BlockHit Int for blockhit
                    SetAnimTrigger("Hit");
                    DisableHitbox(0.1f);
                }
            }
        }

        if (currentStamina <= 0)
        {
            blocking = false;
            //BlockBreak animation;
            currentStamina = 0;
        }

        return true;
    }

    protected override void Kill()
    {
        DisableHitbox(0.5f);
        cc.enabled = false;
        enabled = false;
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

        cc.Move(romoDirection * Time.deltaTime);

        if (Time.time >= romoStartTime + romoDuration)
        {
            romoStartTime = 0;
            return;
        }
    }

    protected virtual void KnockBack()
    {
        //SetAnimTrigger("Knockback");
    }

    protected virtual void DisableHitbox( float dur )
    {
        iFrames = dur;
    }

    public override bool inAttack()
    {
        return attacking > 0;
    }

    public void LaunchProjectile()
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



}