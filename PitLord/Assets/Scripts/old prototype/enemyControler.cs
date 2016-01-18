using UnityEngine;
using System.Collections;
//using Pathfinding;
public class enemyControler : Attribute {


	Animator anim;
	AnimatorStateInfo animInfo;
	AnimatorStateInfo animInfoL1;
	public Transform target;
	//Seeker seeker;
	//Path path;
	//int currentWaypoint;
	float delayNewPath;
	Vector3 targetLastPos;
	public AudioClip audioStep1;
	public AudioClip audioStep2;   
	public AudioClip audioRoll;   
	AudioSource audioControler;
	public GameObject swordPhysics;    
	BoxCollider swordBoxCollider;
	float currentAttackDelay;          
	float layer1Weight;
	//states
	public enum State
	{
		chase,attack,idle  
	}
	public State state;
	enum BattleState
	{
		attack,chargeAttack,strafe,walkBack,strafeBlock,wait,walk
	}
	public int chanceAttack=0;
	public int chanceCharge=0;
	public int chanceStrafe=1;
	public int chanceWait=0;
	public int chanceWalk=0;
	public int chanceWalkBack=0;
	BattleState battleState;
	//bool changeState;
	float changeStateDelay;
	float randomState;
	float randomState2;



	public GameObject playerTargetSystem;
	bool removedFromTargetSystem;
	// Use this for initialization
	void Start () 
	{
		swordBoxCollider = swordPhysics.GetComponent<BoxCollider> ();              
		swordBoxCollider.enabled = false; 
		anim = gameObject.GetComponent<Animator> ();
		//seeker = GetComponent<Seeker> ();
		if (target) {
						targetLastPos = target.position;
						//seeker.StartPath (transform.position, targetLastPos, OnPathComplete);
				}
		audioControler = GetComponent<AudioSource> ();        
		//battleState = BattleState.attack;
		//anim.speed = 0.5f;


	}
	/*
	public void OnPathComplete(Path p)
	{
		if(!p.error)
		{
		
			//path=p;
			//currentWaypoint = 0;
		}

	}
	*/

