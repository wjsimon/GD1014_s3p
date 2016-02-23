using UnityEngine;
using System.Collections;

public class DestructableObject : Attributes
{
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        RegisterObject();
    }

    // Update is called once per frame
    protected override void Update()
    {
    }

    public override bool ApplyDamage(int healthDmg, int staminaDmg, Attributes source )
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
        Destroy(gameObject);

        SetAnimTrigger("Death");
    }

    protected override void OnDestroy()
    {
        if (onDeath.Count > 0)
        {
            AudioSource player = new AudioSource();
            player.clip = onDeath[Random.Range(0, onDeath.Count)];
            player.Play();
        }

        GameManager.instance.RemoveObject(GetComponent<DestructableObject>());
    }

    protected override void RegisterObject()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().AddObject(GetComponent<DestructableObject>());
    }
}
