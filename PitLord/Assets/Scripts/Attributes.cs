using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Attributes : MonoBehaviour {

    //[HideInInspector]
    public GameObject spawnPoint;

    public int currentHealth;
    public int maxHealth;

    //[HideInInspector]
    public int currentStamina;
    public int maxStamina;

    public List<AudioClip> onHit;
    public List<AudioClip> onDeath;

    public float leashingRange;
    public float detectionRange;
    public float combatRange;

    [HideInInspector]
    public int heals;
    public int maxHeals;
    public int healAmount;

    //[HideInInspector]
    public bool block;

    public bool deactivate;

    public void ApplyDamage(int damage, GameObject source)
    {
        //Debug.LogWarning(damage);
        Vector3 dir = (source.transform.position - transform.position).normalized;
        int facing = (int)Mathf.Clamp01(Mathf.Sign(Vector3.Dot(transform.forward, dir)));

        if(block)
        {
            currentStamina -= damage * facing;
            currentHealth -= damage * (1-facing);
        }
        else
        {
            currentHealth -= damage;
        }
        
        if (currentHealth <= 0)
        {
            Kill(source);
            //Destroy(gameObject);
        }
        else
        {
            SetAnimTrigger("Hit");
            DisableHitbox();
        }

        if (currentStamina <= 0)
        {
            block = false;
        }
    }

    public void SetAnimTrigger(string anim)
    {
        Animator ani = gameObject.transform.FindChild("Model").GetComponent<Animator>();

        ani.SetTrigger(anim);
    }

    public void Kill(GameObject source)
    {
        DisableHitbox();
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Attributes>().enabled = false;
        block = false;

        if (source.tag == "Player")
        {
            source.GetComponent<PlayerController>().lockOn = false;
        }

        SetAnimTrigger("Death");
    }

    public void DisableHitbox()
    {
        if (tag == "Player")
        {
            GetComponent<PlayerController>().meleeWeapon.GetComponent<BoxCollider>().enabled = false;
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

        GetComponent<CharacterController>().enabled = true;
        GetComponent<Attributes>().enabled = true;

        Animator ani = gameObject.transform.FindChild("Model").GetComponent<Animator>();
        ani.SetTrigger("Reset");
    }

    public void RegisterObject()
    {
        if (tag == "Enemy")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().AddEnemy(gameObject);
        }
    }
}
