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
    public int mutationRate = 5;
    public int totalTime = 15;
    public int time;

    void Start()
    {
        if (!PlayerData.Boss)
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
        if (PlayerData.Wins == PlayerData.Losses | PlayerData.Losses == 0)
            scores[0] = 0.5f;
        else
            scores[0] = ((float) PlayerData.Wins / (float) PlayerData.Losses) / 2; // Success
        if (PlayerData.Kills == PlayerData.Hits)
            scores[1] = 0.5f;
        else if (PlayerData.Kills < PlayerData.Hits)
        {
            scores[1] = ((float) PlayerData.Kills / (float) PlayerData.Hits); // Damage
        }
        else if (PlayerData.Hits < PlayerData.Kills)
        {
            scores[1] = 1 - ((float) PlayerData.Hits / (float) PlayerData.Kills); // Damage
        }
        scores[2] = 1 - (float) PlayerData.PreviousScore; // Previous Score
        scores[3] = (float) questManager.complete / (float) questManager.maxQuests; // Quests
        scores[4] = (float) playerController.health / (float) playerController.livesActive; // Health
        scores[5] = (float) (dungeonGenerator.GetDungeons().Count - dungeonGenerator.dungeonColliders.transform.childCount) / 
                    (float) dungeonGenerator.GetDungeons().Count; // Rooms
        float totalScore = (3 * scores[0] + 2 * scores[1] + 2 * scores[2] + 2 * scores[3] + scores[4] + scores[5])/ 12;
        PlayerData.PreviousScore = totalScore;
        Debug.Log($"Success: {scores[0]}, Damage: {scores[1]}, Previous: {scores[2]}, Quests: {scores[3]}, Health: {scores[4]}, Rooms: {scores[5]}");
        Debug.Log($"Total: {totalScore}");
        // Select Chromosome
        if (totalScore > 0.6f)
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
        else if (totalScore < 0.4f)
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
        Reset();
    }

    void Adjust(int behaviour, int sign)
    {
        switch (behaviour)
        { 
            // Difficulty
            case 0:
                dungeonGenerator.difficulty += sign * learningRate * 5; // difficulty +/- 5
                if (Random.Range(0,100) < mutationRate) 
                    dungeonGenerator.difficulty += sign * learningRate * 5; // mutation difficulty +/- 5
                dungeonGenerator.difficulty = Mathf.Clamp(dungeonGenerator.difficulty, 0, 100);
                uiManager.SetDifficulty(dungeonGenerator.difficulty);
                PlayerData.Difficulty = dungeonGenerator.difficulty;
                Debug.Log($"Difficulty {sign * 5}");
                break;
            // Enemy Speed
            case 1:
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    EnemyController c = enemies.transform.GetChild(i).GetComponent<EnemyController>();
                    c.speed += sign * learningRate; // enemy speed +/- 1
                    if (Random.Range(0,100) < mutationRate)
                        c.speed += sign * learningRate; // mutation enemy speed +/- 1
                    c.speed = Mathf.Clamp(c.speed, 10, 30);
                }
                Debug.Log($"Enemy Speed {sign}");
                break;
            // Enemy Attack Time
            case 2:
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    // Magic Time
                    EnemyController c = enemies.transform.GetChild(i).GetComponent<EnemyController>();
                    c.magicTime += sign * learningRate; // magic time +/- 1
                    if (Random.Range(0,100) < mutationRate)
                        c.magicTime += sign * learningRate; // mutation magic time +/- 1
                    c.magicTime = Mathf.Clamp(c.magicTime, 1, 10);
                    // Spawn Time
                    c.spawnTime += sign * learningRate; // spawn time +/- 1
                    if (Random.Range(0,100) < mutationRate)
                        c.spawnTime += sign * learningRate; // mutation spawn time +/- 1
                    c.spawnTime = Mathf.Clamp(c.spawnTime, 1, 10);
                    // Ground Pound Time
                    c.groundPoundTime += sign * learningRate; // ground pound time +/- 1
                    if (Random.Range(0,100) < mutationRate)
                        c.groundPoundTime += sign * learningRate; // mutation ground pound time +/- 1
                    c.groundPoundTime = Mathf.Clamp(c.groundPoundTime, 1, 10);
                }
                Debug.Log($"Enemy Attack Time {sign}");
                break;
            // Enemy Health
            case 3:
                for (int i = 0; i < enemies.transform.childCount; i++)
                {
                    EnemyController c = enemies.transform.GetChild(i).GetComponent<EnemyController>();
                    c.health += sign * learningRate; // enemy health +/- 1
                    if (Random.Range(0,100) < mutationRate)
                        c.health += sign * learningRate; // mutation enemy health +/- 1
                    var max = PlayerData.Difficulty >= 75 ? 20 : PlayerData.Difficulty >= 50 ? 10 : PlayerData.Difficulty >= 25 ? 5 : 3;
                    c.health = Mathf.Clamp(c.health, 1, max);
                }
                Debug.Log($"Enemy Health {sign}");
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
                                Destroy(NPCs.transform.GetChild(npcIndex).gameObject); // NPCs - 1
                                break;
                            }
                        }
                    }
                }
                Debug.Log($"NPCs {sign}");
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
                                Destroy(objects.transform.GetChild(objIndex).gameObject); // objects - 1
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
                Debug.Log($"Objects {sign}");
                break;
        }
    }

    void Reset()
    {
        time = totalTime;
        StartCoroutine(Collect());
    }

}
