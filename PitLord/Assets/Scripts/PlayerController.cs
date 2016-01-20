using UnityEngine;
using System.Collections;

public class PlayerController : Attributes
{

    //combat
    public GameObject target;

    //camera
    float cameraRotationH;
    float cameraRotationSpeedH;
    float cameraRotationV;
    float cameraRotationSpeedV;

    public float cameraSpeed;

    public float cameraDis;
    float currentCameraDis;

    public float cameraSmoothPos = 10;
    public float cameraSmoothRot = 10;

    public float InverseCameraX = 1;
    public float InverseCameraY = 1;

    Quaternion targetRotation;
    public Transform cameraTarget;
    public Transform cameraPos;
    public LayerMask cameraCollision;

    public bool lockOn = false;

    //movement
    bool collisionAHead;
    Vector3 movement;
    public float walkSpeed = .5f;
    public float runSpeed = .5f;
    public Transform inputDir;
    public Transform inputDirTarget;
    Vector3 animDir;
    bool inRoll;
    bool inAttack;
    Vector3 rollDir;
    float gravity = 20;
    public float speed = 4;
    Vector3 moveDir;

    //reference
    CharacterController cc;
    public GameObject playerModel;
    public GameObject meleeWeapon;
    //animation
    public Animator ani;
    AnimatorStateInfo animStateLayer1;
    AnimatorStateInfo animStateLayer2;
    AnimatorTransitionInfo animTransition1;
    AnimatorTransitionInfo animTransition2;
    AnimatorStateInfo animInfo;
    AnimatorStateInfo animInfoL1;
    AnimatorTransitionInfo animInfoTrans;

    public Transform handPos;


    bool potionRefill;

    // Use this for initialization
    void Start()
    {
        cc = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        animStateLayer1 = ani.GetCurrentAnimatorStateInfo(0);
        animTransition1 = ani.GetAnimatorTransitionInfo(0);


        inAttack = animStateLayer1.IsTag("Attack");

        CameraUpdate();
        MovementUpdate();
        CombatUpdate();
        InterActionUpdate();
    }

