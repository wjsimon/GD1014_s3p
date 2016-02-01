using UnityEngine;
using System.Collections;

public class BowEnemy : TestEnemyBehaviour
{
    public Transform projectileSource;
    public GameObject rayTarget;
    public GameObject projectile;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        //base.Attack();
        //Base.Attack portion, needed to modify

        if (inAttack())
        {
            return;
        }

        RaycastHit rayInfo;
        Vector3 rayCastTarget = (rayTarget.transform.position - projectileSource.transform.position).normalized;

        Debug.DrawRay(projectileSource.transform.position, rayCastTarget * 20, Color.red, 20, false);
        if (Physics.Raycast(projectileSource.transform.position, rayCastTarget, out rayInfo, 100)) //transform.position needs to be projectileSource
        {
            //Debug.Log(rayInfo.collider.gameObject.name + " " + rayInfo.collider.gameObject.tag);
            if(rayInfo.collider.gameObject.tag == "Player")
            {
                animator.SetTrigger("Attack");
                attackName = "LightAttack1";
                attacking = AnimationLibrary.Get().SearchByName(attackName).duration;
                attackingInv = 0;

                LaunchProjectile();

                ChangeState(State.IDLE);
            }
        }
    }
}
