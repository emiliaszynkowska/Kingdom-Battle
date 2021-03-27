using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestManager : MonoBehaviour
{
    // Game Objects
    public DungeonGenerator dungeonGenerator;
    public DungeonManager dungeonManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public GameObject NPCs;
    public GameObject quests;
    public GameObject specialDoor;
    public Vector2 specialDoorPos;
    public GameObject mainQuests;
    public GameObject sideQuests;
    // Variables
    public int complete;
    public int maxQuests;
    public List<string> listMainQuests = new List<string>();
    private List<string> listSideQuests = new List<string>();

    public void Start()
    {
        if (!PlayerData.Boss && !SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            // Set max quests
            var difficulty = dungeonGenerator.difficulty;
            if (difficulty >= 75)
                maxQuests = 7;
            if (difficulty >= 50)
                maxQuests = 6;
            if (difficulty >= 25)
                maxQuests = 5;
            else
                maxQuests = 3;
            // Add static quests
            if (PlayerData.Level == 3 | PlayerData.Level == 5 | PlayerData.Level == 6 | PlayerData.Level == 8)
            {
                listMainQuests.Add("Talk to Wigg");
                dungeonManager.PlaceWigg(dungeonGenerator.RandomDungeon());
            }
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
        while (listMainQuests.Count < maxQuests)
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
                    }
                    break;
                // Open chest
                case 1:
                    if (GameObject.Find("Chest") != null && listMainQuests.FirstOrDefault(x => x.Contains("chest")) == null)
                    {
                        n = Random.Range(1, dungeonManager.chestsGenerated);
                        n = Mathf.Clamp(n, 0, 5);
                        if (n == 1)
                            listMainQuests.Add("Open a chest");
                        else
                            listMainQuests.Add($"Open {n} chests                    0/{n}");
                    }
                    break;
                // Open door
                case 2:
                    if (GameObject.Find("Door") != null && listMainQuests.FirstOrDefault(x => x.Contains("door")) == null)
                    {
                        n = Random.Range(1, dungeonManager.doorsGenerated);
                        n = Mathf.Clamp(n, 0, 5);
                        if (n == 1)
                            listMainQuests.Add("Open a door");
                        else
                            listMainQuests.Add($"Open {n} doors                     0/{n}");
                    }
                    break;
                // Find coins
                case 3:
                    if (listMainQuests.FirstOrDefault(x => x.Contains("coins")) == null)
                    {
                        n = Random.Range(5, 30);
                        listMainQuests.Add($"Collect {n} coins               0/{n}");
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
                    }
                    break;
                // Locate room
                case 8:
                    if (listMainQuests.FirstOrDefault(x => x.Contains("rooms")) == null)
                    {
                        n = Mathf.Clamp(Random.Range(3, dungeonGenerator.GetDungeons().Count/2), 3, 15);
                        listMainQuests.Add($"Explore {n} rooms              0/{n}");
                    }
                    break;
                // Fetch quest/Give item/Trade item
                case 9:
                    if (GameObject.Find("WizardFetch") != null)
                    {
                        if (!listMainQuests.Contains("Complete a fetch quest"))
                        {
                            listMainQuests.Add("Complete a fetch quest");
                        }
                    }
                    break;
                // Defeat quest
                case 10:
                    if (GameObject.Find("WizardRescue") != null && !listMainQuests.Contains("Complete a defeat quest"))
                    {
                        listMainQuests.Add("Complete a defeat quest");
                    }
                    break;
                // Rescue quest
                case 11:
                    if (GameObject.Find("WizardDefeat") != null && !listMainQuests.Contains("Complete a rescue quest"))
                    {
                        listMainQuests.Add("Complete a rescue quest");
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
                            listMainQuests.Add($"Defeat 1 {enemy}                     0/1");
                        else
                            listMainQuests.Add($"Defeat {n} {enemy}s                  0/{n}");
                    }
                    break;
                // Infiltrate den
                case 13:
                    if (GameObject.Find("Den") != null && !listMainQuests.Contains("Infiltrate a monster den"))
                    {
                        listMainQuests.Add("Infiltrate a monster den");
                    }
                    break;
            }
        }
    }
    
    public List<string> GetQuests()
    {
        return listMainQuests;
    }

    public bool AddMainQuest(string q)
    {
        if (!listMainQuests.Contains(q))
        {
            listMainQuests.Add(q);
            mainQuests.transform.GetChild(listMainQuests.Count).GetComponent<Text>().text = q;
            mainQuests.transform.GetChild(listMainQuests.Count).GetComponent<Text>().color = Color.white;
            StartCoroutine(uiManager.Notification($"Quest Started: {q}", Color.white));
            soundManager.PlaySound(soundManager.item);
            return true;
        }
        return false;
    }
    
    public bool AddSideQuest(string q)
    {
        if (!listSideQuests.Contains(q))
        {
            listSideQuests.Add(q);
            sideQuests.transform.GetChild(listSideQuests.Count).GetComponent<Text>().text = q;
            StartCoroutine(uiManager.Notification($"Quest Started: {q}", Color.white));
            soundManager.PlaySound(soundManager.item);
            return true;
        }
        return false;
    }

    public void CheckQuests()
    {
        if (complete == listMainQuests.Count + 1 && !specialDoor.GetComponent<DoorController>().opened)
        {
            soundManager.PlaySound(soundManager.open);
            StartCoroutine(uiManager.Speak("", "You heard a noise..."));
            specialDoor.GetComponent<DoorController>().opened = true;
        }
    }

    public bool Event(string e, int t, bool c)
    {
        List<string> lst = t == 0 ? listMainQuests : listSideQuests;
        GameObject obj = t == 0 ? mainQuests : sideQuests;
        var index = lst.FindIndex(x => x.Equals(e)) + 1;
        if (index != 0 && obj.transform.GetChild(index).GetComponent<Text>().color != new Color(0, 0.85f, 0))
        {
            var text = obj.transform.GetChild(index).GetComponent<Text>().text;
            obj.transform.GetChild(index).GetComponent<Text>().text = text.Replace("0/1", "1/1");
            obj.transform.GetChild(index).GetComponent<Text>().color = new Color(0, 0.85f, 0);
            StartCoroutine(uiManager.Notification($"Quest Complete: {text}", new Color(0, 0.85f, 0)));
            soundManager.PlaySound(soundManager.complete);
            if (t == 0)
                complete++;
            if (c)
                CheckQuests();
            return true;
        }
        return false;
    }

    public bool Event(string e, string s, bool c)
    {
        var index = listMainQuests.FindIndex(x => x.Contains($"{e}s")) + 1;
        if (index != 0 && mainQuests.transform.GetChild(index).GetComponent<Text>().color != new Color(0, 0.85f, 0))
        {
            var text = mainQuests.transform.GetChild(index).GetComponent<Text>().text;
            var count = Regex.Match(text, "(?:\\d+/\\d+)").Value;
            var digits = count.Split('/');
            if (int.Parse(digits[0]) < int.Parse(digits[1]))
                    mainQuests.transform.GetChild(index).GetComponent<Text>().text = text.Replace(
                        $"{digits[0]}/{digits[1]}", $"{(int.Parse(digits[0]) + 1).ToString()}/{digits[1]}");
            if (int.Parse(digits[0]) + 1 >= int.Parse(digits[1]))
            {
                mainQuests.transform.GetChild(index).GetComponent<Text>().color = new Color(0, 0.85f, 0);
                if (int.Parse(digits[1]) == 1)
                    StartCoroutine(uiManager.Notification($"Quest Complete: {s} {digits[1]} {e}",
                        new Color(0, 0.85f, 0)));
                else
                    StartCoroutine(uiManager.Notification($"Quest Complete: {s} {digits[1]} {e}s",
                        new Color(0, 0.85f, 0)));
                soundManager.PlaySound(soundManager.complete);
                complete++;
                if (c)
                    CheckQuests();
            }
            return true;
        }
        return false;
    }

}