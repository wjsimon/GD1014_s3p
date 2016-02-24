using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class IngameMenu : MonoBehaviour {

    bool inMenu;
	public EventSystem myEventSystem;
	public GameObject FirstSelected;
	
	public GameObject inGameOptionFirstS;
	
	public GameObject inventoryFirstS;

	public GameObject mainMenu;
	public GameObject mainMenuFirstS;
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
			if(inMenu==false)
			{
				transform.FindChild("Inventory").gameObject.SetActive(false);
				transform.FindChild("InGameOption").gameObject.SetActive(false);
			}
			myEventSystem.SetSelectedGameObject(null);//deselect and then select again otherwise the selected button wont be highlighted
			myEventSystem.SetSelectedGameObject(FirstSelected);

        }
    }
	public void InGameMenuBack()
	{
		inMenu = false;
		transform.FindChild("P_StartMenu").gameObject.SetActive(inMenu);
	}
	public void InGameMenuShowInvetory(bool show)
	{
		if(show)
		{
			transform.FindChild("P_StartMenu").gameObject.SetActive(false);
			transform.FindChild("Inventory").gameObject.SetActive(true);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(inventoryFirstS);
		}
		else
		{
			transform.FindChild("P_StartMenu").gameObject.SetActive(true);
			transform.FindChild("Inventory").gameObject.SetActive(false);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(FirstSelected);
		}
	}
	public void InGameMenuShowOption(bool show)
	{
		if(show)
		{
			transform.FindChild("P_StartMenu").gameObject.SetActive(false);
			transform.FindChild("InGameOption").gameObject.SetActive(true);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(inGameOptionFirstS);
		}
		else
		{
			transform.FindChild("P_StartMenu").gameObject.SetActive(true);
			transform.FindChild("InGameOption").gameObject.SetActive(false);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(FirstSelected);
		}
	}
	public void InGameMenuQuit()
	{
		inMenu = false;
		transform.FindChild("P_StartMenu").gameObject.SetActive(inMenu);
		mainMenu.SetActive(true);
		myEventSystem.SetSelectedGameObject(null);
		myEventSystem.SetSelectedGameObject(mainMenuFirstS);
	}
	public void ChangeCameraX(bool value)
	{
		int CameraX = value == true ? 1:-1;
		Debug.Log ("Camera X " + CameraX);
		//pass CameraX to player

	}
	public void ChangeCameraY(bool value)
	{
		int CameraY = value == true ? 1:-1;
		Debug.Log ("Camera Y " + CameraY);
		//pass CameraY to player
		
	}
	public void ChangeMusicVolume(float value)
	{
		float MusicVolume = value * 10;
		Debug.Log ("Change Music Volume to " + MusicVolume);
	}
	public void ChangeEffectVolume(float value)
	{
		float effectVolume = value * 10;
		Debug.Log ("Change Effect Volume to " + effectVolume);
	}
	public void ChangeNarratorVolume(float value)
	{
		float NarratorVolume = value * 10;
		Debug.Log ("Change Narrator Volume to " + NarratorVolume);
	}

}
