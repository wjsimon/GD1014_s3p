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
    protected float regenCounter = 0;
    protected float tickRate = 0.01f;

    public List<AudioClip> onHit;
    public List<AudioClip> onDeath;

    public float leashingRange;
    public float detectionRange;
    public float combatRange;

    protected float gravity = 9.81f;
    protected float fallSpeed = 0;
    protected bool falling;
    protected float fallHeight = 0;
    protected Vector3 offMeshPos;

    [HideInInspector]

    public bool targettable;
    public bool deactivate;

    public int weaponIndex;
    public List<GameObject> projectiles;

    //Animation Controls
    public string attackName = "default";
    public float attacking;
    public float attackingInv;
    public bool blocking;
    public bool running;

    public float iFrames;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }
    protected virtual void Update()
    {
        iFrames -= Time.deltaTime;
        StaminaRegen();
    }
    public void ApplyDamage( int damage, GameObject source )
    {
        //Debug.LogWarning(damage);
        if(iFrames > 0)
        {
            Debug.Log("INVINCIBLE");
            return;
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
            Kill(source);
        }
        else
        {
            if(!(source.tag == "Player" && gameObject.tag == "Player"))
            {
                SetAnimTrigger("Hit");
            }

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
        Animator ani = GetComponent<Animator>();

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
        Destroy(gameObject, 10.0f);
    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void DisableHitbox()
    {
        iFrames = 0.5f;
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

    public void StaminaRegen()
    {
        //Debug.Log(gameObject + " " + regenCounter);
        if(gameObject.tag == "Player")
        {
            if (gameObject.GetComponent<PlayerController>().inAttack() || gameObject.GetComponent<PlayerController>().inRoll() || gameObject.GetComponent<PlayerController>().blocking || gameObject.GetComponent<PlayerController>().running)
            {
                regenCounter = -0.1f;
            }
        }

        if(gameObject.tag == "Enemy")
        {
            if (gameObject.GetComponent<Enemy>().inAttack() || gameObject.GetComponent<Enemy>().blocking)
            {
                regenCounter = -1.5f;
            }
        }

        //need +0.01 so we can use negative Regen for sprinting
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

    public void LaunchProjectile()
    {
        //can be changed to random, can be overridden in specific enemies;
        GameObject projectile = projectiles[0];
        GameObject projectileSource = gameObject.transform.FindChild("ProjectileSource").gameObject;

        projectile.GetComponent<ProjectileScript>().source = gameObject;
        GameObject.Instantiate(projectile, projectileSource.transform.position, transform.localRotation);
    }
}
