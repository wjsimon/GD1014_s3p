using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alpaca : Attributes
{
    // Use this for initialization
    void Start()
    {
        currentHealth = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool ApplyDamage( int healthDmg, int staminaDmg, Character source )
    {
        if (!base.ApplyDamage(healthDmg, staminaDmg, source)) { return false; }

        currentHealth -= healthDmg;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Kill();
        }

        return true;
    }

    protected override void Kill()
    {
        base.Kill();

        if (onDeath.Count > 0)
        {
            AudioSource player = new AudioSource();
            player.clip = onDeath[Random.Range(0, onDeath.Count)];
            player.Play();
        }

        //Play Sting in BGM
        //Add Upgrade to Player

        //SetAnimTrigger("Death");
        Destroy(gameObject);
    }

    protected override void RegisterObject()
    {
        GameManager.instance.alpacaList.Add(this);
    }
}
