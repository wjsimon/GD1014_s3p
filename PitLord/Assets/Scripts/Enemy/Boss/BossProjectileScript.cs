using UnityEngine;
using System.Collections;

public class BossProjectileScript : MonoBehaviour {

    public float speed = 20;
    public float lifeTime = 10.0f;
    Transform target;

	// Use this for initialization
	void Start () {
        target = GameObject.Find("Player").transform.GetRayCastTarget();

        float dist = Vector3.Distance(transform.position, target.transform.position);
        lifeTime = dist / speed + 0.15f;
	}
	
	// Update is called once per frame
	void Update () {

        if (lifeTime >= 0)
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0)
            {
                StartCoroutine(DestroyTimer());
            }
        }
        else
        {
            return;
        }

        Vector3 move = transform.forward * (speed*Time.deltaTime);

        //Homing
        Quaternion oldRot = transform.rotation;
        transform.LookAt(target);
        Quaternion newRot = transform.rotation;

        transform.rotation = Quaternion.Slerp(oldRot, newRot, 0.025f);

        transform.localPosition += move;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Boss>() != null) { return; }
        if (other.GetComponent<BossTurret>() != null) { return; }

        StartCoroutine(DestroyTimer());
    }

    public IEnumerator DestroyTimer()
    {
        lifeTime = -1;
        GetComponentInChildren<Renderer>().enabled = false;
        var trail = GetComponent<Xft.XWeaponTrail>();

        trail.StopSmoothly(1.0f);

        yield return (new WaitForSeconds(1.0f));

        trail.Destruct();
        Destroy(gameObject);
    }
}
