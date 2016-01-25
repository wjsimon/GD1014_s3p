using UnityEngine;
using System.Collections;

public class player : Attribute {


	//States
	public float healthMax=100;
	public float balanceMax=100;
	public float staminaMax=100;
   
	public int onehandWeaponI;
	public int twohandWeaponI;

	bool haveArmor=false;

	//Movement
	float sprint;
	private float speed = 0;
	private float direction = 0;
	//private Locomotion locomotion = null;
	float layerWeight;
	bool collisionAHead;
	float fallDelayCheck;
	float ccSpeed;
	bool isSprinting;
	float sprintDelay;
	int magicType=1;


	//Combat
	bool isBattleReady;
	int weaponType=1;
	bool weaponCollision;
	float particalDelay=0;
	bool weaponSwing;

	float weapon1Dis;
	float weapon2Dis;

	public bool lockOn;
	public Transform target;

	bool magicLockOn;
	GameObject handMagic;
	//Camera
	public float cameraSpeed;
	public float cameraDis;
	float currentCameraDis;
	float currentCameraSide;
	public float cameraSmoothPos;
	public float cameraSmoothRot;
	Transform cameraPos;
	Transform cameraTarget;
	Transform inputeDir;
	float cameraRotationH;
	float cameraRotationSpeedH;
	float cameraRotationV; 
	float cameraRotationSpeedV;
	float InversCameraX=1;
	float InversCameraY=1;
	Quaternion targetRotation;
	Vector3 animDir;
	public LayerMask cameraCollision;



	//reference
	CharacterController cc;

	Animator anim;
	AnimatorStateInfo animStateLayer1;
	AnimatorStateInfo animStateLayer2;
	AnimatorStateInfo animStateLayer3;
	AnimatorTransitionInfo animTransition1;
	AnimatorTransitionInfo animTransition2;

	public GameObject playerEmptiesPrefab;
	GameObject playerEmpties;
	public GameObject rightHandPos;
	public GameObject leftHandPos;
	public GameObject oneHandWeaponPos;

	public GameObject twoHandWeaponPos;


	public GameObject prefabOneHand1;
	public GameObject prefabOneHand2;
	public GameObject prefabTwoHand1;
	public GameObject prefabTwoHand2;

	GameObject weaponOneHand;
	Transform oneHandWeaponTrail;
	GameObject weaponTwoHand;
	Transform twoHandWeaponTrail;
	float trailAlpha;

	public RectTransform playerHealthBar;
	public RectTransform playerStaminaBar;

	
	//magic
	public GameObject PrefabFireMagicFloor;
	public GameObject PrefabWaterMagicFloor;
	public GameObject PrefabIceMagicFloor;
	public GameObject PrefabLightingMagicFloor;

	public GameObject PrefabFireHand;
	public GameObject PrefabWaterHand;
	public GameObject PrefabIceHand;
	public GameObject PrefabLightingHand;




	//sound
	public GameObject soundArmor;
	public GameObject soundFootstep;
	public GameObject soundSword;
	AudioSource audioSourceArmor;
	AudioSource audioSourceFootstep;
	AudioSource audioSourceSword;
	public AudioClip footstepLeftClip;
	public AudioClip footstepRightClip;
	public AudioClip drawSwordClip;
	public AudioClip hitWallClip;
	public AudioClip swordSwingClip;
	public AudioClip hitClip;

	public GameObject prefabHitPartical;
	public GameObject prefabBloodHitPartical;

	public GameObject gamelogicGo;
	//animation


	//inventar
	bool showInventar;

	// Use this for initialization
	void Start () 
	{
		//reference
		cc =GetComponent<CharacterController> ();
		anim = GetComponent<Animator> (); 
		//locomotion = new Locomotion(anim);
		audioSourceArmor = soundArmor.GetComponent<AudioSource> ();
		audioSourceFootstep = soundFootstep.GetComponent<AudioSource> ();
		audioSourceSword = soundSword.GetComponent<AudioSource> ();

		//states
		resetStates ();
		weaponType = 1;
		anim.SetInteger ("WeaponType", 1);
		//camera setup
		playerEmpties = Instantiate (playerEmptiesPrefab, transform.position, Quaternion.identity) as GameObject;
		cameraTarget=playerEmpties.transform.FindChild("cameraTarget");
		cameraPos=cameraTarget.FindChild("cameraPos");
		inputeDir=cameraTarget.FindChild("inputeDir");

		//setup weapon
		setWeapon ();


		isBattleReady = false;
	}
	
