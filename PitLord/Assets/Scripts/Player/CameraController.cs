using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PlayerController player;
    public Transform CameraSmooth;
    public Transform CameraTarget;
    public float distance = 6;
    public float angleV;
    public float angleDiffH;

    public float attenuation = 0.09f;
    public float rotationSpeed = 80;

    public bool simpleCam = false;
    public bool InverseX;
    public bool InverseY;

	// Use this for initialization
	void Start () {
        angleV = transform.rotation.eulerAngles.x;
        CameraSmooth.transform.position = transform.position;
        CameraSmooth.transform.rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        CameraControl();
        LookAtTarget();
        FollowTarget();

        if(simpleCam)
        {
            CameraCollision_simple();
        }
        else
        {
            CameraCollision();
        }

        CameraSmoothing();
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

    void CameraCollision_simple()
    {
        RaycastHit hitInfo;
        //Debug.DrawRay(CameraTarget.position, (transform.position - CameraTarget.transform.position), Color.green);

        if (Physics.Raycast(CameraTarget.position, (transform.position - CameraTarget.transform.position), out hitInfo, distance,~(1<<LayerMask.NameToLayer("NoCameraRaycast"))))
        {
            transform.position = hitInfo.point - (transform.position - CameraTarget.transform.position).normalized * 1.0f;
            float fDist=(transform.position - CameraTarget.position).magnitude;
            if (fDist < 3.0f)
            {
                transform.position = hitInfo.point - (transform.position - CameraTarget.transform.position).normalized * fDist/3;
            }
        }
    }

    void CameraCollision()
    {
        float minDistance = distance;
        RaycastHit hitInfo;

        Vector3 camDir=(transform.position-CameraTarget.transform.position).normalized;
        Vector3 camMaxPos = CameraTarget.transform.position+camDir*distance;
        Vector3 camRight = camMaxPos + 0.3f * CameraTarget.right;
        Vector3 camLeft = camMaxPos + 0.3f * -CameraTarget.right;
        Vector3 charRight = CameraTarget.position + 0.3f * CameraTarget.right;
        Vector3 charLeft = CameraTarget.position + 0.3f * -CameraTarget.right;

        Debug.DrawRay(charRight, (camRight - CameraTarget.transform.position), Color.red);
        Debug.DrawRay(charLeft, (camLeft - CameraTarget.transform.position), Color.green);

        if (Physics.Raycast(charRight, (camRight - CameraTarget.transform.position), out hitInfo, distance, ~(1 << LayerMask.NameToLayer("Character"))))
        {
            float colDistance = (CameraTarget.position - hitInfo.point).magnitude;

            if (colDistance < minDistance)
            {
                minDistance = colDistance;
            }
        }
        if (Physics.Raycast(charLeft, (camLeft - CameraTarget.transform.position), out hitInfo, distance, ~(1 << LayerMask.NameToLayer("Character"))))
        {
            float colDistance = (CameraTarget.position - hitInfo.point).magnitude;

            if (colDistance < minDistance)
            {
                minDistance = colDistance;
            }
        }

        if(minDistance != distance)
        {
            minDistance -= 0.75f;
            if(minDistance < 0.75f)
            {
                minDistance = 0.75f;
            }

            transform.position = CameraTarget.transform.position + camDir * minDistance;
        }
    }
    void CameraSmoothing()
    {
        CameraSmooth.transform.position = Vector3.Lerp(CameraSmooth.transform.position, transform.position, 0.04f);
        CameraSmooth.transform.rotation = Quaternion.Slerp(CameraSmooth.transform.rotation, transform.rotation, 0.04f);
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
        CameraSmooth.RotateAround(CameraTarget.position, Vector3.up, angleH);
    }
}
