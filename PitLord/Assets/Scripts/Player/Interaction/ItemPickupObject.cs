using UnityEngine;
using System.Collections;

public class ItemPickupObject : MonoBehaviour {

    public Item item;
    ScreenPrompt prompt;
	// Use this for initialization
	void Start () {
        prompt = GameObject.Find("ScreenPrompt").GetComponent<ScreenPrompt>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void PickUp()
    {
        if(item != null)
        {
            GameManager.instance.inventory.AddItem(item);
        }

        GameManager.instance.player.SetInteraction(null);
        prompt.TogglePrompt();
        Destroy(gameObject);
    }

    void OnTriggerEnter( Collider other )
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(new InteractionItemPickup(this));
            prompt.TogglePrompt();
        }
    }
    void OnTriggerExit( Collider other )
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(null);
            prompt.TogglePrompt();
        }
    }
}
