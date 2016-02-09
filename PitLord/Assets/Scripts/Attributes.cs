using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Attributes : MonoBehaviour
{
    //KILL ME


    //[HideInInspector]
    public GameObject spawnPoint;

    public int currentHealth;
    public int maxHealth = 10;

    //[HideInInspector]
    public List<AudioClip> onHit;
    public List<AudioClip> onDeath;

    public float leashingRange;
    public float detectionRange;
    public float combatRange;

    protected float gravity = 9.81f;
    protected float fallSpeed = 10;
    protected bool falling;
    protected float fallHeight = 0;

    [HideInInspector]

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }
    protected virtual void Update()
    {
    }
    public virtual bool ApplyDamage( int damage, Character source)
    {
        bool selfIsPlayer = this is PlayerController;
        bool selfIsEnemy = this is Enemy;
        bool selfIsDesObj = this is DestructableObject;
        bool sourceIsPlayer = source is PlayerController;
        bool sourceIsEnemy = source is Enemy;

        if (selfIsPlayer && sourceIsPlayer) { return false; }
        if (selfIsEnemy && sourceIsEnemy) { return false; }
        //if (selfIsDesObj && sourceIsEnemy) { return false; }

        return true;
    }

    /*
    public void DamageUpdate()
    {
        for (int i = 0; i < buffer.Count; i++)
        {
            buffer[i].delay -= Time.deltaTime;
            Debug.Log(buffer.Count + " " + buffer[i].delay);


            if (buffer[i].delay <= 0)
            {
                ApplyDamage(buffer[i].damage, buffer[i].source, 0);
                buffer.Remove(buffer[i]);
            }
        }
    }
    /**/

    public void SetAnimTrigger( string anim )
    {
        Animator ani = GetComponent<Animator>();
        ani.SetTrigger(anim);
    }

    protected virtual void Kill()
    {
    }

    protected virtual void OnDestroy()
    {
    }

    public virtual void SoftReset()
    {
        //Alternative; Spawn prefab of enemy instead of reusing same - can't forget resetting values that way, may prevent bugs ?
        currentHealth = maxHealth;
        transform.position = spawnPoint.transform.position;
        enabled = true;

        SetAnimTrigger("Reset");
    }

    protected virtual void RegisterObject()
    {
    }

    public virtual bool inAttack()
    {
        return false;
    }
}
