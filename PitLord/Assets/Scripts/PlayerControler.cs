using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour {

	//combat
	Transform target;





	//camera
	float cameraRotationH;
	float cameraRotationSpeedH;
	float cameraRotationV; 
	float cameraRotationSpeedV;
	public float cameraSpeed;
	Vector3 cameraVec;
	Quaternion targetRotation;
	public float cameraDis;
	float currentCameraDis;
	public bool lockOn=false;
	public float cameraSmoothPos=10;
	public float cameraSmoothRot=10; 
	public LayerMask cameraCollision;
	public float InversCameraX=1;
	public float InversCameraY=1;
	public Transform cameraTarget;  
	public Transform cameraPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CameraUpdate ();
	}
	void CameraUpdate()
	{
		Ray cameraRay = new Ray (cameraTarget.position, cameraPos.position - cameraTarget.position);  
		RaycastHit cameraRayInfo;
		if (Physics.Raycast (cameraRay, out cameraRayInfo, cameraDis, cameraCollision)) 
		{
			currentCameraDis = Vector3.Distance (cameraTarget.position, cameraRayInfo.point) - Vector3.Distance (cameraTarget.position, cameraTarget.position + (cameraPos.position - cameraTarget.position).normalized);
			currentCameraDis = Mathf.Clamp (currentCameraDis, 0.64f, cameraDis);

		}
		else 
		{    
			currentCameraDis = cameraDis;                    
		}
		//Debug.DrawRay (cameraTarget.position, cameraPos.position-cameraTarget.position);
		float ch = Input.GetAxis ("CameraH");
		float cV = Input.GetAxis ("CameraV");  
		if (lockOn == true && target != null) {
			//cameraRotationV += Time.deltaTime*cV*cameraSpeed ;  
			//cameraRotationV = Mathf.Clamp (cameraRotationV, -20, 0);    
			cameraRotationV = cameraTarget.eulerAngles.x;
			cameraRotationH = cameraTarget.eulerAngles.y;
			//cameraTarget.eulerAngles = new Vector3 (0, playerModel.transform.eulerAngles.y, 0);  
			cameraTarget.eulerAngles = new Vector3 (0, Quaternion.LookRotation ((target.position + Vector3.up) - Camera.main.transform.position).eulerAngles.y, 0);
			cameraPos.localPosition = new Vector3 (0, 1, -currentCameraDis * 1.4f);  
			cameraPos.LookAt (cameraTarget.position);
			Camera.main.transform.position = cameraPos.position;              
			Camera.main.transform.rotation = Quaternion.RotateTowards (Camera.main.transform.rotation, Quaternion.LookRotation ((target.position + Vector3.up) - Camera.main.transform.position), 150 * Time.deltaTime);       
			///Camera.main.transform.LookAt(target.position+Vector3.up);
			/// 
		} else {
			//cameraRotationH += Time.deltaTime*ch*cameraSpeed*InversCameraX ;                 
			//cameraRotationV += Time.deltaTime*cV*cameraSpeed*InversCameraY ; 
			cameraRotationSpeedH = Mathf.Lerp (cameraRotationSpeedH, Time.deltaTime * ch * cameraSpeed * InversCameraX, Time.deltaTime * cameraSpeed / 2);
			cameraRotationSpeedV = Mathf.Lerp (cameraRotationSpeedV, Time.deltaTime * cV * cameraSpeed * InversCameraY, Time.deltaTime * cameraSpeed / 2);
			
			cameraRotationH += cameraRotationSpeedH;
			cameraRotationV += cameraRotationSpeedV;
			
			cameraRotationV = Mathf.Clamp (cameraRotationV, -40, 25);   
			cameraTarget.eulerAngles = new Vector3 (cameraRotationV , cameraRotationH , 0);
			cameraPos.localPosition = new Vector3 (0, currentCameraDis / 2, -currentCameraDis);      
			cameraPos.LookAt (cameraTarget.position);    
			Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, cameraPos.position, cameraSmoothPos * Time.deltaTime);  
			Camera.main.transform.forward = Vector3.Lerp (Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime);   
		}

	}
}
