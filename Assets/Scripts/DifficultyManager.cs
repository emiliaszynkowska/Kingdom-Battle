using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DifficultyManager : MonoBehaviour
{
    // Game Objects
    public PlayerController playerController;
    public DungeonGenerator dungeonGenerator;
    public DungeonManager dungeonManager;
    public QuestManager questManager;
    public UIManager uiManager;
    public GameObject objects;
    public GameObject enemies;
    public GameObject NPCs;
    // Variables
    public int learningRate = 1;
    public int mutationRate = 10;
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
        // Calculate Distance Score
        float myDist = Math.Abs(questManager.specialDoorPos.magnitude - playerController.transform.position.magnitude);
        float totalDist = Math.Abs(questManager.specialDoorPos.magnitude - dungeonManager.campfirePos.magnitude);
        // Calculate Total Score
        float[] scores = new float[6];
        scores[0] = (PlayerData.Wins / PlayerData.Losses) / 2; // Success
        scores[1] = (PlayerData.Kills / PlayerData.Hits) / 2; // Damage
        scores[2] = PlayerData.PreviousScore; // Previous Score
        scores[3] = questManager.complete / questManager.maxQuests; // Quests
        scores[4] = playerController.health / playerController.livesActive; // Health
        scores[5] = (dungeonGenerator.GetDungeons().Count - dungeonGenerator.dungeonColliders.transform.childCount) / 
                    dungeonGenerator.GetDungeons().Count; // Rooms
        float totalScore = (3 * scores[0] + 3 * scores[1] + 2 * scores[2] + 2 * scores[3] + scores[4] + scores[5])/ 12;
        PlayerData.PreviousScore = totalScore;
        // Select Chromosome
        if (totalScore <= 0.5f)
        {
            // Win 
            for (int i = 0; i < 6; i++)
            {
                if (Random.Range(0.0f, 1.0f) < myDist / totalDist)
                {
                   Adjust(i,1); 
                }
            }
        }
        else if (totalScore > 0.5f)
        {
           // Lose
           for (int i = 0; i < 6; i++)
           {
               if (Random.Range(0.0f, 1.0f) < 1 - myDist / totalDist)
               {
                    Adjust(i,-1);
               }
           }
        }
    }

    void Adjust(int behaviour, int sign)
    {
        switch (behaviour)
        { 
            // Difficulty
            case 0:
                if (dungeonGenerator.difficulty >= 5 && dungeonGenerator.difficulty <= 95) 
                    dungeonGenerator.difficulty += sign * learningRate * 5; // difficulty +/- 5
                if (Random.Range(0,100) < mutationRate && dungeonGenerator.difficulty >= 5 && dungeonGenerator.difficulty <= 95) 
                    dungeonGenerator.difficulty += sign * learningRate * 5; // mutation difficulty +/- 5
                uiManager.SetDifficulty(dungeonGenerator.difficulty);
                PlayerData.Difficulty = dungeonGenerator.difficulty;
                break;
            // Enemy Speed
            case 1:
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    EnemyController c = enemies.transform.GetChild(i).GetComponent<EnemyController>();
                    if (c.speed > 5 && c.speed <= 25) 
                        c.speed += sign * learningRate * 5; // enemy speed +/- 5
                    if (Random.Range(0,100) < mutationRate && c.speed > 5 && c.speed <= 25)
                        c.speed += sign * learningRate * 5; // mutation enemy speed +/- 5
                }
                break;
            // Enemy Attacks
            case 2:
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    // Magic Time
                    EnemyController c = enemies.transform.GetChild(i).GetComponent<EnemyController>();
                    if (c.magicTime > 0 && c.magicTime < 10)
                        c.magicTime += sign * learningRate; // magic time +/- 1
                    if (Random.Range(0,100) < mutationRate && c.magicTime > 0 && c.magicTime < 10)
                        c.magicTime += sign * learningRate; // mutation magic time +/- 1
                    // Spawn Time
                    if (c.spawnTime > 0 && c.spawnTime < 10)
                        c.spawnTime += sign * learningRate; // spawn time +/- 1
                    if (Random.Range(0,100) < mutationRate && c.spawnTime > 0 && c.spawnTime < 10)
                        c.spawnTime += sign * learningRate; // mutation spawn time +/- 1
                    // Ground Pound Time
                    if (c.groundPoundTime > 0 && c.groundPoundTime < 10)
                        c.groundPoundTime += sign * learningRate; // ground pound time +/- 1
                    if (Random.Range(0,100) < mutationRate && c.groundPoundTime > 0 && c.groundPoundTime < 10)
                        c.groundPoundTime += sign * learningRate; // mutation ground pound time +/- 1
                }
                break;
            // Enemy Health
            case 3:
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    EnemyController c = enemies.transform.GetChild(i).GetComponent<EnemyController>();
                    if (c.health > 1 && c.health < 30)
                        c.health += sign * learningRate; // enemy health +/- 1
                    if (Random.Range(0,100) < mutationRate && c.health > 1 && c.health < 30)
                        c.speed += sign * learningRate; // mutation enemy health +/- 1
                }
                break;    
            // NPCs
            case 4:
                var npcName = "";
                var npcIndex = -1;
                int n = learningRate;
                if (Random.Range(0, 100) < mutationRate)
                    n += 1; // mutation NPCs +/- 1
                for (int i = 0; i < n; i++)
                {
                    if (sign > 0) // positive case
                    {
                        int choice1 = Random.Range(0, 2);
                        switch (choice1) 
                        {
                            case 0:
                                dungeonManager.PlaceMerchant(dungeonGenerator.GetDungeons()
                                    [Random.Range(0, dungeonGenerator.GetDungeons().Count)]); // NPCs + 1
                                break;
                            case 1:
                                dungeonManager.PlaceWizard(dungeonManager.RandomPosition(dungeonGenerator.GetDungeons()
                                    [Random.Range(0, dungeonGenerator.GetDungeons().Count)].room, false), PlayerData.Difficulty); // NPCs + 1
                                break;
                        }
                    }
                    else if (sign < 0) // negative case
                    {
                        for (int j = 0; j < NPCs.transform.childCount; j++)
                        {
                            if (npcName.Equals("") && npcIndex == -1 && Math.Abs(NPCs.transform.GetChild(j).position.magnitude - playerController.transform.position.magnitude) > 10)
                            {
                                npcName = NPCs.transform.GetChild(i).name;
                                npcIndex = j;
                            }
                            else if (j != npcIndex && NPCs.transform.GetChild(j).name.Equals(npcName))
                            {
                                Destroy(NPCs.transform.GetChild(npcIndex)); // NPCs - 1
                                break;
                            }
                        }
                    }
                }
                break;
            // Objects
            case 5:
                var objName = "";
                var objIndex = -1;
                int l = learningRate;
                if (Random.Range(0, 100) < mutationRate)
                    l += 1; // mutation NPCs +/- 1
                for (int i = 0; i < l; i++)
                {
                    if (sign > 0) // positive case
                    {
                        for (int j = 0; j < objects.transform.childCount; j++)
                        {
                            if (objName.Equals("") && objIndex == -1 && Math.Abs(objects.transform.GetChild(j).position.magnitude - playerController.transform.position.magnitude) > 10)
                            {
                                objName = objects.transform.GetChild(i).name;
                                objIndex = j;
                            }
                            else if (j != objIndex && objects.transform.GetChild(j).name.Equals(objName))
                            {
                                Destroy(objects.transform.GetChild(objIndex)); // objects - 1
                                break;
                            }
                        }
                    }
                    else if (sign < 0) // negative case
                    {
                        int choice1 = Random.Range(0, 2);
                        switch (choice1) 
                        {
                            case 0:
                                dungeonGenerator.PlaceDoor(); // objects + 1
                                break;
                            case 1:
                                dungeonManager.PlaceChest(dungeonGenerator.GetDungeons()
                                    [Random.Range(0, dungeonGenerator.GetDungeons().Count)]); // objects + 1
                                break;
                        }
                    }
                }
                break;
        }
    }

    void Reset()
    {
        PlayerData.Kills = 0;
        PlayerData.Hits = 0;
        time = totalTime;
        StartCoroutine(Collect());
    }
    
}
