using UnityEngine;
using System.Collections;

public class Attribute : MonoBehaviour {
	[HideInInspector]
	public float health;

	public float defence;
	public float defenceFire;
	public float defenceWater;
	public float defenceIce;
	public float defenceLighting;
	[HideInInspector]
	public bool burn;
	[HideInInspector]
	public bool wet;
	[HideInInspector]
	public bool freeze;
	[HideInInspector]
	public bool froze;

	[HideInInspector]
	public float delayBurnHeal;
	[HideInInspector]
	public float delayBurn;
	[HideInInspector]
	public float delayWetHeal;
	[HideInInspector]
	public float delayFreezeHeal;
	[HideInInspector]
	public float delayFrozeHeal;


	[HideInInspector]
	public float weaponDefence;
	[HideInInspector]
	public float balance;
	[HideInInspector]
	public float stamina;

	public float staminaRegen;

	[HideInInspector]
	public bool hit;
	[HideInInspector]
	public float hitSrength;
	[HideInInspector]
	public bool block;
	[HideInInspector]
	public bool dead;
	[HideInInspector]
	public bool dodge;
	[HideInInspector]
	public float blockForce;
	[HideInInspector]
	public float blockDelay;
	[HideInInspector]
	public float hitDir;
	[HideInInspector]
	public float hitDamage;

	// Use this for initialization
	void Start () {
	  
	}
	
	// Update is called once per frame
	void Update () 
	{
	  
	}
	public void applyDamage(float damage,float damageFire,float damageWater,float damageIce,float damageLighting,Vector3 pos,float strength)
	{
	
		Vector3 dir = (pos-transform.position).normalized;  
		hitDir = Mathf.Sign (Vector3.Dot (transform.forward, dir));
		int chanceBurn=Random.Range(((int)damageFire*100-((int)damageFire*(int)defenceFire))/100,(int)damageFire+1);
		int chanceWet=Random.Range(((int)damageWater*100-((int)damageWater*(int)defenceWater))/100,(int)damageWater+1);
		int chanceFreeze=Random.Range(((int)damageIce*100-((int)damageIce*(int)defenceIce))/100,(int)damageIce+1);
		if(defenceFire==100){chanceBurn=0;}
		if(defenceWater==100){chanceWet=0;}
		if(defenceIce==100){chanceFreeze=0;}
		if(chanceBurn==(int)damageFire&&damageFire!=0){burn=true;delayBurnHeal=1;delayBurn=0;delayWetHeal=0;delayFreezeHeal=0;delayFrozeHeal=0;}
		if(chanceWet==(int)damageWater&&damageWater!=0){wet=true;delayWetHeal=1;delayBurnHeal=0;}
		if(chanceFreeze==(int)damageIce&&damageIce!=0){freeze=true;delayFreezeHeal=1;delayBurnHeal=0;}
		if(freeze==true&&wet==true){froze=true;delayFrozeHeal=1;freeze=true;wet=true;}



		if(dodge==false)
		{


			float allDamage=0;
			allDamage+=damage-damage*(defence/100);
			allDamage+=(damageFire-damageFire*(defenceFire/100));
			allDamage+=(damageWater-damageWater*(defenceWater/100));
			allDamage+=(damageIce-damageIce*(defenceIce/100));
			if(wet==true){allDamage+=(damageLighting-damageLighting*(defenceLighting/100))*2;}
			else{allDamage+=(damageLighting-damageLighting*(defenceLighting/100));}
			health -= allDamage;
			hitDamage=allDamage;
			if(block==true&&stamina-strength>0&&hitDir>=0)
			{
				dir = pos-transform.position; 
				//hit = true;
				stamina-=strength;

				hitDamage=(damage-(damage*weaponDefence))-(damage*defence);
			}
			else
			{
				dir = (pos-transform.position).normalized;              
				dir.y = 0;
				//transform.rotation = Quaternion.FromToRotation (transform.forward, dir);
				if(hitDir>=1)
				{
					//transform.forward = dir;      
				}
				else
				{
					//transform.forward = -dir;
				}

				if(froze==false)
				{
					transform.forward = dir;  
				}
				hit = true;
				hitSrength = strength;
			}
			if(health<=0)
			{  
				dead=true;
				//Destroy(gameObject);        
			}
			//Debug.Log(health);
		}
	}
}
