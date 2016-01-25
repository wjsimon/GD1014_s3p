using UnityEngine;
using System.Collections;
//using Pathfinding;
//using Xft;

public class enemy1 : Attribute 
{






	Vector3 dir;

	//States
	public float healthMax=100;
	public float balanceMax=100;
	public float staminaMax=100;

	//pathfinding
	//Path path;
	int currentWaypoint;
	float delayNewPath;
	public LayerMask colliderMask;


	//refernce

	public GameObject healthBar;
	public GameObject text;
	TextMesh textMesh;
	float textDelay;
	Color textColor;
	public Transform target;
	//Seeker seeker;

	Animator anim;
	AnimatorStateInfo animStateLayer1;
	AnimatorStateInfo animStateLayer2;
	AnimatorTransitionInfo animTransition1;
	float layerWeight;

	Vector3 targetLastPos;

	public GameObject weapon;
	Transform weaponTrail;
	float trailAlpha;
	public GameObject PrefabMagicAOI;
	public GameObject PrefabMagicFloor;
	public ParticleSystem swordParticle;

	public ParticleSystem burnParticle;
	public Material myMaterial;
	public Material myWetMaterial;
	public Material myFreezeMaterial;
	public Material myFrozeMaterial;
	public SkinnedMeshRenderer myModel;
	//sound
	public AudioClip footstepLeftClip;
	public AudioClip footstepRightClip;
	AudioSource audioSourceFootstep;
	public GameObject soundFootstep;

	public GameObject soundSword;
	AudioSource audioSourceSword;
	public AudioClip drawSwordClip;
	public AudioClip hitWallClip;
	public AudioClip swordSwingClip;


	public AudioClip hitClip;
	public AudioClip dieClip;

	public GameObject playerTargetSystem;

