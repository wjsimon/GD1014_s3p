using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public PlayerController player;
    public Inventory inventory = new Inventory();
    public List<Enemy> enemyList = new List<Enemy>();
    public List<CombatTrigger> triggerList = new List<CombatTrigger>();
    public List<DestructableObject> objectsList = new List<DestructableObject>();
    public List<Character> alpacaList = new List<Character>();

    float playGameOver;
    public float respawnTimer;

    public GameState currentGameState;
    public enum GameState
    {
        MAINMENU,
        INGAME,
        GAMEOVER
    }

    public GameManager()
    {
        if (instance != null)
        {
            Destroy(this);

            instance.inventory = inventory;

            return;
        }

        instance = this;
    }

    //ITEM LIST List<PickUp> pickUpList= new List<PickUp>();
    //COMBAT List<CombatZone> combatZones = new List<CombatZone>();

    // Use this for initialization
    void Start()
    {
        inventory.Start();
        currentGameState = GameState.INGAME;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            SoftReset();
        }
        if (Input.GetButtonDown("GameOver"))
        {
            GameOver();
        }

        if ((player.isDead() || respawnTimer > 0) && currentGameState == GameState.INGAME)
        {
            if (respawnTimer <= 0)
            {
                StartRespawn();
            }

            if (respawnTimer > 0)
            {
                respawnTimer -= Time.deltaTime;
                GameObject.Find("DeathScreen").GetComponent<FadeImage>().FadeIn();

                if (respawnTimer <= 0f)
                {
                    respawnTimer = 0;
                    RespawnPlayer();
                    GameObject.Find("DeathScreen").GetComponent<FadeImage>().FadeOut();
                }
            }
        }
    }

    public void StartRespawn()
    {
        respawnTimer = 1.1f;
    }
    public void RespawnPlayer()
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
            enemyList[i].animator.SetTrigger("Reset");
        }
        for (int i = 0; i < triggerList.Count; i++)
        {
            triggerList[i].GetComponent<CombatTrigger>().triggered = false;
        }

        player.SoftReset();
        Camera.main.GetComponent<CameraController>().ResetCam();
    }

    public void AddEnemy( Enemy obj )
    {
        enemyList.Add(obj);
    }
    public void RemoveEnemy( Enemy obj )
    {
        enemyList.Remove(obj);
    }
    public void AddObject( DestructableObject obj )
    {
        objectsList.Add(obj);
    }
    public void RemoveObject( DestructableObject obj )
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
        currentGameState = GameState.GAMEOVER;
        playGameOver = 5.0f;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
