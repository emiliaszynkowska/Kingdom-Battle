using System;
using System.Collections;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;
    public int totalTime = 10;
    public int time;

    void Start()
    {
        Reset();
    }

    IEnumerator Collect()
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
        }
        Process();
    }

    void Process()
    {
        var adjustment = 0;
        if (PlayerData.Wins != 0 && PlayerData.Losses != 0)
        {
            // Check Wins
            if (PlayerData.Wins / PlayerData.Losses > 1.25)
                adjustment++;
            // Check Losses
            else if (PlayerData.Losses / PlayerData.Wins > 1.25)
                adjustment--;
        }
        if (PlayerData.Kills != 0 && PlayerData.Hits != 0)
        {
            // Check Kills
            if (PlayerData.Kills / PlayerData.Hits > 1.25)
                adjustment++;
            // Check Hits
            else if (PlayerData.Hits / PlayerData.Kills > 1.25)
                adjustment--;
        }
        if (adjustment > 0)
            Adjust(1);
        else if (adjustment < 0)
            Adjust(-1);
    }

    void Adjust(int i)
    {
        if (i == 1)
            dungeonGenerator.AddDifficulty();
        else if (i == -1)
            dungeonGenerator.SubtractDifficulty();
        Reset();
    }

    void Reset()
    {
        PlayerData.Kills = 0;
        PlayerData.Hits = 0;
        time = totalTime;
        StartCoroutine(Collect());
    }
    
}