    void CombatUpdate()
    {
        if (Input.GetButtonDown("Attack"))
        {

            if (inAttack == false)
            {
                ani.SetTrigger("Attack");
            }
        }

        if (inAttack)
        {
            if (animStateLayer1.IsName("LightAttack1") == true)
            {
                float start = AnimationLibrary.Get().SearchByName("LightAttack1").start;
                float end = AnimationLibrary.Get().SearchByName("LightAttack1").end;

                if (animStateLayer1.normalizedTime >= start && animStateLayer1.normalizedTime <= end)
                {
                    meleeWeapon.GetComponent<BoxCollider>().enabled = true;
                }
                else
                {
                    meleeWeapon.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }

    void CameraUpdate()
    {
        Ray cameraRay = new Ray(cameraTarget.position, cameraPos.position - cameraTarget.position);
        RaycastHit cameraRayInfo;

        if (Physics.Raycast(cameraRay, out cameraRayInfo, cameraDis, cameraCollision))
        {
            currentCameraDis = Vector3.Distance(cameraTarget.position, cameraRayInfo.point) - Vector3.Distance(cameraTarget.position, cameraTarget.position + (cameraPos.position - cameraTarget.position).normalized);
            currentCameraDis = Mathf.Clamp(currentCameraDis, 0.64f, cameraDis);
        }

        else
        {
            currentCameraDis = cameraDis;
        }

        //Debug.DrawRay(cameraTarget.position, cameraPos.position-cameraTarget.position);
        //float ch = Input.GetAxis ("CameraH");
        //float cV = Input.GetAxis ("CameraV");


        float ch = Input.GetAxis("Mouse X");
        float cV = Input.GetAxis("Mouse Y");
        /**/

        if (lockOn == true && target != null)
        {
            //cameraRotationV += Time.deltaTime * cV * cameraSpeed ;  
            //cameraRotationV = Mathf.Clamp(cameraRotationV, -20, 0);    
            cameraRotationV = cameraTarget.eulerAngles.x;
            cameraRotationH = cameraTarget.eulerAngles.y;
            //cameraTarget.eulerAngles = new Vector3 (0, playerModel.transform.eulerAngles.y, 0);  
            cameraTarget.eulerAngles = new Vector3(0, Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position).eulerAngles.y, 0);
            cameraPos.localPosition = new Vector3(0, 1, -currentCameraDis * 1.4f);
            cameraPos.LookAt(cameraTarget.position);
            Camera.main.transform.position = cameraPos.position;
            Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position), 150 * Time.deltaTime);
            //Camera.main.transform.LookAt(target.position+Vector3.up);
        }
        else
        {
            //cameraRotationH += Time.deltaTime*ch*cameraSpeed*InverseCameraX ;                 
            //cameraRotationV += Time.deltaTime*cV*cameraSpeed*InverseCameraY ; 
            cameraRotationSpeedH = Mathf.Lerp(cameraRotationSpeedH, Time.deltaTime * ch * cameraSpeed * InverseCameraX, Time.deltaTime * cameraSpeed / 2);
            cameraRotationSpeedV = Mathf.Lerp(cameraRotationSpeedV, Time.deltaTime * cV * cameraSpeed * InverseCameraY, Time.deltaTime * cameraSpeed / 2);

            cameraRotationH += cameraRotationSpeedH;
            cameraRotationV += cameraRotationSpeedV;

            cameraRotationV = Mathf.Clamp(cameraRotationV, -40, 25);
            cameraTarget.eulerAngles = new Vector3(cameraRotationV, cameraRotationH, 0);
            cameraPos.localPosition = new Vector3(0, currentCameraDis / 2, -currentCameraDis);
            cameraPos.LookAt(cameraTarget.position);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPos.position, cameraSmoothPos * Time.deltaTime);
            Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward, cameraPos.forward, cameraSmoothRot * Time.deltaTime);
        }
    }

    public void CameraRotateToTarget()
    {
        cameraTarget.eulerAngles = new Vector3(0, Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position).eulerAngles.y, 0);
        cameraPos.localPosition = new Vector3(0, 1, -currentCameraDis * 1.4f);
        cameraPos.LookAt(cameraTarget.position);
        Camera.main.transform.position = cameraPos.position;
        Camera.main.transform.rotation = Quaternion.LookRotation((target.transform.position + Vector3.up) - Camera.main.transform.position);

    }
    void MovementUpdate()
    {
        if (inAttack == true)
            return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
            inputDirTarget.localPosition = new Vector3(h, 0, v);
        inputDir.eulerAngles = new Vector3(0, cameraTarget.eulerAngles.y, 0);


        if (cc.isGrounded)
        {
            if (lockOn == true)
            {
                transform.LookAt(target.transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                moveDir = new Vector3(h, 0, v);
                moveDir = transform.TransformDirection(moveDir);
                moveDir *= speed;
                ani.SetFloat("X", Input.GetAxis("Horizontal"));
                ani.SetFloat("Y", Input.GetAxis("Vertical"));
            }
            else
            {
                moveDir = transform.forward * Vector3.Distance(Vector3.zero, new Vector3(h, 0, v));

                moveDir *= speed;
                ani.SetFloat("X", 0);
                ani.SetFloat("Y", Vector3.Distance(Vector3.zero, new Vector3(h, 0, v)) * 2);

                transform.forward = new Vector3(-inputDir.position.x + inputDirTarget.position.x, 0, -inputDir.position.z + inputDirTarget.position.z);
            }

        }

        moveDir.y -= gravity * Time.deltaTime;
        cc.Move(moveDir * Time.deltaTime);
    }

    void InterActionUpdate()
    {
        if (Input.GetButtonDown("placeholder"))
        {
            if (potionRefill)
            {
                RefillPotion();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "PotionRefill")
        {
            //RefillPotion();
            potionRefill = true;
            //Show Screen Prompt for Potion Refilling, enable key (bool?)
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "PotionRefill")
        {
            //Hide Prompt, disable key
            potionRefill = false;
        }
    }


    public void RefillPotion()
    {
        //stuff here
        Debug.LogWarning("Potions refilled");
    }
}
