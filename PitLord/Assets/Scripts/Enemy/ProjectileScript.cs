using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

    public float speed = 10;
    public Character source;
	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (source.gameObject.tag == "Enemy" && other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Attributes>().ApplyDamage(2, 0, source);
        }
    }
}
