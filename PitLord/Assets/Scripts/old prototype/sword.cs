using UnityEngine;
using System.Collections;

public class sword : MonoBehaviour {


	public float damage;
	public float damageFire;
	public float damageWater;
	public float damageIce;
	public float damageLighting;
	public float force;
	public float knockback;
	public float speed;
	public float weight;
	public float defence;
	public float balance;
	public float distance;
	public int elementChance;
	public string targetTag;


	public Transform owner;
	[HideInInspector]
	public float originDamage;
	[HideInInspector]
	public float originDamageFire;
	[HideInInspector]
	public float originDamageWater;
	[HideInInspector]
	public float originDamageIce;
	[HideInInspector]
	public float originDamageLighting;
	[HideInInspector]
	public float originForce;
	// Use this for initialization
	void Start () {  
		originDamage = damage;
		originDamageFire = damageFire;
		originDamageWater = damageWater;
		originDamageIce = damageIce;
		originDamageLighting = damageLighting;
		originForce = force;
		GetComponent<BoxCollider> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider c)
	{
		int elementChance2 = Random.Range (0, elementChance);

		if(c.gameObject.tag==targetTag)
		{    
			Debug.Log (elementChance2);  
			if(elementChance2!=0)
			{

				c.gameObject.GetComponent<Attribute>().applyDamage(damage,0,0,0,0,owner.position,force); 
			}
			else
			{
				c.gameObject.GetComponent<Attribute>().applyDamage(damage,damageFire,damageWater,damageIce,damageLighting,owner.position,force); 
			}
			                                                   
		}

	}

}
