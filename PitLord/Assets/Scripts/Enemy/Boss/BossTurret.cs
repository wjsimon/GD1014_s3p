using UnityEngine;
using System.Collections;

public class BossTurret : MonoBehaviour {

    public bool active;
    Boss boss;
    public BossProjectileScript projectile;
    public float lifeTime = 30.0f;

    float fireRate = 2.0f;
	// Use this for initialization
	void Start () {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        active = true;
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!active) { return; }

        transform.LookAt(boss.target.position);

        if (fireRate >= 0)
        {
            fireRate -= Time.deltaTime;

            if(fireRate <= 0)
            {
                fireRate = 2.0f;
                LaunchProjectile();
            }
        }
	}

    void LaunchProjectile()
    {
        GameObject.Instantiate(projectile, transform.position, transform.rotation);
    }
}
