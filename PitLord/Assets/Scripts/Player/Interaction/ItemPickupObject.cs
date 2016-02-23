using UnityEngine;
using System.Collections;

public class ItemPickupObject : MonoBehaviour {

    public string item;
    ScreenPrompt prompt;

	// Use this for initialization
	void Start () {
        prompt = GameObject.Find("ScreenPrompt").GetComponent<ScreenPrompt>();

        if((PlayerPrefs.GetInt("ItemPickup/" + item) == 1))
        {
            PickUp();
        }
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

        PlayerPrefs.SetInt("ItemPickup/" + item, 1);
        PlayerPrefs.Save();

        GameManager.instance.player.SetInteraction(null);
        prompt.TogglePrompt(false);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter( Collider other )
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(new InteractionItemPickup(this));
            prompt.TogglePrompt(true);
        }
    }
    void OnTriggerExit( Collider other )
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(null);
            prompt.TogglePrompt(false);
        }
    }
}
