using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    Vector3 lastPos;
    public int damage;
    public Character owner;
    public Vector3 hitDirection;

	// Use this for initialization
	void Start () {
        owner = GetComponentInParent<Character>();
	}
	
	// Update is called once per frame
	void Update () {
        lastPos = transform.position;
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
            CalcHitDirection();
            enemy.ApplyDamage(damage, owner);
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
