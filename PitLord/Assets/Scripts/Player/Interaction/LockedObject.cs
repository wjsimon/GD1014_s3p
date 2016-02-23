using UnityEngine;
using System.Collections;

public class LockedObject : MonoBehaviour
{
    public string keyName;
    public bool disabled;
    ScreenPrompt prompt;

    // Use this for initialization
    void Start()
    {
        disabled = false;
        prompt = GameObject.Find("ScreenPrompt").GetComponent<ScreenPrompt>();

        if((PlayerPrefs.GetInt("LockedObject/" + keyName) == 1))
        {
            Disable();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Unlock()
    {
        if(keyName == "")
        {
            Disable();
            GameManager.instance.player.SetInteraction(null);
            return;
        }

        for (int i = 0; i < GameManager.instance.inventory.items.Count; i++)
        {
            if (GameManager.instance.inventory.items[i] == keyName)
            {
                //GameManager.instance.inventory.RemoveKey(GameManager.instance.inventory.keys[i]);
                Disable();
                GameManager.instance.player.SetInteraction(null);
                //Play Sound, Animation, w/e?
                break;
            }
        }
        /**/
    }

    void Disable()
    {
        Collider[] colliders = GetComponents<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        if (GetComponent<MeshRenderer>() != null) { GetComponent<MeshRenderer>().enabled = false; }
        if (GetComponent<NavMeshObstacle>() != null) { GetComponent<NavMeshObstacle>().enabled = false; }

        /**/
        disabled = true;
        prompt.TogglePrompt(false);

        PlayerPrefs.SetInt("LockedObject/" + keyName, 1);
        PlayerPrefs.Save();
    }

    void OnTriggerEnter( Collider other )
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(new InteractionLockedObject(this));
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
