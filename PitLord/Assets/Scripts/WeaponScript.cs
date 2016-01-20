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
        if ((gameObject.tag == "Player" && other.gameObject.tag == "Enemy") || (gameObject.tag == "Enemy" && other.gameObject.tag == "Player"))
        {
            /*
            Vector3 dir = (pos-transform.position).normalized;  
		    hitDir = Mathf.Sign (Vector3.Dot (transform.forward, dir));
            /**/

            Debug.LogWarning("Player hit");
            other.GetComponent<Attributes>().ApplyDamage(damage, owner);
        }
    }
}
