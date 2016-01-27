using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PlayerController player;
    public Transform CameraTarget;
    public float distance = 20;
    public float angleV;
    public float angleDiffH;

    public float attenuation = 0.09f;
    public float rotationSpeed = 80;

    public bool InverseX;
    public bool InverseY;

	// Use this for initialization
	void Start () {
        angleV = transform.rotation.eulerAngles.x;
	}
	
	// Update is called once per frame
	void Update () {

        CameraControl();
        LookAtTarget();
        FollowTarget();
	}

    void LookAtTarget()
    {
        transform.LookAt(CameraTarget);
    }
    void FollowTarget()
    {
        Vector3 moveTarget;

        moveTarget = -transform.forward;
        if (player.lockOnTarget != null)
        {
            moveTarget = player.transform.position - player.lockOnTarget.transform.position;
            angleV = 30;
        }

        moveTarget.y = 0;
        moveTarget.Normalize();
        moveTarget *= distance;

        Transform dummy = transform.GetChild(0);
        dummy.position = CameraTarget.position+moveTarget;
        dummy.RotateAround(CameraTarget.position, transform.right, angleV);
        moveTarget = dummy.position;

        transform.position = Vector3.Lerp(transform.position, moveTarget, attenuation);
    }
    void CameraControl()
    {
        float cH = Input.GetAxis("CameraH");
        float cV = Input.GetAxis("CameraV");

        angleV += -cV*Time.deltaTime*rotationSpeed * (InverseY ? -1f : 1f);
        angleV = Mathf.Clamp(angleV, 5f, 60f);

        angleDiffH += cH * Time.deltaTime * rotationSpeed * (InverseX ? -1f : 1f);
        float angleH = angleDiffH * attenuation;
        angleDiffH -= angleH;

        transform.RotateAround(CameraTarget.position, Vector3.up, angleH);
    }
}
