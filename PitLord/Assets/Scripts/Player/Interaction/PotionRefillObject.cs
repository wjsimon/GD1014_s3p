using UnityEngine;
using System.Collections;

public class PotionRefillObject : MonoBehaviour {

    ScreenPrompt prompt;
	// Use this for initialization
	void Start () {
        prompt = GameObject.Find("ScreenPrompt").GetComponent<ScreenPrompt>();
    }
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(new InteractionPotionRefill(this));
            prompt.TogglePrompt(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.player.SetInteraction(null);
            prompt.TogglePrompt(false);
        }
    }

    public void Refill()
    {
        PlayerController p = GameManager.instance.player;
        p.heals = p.maxHeals;

        GameManager.instance.StartRespawn();
    }
}
