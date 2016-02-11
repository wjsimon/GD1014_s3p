using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealItemDisplay : MonoBehaviour {

    Image image;
    Sprite[] sprites;
    int heals;

    // Use this for initialization
    void Start () {
        image = GetComponent<Image>();
        sprites = Resources.LoadAll<Sprite>("UI/placeholder/HealItemDisplay");
        /**/
	}
	
	// Update is called once per frame
	void Update () {
        heals = GameManager.instance.player.heals;
        image.sprite = sprites[heals];
    }
}
