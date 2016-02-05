using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TargetSystem : MonoBehaviour
{
    public List<GameObject> targetList;
    bool lockOn;
    public GameObject pc;
    float switchLockOnDelay;
    int currentLockOn;
    // Use this for initialization
    void Start()
    {
        targetList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        if (Input.GetButtonDown("LockOn"))
        {
            if (lockOn == false)
            {
                if (targetList.Count > 0)
                {
                    if (findLockOn())
                    {
                        lockOn = true;
                        sortListByDistance();
                        //pc.GetComponent<PlayerController>().CameraRotateToTarget();
                    }

                }
            }
            else
            {
                //pc.GetComponent<PlayerController>().lockOn = false;
                lockOn = false;

            }
        }

        if (lockOn == true)
        {
            float ch = Input.GetAxis("CameraH");

            if (switchLockOnDelay >= 0)
                switchLockOnDelay -= Time.deltaTime;
            if (ch != 0 && switchLockOnDelay <= 0)
            {
                //Debug.Log(Mathf.Sign(ch));
                if (ch > 0)
                {
                    switchLockOn(1);
                }
                else
                {
                    switchLockOn(-1);
                }
                switchLockOnDelay = .4f;
            }
        }
    }
    /**/
        /*
        bool findLockOn()
        {
            if (targetList.Count <= 0)
            {
                pc.GetComponent<PlayerController>().target = null;
                pc.GetComponent<PlayerController>().lockOn = false;
                return false;
            }

            GameObject newTarget;
            newTarget = targetList[0];

            if (!newTarget.GetComponent<Attributes>().targettable)
            {
                Debug.LogWarning(newTarget.GetComponent<Attributes>().targettable);
                pc.GetComponent<PlayerController>().target = null;
                pc.GetComponent<PlayerController>().lockOn = false;
                return false;
            }

            if (newTarget != null)
            {
                pc.GetComponent<PlayerController>().target = newTarget;
                pc.GetComponent<PlayerController>().lockOn = true;
                return true;
            }

            return false;

        }
        /**/
    }

    void switchLockOn( int add )
    {
        if (add == 1)
        {
            if (1 + currentLockOn + add < targetList.Count) { currentLockOn += add; }
            else { currentLockOn = 0; }
        }
        else
        {
            if (1 + currentLockOn + add > 0) { currentLockOn += add; }
            else { currentLockOn = targetList.Count - 1; }
        }
        if (targetList.Count <= 0)
        {
            return;
        }

        GameObject newTarget;
        newTarget = targetList[currentLockOn];
        if (newTarget != null)
        {
            //pc.GetComponent<PlayerController>().target = newTarget;
            //pc.GetComponent<playerControler>().lockOn=true;
        }
    }

    void sortListByDistance()
    {
        currentLockOn = 0;
            targetList.Sort(delegate( GameObject g1, GameObject g2 )
            {
                return (Vector3.Distance(g1.transform.position, g2.transform.position).CompareTo(Vector3.Distance(g2.transform.position, transform.position)));
            }
        );
    }
    void OnTriggerEnter( Collider c )
    {
        if (c.gameObject.tag == "Enemy")
        {
            targetList.Add(c.gameObject);
        }
    }
    void OnTriggerExit( Collider c )
    {
        if (c.gameObject.tag == "Enemy")
        {
            targetList.Remove(c.gameObject);
        }
    }

    public void removeObject( GameObject removeGo )
    {
        targetList.Remove(removeGo);
        sortListByDistance();
        //if (lockOn == true) findLockOn();
    }
}
