using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Attributes : MonoBehaviour {
    
    //[HideInInspector]
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

    public int heals;
    public int healAmount;

    //[HideInInspector]
    public bool block;
    
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
            Kill();
            //Destroy(gameObject);
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

    public void Kill()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Attributes>().enabled = false;

        SetAnimTrigger("Death");
    }
}
