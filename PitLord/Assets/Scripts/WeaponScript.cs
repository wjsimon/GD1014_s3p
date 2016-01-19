using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

    public int damage;
    public GameObject owner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            /*
            Vector3 dir = (pos-transform.position).normalized;  
		    hitDir = Mathf.Sign (Vector3.Dot (transform.forward, dir));
            /**/

            other.GetComponent<Attributes>().ApplyDamage(damage, owner);
        }
    }
}
