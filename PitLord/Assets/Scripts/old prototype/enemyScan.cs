using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class enemyScan : MonoBehaviour {

	public GameObject myControler;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.tag=="Player")
		{
			myControler.GetComponent<enemy1>().target=c.gameObject.transform;
		}
		if(c.gameObject.tag=="targetSystem")
		{
			myControler.GetComponent<enemy1>().playerTargetSystem=c.gameObject;
		}
	
	}
	void OnTriggerExit(Collider c)
	{

	}


}
