using UnityEngine;
using System.Collections;

public class EnemyProjectileScript : MonoBehaviour {

    public float speed = 10;
    public int healthDmg;
    public int staminaDmg;
    public Character source;

    bool stop;
	// Use this for initialization
	void Start () 
    {
        Destroy(gameObject, 10.0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!stop)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            other.gameObject.GetComponent<Attributes>().ApplyDamage(healthDmg, staminaDmg, source);
            if (other.GetComponent<PlayerController>().iFrames <= 0)
            {
                Destroy(gameObject);
            }
        }

        else if((other.GetComponent<Enemy>() == null) && (other.gameObject.layer != LayerMask.NameToLayer("Trigger")))
        {
            Debug.Log(other.name);
            stop = true;
            Destroy(gameObject, 0.5f);
        }
    }
}
