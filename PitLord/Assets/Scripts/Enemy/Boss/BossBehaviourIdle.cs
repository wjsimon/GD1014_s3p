using UnityEngine;
using System.Collections;

public class BossBehaviourIdle : IBossBehaviour {

    public float stateTimer;
    public BossBehaviourIdle(Boss boss)
    {
        boss.animator.SetFloat("X", 0);
        boss.animator.SetFloat("Y", 0);

        stateTimer = 2.0f;
    }

    public override bool Execute()
    {
        stateTimer -= Time.deltaTime;
        return stateTimer > 0;
    }
}
