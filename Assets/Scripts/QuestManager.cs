using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestManager : MonoBehaviour
{
    // Game Objects
    public DungeonGenerator dungeonGenerator;
    public SoundManager soundManager;
    public UIManager uiManager;
    public GameObject NPCs;
    public GameObject quests;
    public GameObject mainQuests;
    public GameObject sideQuests;
    // Variables
    public int maxQuests;
    private List<string> listMainQuests = new List<string>();
    //private List<string> listSideQuests = new List<string>();

    public void Start()
    {
        if (!PlayerData.Boss)
        {
            // Gain ability
            // Use ability
            // Find armour
            var difficulty = dungeonGenerator.difficulty;
            if (difficulty >= 75)
                maxQuests = 9;
            if (difficulty >= 50)
                maxQuests = 7;
            if (difficulty >= 25)
                maxQuests = 5;
            else
                maxQuests = 3;
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        while (!dungeonGenerator.generated)
        {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2);
        Generate();
        for (int i = 0; i < maxQuests; i++)
        {
            mainQuests.transform.GetChild(i+1).GetComponent<Text>().text = listMainQuests[i];
        }
    }

    public void Generate()
    {
        int n;
        var generatedQuests = 0;
        while (generatedQuests < maxQuests)
        {
            var questChoice = Random.Range(0, 14);
            switch (questChoice)
            {
                // Talk to NPC
                case 0:
                    if (NPCs.transform.childCount > 0 && listMainQuests.FirstOrDefault(x => x.Contains("Talk to")) == null)
                    {
                        var npcChoice = Random.Range(0, NPCs.transform.childCount - 1);
                        var npcObject = NPCs.transform.GetChild(npcChoice).gameObject;
                        var npcName = npcObject.name;
                        if (npcName.Contains("Wizard"))
                            npcName = npcObject.GetComponent<Quest>().GetName();
                        listMainQuests.Add($"Talk to {npcName}");
                        generatedQuests++;
                    }
                    break;
                // Open chest
                case 1:
                    if (GameObject.Find("Chest") != null && listMainQuests.FirstOrDefault(x => x.Contains("chest")) == null)
                    {
                        n = Random.Range(1, 4);
                        if (n == 1)
                            listMainQuests.Add("Open a chest");
                        else
                            listMainQuests.Add($"Open {n} chests                       0/{n}");
                        generatedQuests++;
                    }
                    break;
                // Open door
                case 2:
                    if (GameObject.Find("Door") != null && listMainQuests.FirstOrDefault(x => x.Contains("door")) == null)
                    {
                        n = Random.Range(1, 4);
                        if (n == 1)
                            listMainQuests.Add("Open a door");
                        else
                            listMainQuests.Add($"Open {n} doors                        0/{n}");
                        generatedQuests++;
                    }
                    break;
                // Find coins
                case 3:
                    if (listMainQuests.FirstOrDefault(x => x.Contains("coins")) == null)
                    {
                        n = Random.Range(5, 30);
                        listMainQuests.Add($"Collect {n} coins                  0/{n}");
                        generatedQuests++;
                    }
                    break;
                // Find item
                case 4:
                    var itemChoice = Random.Range(0, 5);
                    var item = itemChoice == 0 ? "Key" :
                        itemChoice == 1 ? "Wigg's Brew" :
                        itemChoice == 2 ? "Liquid Luck" :
                        itemChoice == 3 ? "Ogre's Strength" : "Elixir of Speed";
                    if (!listMainQuests.Contains($"Find {item}"))
                    {
                        listMainQuests.Add($"Find {item}");
                        generatedQuests++;
                    }
                    break;
                // Buy item
                case 5:
                    if (GameObject.Find("Merchant") != null)
                    {
                        var buyChoice = Random.Range(0, 5);
                        var buy = buyChoice == 0 ? "Key" :
                            buyChoice == 1 ? "Wigg's Brew" :
                            buyChoice == 2 ? "Liquid Luck" :
                            buyChoice == 3 ? "Ogre's Strength" : "Elixir of Speed";
                        if (!listMainQuests.Contains($"Buy {buy}"))
                        {
                            listMainQuests.Add($"Buy {buy}");
                            generatedQuests++;
                        }
                    }
                    break;
                // Use item
                case 6:
                    var useChoice = Random.Range(0, 5);
                    var use = useChoice == 0 ? "Key" :
                        useChoice == 1 ? "Wigg's Brew" :
                        useChoice == 2 ? "Liquid Luck" :
                        useChoice == 3 ? "Ogre's Strength" : "Elixir of Speed";
                    if (!listMainQuests.Contains($"Use {use}"))
                    {
                        listMainQuests.Add($"Use {use}");
                        generatedQuests++;
                    }
                    break;
                // Use powerup
                case 7:
                    var powerupChoice = Random.Range(0, 4);
                    var powerup = powerupChoice == 0 ? "Restore some health" :
                        powerupChoice == 1 ? "Gain an attack boost" :
                        powerupChoice == 2 ? "Poison an enemy" : "Gain a speed boost";
                    if (!listMainQuests.Contains(powerup))
                    {
                        listMainQuests.Add(powerup);
                        generatedQuests++;
                    }
                    break;
                // Locate room
                case 8:
                    if (listMainQuests.FirstOrDefault(x => x.Contains("rooms")) == null)
                    {
                        n = Mathf.Clamp(Random.Range(3, dungeonGenerator.GetDungeons().Count), 3, 30);
                        listMainQuests.Add($"Explore {n} rooms                 0/{n}");
                        generatedQuests++;
                    }
                    break;
                // Fetch quest/Give item/Trade item
                case 9:
                    if (GameObject.Find("WizardFetch") != null)
                    {
                        if (!listMainQuests.Contains("Complete a fetch quest"))
                        {
                            listMainQuests.Add("Complete a fetch quest");
                            generatedQuests++;
                        }
                    }
                    break;
                // Defeat quest
                case 10:
                    if (GameObject.Find("WizardRescue") != null && !listMainQuests.Contains("Complete a defeat quest"))
                    {
                        listMainQuests.Add("Complete a defeat quest");
                        generatedQuests++;
                    }
                    break;
                // Rescue quest
                case 11:
                    if (GameObject.Find("WizardDefeat") != null && !listMainQuests.Contains("Complete a rescue quest"))
                    {
                        listMainQuests.Add("Complete a rescue quest");
                        generatedQuests++;
                    }
                    break;
                // Defeat enemy
                case 12:
                    int enemyChoice;
                    if (dungeonGenerator.difficulty < 25)
                        enemyChoice = 0;
                    else if (dungeonGenerator.difficulty < 50)
                        enemyChoice = Random.Range(0, 2);
                    else if (dungeonGenerator.difficulty < 75)
                        enemyChoice = Random.Range(0, 6);
                    else 
                        enemyChoice = Random.Range(0, 8);
                    var enemy = enemyChoice == 0 ? "Imp" :
                        enemyChoice == 1 ? "Goblin" :
                        enemyChoice == 2 ? "Chort" :
                        enemyChoice == 3 ? "Necromancer" :
                        enemyChoice == 4 ? "Knight" :
                        enemyChoice == 5 ? "Orc" :
                        enemyChoice == 6 ? "Demon" : "Ogre";
                    if (listMainQuests.FirstOrDefault(x => x.Contains(enemy)) == null)
                    {
                        n = Random.Range(1, 6);
                        if (n == 1) 
                            listMainQuests.Add($"Defeat 1 {enemy}                        0/1");
                        else
                            listMainQuests.Add($"Defeat {n} {enemy}s                     0/{n}");
                        generatedQuests++; 
                    }
                    break;
                // Infiltrate den
                case 13:
                    if (GameObject.Find("Den") != null && !listMainQuests.Contains("Infiltrate a monster den"))
                    {
                        listMainQuests.Add("Infiltrate a monster den");
                        generatedQuests++;
                    }
                    break;
            }
        }
    }

    public void Event(string e)
    {
        try
        {
            var index = listMainQuests.FindIndex(x => x.Equals(e)) + 1;
            if (index != 0 && mainQuests.transform.GetChild(index).GetComponent<Text>().color != new Color(0, 0.85f, 0))
            {
                mainQuests.transform.GetChild(index).GetComponent<Text>().color = new Color(0, 0.85f, 0);
                StartCoroutine(uiManager.Notification($"Quest Complete: {e}"));
                soundManager.PlaySound(soundManager.complete);
            }
        }
        catch {}
    }

    public void Event(string e, string s)
    {
        try
        {
            var index = listMainQuests.FindIndex(x => x.Contains($"{e}s")) + 1;
            if (index != 0)
            {
                var text = mainQuests.transform.GetChild(index).GetComponent<Text>().text;
                var count = Regex.Match(text, "(?:\\d+/\\d+)").Value;
                var digits = count.Split('/');
                if (int.Parse(digits[0]) < int.Parse(digits[1]))
                {
                    mainQuests.transform.GetChild(index).GetComponent<Text>().text = text.Replace($"{digits[0]}/{digits[1]}", $"{(int.Parse(digits[0]) + 1).ToString()}/{digits[1]}");
                }
                if (int.Parse(digits[0]) + 1 == int.Parse(digits[1]))
                {
                    mainQuests.transform.GetChild(index).GetComponent<Text>().color = new Color(0, 0.85f, 0);
                    if (int.Parse(digits[1]) == 1)
                        StartCoroutine(uiManager.Notification($"Quest Complete: {s} {digits[1]} {e}"));
                    else
                        StartCoroutine(uiManager.Notification($"Quest Complete: {s} {digits[1]} {e}s"));
                    soundManager.PlaySound(soundManager.complete);
                }
            }
        }
        catch {}
    }

}