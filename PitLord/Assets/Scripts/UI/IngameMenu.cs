using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngameMenu : MonoBehaviour {

    bool inMenu;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        MenuControl();
	}

    void MenuControl()
    {
        if(Input.GetButtonDown("Menu"))
        {
            inMenu = !inMenu;
            transform.FindChild("P_StartMenu").gameObject.SetActive(inMenu);
        }
    }
}
