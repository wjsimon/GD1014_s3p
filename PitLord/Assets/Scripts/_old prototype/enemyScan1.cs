using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class enemyScan1 : MonoBehaviour {

	public GameObject myControler;
	public List<GameObject>friendList;
	public bool cluster;
	float checkDelay;
	bool spottedPlayer;
	// Use this for initialization
	void Start () 
	{
		cluster = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(checkDelay>0)
		{
			checkDelay-=Time.deltaTime;
		}
		else
		{
			checkDelay=8;
			if(friendList.Count>0&&spottedPlayer==true)
			{
				checkList();
			}
		}
	}
	void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.tag=="Player")
		{
			myControler.GetComponent<enemyControler>().target=c.gameObject.transform;                    
			//myControler.GetComponent<enemyControler>().switchState("attack"); 
			spottedPlayer=true;
			if(friendList.Count>0&&spottedPlayer==true)
			{
				friendAgro(c.gameObject.transform);
			}

		}
		if(c.gameObject.tag=="enemy")
		{
			friendList.Add(c.gameObject);   
			checkList();
		}
		if(c.gameObject.tag=="targetSystem")
		{
			myControler.GetComponent<enemyControler>().playerTargetSystem=c.gameObject;
		}
	}
	void OnTriggerExit(Collider c)
	{
		if(c.gameObject.tag=="enemy")
		{
			friendList.Remove(c.gameObject);      
		}
	}

	void checkList()
	{
		cluster = false;
		foreach(GameObject friends in friendList )
		{
			if(Vector3.Distance(transform.position,friends.transform.position)<3.5f)
			{
				cluster=true;
			}
			if(cluster==true)break;
		}
		if(cluster==true)
		{
			myControler.GetComponent<enemyControler>().chanceAttack=2;
			myControler.GetComponent<enemyControler>().chanceCharge=1;
			myControler.GetComponent<enemyControler>().chanceStrafe=4;
			myControler.GetComponent<enemyControler>().chanceWait=0;
			myControler.GetComponent<enemyControler>().chanceWalk=1;
		}
		else
		{
			myControler.GetComponent<enemyControler>().chanceAttack=3;
			myControler.GetComponent<enemyControler>().chanceCharge=2;
			myControler.GetComponent<enemyControler>().chanceStrafe=1;
			myControler.GetComponent<enemyControler>().chanceWait=1;
			myControler.GetComponent<enemyControler>().chanceWalk=2;
		}
	}
	void friendAgro(Transform target)
	{
		foreach(GameObject friends in friendList )
		{
			friends.GetComponent<enemyControler>().target=target; 
		}
	}

}
