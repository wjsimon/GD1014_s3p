using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public PlayerController player;
    public Transform playerSpawn;
    public Inventory inventory = new Inventory();
    public List<Enemy> enemyList = new List<Enemy>();
    public List<CombatTrigger> triggerList = new List<CombatTrigger>();
    public List<DestructableObject> objectsList = new List<DestructableObject>();
    public List<Character> alpacaList = new List<Character>();

    float playGameOver;

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
    void Update () 
    {
        if(Input.GetButtonDown("Reset"))
        {
            SoftReset();
        }
    }

    public void SpawnPlayer()
    {
        //Spawns Player, Resets Positions (usually on Death) <- Scene Reload pretty much
        SoftReset();
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
        for (int i = 0; i < triggerList.Count; i++)
        {
            triggerList[i].GetComponent<CombatTrigger>().triggered = false;
        }

        player.SoftReset();
    }

    public void AddEnemy(Enemy obj)
    {
        enemyList.Add(obj);

        //Debug.LogWarning(enemyList.Count);
    }
    public void RemoveEnemy(Enemy obj )
    {
        enemyList.Remove(obj);
    }

    public void AddObject(DestructableObject obj)
    {
        objectsList.Add(obj);
    }
    public void RemoveObject(DestructableObject obj )
    {
        objectsList.Remove(obj);
    }
    public void AddCombatTrigger( CombatTrigger obj )
    {
        triggerList.Add(obj);
    }
    public void RemoveCombatTrigger( CombatTrigger obj )
    {
        triggerList.Remove(obj);
    }

    public void GameOver()
    {
        playGameOver = 5.0f;
        SpawnPlayer();
    }
}