	Vector3 dir ;
	// Update is called once per frame
	void Update ()
	{
		if(dead==false)
		{
			if(target)
			{
				/*
				if (Vector3.Distance (target.position, transform.position)<2) 
				{

					if (Vector3.Distance (target.position, transform.position)<1.2f) 
					{
					
							if(animInfoL1.IsName("Attack1")==false)
							{
								anim.SetBool("Attack",true);              
							}

						anim.SetFloat("Speed",0f);
					}
					else
					{
						animInfo=anim.GetCurrentAnimatorStateInfo(0);
						anim.SetFloat("Speed",.2f);
						anim.SetBool("Attack",false); 
						if(animInfo.IsName("fallover")==false)
						{
						dir = (target.position - transform.position).normalized;
						//transform.rotation=Quaternion.FromToRotation(transform.forward,new Vector3(dir.x,0,dir.z)); 
						Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
						}
						
					}
					animInfoL1=anim.GetCurrentAnimatorStateInfo(1);
					if(animInfoL1.IsName("Attack1")==true)
					{
						anim.SetLayerWeight(1,1);              
						if(animInfoL1.normalizedTime>0.4f)
						{
						}
						else
						{
							
							anim.SetBool("Attack",false);              
						}
					}
					else
					{
						anim.SetLayerWeight(1,.2f);
						if(animInfo.IsName("fallover")==false)
						{	//animInfo=anim.GetCurrentAnimatorStateInfo(0);
							//dir = (target.position - transform.position).normalized;
							//transform.rotation=Quaternion.FromToRotation(transform.forward,new Vector3(dir.x,0,dir.z)); 
							//Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));  
							//anim.SetFloat("Speed",0f);
							targetLastPos=target.position;            
						}
					}

					// Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));    
			

				}
				else
				{

					if(Vector3.Distance (target.position, transform.position)<20&&currentWaypoint<path.vectorPath.Count)
					{
						dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
						if(Vector3.Distance(path.vectorPath[currentWaypoint],transform.position)>1)
						{
							transform.rotation=Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
							anim.SetFloat("Speed",0.4f);  
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
				*/
				switch(state)
				{
				case State.attack:
					attackControler();
					
					break;
				case State.idle:
					
					if(Vector3.Distance(target.position,transform.position)<20)
					{
						state=State.attack;
						changeBattleState(0);                       
					}
					break;    
				}
			}
			else
			{
				//GameObject tempPlayer=GameObject.FindGameObjectWithTag("Player");              
				//target=tempPlayer.transform;
			}
			animControler();

		}
		else
		{
			if(swordBoxCollider.enabled==true)
			{
				attack(0);
			}
			anim.SetBool("Dead",true);
			anim.SetBool("Attack",false);
			anim.SetLayerWeight(1,0); 
			if(GetComponent<CapsuleCollider>().enabled==true)
			{
				GetComponent<CapsuleCollider>().enabled=false;
			}
			if(playerTargetSystem&&removedFromTargetSystem==false)
			{
				//playerTargetSystem.GetComponent<targetSystem>().removeObject(gameObject);
				removedFromTargetSystem=true;
			}
		}
	}
	AnimatorTransitionInfo animInfoTrans;
	void attackControler()
	{
		if(target)
		{
			animInfoTrans=anim.GetAnimatorTransitionInfo(0);
			dir = (target.position - transform.position).normalized;
			//transform.rotation=Quaternion.FromToRotation(transform.forward,new Vector3(dir.x,0,dir.z));
			if(animInfo.IsName("chargeAttack")==false&&animInfo.IsName("Attack1")==false&&animInfo.IsName("Hit")==false&&animInfo.IsName("Hit1")==false&&animInfo.IsName("fallover")==false&&animInfo.IsName("fallover1")==false||animInfoTrans.IsUserName("Turn")==true)
			{
				//transform.rotation=Quaternion.LookRotation(new Vector3(dir.x,0,dir.z));
				transform.rotation=Quaternion.RotateTowards(Quaternion.LookRotation(transform.forward),Quaternion.LookRotation(new Vector3(dir.x,0,dir.z)),100*Time.deltaTime);
			}


			if(animInfo.IsName("chargeAttack")==true||animInfo.IsTag("Attack")==true||animInfo.IsName("Hit")==true)
			{
				//anim.SetLayerWeight(1,0);
				layer1Weight=Mathf.Lerp(layer1Weight,0,25*Time.deltaTime);
			}
			else
			{
				if(anim.GetBool("Block")==true||anim.GetBool("Charge")==true)
				{
					//anim.SetLayerWeight(1,1);

					layer1Weight=Mathf.Lerp(layer1Weight,1,25*Time.deltaTime);            
				}
				else
				{
					//anim.SetLayerWeight(1,.8f);
					layer1Weight=Mathf.Lerp(layer1Weight,1f,8*Time.deltaTime);
				}
			}
			if(anim.GetLayerWeight(1)!=layer1Weight)
			{
				anim.SetLayerWeight(1,layer1Weight);        
			}

			if(anim.GetBool("Block")==true&&animInfo.IsName("chargeAttack")==false&&animInfo.IsName("Attack1")==false&&blockForce>0)
			{
				block=true;
			}
			else
			{
				if( blockDelay>0)
				{
					blockDelay-=Time.deltaTime;
					blockForce=0;
				}
				{
					if(blockForce<100)
					{
						blockForce+=10*Time.deltaTime;
					}
				}
				block=false;
			}
			switch(battleState)
			{
				case BattleState.attack:
				if(Vector3.Distance(target.position,transform.position)>2.1f)
				{

					      
				}
				else
				{
					anim.SetBool("Attack",true);                


				}

				if(animInfo.IsName("Attack1")==true)
				{

					if(Vector3.Distance(target.position,transform.position)<2)  
					{
						changeBattleState(3);                
					}
					else
					{
						changeBattleState(0);   
					}
				}


				break;
				case BattleState.walk:
					if(Vector3.Distance(target.position,transform.position)>2.1f)
					{
						
						
					}
					else
					{
						
						
							changeBattleState(0);   
					}
					
					
				break;
				case BattleState.chargeAttack:
				if(Vector3.Distance(target.position,transform.position)>1.4f)
				{

					anim.SetBool("Charge",true);                                  
				}
				else
				{

					anim.SetBool("Charge",false);
					anim.SetBool("ChargeAttack",true);
					
				}

				if(animInfo.IsName("chargeAttack")==true)
				{
					if(Vector3.Distance(target.position,transform.position)<4)
					{
						changeBattleState(3);
						
					}
					else
					{
						changeBattleState(0);  
						anim.SetBool("ChargeAttack",false);            
					}
				}
				if(changeStateDelay>=0)
				{
					changeStateDelay-=Time.deltaTime;
				}
				else
				{
					
					changeBattleState(4);      
					
				}
				break;

				case BattleState.strafe:

				anim.SetBool("Attack",false);
				if(changeStateDelay>=0)
				{
					changeStateDelay-=Time.deltaTime;
				}
				else
				{
					                
					changeBattleState(0);      
					
				}

				break;
				case BattleState.walkBack:

				anim.SetBool("Attack",false);
				if(Vector3.Distance(target.position,transform.position)>4)
				{
					changeBattleState(0);  
				}
				if(changeStateDelay>=0)
				{
					changeStateDelay-=Time.deltaTime;
				}
				else
				{
					//changeStateDelay=Random.Range(2,4);                  
					changeBattleState(0);                                                   
					
				}
				break;
				case BattleState.wait:
				
				anim.SetBool("Attack",false);
				if(changeStateDelay>=0)
				{
					changeStateDelay-=Time.deltaTime;
				}
				else
				{
					
					changeBattleState(0);      
					
				}
				
				break;
			}
			/*
			if(changeStateDelay>=0)
			{
				changeStateDelay-=Time.deltaTime;
			}
			else
			{
				changeStateDelay=Random.Range(2,4);
				changeBattleState();      

			}
			*/
		}
		else
		{
			state=State.idle;  
		}

	}
	void changeBattleState(int state)
	{
		anim.SetBool("ChargeAttack",false);
		anim.SetBool("Charge",false);
		anim.SetBool ("Block", false);
		anim.SetBool("Attack",false);
		if(state==0)
		{
			randomState =Mathf.Abs( Random.Range (1, chanceAttack+chanceStrafe+chanceCharge+chanceWait+chanceWalk+1));
			//attack 0  strafe 3  charge 1
			if(randomState>=1&&randomState<=chanceAttack)//1<>0
			{
				anim.SetFloat("Speed",.3f);
				anim.SetFloat("MoveY",1);    
				anim.SetFloat("MoveX",0);
				battleState=BattleState.attack;
			}
			if(randomState>=1+chanceAttack&&randomState<=chanceAttack+chanceStrafe)//1<>3
			{
				randomState2 =Mathf.Abs( Random.Range (1, 3));
				if(randomState2==1)
				{
					anim.SetBool("Block",true);
					anim.SetFloat("Speed",Random.Range(0.2f,1));
				}
				else
				{
					anim.SetFloat("Speed",Random.Range(1f,1));
				}
				randomState2 =Mathf.Abs( Random.Range (1, 3));
				if(randomState2==1)
				{
					anim.SetFloat("MoveX",1);
				}
				else
				{
					anim.SetFloat("MoveX",-1);
				}
				anim.SetFloat("MoveY",0);
				changeStateDelay=Random.Range(2,4);  
				battleState=BattleState.strafe;
			}
			if(randomState>=1+chanceAttack+chanceStrafe&&randomState<=chanceAttack+chanceStrafe+chanceCharge)//4<>4
			{
				if(Vector3.Distance(transform.position,target.transform.position)>6)
				{
				anim.SetFloat("Speed",1f);
				anim.SetFloat("MoveY",1);    
				anim.SetFloat("MoveX",0);
				changeStateDelay=Random.Range(3,5); 
				battleState=BattleState.chargeAttack; 
				}
				else
				{
					anim.SetFloat("Speed",.3f);
					anim.SetFloat("MoveY",1);    
					anim.SetFloat("MoveX",0);
					battleState=BattleState.attack;
				}
			}
			if(randomState>=1+chanceAttack+chanceStrafe+chanceCharge&&randomState<=chanceAttack+chanceStrafe+chanceCharge+chanceWait)
			{
				randomState2 =Mathf.Abs( Random.Range (1, 3));
				if(randomState2==1)
				{
					anim.SetBool("Block",true);
				}
				anim.SetFloat("Speed",0);
				anim.SetFloat("MoveY",0);      
				anim.SetFloat("MoveX",0);  
				changeStateDelay=Random.Range(1,2); 
				battleState=BattleState.wait;
			}
			if(randomState>=1+chanceAttack+chanceStrafe+chanceCharge+chanceWait&&randomState<=chanceAttack+chanceStrafe+chanceCharge+chanceWait+chanceWalk)
			{
				randomState2 =Mathf.Abs( Random.Range (1, 3));
				if(randomState2==1)
				{
					anim.SetBool("Block",true);
				}
				anim.SetFloat("Speed",.5f);
				anim.SetFloat("MoveY",1);      
				anim.SetFloat("MoveX",0);  
				battleState=BattleState.walk;
			}
		}
		if(state==1)
		{
			anim.SetFloat("Speed",.6f);
			anim.SetFloat("MoveY",1);    
			anim.SetFloat("MoveX",0);
			battleState=BattleState.attack;
		}
		if(state==2)
		{
			randomState =Mathf.Abs( Random.Range (1, 3));
			if(randomState==1)
			{
				anim.SetBool("Block",true);
			}
			anim.SetFloat("Speed",Random.Range(0.2f,1));
			anim.SetFloat("MoveY",0);
			anim.SetFloat("MoveX",1);
			changeStateDelay=Random.Range(2,4);  
			battleState=BattleState.strafe;
		}
		if(state==3)
		{
			randomState =Mathf.Abs( Random.Range (1, 3));
			
			if(randomState==1)
			{
				randomState2 =Mathf.Abs( Random.Range (1, 3));
				if(randomState2==1)
				{
					anim.SetBool("Block",true);
				}
				anim.SetFloat("Speed",.6f);
				anim.SetFloat("MoveY",-1);      
				anim.SetFloat("MoveX",0);  
				changeStateDelay=Random.Range(2,4);  
				battleState=BattleState.walkBack;  
				                      
			}
			if(randomState==2)
			{
				anim.SetFloat("Speed",.3f);
				anim.SetFloat("MoveY",1);    
				anim.SetFloat("MoveX",0);
				battleState=BattleState.attack;
			}
		}
		if(state==4)
		{
			randomState2 =Mathf.Abs( Random.Range (1, 3));
			if(randomState2==1)
			{
				anim.SetBool("Block",true);
			}
			anim.SetFloat("Speed",0);
			anim.SetFloat("MoveY",0);      
			anim.SetFloat("MoveX",0);  
			changeStateDelay=Random.Range(1,2);
			battleState=BattleState.wait;
		}
		//Debug.Log ("battleState." + battleState);
	}
	void animControler()
	{
		if(swordBoxCollider.enabled==true&&animInfo.IsName("Attack1")==false&&animInfo.IsName("chargeAttack")==false)
		{
			attack(0);
		}
		animInfo=anim.GetCurrentAnimatorStateInfo(0);
		if(animInfo.IsName("fallover")==true)
		{
			
			anim.SetBool("FallOver",false);    
			hit=false;
			hitSrength=0;
			hitDir=0;
			
		}
		if(animInfo.IsName("fallover1")==true)
		{
			
			anim.SetBool("FallOver",false);    
			hit=false;
			hitSrength=0;
			hitDir=0;
			
		}
		if(animInfo.IsName("Hit")==true)
		{
			
			anim.SetBool("Hit",false);  
			hit=false;
			hitSrength=0; 
			hitDir=0;
			
		}
		if(animInfo.IsName("Hit1")==true)
		{
			
			anim.SetBool("Hit",false);  
			hit=false;
			hitSrength=0; 
			hitDir=0;
			
		}
		if(animInfo.IsName("hitBlock")==true)
		{
			if(animInfo.normalizedTime>=0.5f)
			{
				anim.SetBool("Hit",false);    
				hit=false;
				hitSrength=0;
			}
		}
		if(hit==true)
		{
			anim.SetFloat("HitDirection",hitDir);
			if(hitDir<0)
			{
				anim.SetBool("Block",false);
			}
			if(hitSrength>50)
			{
				anim.SetBool("FallOver",true);
				
			}
			else
			{
				anim.SetBool ("Hit", true);  
			}
		}
		if(swordBoxCollider.enabled==true)
		{
			dodge=true;    
		}
		else
		{
			dodge=false;  
		}
	}
	public void animHit()
	{
		anim.SetBool ("Hit", true);
	}
	void step1()
	{
		audioControler.clip = audioStep1;
		//audioControler.Play();
	}
	void step2()
	{
		audioControler.clip = audioStep2;
		//audioControler.Play();
	}
	void roll()
	{
		audioControler.clip = audioRoll;  
		//audioControler.Play();
	}
	void startAttack()
	{
		swordBoxCollider.enabled = true;
	}
	void endAttack()
	{
		swordBoxCollider.enabled = false;  
	}
	public void switchState(string stateName)
	{
		if(stateName=="attack")
		{
			state=State.attack;        
		}
	}
	void attack(float force)
	{
		if(force<=.2f)
		{
			swordBoxCollider.enabled = false;   
			//targetSystemGo.GetComponent<targetSystem>().attack();
		}
		if(force>=.2f)
		{
			swordBoxCollider.enabled = true;  
			swordPhysics.GetComponent<sword>().damage=swordPhysics.GetComponent<sword>().originDamage*force;
			swordPhysics.GetComponent<sword>().force=swordPhysics.GetComponent<sword>().originForce*force;
		}
		
	}
	void OnGUI()
	{
		GUI.Box (new Rect (Screen.width - 210, 10, 200, 20), "health" + health);
		GUI.Box (new Rect (Screen.width - 210, 40, 200, 20), "hitStrength" + hitSrength);

	
	}
}