	// Update is called once per frame
	void Update () 
	{


		//Combat
		animStateLayer1 = anim.GetCurrentAnimatorStateInfo (0);
		animTransition1 = anim.GetAnimatorTransitionInfo (0);
		bool inAttack = animStateLayer1.IsName("twoHand.HeavyAttack")==true|animStateLayer1.IsName("twoHand.HeavyAttack2")==true|animStateLayer1.IsName("twoHand.Attack")==true|animStateLayer1.IsName("twoHand.Attack2")==true|animStateLayer1.IsName("oneHand.HeavyAttack")==true|animStateLayer1.IsName("oneHand.HeavyAttack2")==true|animStateLayer1.IsName("oneHand.Attack")==true|animStateLayer1.IsName("oneHand.Attack2")==true;
		bool inAttackTransition=animTransition1.IsUserName("Attack");
		bool NotTurn=animStateLayer1.IsTag("NotTurn")|animStateLayer1.IsTag("Hit");
		bool inRoll=animStateLayer1.IsName("dodge")|animStateLayer1.IsName("Roll");
		bool PleaseTurn=animTransition1.IsUserName("PleaseTurn")|inAttackTransition;
		bool lockRollInpute=false;
		bool inMagicAttack=animStateLayer1.IsTag("Magic");
		bool inMagicAttackTransition=animTransition1.IsUserName("Magic");
		//drawWeapon
		animStateLayer2 = anim.GetCurrentAnimatorStateInfo (1);
		animTransition2 = anim.GetAnimatorTransitionInfo (1);
		animStateLayer3 = anim.GetCurrentAnimatorStateInfo (2);
	
		if(animStateLayer3.IsTag("Draw"))
		{

			if(animStateLayer3.normalizedTime>0.9f)
			{
				anim.SetBool("Draw",false);
				anim.SetBool("Back",false);

				if(anim.GetBool("SwitchWeapon")==true)
				{
					if(weaponType==1)
					{
						weaponType=2;
						//weaponDefence = weaponTwoHand.GetComponent<sword> ().defence;
					}
					else
					{
						weaponType=1;
						//weaponDefence = weaponOneHand.GetComponent<sword> ().defence;
					}
					anim.SetBool("Draw",true);
					anim.SetBool("SwitchWeapon",false);
					anim.SetInteger("WeaponType",weaponType);
				}
			}

		}


		if(Input.GetButton("Block"))
		{
			if(anim.GetBool("Block")==false)
			{
				anim.SetBool("Block",true);
				//block=true;
			}
		}
		else
		{
			if(anim.GetBool("Block")==true)
			{
				anim.SetBool("Block",false);
				//block=false;
			}

		}



		if(Input.GetButtonDown("Attack"))
		{
			if(magicLockOn==false)
			{
				if(stamina>20&&inAttack==false&&inRoll==false)
				{
					anim.SetBool("BreakAttack",false);
					if(anim.GetBool("Attack")==false)
					{
						anim.SetBool("Attack",true);
						stamina-=20;
					}
				}
			}
			else
			{
				anim.SetInteger("MagicAttack",1);
				magicLockOn=false;
			}
		}
		if (animStateLayer1.IsName("oneHand.Attack")==true||animStateLayer1.IsName("twoHand.Attack")==true||animStateLayer1.IsName("oneHand.Attack2")==true||animStateLayer1.IsName("twoHand.Attack2"))
		{
			if(animStateLayer1.normalizedTime<0.30f)
			{
				anim.SetBool ("Attack",false);
				lockRollInpute=true;
			}
			else
			{
				lockRollInpute=false;
				if(Input.GetButtonDown("Attack")&&stamina>20)
				{
					
					anim.SetBool("BreakAttack",false);
					if(anim.GetBool("Attack")==false)
					{
						anim.SetBool("Attack",true);
						stamina-=20;
					}
				}
			}
		}
		//Magic
		if(anim.GetBool("DrawMagic")==true)
		{
			anim.SetBool("DrawMagic",false);
		}
		if(magicLockOn==false)
		{
			if(Input.GetAxisRaw("HeavyAttack")>0.1f&&inAttack==false&&inRoll==false)
			{
				anim.SetBool("DrawMagic",true);
				magicLockOn=true;
			}
		}
		else
		{
			if(Input.GetAxisRaw("HeavyAttack")>0.1f&&inAttack==false&&inRoll==false)
			{
				//magicLockOn=false;
			}
		}
		if(inMagicAttack==true)
		{
			anim.SetInteger("MagicAttack",0);
		}
		if(Input.GetAxisRaw("HeavyAttack")<-0.1f&&stamina>40&&inAttack==false&&inRoll==false)
		{
			
			anim.SetBool("BreakAttack",false);
			if(anim.GetBool("HeavyAttack")==false)
			{
				anim.SetBool("HeavyAttack",true);
				stamina-=40;
			}
		}
		if (animStateLayer1.IsName("twoHand.HeavyAttack2")==true||animStateLayer1.IsName("oneHand.HeavyAttack")==true||animStateLayer1.IsName("twoHand.HeavyAttack")==true||animStateLayer1.IsName("oneHand.HeavyAttack2")==true)
		{
			if(animStateLayer1.normalizedTime<0.30f)
			{
				anim.SetBool ("HeavyAttack",false);
				lockRollInpute=true;
			}
			else
			{
				if(Input.GetAxisRaw("HeavyAttack")<-0.1f&&stamina>20)
				{
				
					if(anim.GetBool("HeavyAttack")==false)
					{
						anim.SetBool("HeavyAttack",true);
						stamina-=20;
					}
				}
				lockRollInpute=false;
			}

		}

		if(inAttack==false&&inRoll==false&&inMagicAttack==false||inAttackTransition==true&&inRoll==false||animTransition1.IsUserName("Roll")==true)
		{
			if(layerWeight<1)layerWeight =Mathf.Lerp(layerWeight,1,Time.deltaTime*10);
		}
		else
		{
			if(layerWeight>0)layerWeight =Mathf.Lerp(layerWeight,0,Time.deltaTime*10);
		}

		if(inAttack==true)
		{
			 
			RaycastHit weaponRayInfo;

			if(weaponType==1)
			{
				Ray weaponRay = new Ray (weaponOneHand.transform.position, -weaponOneHand.transform.up*1.5f); 

				if (Physics.Raycast (weaponRay, out weaponRayInfo,weapon1Dis)) 
				{
					if(weaponCollision==true&&weaponRayInfo.transform.tag=="levelGeometry")
					{
						if(anim.GetBool("BreakAttack")==false)
						{
							Instantiate(prefabHitPartical,weaponRayInfo.point,Quaternion.identity);
						}
							anim.SetBool("BreakAttack",true);
						if(weaponType==1)weaponOneHand.GetComponent<BoxCollider>().enabled=false;
						if(weaponType==2)weaponTwoHand.GetComponent<BoxCollider>().enabled=false;
					}
					if(particalDelay<=0&&weaponSwing==true)
					{
						if(weaponRayInfo.transform.tag=="levelGeometry")
						{
							Instantiate(prefabHitPartical,weaponRayInfo.point,Quaternion.identity);
							particalDelay=.1f;
						}
						if(weaponRayInfo.transform.tag=="enemy")
						{
							Instantiate(prefabBloodHitPartical,weaponRayInfo.point,Quaternion.identity);
							particalDelay=.1f;
						}
					}
				}
				Debug.DrawRay(weaponOneHand.transform.position, -weaponOneHand.transform.up*weapon1Dis);

			}
			if(weaponType==2)
			{

				Ray weaponRay = new Ray (weaponTwoHand.transform.position, -weaponTwoHand.transform.up*1.5f); 
				if (Physics.Raycast (weaponRay, out weaponRayInfo,weapon2Dis)) 
				{
					if(weaponCollision==true&&weaponRayInfo.transform.tag=="levelGeometry")
					{
						if(anim.GetBool("BreakAttack")==false)
						{
							Instantiate(prefabHitPartical,weaponRayInfo.point,Quaternion.identity);
						}
							anim.SetBool("BreakAttack",true);
						if(weaponType==1)weaponOneHand.GetComponent<BoxCollider>().enabled=false;
						if(weaponType==2)weaponTwoHand.GetComponent<BoxCollider>().enabled=false;
					}
					if(particalDelay<=0&&weaponSwing==true)
					{
						if(weaponRayInfo.transform.tag=="levelGeometry")
						{
							Instantiate(prefabHitPartical,weaponRayInfo.point,Quaternion.identity);
							particalDelay=.1f;
						}
						if(weaponRayInfo.transform.tag=="enemy")
						{
							Instantiate(prefabBloodHitPartical,weaponRayInfo.point,Quaternion.identity);
							particalDelay=.1f;
						}
					}
				}
				Debug.DrawRay(weaponTwoHand.transform.position, -weaponTwoHand.transform.up*weapon2Dis);

			}

			if(particalDelay>0)
			{
				particalDelay-=Time.deltaTime;
			}
		
			trailAlpha=Mathf.Lerp(trailAlpha,.2f,Time.deltaTime*3);
		}
		else
		{
			trailAlpha=Mathf.Lerp(trailAlpha,0,Time.deltaTime*3);
	
		}

		//switach weapon
		if(Input.GetButtonDown("SwitchWeapon")&&inAttack==false&&inRoll==false)
		{

				anim.SetBool("Back",true);
				anim.SetBool("SwitchWeapon",true);
				
		}
		if(stamina<staminaMax&&animStateLayer2.IsTag("Block")==false&&anim.GetFloat("Speed")<6.5f)
		{
			stamina+=Time.deltaTime*staminaRegen;
		}
		if(inRoll==true)
		{
			dodge=true;
		}
		else
		{
			dodge=false;
		}
		//Camera
		
		//cameraCollision
		currentCameraDis = cameraDis; 

		float ch = Input.GetAxis ("CameraH");
		float cV = Input.GetAxis ("CameraV");  

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


			
			//camera Dir
			//cameraRotationV += Time.deltaTime*cV*cameraSpeed*InversCameraY ;   

			if(lockOn==true)
			{
				cameraPos.LookAt (target.position+target.up-(target.position-transform.position)/3);
				cameraTarget.forward=new Vector3 ((target.position-transform.position).normalized.x, 0,(target.position-transform.position).normalized.z);
				Camera.main.transform.forward = Vector3.Lerp (Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime/2); 
				anim.SetBool("LockOn",true);
			}
			else
			{
				if(magicLockOn==true||block==true)
				{
					cameraRotationSpeedH = Mathf.Lerp (cameraRotationSpeedH, Time.deltaTime * ch * cameraSpeed * InversCameraX, Time.deltaTime * cameraSpeed / 2);
					cameraRotationSpeedV = Mathf.Lerp (cameraRotationSpeedV, Time.deltaTime * cV * cameraSpeed * InversCameraY, Time.deltaTime * cameraSpeed / 2);
					
					cameraRotationH += cameraRotationSpeedH;
					cameraRotationV += cameraRotationSpeedV;
					
					cameraRotationV = Mathf.Clamp (cameraRotationV, -40, 25);   
					cameraTarget.eulerAngles = new Vector3 (cameraRotationV , cameraRotationH , 0);
					
					cameraPos.LookAt (cameraTarget.position+cameraTarget.right/5);
					Camera.main.transform.forward = Vector3.Lerp (Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime); 
					anim.SetBool("LockOn",true);
				}
				else
				{
					cameraRotationSpeedH = Mathf.Lerp (cameraRotationSpeedH, Time.deltaTime * ch * cameraSpeed * InversCameraX, Time.deltaTime * cameraSpeed / 2);
					cameraRotationSpeedV = Mathf.Lerp (cameraRotationSpeedV, Time.deltaTime * cV * cameraSpeed * InversCameraY, Time.deltaTime * cameraSpeed / 2);
					
					cameraRotationH += cameraRotationSpeedH;
					cameraRotationV += cameraRotationSpeedV;
					
					cameraRotationV = Mathf.Clamp (cameraRotationV, -40, 25);   
					cameraTarget.eulerAngles = new Vector3 (cameraRotationV , cameraRotationH , 0);
					
					cameraPos.LookAt (cameraTarget.position+cameraTarget.right/5);
					Camera.main.transform.forward = Vector3.Lerp (Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime); 
					anim.SetBool("LockOn",false);
				}
			}

		//cameraPos
		cameraPos.localPosition = new Vector3 (0, currentCameraDis / 2, -currentCameraDis);
		Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, cameraPos.position, (cameraSmoothPos) * (1 + sprint) * Time.deltaTime); 

