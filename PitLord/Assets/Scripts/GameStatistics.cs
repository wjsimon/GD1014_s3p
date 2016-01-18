using UnityEngine;
using System.Collections;

public class GameStatistics
{
    private static GameStatistics instance;

    public float time;
    public int deaths;
    public int kills;
    public int upgrades;

    public GameStatistics()
    {
        Init();
    }

    public void Init()
    {

    }

    public void Update()
    {
        time += Time.deltaTime;
    }

    public bool ToTextFile()
    {
        bool export = false;

        return export;
    }

    public static GameStatistics Get()
    {
        if (instance == null)
        {
            instance = new GameStatistics();
        }

        return instance;
    }
}
