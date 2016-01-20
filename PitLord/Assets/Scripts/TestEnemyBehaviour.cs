using UnityEngine;
using System.Collections;

public class TestEnemyBehaviour : Enemy {

	// Use this for initialization
	void Start () {
        Init();
        state = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (Vector3.Distance(target.position, transform.position) > 10)
        {
            ChangeState(0); //Idle
        }
        if (Vector3.Distance(spawnPoint.transform.position, transform.position) > 20 && state != 0)
        {
            ChangeState(2); //Retreat
        }
        if (Vector3.Distance(target.position, transform.position) < 10)
        {
            ChangeState(1); //Approach or BackOff, or Strafe. <- Need to implement random here
        }
        if (Vector3.Distance(target.position, transform.position) <= agent.stoppingDistance)
        {
            ChangeState(5); //Attack
        }

        //Im a pro.
        if (state != 5)
        {
            Behaviour(state);
        }
        else if (state == 5)
        {
            Attack();
        }
	}

    void Attack()
    {

    }
}