		playerEmpties.transform.position = transform.position;



		//movement
		if(Input.GetButtonUp("Roll")&&sprintDelay>0&&isSprinting==false&&lockRollInpute==false)
		{
			if(anim.GetBool("Roll")==false)
			{
				anim.SetBool("Roll",true);
				stamina-=20;
			}
		}
		if(Input.GetButton("Roll")&&animStateLayer1.IsName("Roll")==false&&animStateLayer1.IsName("dodge")==false)
		{
			if(sprintDelay>=0)
			{
				sprintDelay-=Time.deltaTime;

			}
			else
			{
				isSprinting=true;
				stamina-=Time.deltaTime*4;
			}

		}
		else
		{
			isSprinting=false;
			sprintDelay=.2f;
		}


		if(animStateLayer1.IsName("dodge")==true||animStateLayer1.IsName("Roll")==true)
		{
			if(animStateLayer1.normalizedTime>0.8f)
			{
				anim.SetBool("Roll",false);
			}
		}

		if (anim && Camera.main)
		{
			//JoystickToEvents.Do(transform,Camera.main.transform, ref speed, ref direction,isSprinting);
			/*
			if(isBattleReady==false)
			{
				JoystickToEvents.Do(transform,Camera.main.transform, ref speed, ref direction,isSprinting);
			}
			else
			{
				JoystickToEvents.DoBattleReady(transform,Camera.main.transform, ref speed, ref direction);
			}
			*/
			//locomotion.Do(speed * 6, direction * 180,isBattleReady,collisionAHead);
		}		
		AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
		bool inTurn = state.IsName("TurnOnSpot")|state.IsName("PlantNTurnLeft");
		bool inTransition = anim.IsInTransition(0);

