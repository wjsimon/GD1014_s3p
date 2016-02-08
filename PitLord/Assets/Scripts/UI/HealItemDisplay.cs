using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealItemDisplay : MonoBehaviour {

    Image image;
    Sprite[] sprites;
   
	// Use this for initialization
	void Start () {

        image = GetComponent<Image>();
        sprites = Resources.LoadAll<Sprite>("UI/placeholder/HealItemDisplay");
        UpdateDisplay();
        
        for(int i = 0; i < sprites.Length; i++)
        {
            Debug.Log(sprites[i].name);
        }
        /**/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateDisplay()
    {
        int heals = GameManager.instance.player.heals > 0 ? GameManager.instance.player.heals : 5;
        Sprite current;
        Debug.Log(sprites.Length + " " + heals);
        current = sprites[heals-1];
        image.sprite = current;
    }
}
