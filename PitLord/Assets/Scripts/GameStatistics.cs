using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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

        StringBuilder log = new StringBuilder();

        int minutes = (int)time / 60;
        int hours = minutes / 60;

        log.Append("play time " + minutes.ToString() + ":" + hours.ToString());
        log.Append("deaths " + deaths.ToString());
        log.Append("enemies killed " + kills.ToString());
        log.Append("upgrades taken " + upgrades.ToString());


        if (File.Exists(Application.dataPath + "stats.txt"))
        {
            File.AppendAllText(Application.dataPath + "stats.txt", log.ToString());
            export = true;
        }
        else
        {
            File.WriteAllText(Application.dataPath + "stats.txt", log.ToString());
            export = true;
        }

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
