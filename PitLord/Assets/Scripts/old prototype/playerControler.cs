using UnityEngine;
using System.Collections;

public class playerControler : MonoBehaviour {






	//movement
	public float cameraSpeed;
	bool collisionAHead;
	Vector3 movement;
	public float walkSpeed=.5f;
	public float runSpeed=.5f;


	//combat
	bool castSpell;
	public Transform target;
	bool isBattleReady;
	public int charge;

	//camera
	float cameraRotationH;
	float cameraRotationSpeedH;
	float cameraRotationV; 
	float cameraRotationSpeedV;
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
	public float cameraShake=10;
	//reference
	CharacterController cc;
	public Transform cameraTarget;  
	public Transform cameraPos;
	public GameObject playerModel;
	public Transform inputeDirection;
	public Transform LeftHandPos;
	public GameObject prefabSpell;
	SkinnedMeshRenderer playerModelRenderer;

	//animation
	int walk;
	float animSpeed;
	float animRotation;
	Animator anim;
	Vector3 animDir;
	float sprint;
	AnimatorStateInfo animInfo;
	AnimatorStateInfo animInfoL1;
	AnimatorTransitionInfo animInfoTrans;
	float layer1Weight;


	float respawnDelay;
	Vector3 startPos;
	//public Transform targetRotation;
	// Use this for initialization
	void Start () 
	{
		cc =playerModel.GetComponent<CharacterController> ();
		anim = playerModel.GetComponent<Animator> ();  
		currentCameraDis = cameraDis;

		playerModelRenderer = playerModel.GetComponentInChildren <SkinnedMeshRenderer>();

		charge = 3;
		//playerModel.GetComponent<castSpell> ().charge = charge;
		anim.SetBool ("Sleep", true);
		startPos = transform.position;
	}
	
	// Update is called once per frame