		float h = Input.GetAxisRaw ("Horizontal");                  
		float v = Input.GetAxisRaw ("Vertical");
		if(animStateLayer1.IsTag("Hit")==false)
		{

			if(h!=0||v!=0)
			{
				/*
				if(isBattleReady==false)
				{
					if(animStateLayer1.IsName("Roll")==false)
					{
						inputeDir.localPosition = new Vector3 (h, 0, v);
						animDir = new Vector3 (inputeDir.transform.position.x - transform.position.x, 0, inputeDir.transform.position.z - transform.position.z);    
						targetRotation = Quaternion.LookRotation (animDir);

						transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 400 * Time.deltaTime); 
					}
				}
				else
				{
					if(NotTurn==false||inAttackTransition==true||PleaseTurn==true)
					{
						inputeDir.localPosition = new Vector3 (h, 0, v);
						animDir = new Vector3 (Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);    
						targetRotation = Quaternion.LookRotation (animDir);
						transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 400 * Time.deltaTime); 

							


					}
				}
			*/
			
					if(lockOn==true&&target&&animTransition1.IsUserName("Roll")==false&&inRoll==false)
					{

						animDir = new Vector3 (target.position.x - transform.position.x, 0, target.position.z - transform.position.z);    
						targetRotation = Quaternion.LookRotation (animDir);
						
						transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 400 * Time.deltaTime);
					}
					else
					{
						if(magicLockOn==true||block==true)
						{
							inputeDir.localPosition = new Vector3 (h, 0, v);
							animDir = new Vector3 (Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);    
							targetRotation = Quaternion.LookRotation (animDir);
							transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 400 * Time.deltaTime);
						}
						else
						{
							if(animStateLayer1.IsName("Roll")==false&&inAttack==false&&inMagicAttack==false&&inMagicAttackTransition==false)
							{
								inputeDir.localPosition = new Vector3 (h, 0, v);
								animDir = new Vector3 (inputeDir.transform.position.x - transform.position.x, 0, inputeDir.transform.position.z - transform.position.z);    
								targetRotation = Quaternion.LookRotation (animDir);
								
								transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 400 * Time.deltaTime); 
							}
						}
					}
				
					if(PleaseTurn==true||animTransition1.IsUserName("Roll")==true)
					{
						inputeDir.localPosition = new Vector3 (h, 0, v);
						animDir = new Vector3 (inputeDir.transform.position.x - transform.position.x, 0, inputeDir.transform.position.z - transform.position.z);    
						targetRotation = Quaternion.LookRotation (animDir);
						
						transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 800 * Time.deltaTime); 
						
					}
			}
			
			
			

			if(h!=0||v!=0)
			{
				if(inRoll==false)
				{
							anim.SetFloat("MoveX",h,0.3f,Time.deltaTime);
							anim.SetFloat("MoveY",v,0.3f,Time.deltaTime);
				}
			}
			else
			{
				anim.SetFloat("MoveX",0,0.3f,Time.deltaTime);
				anim.SetFloat("MoveY",0,0.3f,Time.deltaTime);
			}
			Ray collisionAHeadRay=new Ray(transform.position+transform.up/1.5f,new Vector3 (inputeDir.transform.position.x - transform.position.x+transform.up.x/1.5f, 0, inputeDir.transform.position.z - transform.position.z+transform.up.z/1.5f));

			//Debug.DrawRay(collisionAHeadRay);
				
			if(Physics.Raycast(collisionAHeadRay,0.5f,cameraCollision))
			{
				collisionAHead=true;
			}
			else
			{
				collisionAHead=false;
			}

		}

		if(animStateLayer1.IsName("Fall")==true)
		{
			inputeDir.localPosition = new Vector3 (h, 0, v);
			cc.Move(new Vector3((inputeDir.position.x-transform.position.x)*Time.deltaTime,0,(inputeDir.position.z-transform.position.z)*Time.deltaTime));
		}
		/*
		if(animStateLayer1.IsName("BattleReadyWalk")==true||animTransition1.IsUserName("Attack")==true||animStateLayer1.IsName("ToFall")==true||animTransition1.IsUserName("Roll")==true)
		{
			inputeDir.localPosition = new Vector3 (h, 0, v);
			ccSpeed=Mathf.Lerp(ccSpeed,3.8f,Time.deltaTime*3);
			cc.Move(new Vector3((inputeDir.position.x-transform.position.x)*Time.deltaTime*ccSpeed,0,(inputeDir.position.z-transform.position.z)*Time.deltaTime*ccSpeed));

		}
		else
		{
			inputeDir.localPosition = new Vector3 (h, 0, v);
			ccSpeed=Mathf.Lerp(ccSpeed,speed*8,Time.deltaTime*10);
			cc.Move(new Vector3((inputeDir.position.x-transform.position.x)*Time.deltaTime*ccSpeed,0,(inputeDir.position.z-transform.position.z)*Time.deltaTime*ccSpeed));
		}
		*/
		if(animStateLayer1.IsName("TurnOnSpot")==false&&inRoll==false&&inAttack==false&&animStateLayer1.IsTag("Hit")==false&&PleaseTurn==false&&inMagicAttack==false||animTransition1.IsUserName("Roll")==true||inAttackTransition==true)
		{
			inputeDir.localPosition = new Vector3 (h, 0, v);
			ccSpeed=Mathf.Lerp(ccSpeed,speed*6.5f,Time.deltaTime*10);
			cc.Move(new Vector3((inputeDir.position.x-transform.position.x)*Time.deltaTime*ccSpeed,0,(inputeDir.position.z-transform.position.z)*Time.deltaTime*ccSpeed));
		}
		if(animStateLayer1.IsName("dodge")==false&&animTransition1.IsUserName("Roll")==false)
		{
			Ray checkFloorRay = new Ray (transform.position+transform.up/2, -transform.up);
			Debug.DrawRay(transform.position+transform.up/2, -transform.up);
			if(Physics.Raycast(checkFloorRay,1.5f,cameraCollision))
			{
				if(anim.GetBool("Fall")==true)anim.SetBool("Fall",false);
		
			}
			else
			{
				if(anim.GetBool("Fall")==false&&animStateLayer1.IsName("Fall")==false){anim.SetBool("Fall",true);}
				else{if(anim.GetBool("Fall")==true)anim.SetBool("Fall",false);}
				if(anim.GetBool("Land")==true)anim.SetBool("Land",false);

			}
		}

		//headLookcontroler
		//GetComponent<HeadLookController> ().target = Camera.main.transform.position + Camera.main.transform.forward * 5;
		//GetComponent<HeadLookController> ().effect = layerWeight;
		anim.SetLayerWeight (1, layerWeight);

		//animation
		if(hit==true)
		{
			if(anim.GetBool("Hit")==false)
			{
				anim.SetBool("Hit",true);
				anim.SetFloat("HitStrength",hitSrength);
				anim.SetFloat("HitDir",hitDir);
				hit=false;
			}
		}
		if(animStateLayer1.IsTag("Hit")==true)
		{
			anim.SetBool("Hit",false);
			
		}

		//invetar
		if (Input.GetKeyDown(KeyCode.P)) 
		{
			if(showInventar==true)
			{
				showInventar=false;
				Cursor.visible=false;
			}
			else
			{
				showInventar=true;
				Cursor.visible=true;
			}
		}
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			magicType=1;
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			magicType=2;
		}
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			magicType=3;
		}
		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			magicType=4;
		}
		if(magicLockOn==false)
		{
			if(Input.GetAxisRaw("MagicSelect1")!=0)
			{
				if(Input.GetAxisRaw("MagicSelect1")<0)
				{
					magicType=1;
				}
				else
				{
					magicType=3;
				}
			}
			if(Input.GetAxisRaw("MagicSelect2")!=0)
			{
				if(Input.GetAxisRaw("MagicSelect2")>0)
				{
					magicType=2;
				}
				else
				{
					magicType=4;
				}
			}
		}
		UIManager ();
	}
	void UIManager()
	{
		playerHealthBar.localScale =new Vector3( health / healthMax,.8f,.8f);
		playerStaminaBar.localScale = new Vector3(stamina / staminaMax,1,1);
	}
	void resetStates()
	{
		health = healthMax;
		balance = balanceMax;
		stamina = staminaMax;
	}
	public void setWeapon()
	{
		if (weaponOneHand){Destroy (weaponOneHand);}
		if (weaponTwoHand){Destroy (weaponTwoHand);}
		if(onehandWeaponI==1){weaponOneHand = Instantiate (prefabOneHand1, oneHandWeaponPos.transform.position, Quaternion.identity) as GameObject;}
		if(onehandWeaponI==2){weaponOneHand = Instantiate (prefabOneHand2, oneHandWeaponPos.transform.position, Quaternion.identity) as GameObject;}


		if(weaponOneHand)
		{
			weaponOneHand.transform.parent=rightHandPos.transform;
			weaponOneHand.transform.localPosition= Vector3.zero;
			weaponOneHand.transform.localEulerAngles=Vector3.zero;
			weaponOneHand.GetComponent<sword>().owner=transform;
			weaponOneHand.GetComponent<sword>().targetTag="enemy";
			oneHandWeaponTrail=weaponOneHand.transform.FindChild("Trail");
			weapon1Dis=weaponOneHand.GetComponent<sword>().distance;
		}


		if(twohandWeaponI==1){weaponTwoHand = Instantiate (prefabTwoHand1, twoHandWeaponPos.transform.position, Quaternion.identity) as GameObject;}
		if(twohandWeaponI==2){weaponTwoHand = Instantiate (prefabTwoHand2, twoHandWeaponPos.transform.position, Quaternion.identity) as GameObject;}

		
		if(weaponTwoHand)
		{
			weaponTwoHand.transform.parent=twoHandWeaponPos.transform;
			weaponTwoHand.transform.localPosition= Vector3.zero;
			weaponTwoHand.transform.localEulerAngles=Vector3.zero;
			weaponTwoHand.GetComponent<sword>().owner=transform;
			weaponTwoHand.GetComponent<sword>().targetTag="enemy";
			twoHandWeaponTrail=weaponTwoHand.transform.FindChild("Trail");
			weapon2Dis=weaponTwoHand.GetComponent<sword>().distance;
		}

	}

	void drawWeapon(int i)
	{
		if(i==1)
		{
			if(weaponType==1)
			{

				weaponOneHand.transform.parent=rightHandPos.transform;
				weaponOneHand.transform.localPosition= Vector3.zero;
				weaponOneHand.transform.localEulerAngles=Vector3.zero;
				weapon1Dis=weaponOneHand.GetComponent<sword>().distance;
			}
			if(weaponType==2)
			{
				
				weaponTwoHand.transform.parent=rightHandPos.transform;
				weaponTwoHand.transform.localPosition= Vector3.zero;
				weaponTwoHand.transform.localEulerAngles=Vector3.zero;
				weapon2Dis=weaponTwoHand.GetComponent<sword>().distance;
			}
		}
		if(i==2)
		{
			if(weaponType==1)
			{
				weaponOneHand.transform.parent=oneHandWeaponPos.transform;
				weaponOneHand.transform.localPosition= Vector3.zero;
				weaponOneHand.transform.localEulerAngles=Vector3.zero;
			}
			if(weaponType==2)
			{
				weaponTwoHand.transform.parent=twoHandWeaponPos.transform;
				weaponTwoHand.transform.localPosition= Vector3.zero;
				weaponTwoHand.transform.localEulerAngles=Vector3.zero;
			}
		}
	}
	void SwordCollision(int i)
	{
		if(i==1)
		{
			weaponCollision=true;

		}
		if(i==2)
		{
			weaponCollision=false;
		}
	}

	void swordAttack(float force)
	{
		if(force<=.2f)
		{
			if(weaponType==1)weaponOneHand.GetComponent<BoxCollider>().enabled=false;
			if(weaponType==2)weaponTwoHand.GetComponent<BoxCollider>().enabled=false;

			weaponSwing=false;
		}
		if(force>=.2f)
		{
			if(weaponType==1)
			{
				weaponOneHand.GetComponent<BoxCollider>().enabled=true;
				weaponOneHand.GetComponent<sword>().damage=weaponOneHand.GetComponent<sword>().originDamage*force;
				weaponOneHand.GetComponent<sword>().damageFire=weaponOneHand.GetComponent<sword>().originDamageFire*force;
				weaponOneHand.GetComponent<sword>().damageWater=weaponOneHand.GetComponent<sword>().originDamageWater*force;
				weaponOneHand.GetComponent<sword>().damageIce=weaponOneHand.GetComponent<sword>().originDamageIce*force;
				weaponOneHand.GetComponent<sword>().damageLighting=weaponOneHand.GetComponent<sword>().originDamageLighting*force;
				weaponOneHand.GetComponent<sword>().force=weaponOneHand.GetComponent<sword>().originForce*force;
			}
			if(weaponType==2)
			{
				weaponTwoHand.GetComponent<BoxCollider>().enabled=true;
				weaponTwoHand.GetComponent<sword>().damage=weaponTwoHand.GetComponent<sword>().originDamage*force;
				weaponTwoHand.GetComponent<sword>().damageFire=weaponTwoHand.GetComponent<sword>().originDamageFire*force;
				weaponTwoHand.GetComponent<sword>().damageWater=weaponTwoHand.GetComponent<sword>().originDamageWater*force;
				weaponTwoHand.GetComponent<sword>().damageIce=weaponTwoHand.GetComponent<sword>().originDamageIce*force;
				weaponTwoHand.GetComponent<sword>().damageLighting=weaponTwoHand.GetComponent<sword>().originDamageLighting*force;
				weaponTwoHand.GetComponent<sword>().force=weaponTwoHand.GetComponent<sword>().originForce*force;
			}
			weaponSwing=true;
		}
		
	}
		GameObject magic;
	

	void OnGUI()
	{
		//GUI.Box (new Rect (10, 10, 100, 20), "health  " + health);
		//GUI.Box (new Rect (10, 55, 100, 20), "stamina " + stamina);
		if(showInventar==true)
		{
			GUI.Box (new Rect (Screen.width-100-60-10, 10, 100, 20), "Camera X axis  ");
			GUI.Box (new Rect (Screen.width-100-60-10, 35, 100, 20), "Camera Y axis  ");
			if(GUI.Button(new Rect(new Rect(Screen.width-10-50, 10, 50, 20)),""+InversCameraX))
			{
				if(InversCameraX==1)
				{
					InversCameraX=-1;
				}
				else
				{
					InversCameraX=1;
				}

			}
			if(GUI.Button(new Rect(new Rect(Screen.width-10-50, 35, 50, 20)),""+InversCameraY))
			{
				if(InversCameraY==1)
				{
					InversCameraY=-1;
				}
				else
				{
					InversCameraY=1;
				}

			}
		}
	}
}
