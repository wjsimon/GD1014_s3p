using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour {

    public struct DamageWrapper
    {
        public int healthDmg;
        public int StaminaDmg;

        public DamageWrapper(int hD, int sD)
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
	void Start () {
        owner = GetComponentInParent<Character>();
	}
	
	// Update is called once per frame
	void Update () {
        lastPos = transform.position;
        //Debug.Log(owner.colliderSwitch);
	}

    void OnTriggerEnter(Collider other)
    {
        Attributes enemy = other.GetComponent<Attributes>();
        if(enemy == null) { return; }
        //Debug.LogError("COLLISION");
        //if ((owner.gameObject.tag == "Player" && other.gameObject.tag == "Enemy" || other.gameObject.tag == "DesObj") || (owner.gameObject.tag == "Enemy" && other.gameObject.tag == "Player"))
        {
            /*
            Vector3 dir = (pos-transform.position).normalized;  
		    hitDir = Mathf.Sign (Vector3.Dot (transform.forward, dir));
            /**/

            //Debug.LogWarning("Player hit");
            DamageWrapper wrapper;
            bool attackFound = attackNameToDamage.TryGetValue(owner.attackName, out wrapper);
            int hD = wrapper.healthDmg;
            int sD = wrapper.StaminaDmg;

            if(attackFound)
            {
                bool hit = enemy.ApplyDamage(hD, sD, owner);

                if (hit)
                {
                    CalcHitDirection();

                    if(owner.GetComponent<PlayerController>() == null)
                    {
                        GetComponent<BoxCollider>().enabled = false;
                        owner.colliderSwitch = false;
                    }
                }
            }
        }
    }

    void CalcHitDirection()
    {
        Debug.DrawRay(transform.position, lastPos - transform.position, Color.blue);
        hitDirection = lastPos - transform.position;
    }

    void SpawnParticles()
    {
        //Stuff
    }
}