	void Update () 
	{
		animInfo=anim.GetCurrentAnimatorStateInfo(0);
		if(animInfo.IsName("Reviving")==true)
		{
			anim.SetBool("Sleep",false);
		}
		if (anim.GetBool ("Dead") == false) 
		{
			if(animInfo.IsName("Reviving")==true)
			{
				anim.SetBool("Sleep",false);
			}
						respawnDelay = 2;
						//combat
	
						animInfo = anim.GetCurrentAnimatorStateInfo (0);
						animInfoL1 = anim.GetCurrentAnimatorStateInfo (1);

						if (animInfoL1.IsName ("SpellStat") == true) {
								if (animInfo.normalizedTime >= 0.5f) {

										anim.SetBool ("Spell", false);                    
				     
								}
						}
	
						if (Input.GetButtonDown ("Attack") && animInfo.IsName ("Attack1") == false && animInfo.IsName ("Attack2") == false && isBattleReady == true && charge > 0 && animInfo.IsName ("RollLockOn") == false && animInfo.IsName ("Hit") == false && animInfo.IsName ("Hit1") == false) {
								anim.SetBool ("Attack", true); 
								//charge-=1;

						}

						animInfoL1 = anim.GetCurrentAnimatorStateInfo (1);            
						if (animInfo.IsName ("Attack1") == true) {
								layer1Weight = Mathf.Lerp (layer1Weight, 0, 25 * Time.deltaTime);
								if (animInfo.normalizedTime > 0.2f) {
										if (Input.GetButtonDown ("Attack")) {
												anim.SetBool ("Attack", true);                          
										}
								} else {

										anim.SetBool ("Attack", false);                
								}

						}
						if (animInfo.IsName ("Attack2") == true) {
								layer1Weight = Mathf.Lerp (layer1Weight, 0, 25 * Time.deltaTime);
								if (animInfo.normalizedTime > 0.2f) {
										if (Input.GetButtonDown ("Attack")) {
												anim.SetBool ("Attack", true);                                              
										}
								} else {
				
										anim.SetBool ("Attack", false);                
								}
			
						}
						//Debug.Log(Input.GetAxisRaw("HeavyAttack"));
						if (Input.GetAxisRaw ("HeavyAttack") < -.2f && animInfo.IsName ("HeavyAttack1") == false && animInfo.IsName ("HeavyAttack2") == false && isBattleReady == true && charge > 0) {
								anim.SetBool ("HeavyAttack", true); 
						}

						if (animInfo.IsName ("HeavyAttack1") == true) {
								layer1Weight = Mathf.Lerp (layer1Weight, 0, 25 * Time.deltaTime);
								if (animInfo.normalizedTime > 0.2f) {
										if (Input.GetAxisRaw ("HeavyAttack") < -.2f) {
												anim.SetBool ("HeavyAttack", true);                          
										}
								} else {
				
										anim.SetBool ("HeavyAttack", false);                
								}
			
						}
						if (animInfo.IsName ("HeavyAttack2") == true) {
								layer1Weight = Mathf.Lerp (layer1Weight, 0, 25 * Time.deltaTime);
								if (animInfo.normalizedTime > 0.2f) {
										if (Input.GetAxisRaw ("HeavyAttack") < -.2f) {
												anim.SetBool ("HeavyAttack", true);                          
										}
								} else {
					
										anim.SetBool ("HeavyAttack", false);                
								}
				
						}
						if (animInfo.IsName ("Attack1") == true || animInfo.IsName ("Hit1") == true || animInfo.IsName ("fallover1") == true || animInfo.IsTag ("Roll") || animInfo.IsName ("Attack2") == true || animInfo.IsName ("Hit") == true || animInfo.IsName ("HeavyAttack1") == true) {
								layer1Weight = Mathf.Lerp (layer1Weight, 0, 8 * Time.deltaTime);
						} else {
								layer1Weight = Mathf.Lerp (layer1Weight, .8f, 8 * Time.deltaTime);    
						}
						anim.SetLayerWeight (1, layer1Weight);


						if (Input.GetButtonDown ("BattleReady")) {
								if (isBattleReady == true) {
										isBattleReady = false;

								} else {
										isBattleReady = true;
								}
								anim.SetBool ("BattleReady", isBattleReady);        
						}


						transform.position = playerModel.transform.position+playerModel.transform.right/4;
						//movement

						float h = Input.GetAxisRaw ("Horizontal");                  
						float v = Input.GetAxisRaw ("Vertical");  


						animInfo = anim.GetCurrentAnimatorStateInfo (0);
						animInfoTrans = anim.GetAnimatorTransitionInfo (0);
						//lockOn = true;  
						if (lockOn == true || isBattleReady == true) {
								//anim.SetLayerWeight(1,0);
								//anim.SetLayerWeight(1,1f);
								Quaternion cameraRot = Quaternion.Euler (0, Camera.main.transform.eulerAngles.y, 0);            
								inputeDirection.rotation = cameraRot;
								if (animInfo.IsName ("RollLockOn") == false) {
										inputeDirection.localPosition = new Vector3 (h, 0, v);
								}
								//animDir=new Vector3(inputeDirection.transform.position.x-playerModel.transform.position.x,0,inputeDirection.transform.position.z-playerModel.transform.position.z);
								//Debug.Log(inputeDirection.localPosition);
								if (animInfo.IsName ("Attack1") == true && animInfo.IsName ("Attack2") == true) {
										playerModel.transform.rotation = Quaternion.RotateTowards (playerModel.transform.rotation, cameraRot, 250 * Time.deltaTime);                    
								}
								if (animInfo.IsName ("Turn") == false && anim.GetBool ("Roll") == false && animInfo.IsName ("Attack1") == false && animInfo.IsName ("Attack2") == false && animInfo.IsName ("Hit") == false && animInfo.IsName ("Hit1") == false && animInfo.IsName ("fallover") == false && animInfo.IsName ("fallover1") == false && animInfo.IsName ("HeavyAttack1") == false && animInfo.IsName ("HeavyAttack2") == false || animInfoTrans.IsUserName ("Turn") == true) {
										playerModel.transform.rotation = Quaternion.RotateTowards (playerModel.transform.rotation, cameraRot, 500 * Time.deltaTime);
								}

								anim.SetBool ("LockOn", true);
								if (h != 0 || v != 0) {
										animInfo = anim.GetCurrentAnimatorStateInfo (0);                        
										if (animInfo.IsName ("RollLockOn") == false) {
												anim.SetFloat ("MoveX", inputeDirection.localPosition.normalized.x, .2f, Time.deltaTime);
												anim.SetFloat ("MoveY", inputeDirection.localPosition.normalized.z, .2f, Time.deltaTime);
										}

										sprint = 0;

				
										anim.SetFloat ("Speed", (.5f + sprint), .2f, Time.deltaTime);          
								} else {

										if (animInfo.IsName ("RollLockOn") == true) {
												anim.SetFloat ("MoveX", 0);
												anim.SetFloat ("MoveY", -1);
										}
										anim.SetFloat ("Speed", 0, .1f, Time.deltaTime);                   
								}
						} else {





								if (h != 0 || v != 0) {    
										collisionAHead = false;
										//Debug.DrawRay (transform.position + Vector3.up, new Vector3(inputeDirection.transform.position.x-playerModel.transform.position.x,0,inputeDirection.transform.position.z-playerModel.transform.position.z));                
				
				
										inputeDirection.localPosition = new Vector3 (h, 0, v);
										inputeDirection.rotation = Camera.main.transform.rotation;
										//Debug.Log(Quaternion.LookRotation(inputeDirection.position-cameraPos.position).eulerAngles.y-Quaternion.LookRotation(playerModel.transform.forward).eulerAngles.y);
										//Debug.Log(Mathf.Abs(Quaternion.LookRotation(inputeDirection.position-transform.position).eulerAngles.y)-Mathf.Abs( Quaternion.LookRotation(playerModel.transform.forward).eulerAngles.y));
										//anim.SetFloat("Rotation",Quaternion.LookRotation(inputeDirection.position-transform.position).eulerAngles.y-Quaternion.LookRotation(playerModel.transform.forward).eulerAngles.y);
										animDir = new Vector3 (inputeDirection.transform.position.x - playerModel.transform.position.x, 0, inputeDirection.transform.position.z - playerModel.transform.position.z);    
										targetRotation = Quaternion.LookRotation (animDir);
										Ray rayCollision = new Ray (transform.position + Vector3.up, animDir);
										if (Physics.Raycast (rayCollision, 1, cameraCollision)) {
												anim.SetFloat ("Speed", 0, .1f, Time.deltaTime);
												collisionAHead = true;      
					
										}
										if (collisionAHead == false) {
												//Debug.Log(animDir); 
												if (Mathf.Abs ((playerModel.transform.rotation.eulerAngles - targetRotation.eulerAngles).y) > 160 && Mathf.Abs ((playerModel.transform.rotation.eulerAngles - targetRotation.eulerAngles).y) < 200) {
														//if(anim.GetBool("Turn")==false)targetRotation=playerModel.transform.rotation;
														anim.SetBool ("Turn", true); 
														targetRotation = playerModel.transform.rotation;
						
												} else {
														if (anim.GetBool ("Turn") == true) {
																anim.SetBool ("Turn", false);   
														}  
														//targetRotation=playerModel.transform.rotation;}
												}
												if (Input.GetButton ("Sprint")) {
														sprint = runSpeed;                   
												} else {
														sprint = 0;            
												}                
												if (Mathf.Abs ((playerModel.transform.rotation.eulerAngles - targetRotation.eulerAngles).y) >= -10 && Mathf.Abs ((playerModel.transform.rotation.eulerAngles - targetRotation.eulerAngles).y) <= 10 || anim.GetFloat ("Speed") > 0.1f) {
														anim.SetFloat ("Speed", (walkSpeed + sprint), .2f, Time.deltaTime);      
												} else {
														anim.SetFloat ("Speed", 0, .5f, Time.deltaTime); 
												}
												animInfo = anim.GetCurrentAnimatorStateInfo (0);  
										}
				
								} else {
										anim.SetFloat ("Speed", 0, .1f, Time.deltaTime);                       
								}
			
				if (animInfo.IsName("Reviving")==false&&animInfo.IsName ("Turn") == false && anim.GetBool ("Roll") == false && animInfo.IsName ("Attack1") == false && animInfo.IsName ("Attack2") == false && animInfo.IsName ("Hit") == false && animInfo.IsName ("Hit1") == false && animInfo.IsName ("fallover") == false && animInfo.IsName ("fallover1") == false || animInfoTrans.IsUserName ("Turn") == true) {
				
										playerModel.transform.rotation = Quaternion.RotateTowards (playerModel.transform.rotation, targetRotation, 400 * Time.deltaTime);            
								}
			
								anim.SetBool ("LockOn", false);    
						}
						if (h != 0 || v != 0) {
								animInfo = anim.GetCurrentAnimatorStateInfo (0);  
								if (animInfo.IsName ("RollLockOn") == false) {
										anim.SetFloat ("MoveX", inputeDirection.localPosition.x, .2f, Time.deltaTime);
										anim.SetFloat ("MoveY", inputeDirection.localPosition.z, .2f, Time.deltaTime);
								}
		
						}

						if (Input.GetButtonDown ("Roll")) {
								anim.SetBool ("Roll", true);          
						}
						/*
		if(animInfoTrans.IsUserName("RollOut")==true)
		{
			anim.SetBool("Roll",false);
		}
		*/
						if (animInfo.IsName ("RollLockOn") == true || animInfo.IsName ("Roll") == true) {
								anim.SetBool ("Roll", false);
						}
						//Debug.Log (animInfoTrans.IsUserName ("RollOut"));
						if (cc.isGrounded == false) {
								if (movement.y > -0.2) {
										movement.y -= 2 * Time.deltaTime;
								}     
								//anim.SetBool("Fall",true);
						} else {
								movement.y = 0;
								//if(anim.GetBool("Fall")==true){anim.SetBool("Fall",false);}
						}

						cc.Move (movement);



						//camera

		   
						//cameraVec = new Vector3 (cameraX, 2, -3);
						//currentCameraDis = cameraDis;
						Ray cameraRay = new Ray (cameraTarget.position, cameraPos.position - cameraTarget.position);  
						RaycastHit cameraRayInfo;
						if (Physics.Raycast (cameraRay, out cameraRayInfo, cameraDis, cameraCollision)) {
								currentCameraDis = Vector3.Distance (cameraTarget.position, cameraRayInfo.point) - Vector3.Distance (cameraTarget.position, cameraTarget.position + (cameraPos.position - cameraTarget.position).normalized);
								currentCameraDis = Mathf.Clamp (currentCameraDis, 0.64f, cameraDis);
								if (currentCameraDis < 0.6f) {
										if (playerModelRenderer.enabled == true)
												playerModelRenderer.enabled = false;                
								} else {
										if (playerModelRenderer.enabled == false)
												playerModelRenderer.enabled = true;
								}
						} else {
								if (playerModelRenderer.enabled == false)
										playerModelRenderer.enabled = true;              
								currentCameraDis = cameraDis;                    
						}
						//Debug.DrawRay (cameraTarget.position, cameraPos.position-cameraTarget.position);
						float ch = Input.GetAxis ("CameraHorizontal");
						float cV = Input.GetAxis ("CameraVertical");  
						if (isBattleReady == false) {

								//cameraRotationH += Time.deltaTime*ch*cameraSpeed*InversCameraX ;   
								//cameraRotationV += Time.deltaTime*cV*cameraSpeed*InversCameraY ;      
								cameraRotationSpeedH = Mathf.Lerp (cameraRotationSpeedH, Time.deltaTime * ch * cameraSpeed * InversCameraX, Time.deltaTime * cameraSpeed / 2);
								cameraRotationSpeedV = Mathf.Lerp (cameraRotationSpeedV, Time.deltaTime * cV * cameraSpeed * InversCameraY, Time.deltaTime * cameraSpeed / 2);
				
								cameraRotationH += cameraRotationSpeedH;
								cameraRotationV += cameraRotationSpeedV;
								cameraRotationV = Mathf.Clamp (cameraRotationV, -40, 25);   
								cameraTarget.eulerAngles = new Vector3 (cameraRotationV + Random.Range (-cameraShake, cameraShake), cameraRotationH + Random.Range (-cameraShake, cameraShake), 0);
								cameraPos.localPosition = new Vector3 (0, currentCameraDis / 2, -currentCameraDis);
								cameraPos.LookAt (cameraTarget.position);
								Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, cameraPos.position, (cameraSmoothPos) * (1 + sprint) * Time.deltaTime);  
								Camera.main.transform.forward = Vector3.Lerp (Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime);        
						} else {
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
										cameraTarget.eulerAngles = new Vector3 (cameraRotationV + Random.Range (-cameraShake, cameraShake), cameraRotationH + Random.Range (-cameraShake, cameraShake), 0);
										cameraPos.localPosition = new Vector3 (0, currentCameraDis / 2, -currentCameraDis);      
										cameraPos.LookAt (cameraTarget.position);    
										Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, cameraPos.position, cameraSmoothPos * Time.deltaTime);  
										Camera.main.transform.forward = Vector3.Lerp (Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime);   
								}
						}


				} 
		else 
		{
			if(respawnDelay>=0)
			{
				respawnDelay-=Time.deltaTime;
			}
			else
			{
				transform.position=startPos;
				playerModel.transform.position=startPos;
				anim.SetBool("Sleep",true);
				lockOn=false;
				anim.SetBool("BattleReady",false);
				anim.SetBool("Attack",false);
				anim.SetBool("Block",false);
				anim.SetBool("Roll",false);
				anim.SetBool("Hit",false);
				anim.SetBool("HeavyAttack",false);
			}
		}
		if(cameraShake>0)
		{
			//cameraShake-=Time.deltaTime*50;
			cameraShake=Mathf.Lerp(cameraShake,0,Time.deltaTime*50);
		}
		else
		{
			cameraShake=0;
		}
		//playerModel.GetComponent<HeadLookController> ().target = Camera.main.transform.position + Camera.main.transform.forward * 5;
		//playerModel.GetComponent<HeadLookController> ().effect = layer1Weight;
	}
	
	void CastSpell()
	{
		Instantiate(prefabSpell,LeftHandPos.position,playerModel.transform.rotation);  

	}
	void OnGUI()
	{
		if(charge>0)GUI.Box (new Rect (10, 35, 20, 10), "");
		if(charge>1)GUI.Box (new Rect (35, 35, 20, 10), "");
		if(charge>2)GUI.Box (new Rect (60, 35, 20, 10), "");
	}
}
    