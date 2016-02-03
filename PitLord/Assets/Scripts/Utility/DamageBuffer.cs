using UnityEngine;
using System.Collections;

public class DamageBuffer {

    public int damage;
    public GameObject source;
    public float delay;

    public DamageBuffer(int damage, GameObject source, float delay)
    {
        this.damage = damage;
        this.source = source;
        this.delay = delay;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
