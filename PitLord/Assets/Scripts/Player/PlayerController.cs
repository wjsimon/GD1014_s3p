using UnityEngine;
using System.Collections;

public class PlayerController : Attributes
{

    CharacterController cc;
    [HideInInspector]
    public Transform lockOnTarget;

    public float walkSpeed = 4;
    public float runSpeed = 8;

    float fallSpeed = 0;
    float gravity = 20;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        lockOnTarget = GameObject.Find("TestEnemy").transform;

        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        MovementUpdate();
    }


    void MovementUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 move = forward * v + right * h;
        move *= walkSpeed;

        if (move.magnitude > 0.01)
        {
            transform.LookAt(transform.position + move);
            //Debug.Log(move.magnitude);
        }
        if (lockOnTarget != null)
        {
            transform.LookAt(lockOnTarget.position);
        }

        if (cc.isGrounded)
        {
            fallSpeed = 0;
        }
        else
        {
            fallSpeed += gravity * Time.deltaTime;
            move.y -= fallSpeed;
        }

        if (move.magnitude > 0.01)
        {
            cc.Move(move * Time.deltaTime);
        }
    }

    void CombatUpdate()
    {

    }
}
