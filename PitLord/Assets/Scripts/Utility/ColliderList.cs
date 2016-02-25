using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class ColliderList : MonoBehaviour {

    public Collider col;

    public List<Collider> colList = new List<Collider>();
	// Use this for initialization
	void Start () {
        col = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnCollisionEnter(Collision col)
    {
        OnTriggerEnter(col.collider); 
    }
    void OnCollisionExit(Collision col)
    {
        OnTriggerExit(col.collider); 
    }

    void OnTriggerEnter(Collider other)
    {
        colList.Add(other);
    }
    void OnTriggerExit(Collider other)
    {
        colList.Remove(other);
    }
}
