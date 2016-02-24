using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alpaca : Attributes
{
    public string upgradeName;

    void Start()
    {
        currentHealth = 1;

        if(PlayerPrefs.GetInt("Alpaca/" + upgradeName) != 0)
        {
            currentHealth = 0;
            Deactivate();
        }

        RegisterObject();
    }

    void Update()
    {

    }

    public override bool ApplyDamage( int healthDmg, int staminaDmg, Attributes source )
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

        Deactivate();
    }

    public void Deactivate()
    {
        AddUpgradeToPlayer();
        GameManager.instance.player.heals = GameManager.instance.player.maxHeals; //PotionRefill
        //SetAnimTrigger("Death");
        PlayerPrefs.SetInt("Alpaca/" + upgradeName, 1);
        PlayerPrefs.Save();

        //Temp - Switch with anim + timed destroy
        Destroy(gameObject);
    }

    public void AddUpgradeToPlayer()
    {
        if (upgradeName == "") { return; }

        GameManager.instance.inventory.AddUpgrade(upgradeName);
    }

    protected override void RegisterObject()
    {
        GameManager.instance.alpacaList.Add(this);
    }
}
