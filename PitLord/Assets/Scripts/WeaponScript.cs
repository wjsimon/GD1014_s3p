using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour
{

    public ParticleSystem playerHit;
    public ParticleSystem enemyHit;
    public ParticleSystem envHit;

    public struct DamageWrapper
    {
        public int healthDmg;
        public int StaminaDmg;

        public DamageWrapper( int hD, int sD )
        {
            healthDmg = hD;
            StaminaDmg = sD;
        }
    }

    Vector3 lastPos;

    public DamageWrapper[] damageValues = new DamageWrapper[4];

    public Dictionary<string, DamageWrapper> attackNameToDamage = new Dictionary<string, DamageWrapper>()
    {
        {"P_ShortLight01", new DamageWrapper(4,2)},
        {"P_ShortLight02", new DamageWrapper(4,2)},
        {"P_ShortHeavy", new DamageWrapper(0,10)},
        {"P_GreatLight01", new DamageWrapper(7,5)},
        {"P_GreatLight02", new DamageWrapper(7,5)},
        {"P_GreatHeavy01", new DamageWrapper(10,10)},

        {"E_SwordCombo01", new DamageWrapper(4,3)},
        {"E_SwordCombo02", new DamageWrapper(4,3)},
        {"E_SwordCombo03", new DamageWrapper(4,3)},
        {"E_SwordHeavy01", new DamageWrapper(6,5)},

        {"E_SpearLight01", new DamageWrapper(2,2)},
        {"E_SpearLight02", new DamageWrapper(2,2)},
        {"E_SpearHeavy01", new DamageWrapper(5,5)},

        {"E_BowLight01", new DamageWrapper(2,1)},
        {"E_MageSpell01", new DamageWrapper(9,9)}, //Not in use
    };

    public Character owner;
    public Vector3 hitDirection;

    // Use this for initialization
    void Start()
    {
        owner = GetComponentInParent<Character>();
        playerHit = Resources.Load<ParticleSystem>("Particles/Prefabs/OnHit/FX_BloodHit_Player");
        enemyHit = Resources.Load<ParticleSystem>("Particles/Prefabs/OnHit/FX_BloodHit_EnemySlow");
        envHit = Resources.Load<ParticleSystem>("Particles/Prefabs/OnHit/FX_EnvHit");
    }

    // Update is called once per frame
    void Update()
    {
        lastPos = transform.position;
        //Debug.Log(owner.colliderSwitch);
    }


    void OnTriggerEnter( Collider other )
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Geometry"))
        {
            //--
        }

        Attributes enemy = other.GetComponent<Attributes>();
        if (enemy == null) { return; }
        if (enemy == owner) { return; }
        //Debug.LogError("COLLISION");
        //if ((owner.gameObject.tag == "Player" && other.gameObject.tag == "Enemy" || other.gameObject.tag == "DesObj") || (owner.gameObject.tag == "Enemy" && other.gameObject.tag == "Player"))
        {
            //Debug.LogWarning("Player hit");
            DamageWrapper wrapper;
            bool attackFound = attackNameToDamage.TryGetValue(owner.attackName, out wrapper);
            int hD = wrapper.healthDmg;
            int sD = wrapper.StaminaDmg;

            if (GameManager.instance.inventory.upgrades.Contains("rtsr"))
            {
                if (owner.GetComponent<PlayerController>() != null)
                {
                    PlayerController p = owner.GetComponent<PlayerController>();

                    if (p.currentHealth <= (p.maxHealth * 0.2f))
                    {
                        hD *= 2;
                        sD *= 2;
                    }
                }
            }

            if (attackFound)
            {
                bool hit = enemy.ApplyDamage(hD, sD, owner);

                if (hit)
                {
                    CalcHitDirection();

                    if (owner.GetComponent<PlayerController>() == null)
                    {
                        GetComponent<BoxCollider>().enabled = false;
                        owner.colliderSwitch = false;
                    }
                }
            }

            if (other.GetComponent<PlayerController>() != null)
            {
                ParticleSystem sys = GameObject.Instantiate<ParticleSystem>(playerHit);
                SpawnParticle(sys, enemy.transform.FindChildRecursive("RayCastTarget").position, hitDirection);
            }
            else if (other.GetComponent<Enemy>() != null)
            {
                if (enemy.GetComponent<Enemy>() != null && owner.GetComponent<Enemy>() != null) { return; }

                if (other.GetComponent<Enemy>().blocking)
                {
                    //envhit
                }
                else
                {
                    ParticleSystem sys = GameObject.Instantiate<ParticleSystem>(enemyHit);
                    SpawnParticle(sys, enemy.transform.FindChildRecursive("RayCastTarget").position, hitDirection);
                }
            }
            else if (other.GetComponent<Boss>() != null)
            {
                //enemy slow
            }
        }
    }
    /**/

    /*
    void OnCollisionEnter(Collision col)
    {
        Collider other = col.collider;
      
        Attributes enemy = other.gameObject.GetComponent<Attributes>();
        if (enemy == null) { return; }
        if (enemy == owner) { return; }
        Debug.Log(col.gameObject.name);

        //Debug.LogError("COLLISION");
        //if ((owner.gameObject.tag == "Player" && other.gameObject.tag == "Enemy" || other.gameObject.tag == "DesObj") || (owner.gameObject.tag == "Enemy" && other.gameObject.tag == "Player"))
        {
            //Debug.LogWarning("Player hit");
            DamageWrapper wrapper;
            bool attackFound = attackNameToDamage.TryGetValue(owner.attackName, out wrapper);
            int hD = wrapper.healthDmg;
            int sD = wrapper.StaminaDmg;

            if (GameManager.instance.inventory.upgrades.Contains("rtsr"))
            {
                if (owner.GetComponent<PlayerController>() != null)
                {
                    PlayerController p = owner.GetComponent<PlayerController>();

                    if (p.currentHealth <= (p.maxHealth * 0.2f))
                    {
                        hD *= 2;
                        sD *= 2;
                    }
                }
            }

            if (attackFound)
            {
                bool hit = enemy.ApplyDamage(hD, sD, owner);

                if (hit)
                {
                    Vector3 hitDir = CalcHitDirection();

                    if (owner.GetComponent<PlayerController>() == null)
                    {
                        GetComponent<BoxCollider>().enabled = false;
                        owner.colliderSwitch = false;
                    }
                }
            }

            if (other.GetComponent<PlayerController>() != null)
            {
                //player
            }
            else if (other.GetComponent<Enemy>() != null)
            {
                if (other.GetComponent<Enemy>().blocking)
                {
                    //envhit
                }
                else
                {
                    Debug.Log("spawn system");
                    ParticleSystem sys = GameObject.Instantiate<ParticleSystem>(enemyHit);
                    sys.transform.position = hitDirection;
                    sys.loop = false;
                    sys.Play();
                }
            }
            else if (other.GetComponent<Boss>() != null)
            {
                //enemy slow
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Geometry"))
            {
                //envHit
            }
        }
    }
    /**/

    void CalcHitDirection()
    {
        Vector3[] output = new Vector3[2];
        Debug.DrawRay(transform.position, lastPos - transform.position, Color.blue);
        hitDirection = lastPos - transform.position;
    }

    void SpawnParticle( ParticleSystem sys, Vector3 pos, Vector3 dir )
    {
        sys.transform.position = pos;
        sys.transform.forward = dir;
        sys.loop = false;
        sys.Play();
        Destroy(sys.gameObject, sys.duration);
    }
}
