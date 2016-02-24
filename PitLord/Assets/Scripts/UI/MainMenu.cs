using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public EventSystem myEventSystem;

	public void StartGame()
	{
		transform.FindChild("MainMenu").gameObject.SetActive(false);
	}
	public void Option(bool show)
	{
		if(show)
		{
			transform.FindChild("MainMenu").transform.FindChild("MainMenu").gameObject.SetActive(false);
			transform.FindChild("MainMenu").transform.FindChild("Option").gameObject.SetActive(true);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(transform.FindChild("MainMenu").transform.FindChild("Option").GetChild(0).gameObject);
		}
		else
		{
			transform.FindChild("MainMenu").transform.FindChild("MainMenu").gameObject.SetActive(true);
			transform.FindChild("MainMenu").transform.FindChild("Option").gameObject.SetActive(false);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(transform.FindChild("MainMenu").transform.FindChild("MainMenu").GetChild(0).gameObject);
		}
	}
	public void Credit(bool show)
	{
		if(show)
		{
			transform.FindChild("MainMenu").transform.FindChild("MainMenu").gameObject.SetActive(false);
			transform.FindChild("MainMenu").transform.FindChild("Credit").gameObject.SetActive(true);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(transform.FindChild("MainMenu").transform.FindChild("Credit").GetChild(0).gameObject);
		}
		else
		{
			transform.FindChild("MainMenu").transform.FindChild("MainMenu").gameObject.SetActive(true);
			transform.FindChild("MainMenu").transform.FindChild("Credit").gameObject.SetActive(false);
			myEventSystem.SetSelectedGameObject(null);
			myEventSystem.SetSelectedGameObject(transform.FindChild("MainMenu").transform.FindChild("MainMenu").GetChild(0).gameObject);
		}
	}
	public void QuitGame()
	{

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
	public void ResetGame(bool ShowConfirmBox,bool reset)
	{
		if(ShowConfirmBox)
		{

		}
		else
		{

		}
		if(reset)
		{
			//resetGame
		}
	}
	public void SetScreenSize()
	{

	}

}
