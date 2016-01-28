using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Attributes : MonoBehaviour
{
    //[HideInInspector]
    public GameObject spawnPoint;

    public int currentHealth;
    public int maxHealth = 10;

    //[HideInInspector]
    public float currentStamina;
    public float maxStamina = 10;

    public float staminaTick = 3.0f;
    float regenCounter = 0;
    float tickRate = 0.01f;

    public List<AudioClip> onHit;
    public List<AudioClip> onDeath;

    public float leashingRange;
    public float detectionRange;
    public float combatRange;

    [HideInInspector]
    public int heals;
    public int maxHeals = 5;
    public int healAmount = 10;

    public bool targettable;
    public bool deactivate;

    public int weaponIndex;

    //Animation Controls
    public string attackName = "default";
    public float attacking;
    public float attackingInv;
    public bool blocking;
    public bool running;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }
    protected virtual void Update()
    {
        StaminaRegen();
    }
    public void ApplyDamage( int damage, GameObject source )
    {
        //Debug.LogWarning(damage);
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
            Kill(source);
        }
        else
        {
            SetAnimTrigger("Hit");
            DisableHitbox();
        }

        if (currentStamina <= 0)
        {
            blocking = false;
            currentStamina = 0;
        }
    }

    public void SetAnimTrigger( string anim )
    {
        Animator ani = gameObject.transform.FindChild("Model").GetComponent<Animator>();

        ani.SetTrigger(anim);
    }

    public void Kill( GameObject source )
    {
        if(gameObject.tag == "DesObj")
        {
            Destroy(gameObject);
            return;
        }

        DisableHitbox();
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Attributes>().enabled = false;
        blocking = false;
        targettable = false;

        if (source.tag == "Player")
        {
            //source.GetComponent<PlayerController>().lockOn = false;
            GameObject.Find("GameManager").GetComponent<GameManager>().GameOver();
        }

        SetAnimTrigger("Death");
    }

    public void DisableHitbox()
    {
        if (tag == "Player")
        {
            GetComponent<PlayerController>().weapon.GetComponent<BoxCollider>().enabled = false;
        }
        else if (tag == "Enemy")
        {
            GetComponent<Enemy>().weapon.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void SoftReset()
    {
        //Alternative; Spawn prefab of enemy instead of reusing same - can't forget resetting values that way, may prevent bugs ?
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        transform.position = spawnPoint.transform.position;

        blocking = false;
        targettable = true;
        GetComponent<CharacterController>().enabled = true;
        GetComponent<Attributes>().enabled = true;

        Animator ani = gameObject.transform.FindChild("Model").GetComponent<Animator>();
        ani.SetTrigger("Reset");
    }

    protected virtual void RegisterObject()
    {

    }

    public bool StaminaCost( GameObject source, string action )
    {
        //I LUV H4RDC0D1NG. xoxo <3<3<3<3<3<3
        if (source.tag == "Enemy")
        {
            if (action == "Block")
            {

            }
        }

        if (source.tag == "Player")
        {
            if (action == "LightAttack")
            {
                if (currentStamina >= 2)
                {
                    currentStamina -= 2;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (action == "HeavyAttack")
            {
                if (currentStamina >= 4)
                {
                    currentStamina -= 4;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (action == "Roll")
            {
                if (currentStamina >= 3)
                {
                    currentStamina -= 3;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    public void StaminaRegen()
    {
        //Debug.Log(gameObject + " " + regenCounter);

        if(gameObject.tag == "Player")
        {
            //if (gameObject.GetComponent<PlayerController>().inAttack || gameObject.GetComponent<PlayerController>().inRoll || gameObject.GetComponent<PlayerController>().inBlock)
            {
                regenCounter = -0.5f;
            }
            if(gameObject.GetComponent<PlayerController>().running)
            {
                regenCounter = 0;
            }
        }

        if(gameObject.tag == "Enemy")
        {
            if (gameObject.GetComponent<Enemy>().isAttacking || gameObject.GetComponent<Enemy>().blocking)
            {
                regenCounter = -1.5f;
            }
        }

        //need +0.01 so we can use negative Regen for sprinting
        if (currentStamina < maxStamina + 0.01)
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

    public bool inAttack()
    {
        if(attacking > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
