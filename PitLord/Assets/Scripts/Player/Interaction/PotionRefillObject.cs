using UnityEngine;
using UnityEngine.UI;
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
            PlayerPrefs.SetInt("GameManager/newSession", 0);
            PlayerPrefs.Save();

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
        //PlayerController p = GameManager.instance.player;
        //p.heals = p.maxHeals;

        ParticleSystem sys = GameObject.Instantiate<ParticleSystem>(Resources.Load<ParticleSystem>("Particles/Prefabs/FX_HealDust"));
        Debug.Log(sys.name);
        sys.transform.position = Camera.main.transform.FindChild("Particles").position;
        sys.transform.forward = Camera.main.transform.forward;
        sys.transform.parent = Camera.main.transform.FindChild("Particles");
        sys.loop = false;
        sys.Play();
        Destroy(sys.gameObject, sys.duration);

        GameObject.Find("DeathScreen").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/placeholder/respawn_screen_white");
        GameManager.instance.StartRespawn();
    }
}
