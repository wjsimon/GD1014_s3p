using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameStatistics stats;
    public PlayerController player;
    public Narrator narrator;
    public Inventory inventory = new Inventory();
    public List<Enemy> enemyList = new List<Enemy>();
    public List<CombatTrigger> triggerList = new List<CombatTrigger>();
    public List<DestructableObject> objectsList = new List<DestructableObject>();
    public List<Alpaca> alpacaList = new List<Alpaca>();

    float playGameOver;
    public bool newSession;
    public float respawnTimer;
    public float idleTimer;
    public float inCombatTimer;
    public float inCombatRandom;
    public bool inCombat;

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

        if (player == null) { player = GameObject.Find("Player").GetComponent<PlayerController>(); }
        if (narrator == null) { narrator = GameObject.Find("Narrator").GetComponent<Narrator>(); }


        newSession = PlayerPrefs.GetInt("GameManager/newSession", 1) <= 0 ? false : true;

        
        if(!newSession)
        {
            RespawnPlayer();
        }
        /**/
    }

    // Update is called once per frame
    void Update()
    {
        CheckIdle();
        CheckInCombat();
        CheckPlayerDeath();

        if (Input.GetButtonDown("Reset"))
        {
            SoftReset();
        }
        if (Input.GetButtonDown("GameOver"))
        {
            GameOver();
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
        narrator.PlayDeath();
    }

    public void CheckIdle()
    {
        //InputCheck for IdleLines
        if (!(Input.anyKey || (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) >= 0.1f))
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= 30.0f)
            {
                narrator.PlayIdle();
                idleTimer = 0;
            }
        }
        else
        {
            idleTimer = 0;
        }

        if(Input.GetButtonDown("PlayIdle"))
        {
            narrator.PlayIdle();
        }
    }

    public void CheckInCombat()
    {
        if(!inCombat) { return; }

        inCombatTimer += Time.deltaTime;

        if (inCombatTimer >= inCombatRandom)
        {
            narrator.PlayInCombat();
            inCombatTimer = 0;
            inCombatRandom = Random.Range(10, 31);
        }

        else
        {
            inCombatTimer = 0;
        }
    }

    public void EnterCombat()
    {
        inCombat = true;
        inCombatTimer = 0;
        inCombatRandom = Random.Range(10, 31);

        Debug.Log("Combat entered...");
    }
    public void ExitCombat()
    {
        inCombat = false;
        inCombatTimer = 0;

        narrator.PlayOnCombatWin();

        Debug.Log("Combat exited...");
    }

    public void CheckPlayerDeath()
    {
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

    public void DisableInputStartMenu()
    {
        //
    }
    public void DisableInputInMenuScreens()
    {

    }

    public void SoftReset()
    {
        player.SoftReset();
        //Reset Enemy Positions, Screen Overlay?
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].GetComponent<Attributes>().SoftReset();
            enemyList[i].animator.SetTrigger("Reset");
        }
        for (int i = 0; i < triggerList.Count; i++)
        {
            triggerList[i].GetComponent<CombatTrigger>().SoftReset();
        }

        Camera.main.GetComponent<CameraController>().ResetCam();
    }

    public bool AllAlpacasDead()
    {
        int cnt=0;
        alpacaList.ForEach(al => { cnt += al.isDead() ? 1 : 0; });
        return cnt==alpacaList.Count;
    }

    public void AddEnemy(Enemy obj)
    {
        enemyList.Add(obj);
    }
    public void RemoveEnemy(Enemy obj)
    {
        enemyList.Remove(obj);
    }
    public void AddObject(DestructableObject obj)
    {
        objectsList.Add(obj);
    }
    public void RemoveObject(DestructableObject obj)
    {
        objectsList.Remove(obj);
    }
    public void AddCombatTrigger(CombatTrigger obj)
    {
        triggerList.Add(obj);
    }
    public void RemoveCombatTrigger(CombatTrigger obj)
    {
        triggerList.Remove(obj);
    }

    public void GameOver()
    {
        currentGameState = GameState.GAMEOVER;
        playGameOver = 5.0f;

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("GameManager/newSession", 1);
        PlayerPrefs.Save();
    }
}
