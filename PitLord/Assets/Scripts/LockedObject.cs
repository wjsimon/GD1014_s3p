using UnityEngine;
using System.Collections;

public class LockedObject : MonoBehaviour {

    public string keyName;
    public Collider triggerZone;

	// Use this for initialization
	void Start () {

        if(triggerZone != null)
        {
            triggerZone.isTrigger = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Unlock()
    {
        Debug.Log("OBJECT UNLOCKED");
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        triggerZone.enabled = false;

        /*
        Inventory inventory = GameManager.instance.inventory;
        
        for(int i = 0; i < inventory.keys.Count; i++)
        {
            if(inventory.keys[i].name == keyName)
            {
                inventory.RemoveKey(inventory.keys[i]);
                Debug.Log("UNLOCKED");
                GetComponent<Collider>().enabled = false;
                //Play Sound, Animation, w/e?
            }
        }
        /**/
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if(other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(new OpenLockedObject(this));
            //ScreenPrompt act
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            //ScreenPrompt deact
        }
    }
}
