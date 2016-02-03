using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public GameObject player;
    public Transform playerSpawn;
    public Inventory inventory;
    public List<GameObject> enemyList = new List<GameObject>();
    public List<GameObject> objectsList = new List<GameObject>();
    public List<GameObject> alpacaList = new List<GameObject>();

    public GameManager()
    {
        instance = this;
    }

    //ITEM LIST List<PickUp> pickUpList= new List<PickUp>();
    //COMBAT List<CombatZone> combatZones = new List<CombatZone>();

    // Use this for initialization
    void Start () 
    {
        //Time.timeScale = 0.1f;
        /*
        GameObject[] collector;
        collector = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < collector.Length; i++)
        {
            enemyList.Add(collector[i]);
        }

        /*
        collector = GameObject.FindGameObjectsWithTag("DesObj");
        for (int i = 0; i < collector.Length; i++)
        {
            objectsList.Add(collector[i]);
        }
        /**/
    }

    // Update is called once per frame
    void Update () {

        if(Input.GetButtonDown("Reset"))
        {
            SoftReset();
        }
    }

    public void SpawnPlayer()
    {
        //Spawns Player, Resets Positions (usually on Death) <- Scene Reload pretty much
    }

    public void DisableInputStartMenu()
    {
        //
    }
    public void DisableInputInMenuScreens()
    {

    }

    public void SoftReset()
    {
        //Reset Enemy Positions, Screen Overlay?
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].GetComponent<Attributes>().SoftReset();
        }
    }

    public void AddEnemy(GameObject obj)
    {
        enemyList.Add(obj);

        //Debug.LogWarning(enemyList.Count);
    }
    public void RemoveEnemy(GameObject obj)
    {
        enemyList.Remove(obj);
    }

    public void AddObject(GameObject obj)
    {
        objectsList.Add(obj);
    }
    public void RemoveObject( GameObject obj )
    {
        objectsList.Remove(obj);
    }

    public void GameOver()
    {

    }
}