	//behavior states
	public enum State
	{
		chase,attack,idle  
	}
	public State state;
	//battle states
	public enum BattleState
	{
		wait,walk,walkBack,strafe,combo,heavyAttack,magic,rushAttack
	}
	public BattleState battleState;
	float battleDelay;
	bool battleDelayStop;
	int randomBattleState;
	int randomBattleState2;
	float strafeDir;
	int tryStrafe;
	bool attackInRange;
	// Use this for initialization
	/*
	void Start () 
	{
		//reference
		seeker = GetComponent<Seeker> ();
		anim = GetComponent<Animator> ();
		audioSourceFootstep = soundFootstep.GetComponent<AudioSource> ();
		audioSourceSword = soundSword.GetComponent<AudioSource> ();


		//states
		resetStates ();
		state = State.idle;
		weaponDefence = weapon.GetComponent<sword> ().defence;
		//weapon
		weaponTrail=weapon.transform.FindChild("Trail");

		textMesh = text.GetComponent<TextMesh> ();

	}
	public void OnPathComplete(Path p)
	{
		if(!p.error)
		{
			
			path=p;
			currentWaypoint = 0;
		}
		
	}
	// Update is called once per frame
	void Update () 
	{

		animStateLayer1 = anim.GetCurrentAnimatorStateInfo (0);
		if(dead==false)
		{
			if(froze==false)
			{
				switch(state)
				{
					case State.idle:
					stateIdle();
					break;

					case State.chase:
					stateChase();
					break;

					case State.attack:
					stateAttack();
					break;
				}
				if(textDelay>=0)
				{
					textDelay-=Time.deltaTime/4;
					textMesh.color=new Color(255,255,255,textDelay);
				}
				if(hit==true)
				{
					if(anim.GetBool("Hit")==false)
					{
						anim.SetBool("Hit",true);
						anim.SetFloat("HitStrength",hitSrength);
						anim.SetFloat("HitDir",hitDir);
						hit=false;
					}
					if(textDelay!=1)
					{
						textMesh.text=hitDamage.ToString();
						textDelay=1;
					}
				}
			}
			healthBarManager();
			animManager();
			statesManager();
		}
		if(animStateLayer1.IsTag("Hit")==true)
		{
			anim.SetBool("Hit",false);
			weapon.GetComponent<BoxCollider>().enabled=false;
			anim.SetBool("HeavyAttack",false);
			anim.SetBool("AttackCombo",false);
			anim.SetInteger("MagicAttack",0);
			anim.SetBool("RushAttack",false);
			anim.SetBool("Rush",false);
			if(swordParticle.isPlaying==true)swordParticle.Stop();
			battleDelayStop=false;



		}
		if(dead==true)
		{
			anim.SetLayerWeight (1, 0);
			if(anim.GetBool("Dead")==false)
			{
				anim.SetBool("Dead",true);
				gameObject.GetComponent<CharacterController>().enabled=false;
				text.SetActive(false);
				healthBar.SetActive(false);
				playerTargetSystem.GetComponent<targetSystem>().removeObject(gameObject);
				if(burnParticle.isPlaying==true){burnParticle.Stop();}
			}
		}

	}
	void animManager()
	{
		bool noLayer1=animStateLayer1.IsTag("Hit")|animStateLayer1.IsTag("Attack");
		if(noLayer1==false)
		{
			if(layerWeight<1)layerWeight =Mathf.Lerp(layerWeight,1,Time.deltaTime*10);
		}
		else
		{
			if(layerWeight>0)layerWeight =Mathf.Lerp(layerWeight,0,Time.deltaTime*10);
		}
		if(anim.GetBool("Block")==true)
		{
			block=true;
		}
		else
		{
			block=false;
		}
		anim.SetLayerWeight (1, layerWeight);
	}
	void stateAttack()
	{
		dir=(target.position-transform.position).normalized;


		Ray checkPlayerRay=new Ray(transform.position+transform.up,(transform.position+transform.up)-(target.position+target.up));
		if(Physics.Raycast(checkPlayerRay,Vector3.Distance(transform.position,target.position)+1,colliderMask))
		{
			state=State.chase;
		}
		else
		{


		}


		switch(battleState)
		{
			case BattleState.wait:
			battleStateWait();
			break;
			case BattleState.walk:
			battleStateWalk();
			break;
			case BattleState.walkBack:
			battleStateWalkBack();
			break;
			case BattleState.strafe:
			battleStateStrafe();
			break;
			case BattleState.combo:
			battleStateCombo();
			break;
			case BattleState.heavyAttack:
			battleStateHeavyAttack();
			break;
			case BattleState.magic:
			battleStateMagic();
			break;
			case BattleState.rushAttack:
			battleStateRushAttack();
			break;
		}
		if(battleDelayStop==false)
		{
			if(battleDelay>0)
			{
				battleDelay-=Time.deltaTime;

			}
			else
			{
				if(anim.GetBool("Rush")==true)
				{
					anim.SetBool("Break",true);
					anim.SetBool("Rush",true);
				}
				changeBattleState();
			}
		}
		if(animStateLayer1.IsTag("Attack")==true)
		{
			trailAlpha=Mathf.Lerp(trailAlpha,.2f,Time.deltaTime*3);
		}
		else
		{
			trailAlpha=Mathf.Lerp(trailAlpha,0,Time.deltaTime*3);
			
		}
		if(weaponTrail.gameObject.GetComponent<XWeaponTrail>().MyColor.a!=trailAlpha)
		{
			weaponTrail.gameObject.GetComponent<XWeaponTrail>().MyColor=new Color(255,255,255,trailAlpha);
		}
	
	}
	void changeBattleState()
	{
		if(anim.GetBool("Break")==true)anim.SetBool("Break",false);
		randomBattleState=Random.Range(1,13);
		battleDelayStop = false;
		anim.SetBool ("Block", false);
		if(randomBattleState==1)
		{
			battleState=BattleState.wait;
			battleDelay=Random.Range(1,2);
			randomBattleState2=Random.Range(1,3);
			battleDelayStop=false;
			
		}
		if(randomBattleState==2)
		{
			battleState=BattleState.walk;
			battleDelay=Random.Range(1,3);
			randomBattleState2=Random.Range(1,3);
			battleDelayStop=false;
			
		}
		if(randomBattleState==3)
		{
			battleState=BattleState.walkBack;
			battleDelay=Random.Range(2,4);
			randomBattleState2=Random.Range(1,3);
			battleDelayStop=false;
			
		}
		if(randomBattleState==4)
		{
			battleState=BattleState.strafe;
			battleDelay=Random.Range(2,4);
			randomBattleState2=Random.Range(1,3);
			if(randomBattleState2==1)
			{
				strafeDir=1;
			}
			else
			{
				strafeDir=-1;
			}
			randomBattleState2=Random.Range(1,3);
			tryStrafe=0;
			battleDelayStop=false;
			
		}
		if(randomBattleState==5||randomBattleState==6)
		{
			battleState=BattleState.combo;

			battleDelay=0;
			battleDelayStop=true;
			attackInRange=false;
		}
		if(randomBattleState==7||randomBattleState==8)
		{
			battleState=BattleState.heavyAttack;
			
			battleDelay=0;
			battleDelayStop=true;
			attackInRange=false;
		}
		if(randomBattleState==9||randomBattleState==10)
		{
			battleState=BattleState.magic;
			
			battleDelay=0;
			battleDelayStop=true;
			attackInRange=false;
		}
		if(randomBattleState==11||randomBattleState==12)
		{
			if(Vector3.Distance(transform.position,target.position)>3)
			{
				battleState=BattleState.rushAttack;
				anim.SetBool("Rush",true);
				battleDelay=0;
				battleDelayStop=true;
				attackInRange=false;
			}
			else
			{
				battleState=BattleState.combo;
				
				battleDelay=0;
				battleDelayStop=true;
				attackInRange=false;
			}
		}
	}
	void battleStateCombo()
	{
		if(attackInRange==false)
		{
			if(Vector3.Distance(transform.position,target.position)>=1.5f)
			{

				anim.SetFloat ("Speed", .5f,1f,Time.deltaTime);
				anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
				anim.SetFloat ("MoveY", 1,1.3f,Time.deltaTime);
			}
			else
			{
				anim.SetFloat ("Speed", 0,1f,Time.deltaTime);
				anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
				anim.SetFloat ("MoveY", 0,1.3f,Time.deltaTime);
			}
			transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*4);
			float targetDir = Vector3.Dot (transform.forward,dir );
			if(Vector3.Distance(transform.position,target.position)<=1.5f&&targetDir>0.9f)
			{
				anim.SetBool("AttackCombo",true);
				attackInRange=true;
			}
			if(Vector3.Distance(transform.position,target.position)>=3.5f)
			{
				battleDelayStop=false;
				anim.SetBool("AttackCombo",false);
			}
		}
		else
		{
			anim.SetFloat ("Speed", 0,1f,Time.deltaTime);
			anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
			anim.SetFloat ("MoveY", 0,1.3f,Time.deltaTime);
			animTransition1 = anim.GetAnimatorTransitionInfo (0);
			if(animTransition1.IsUserName("PleaseTurn")==true)
			{
				transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*4);
				if(Vector3.Distance(transform.position,target.position)>1.5f)
				{
					anim.SetBool("Break",true);
					anim.SetBool("AttackCombo",false);
					battleDelayStop=false;
				}
			}
			if(animStateLayer1.IsName("Combo3")==true)
			{
				if(animStateLayer1.normalizedTime>0.6f)
				{
					battleDelayStop=false;
					anim.SetBool("AttackCombo",false);
				}
			}
		}

	}
	void battleStateHeavyAttack()
	{

		if(attackInRange==false)
		{
			if(Vector3.Distance(transform.position,target.position)>=1.5f)
			{
				
				anim.SetFloat ("Speed", .5f,1f,Time.deltaTime);
				anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
				anim.SetFloat ("MoveY", 1,1.3f,Time.deltaTime);
			}
			else
			{
				anim.SetFloat ("Speed", 0,1f,Time.deltaTime);
				anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
				anim.SetFloat ("MoveY", 0,1.3f,Time.deltaTime);
			}
			transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*4);
			float targetDir = Vector3.Dot (transform.forward,dir );
			if(Vector3.Distance(transform.position,target.position)<=1.5f&&targetDir>0.9f)
			{
				anim.SetBool("HeavyAttack",true);
				attackInRange=true;
			}
			if(Vector3.Distance(transform.position,target.position)>=3.5f)
			{
				battleDelayStop=false;
				anim.SetBool("HeavyAttack",false);
			}
		}
		else
		{
			anim.SetFloat ("Speed", 0,1f,Time.deltaTime);
			anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
			anim.SetFloat ("MoveY", 0,1.3f,Time.deltaTime);
			animTransition1 = anim.GetAnimatorTransitionInfo (0);

			if(animStateLayer1.IsName("HeavyAttack")==true)
			{
				if(animStateLayer1.normalizedTime>0.6f)
				{
					battleDelayStop=false;
					anim.SetBool("HeavyAttack",false);
				}
			}
		}
		
	}
	void battleStateRushAttack()
	{
		
		if(attackInRange==false)
		{
			if(Vector3.Distance(transform.position,target.position)>=1.5f)
			{
				
				anim.SetFloat ("Speed", 2f,.1f,Time.deltaTime);
				anim.SetFloat ("MoveX", 0,.1f,Time.deltaTime);
				anim.SetFloat ("MoveY", 1,.1f,Time.deltaTime);
			}
			else
			{
				anim.SetFloat ("Speed", 0,.1f,Time.deltaTime);
				anim.SetFloat ("MoveX", 0,.1f,Time.deltaTime);
				anim.SetFloat ("MoveY", 0,.1f,Time.deltaTime);
			}
			transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*12);
			float targetDir = Vector3.Dot (transform.forward,dir );
			if(Vector3.Distance(transform.position,target.position)<=1.5f&&targetDir>0.9f)
			{
				anim.SetBool("RushAttack",true);
				attackInRange=true;
			}


		}
		else
		{
			anim.SetFloat ("Speed", 0,3f,Time.deltaTime);
			anim.SetFloat ("MoveX", 0,3.3f,Time.deltaTime);
			anim.SetFloat ("MoveY", 0,3.3f,Time.deltaTime);
			animTransition1 = anim.GetAnimatorTransitionInfo (0);
			
			if(animStateLayer1.IsName("RushAttack")==true)
			{
				if(animStateLayer1.normalizedTime>0.6f)
				{
					anim.SetBool("RushAttack",false);
					anim.SetBool("Rush",false);
					battleDelayStop=false;
				}
			}
		}
		
	}
	void battleStateMagic()
	{
		anim.SetFloat ("Speed", 0f,1f,Time.deltaTime);
		anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
		anim.SetFloat ("MoveY", 0,1.3f,Time.deltaTime);
		if(attackInRange==false)
		{

				
				
		
			transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*4);
			float targetDir = Vector3.Dot (transform.forward,dir );
			if(targetDir>0.9f)
			{
				anim.SetInteger("MagicAttack",1);
				attackInRange=true;
			}
		}
		else
		{


			if(animStateLayer1.IsName("MagicAttack")==true)
			{
				if(animStateLayer1.normalizedTime>0.6f)
				{
					battleDelayStop=false;
					anim.SetInteger("MagicAttack",0);
				}
			}
		}
		
	}
	void battleStateWait()
	{
		anim.SetFloat ("Speed", 0);
		if(randomBattleState2==1)
		{
			anim.SetBool("Block",true);
		}
		else
		{
			anim.SetBool("Block",false);
		}

	}
	void battleStateWalk()
	{
		if(randomBattleState2==1)
		{
			anim.SetBool("Block",true);
			anim.SetFloat ("Speed", .5f,1f,Time.deltaTime);
		}
		else
		{
			anim.SetBool("Block",false);
			anim.SetFloat ("Speed", 1f,1f,Time.deltaTime);
		}

		anim.SetFloat ("MoveX", 0,1.3f,Time.deltaTime);
		anim.SetFloat ("MoveY", 1,1.3f,Time.deltaTime);
		dir=(target.position-transform.position).normalized;
		transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*2);
		if(Vector3.Distance(transform.position,target.position)<1.5f)
		{
			battleDelay=0;
		}
	}
	void battleStateWalkBack()
	{
		if(randomBattleState2==1)
		{
			anim.SetBool("Block",true);
			anim.SetFloat ("Speed", .25f,1f,Time.deltaTime);
		}
		else
		{
			anim.SetBool("Block",false);
			anim.SetFloat ("Speed", 1f,1f,Time.deltaTime);
		}
		anim.SetFloat ("MoveX", 0,.7f,Time.deltaTime);
		anim.SetFloat ("MoveY", -1,0.7f,Time.deltaTime);
		dir=(target.position-transform.position).normalized;
		transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*1);


		Ray rayBack = new Ray (transform.position + transform.up, -transform.forward);
		Debug.DrawRay (transform.position + transform.up, -transform.forward*2);
		if(Physics.Raycast(rayBack,2,colliderMask))
		{
			battleDelay=0;
		}
	}
	void battleStateStrafe()
	{
		if(randomBattleState2==1)
		{
			anim.SetBool("Block",true);
			anim.SetFloat ("Speed", .5f,1f,Time.deltaTime);
		}
		else
		{
			anim.SetBool("Block",false);
			anim.SetFloat ("Speed", 2f,1.3f,Time.deltaTime);
		}
		anim.SetFloat ("MoveX", strafeDir,1.3f,Time.deltaTime);
		anim.SetFloat ("MoveY", 0,1.3f,Time.deltaTime);
		dir=(target.position-transform.position).normalized;
		transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*5);
		if(strafeDir==1)
		{
			Ray rayStrafe = new Ray (transform.position + transform.up, transform.right);
			Debug.DrawRay (transform.position + transform.up, transform.right*2);
			if(Physics.Raycast(rayStrafe,2,colliderMask))
			{
				tryStrafe+=1;
				strafeDir=-1;
			}
		}
		if(strafeDir==-1)
		{
			Ray rayStrafe = new Ray (transform.position + transform.up, -transform.right);
			Debug.DrawRay (transform.position + transform.up, -transform.right*2);
			if(Physics.Raycast(rayStrafe,2,colliderMask))
			{
				tryStrafe+=1;
				strafeDir=1;
			}
		}
		if(tryStrafe>2)
		{
			battleDelay=0;
		}
	}
	void stateIdle()
	{
		if(target)
		{
			if (Vector3.Distance (target.position, transform.position)<20) 
			{
				state=State.chase;
			}
			else
			{
				if(anim.GetFloat("Speed")>0)anim.SetFloat("Speed",0);
			}
		}
	}
	void stateChase()
	{
		if(anim.GetBool("Block")==true)anim.SetBool("Block",false);
		if(target&&path!=null)
		{
			
			Ray checkPlayerRay=new Ray(transform.position+transform.up,(transform.position+transform.up)-(target.position+target.up));
			if(Physics.Raycast(checkPlayerRay,Vector3.Distance(transform.position,target.position)+1,colliderMask))
			{

			}
			else
			{
				if(Vector3.Distance(transform.position,target.position)<4)
				{
					state=State.attack;
				}
			}

			if(currentWaypoint<path.vectorPath.Count)
			{
				dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
				if(Vector3.Distance(path.vectorPath[currentWaypoint],transform.position)>1)
				{
					transform.rotation=Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),Time.deltaTime*5);
					anim.SetFloat("Speed",2f,2f,Time.deltaTime);  
					anim.SetFloat("MoveY",1,.2f,Time.deltaTime);
					anim.SetFloat("MoveX",0,.2f,Time.deltaTime);
				}
				else
				{
					
					if(currentWaypoint<path.vectorPath.Count){currentWaypoint++;}       
				}
				
				if(delayNewPath>=0)
				{
					delayNewPath-=Time.deltaTime;      
				}
				else
				{
					delayNewPath=1;
					if(Vector3.Distance(targetLastPos,target.position)>2)
					{
						targetLastPos=target.position;
						currentWaypoint=0;
						seeker.StartPath (transform.position, target.position, OnPathComplete);                
					}
				}
			}
			else
			{
				if(path!=null)
				{
					if(currentWaypoint>=path.vectorPath.Count)
					{
						seeker.StartPath (transform.position, target.position, OnPathComplete);                  
					}
				}
			}


			
		}
		else
		{
			seeker.StartPath (transform.position, target.position, OnPathComplete);  
		}
	}
	void sound(int i)
	{
		if(i==1)
		{
			audioSourceFootstep.PlayOneShot(footstepLeftClip);
		}
		if(i==2)
		{
			audioSourceFootstep.PlayOneShot(footstepRightClip);
		}
		if(i==4)
		{
			audioSourceFootstep.PlayOneShot(hitClip);
		}
		if(i==5)
		{
			audioSourceFootstep.PlayOneShot(dieClip);
		}
	}
	void SwordSound(int i)
	{

		if(i==1)
		{
			audioSourceSword.PlayOneShot(drawSwordClip);
		
		}
		if(i==2)
		{
			audioSourceSword.PlayOneShot(swordSwingClip);

		}
		if(i==3)
		{
			audioSourceSword.PlayOneShot(hitWallClip);

		}
	}
	void resetStates()
	{
		health = healthMax;
		balance = balanceMax;
		stamina = staminaMax;
	}
	void OnGUI()
	{
		//GUI.Box (new Rect (Screen.width- 110, 10, 100, 20), "health  " + health);
		//GUI.Box (new Rect (Screen.width- 110, 55, 100, 20), "stamina " + stamina);
	}
	void swordAttack(float force)
	{
		if(force<=.2f)
		{
			weapon.GetComponent<BoxCollider>().enabled=false;
		
		}
		if(force>=.2f)
		{
		
				weapon.GetComponent<BoxCollider>().enabled=true;
				weapon.GetComponent<sword>().damage=weapon.GetComponent<sword>().originDamage*force;
				weapon.GetComponent<sword>().force=weapon.GetComponent<sword>().originForce*force;

		}
		
	}
	void swordMagic(int i)
	{
		if(i==1)
		{
			swordParticle.Play();
		}
		if(i==2)
		{
			swordParticle.Stop();
			if(Vector3.Distance(transform.position,target.position)>4)
			{
				Instantiate(PrefabMagicFloor,transform.position+transform.forward,transform.rotation);
			}
			else
			{
				Instantiate(PrefabMagicAOI,transform.position+transform.forward,Quaternion.identity);
			}

		}
	}
	void statesManager()
	{
		if(delayBurnHeal>0){delayBurnHeal-=Time.deltaTime*.2f;}else{if(burn==true){burn=false;}}
		if(delayWetHeal>0){delayWetHeal-=Time.deltaTime*.1f;}else{if(wet==true){wet=false;}}
		if(delayFreezeHeal>0){delayFreezeHeal-=Time.deltaTime*.1f;}else{if(freeze==true){freeze=false;}}
		if(delayFrozeHeal>0){delayFrozeHeal-=Time.deltaTime*.1f;}else{if(froze==true){froze=false;}}
		if (burn == true) 
		{
			if(delayBurn>0){delayBurn-=Time.deltaTime;}else{health-=5;delayBurn=1;if(health<=0){dead=true;}hit=true;hitDamage=5;hitSrength=0;}
			if(burnParticle.isPlaying==false){burnParticle.Play();}
		}
		else
		{
			if(burnParticle.isPlaying==true){burnParticle.Stop();}
		}
		if(froze==true)
		{
	
				myModel.material=myFrozeMaterial;
				anim.speed=0.007f;


		}
		else
		{
			if(wet==true)
			{

					myModel.material=myWetMaterial;

			}
			if(freeze==true)
			{

					myModel.material=myFreezeMaterial;
					anim.speed=0.5f;
					Debug.Log("Freeze");

			}
			else
			{
				if(anim.speed!=1){anim.speed=1;}
			}
		}
		if(freeze==false&&froze==false&&wet==false){if(myModel.material.name!=myMaterial.name){myModel.material=myMaterial;if(anim.speed!=1){anim.speed=1;}}}

	}
	void healthBarManager()
	{
		healthBar.transform.position = transform.position + transform.up * 2-Camera.main.transform.right/2;
		healthBar.transform.rotation = Camera.main.transform.rotation;
		healthBar.transform.localScale = new Vector3(-(health/healthMax),0.3f,1);
		text.transform.position = transform.position + transform.up * 2.1f-Camera.main.transform.right/2;
		text.transform.rotation = Camera.main.transform.rotation;

	}
	*/

}
