using UnityEngine;
using System.Collections;

public class ItemPickupObject : MonoBehaviour
{

    public string item;
    ScreenPrompt prompt;

    public enum ItemType
    {
        ITEM,
        KEY,
        COUNT,
    }

    public ItemType itemType;

    void Start()
    {
        prompt = GameObject.Find("ScreenPrompt").GetComponent<ScreenPrompt>();

        if ((PlayerPrefs.GetInt("ItemPickup/" + item) == 1))
        {
            PickUp();
        }
    }

    void Update()
    {
    }

    public void PickUp()
    {
        if (item != null)
        {
            if (itemType == ItemType.ITEM) { GameManager.instance.inventory.AddItem(item); }
            if (itemType == ItemType.KEY) { GameManager.instance.inventory.AddKey(item); }
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
