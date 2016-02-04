using UnityEngine;
using System.Collections;

public class LockedObject : MonoBehaviour {

    public string keyName;
    ScreenPrompt prompt;

	// Use this for initialization
	void Start () {
        prompt = GameObject.Find("ScreenPrompt").GetComponent<ScreenPrompt>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Unlock()
    {   
        for(int i = 0; i < GameManager.instance.inventory.keys.Count; i++)
        {
            if(GameManager.instance.inventory.keys[i].name == keyName)
            {
                GameManager.instance.inventory.RemoveKey(GameManager.instance.inventory.keys[i]);
                Disable();
                //Play Sound, Animation, w/e?
                break;
            }
        }
        /**/
    }

    void Disable()
    {
        /*
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        /**/
        enabled = false;
        prompt.TogglePrompt();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(new InteractionLockedObject(this));
            prompt.TogglePrompt();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(null);
            prompt.TogglePrompt();
        }
    }
}
