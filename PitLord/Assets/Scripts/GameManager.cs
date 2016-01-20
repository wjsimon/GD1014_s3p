using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {


    public Transform playerSpawn;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SpawnPlayer()
    {
        //Spawns Player, Resets Positions (usually on Death) <- Scene Reload pretty much
    }

    public void SoftReset()
    {
        //Reset Enemy Positions, Screen Overlay?
    }
}
