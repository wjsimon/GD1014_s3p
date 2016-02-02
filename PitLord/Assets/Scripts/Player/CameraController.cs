using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PlayerController player;
    public Transform CameraTarget;
    public float distance = 6;
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
        CameraCollision();
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
    
    void CameraCollision()
    {
        float minDistance = distance;
        RaycastHit hitInfo;

        Vector3 camDir=(transform.position-CameraTarget.transform.position).normalized;
        Vector3 camMaxPos = CameraTarget.transform.position+camDir*distance;
        Vector3 camRight = camMaxPos + 1.0f * CameraTarget.right;
        Vector3 camLeft = camMaxPos + 1.0f * -CameraTarget.right;
        Debug.DrawRay(CameraTarget.position, (camRight - CameraTarget.transform.position), Color.red);
        Debug.DrawRay(CameraTarget.position, (camLeft - CameraTarget.transform.position), Color.green);

        if (Physics.Raycast(CameraTarget.position, (camRight - CameraTarget.transform.position), out hitInfo, distance))
        {
            float colDistance = (CameraTarget.position - hitInfo.point).magnitude;

            if (colDistance < minDistance)
            {
                minDistance = colDistance;
            }
        }
        if (Physics.Raycast(CameraTarget.position, (camLeft - CameraTarget.transform.position), out hitInfo, distance))
        {
            float colDistance = (CameraTarget.position - hitInfo.point).magnitude;

            if (colDistance < minDistance)
            {
                minDistance = colDistance;
            }
        }

        if(minDistance != distance)
        {
            minDistance -= 0.5f;
            if(minDistance < 0.5f)
            {
                minDistance = 0.5f;
            }

            transform.position = CameraTarget.transform.position + camDir * minDistance;
        }
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
