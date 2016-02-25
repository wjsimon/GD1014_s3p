using UnityEngine;
using System.Collections;

public class BossShockwave : MonoBehaviour {

    Boss boss;

    float speed = 5;
    float turnSpeed;

    float fireRate;
    float fireTimer;

    float lifeTime;
    const float particleDur = 0.5f;

    bool active;

	// Use this for initialization
	void Start () 
    {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        active = true;

        Vector3 spawnPos = boss.transform.FindChildRecursive("WeaponTarget").position;
        spawnPos.y = boss.target.transform.position.y;

        transform.position = spawnPos;
        transform.LookAt(boss.target);

        float dist = Vector3.Distance(transform.position, boss.target.transform.position);
        lifeTime = dist / speed + 0.15f;

        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
        if (!active) { return; }

        Move();

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0 && lifeTime > particleDur) //.5f = ParticleEffect Duration
        {
            Fire();
        }
	}

    public void Fire()
    {
        fireTimer = fireRate;

        ColliderList list = GetComponent<ColliderList>();

        GetComponent<ParticleSystem>().Play();

        if(list.colList.Contains(boss.target.GetComponent<Collider>()))
        {
            boss.target.GetComponent<PlayerController>().ApplyDamage(2, 2, boss);
            Destroy(gameObject, particleDur);
            active = false;
        }
    }

    private void Move()
    {
        Vector3 move = transform.forward * (speed * Time.deltaTime);

        //Homing
        Quaternion oldRot = transform.rotation;
        transform.LookAt(boss.target);
        Quaternion newRot = transform.rotation;

        transform.rotation = Quaternion.Slerp(oldRot, newRot, 0.025f);

        transform.localPosition += move;
    }
}
