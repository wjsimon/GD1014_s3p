using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

    public float speed = 10;
    public int healthDmg;
    public int staminaDmg;
    public Character source;
	// Use this for initialization
	void Start () 
    {
        Destroy(gameObject, 10f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (other.GetComponent<PlayerController>() != null)
        {
            other.gameObject.GetComponent<Attributes>().ApplyDamage(healthDmg, staminaDmg, source);
        }
    }
}
