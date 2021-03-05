using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Quest : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    public QuestManager questManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public string npcName;
    public string message;
    public string message2;
    public string reward;
    public bool complete;
    public bool talking;
    
    List<string> npcNames = new List<string>() {"Rhys", "Samuel", "Lucius", "Jedediah", "Matthew", "David"};
    List<string> rewards = new List<string>() {"Key", "Wigg's Brew", "Liquid Luck", "Ogre's Strength", "Elixir of Speed"};
    
    public void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        questManager = GameObject.Find("UI").GetComponent<QuestManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
        npcName = npcNames[Random.Range(0, 6)];
        reward = rewards[Random.Range(0, 5)];
    }

    public void FixedUpdate()
    {
        if (player.transform.position.x - transform.position.x >= 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else
            GetComponent<SpriteRenderer>().flipX = true;
    }

    public string GetName()
    {
        return npcName;
    }

    public IEnumerator Talk()
    {
        if (!talking)
        {
            if (complete)
            {
                talking = true;
                uiManager.AddItem(reward, playerController.GetInventory().Count);
                playerController.AddItem(reward);
                uiManager.StartSpeak(npcName, message2);
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                uiManager.StopSpeak();
                soundManager.PlaySound(soundManager.complete);
                ScoreManager.AddPuzzleSolving(3);
                if (this is QuestFetch)
                {
                    questManager.AddQuest($"Bring {npcName} {GetComponent<QuestFetch>().item}");
                    questManager.Event("Complete a fetch quest", 0);
                    questManager.Event($"Bring {npcName} {GetComponent<QuestFetch>().item}", 1);
                }
                else if (this is QuestDefeat)
                {
                    questManager.AddQuest($"Save {npcName} from the {GetComponent<QuestDefeat>().enemy}");
                    questManager.Event("Complete a defeat quest", 0);
                    questManager.Event($"Save {npcName} from the {GetComponent<QuestDefeat>().enemy}", 1);
                }
                else if (this is QuestRescue)
                {
                    questManager.AddQuest($"Rescue {npcName}");
                    questManager.Event("Complete a rescue quest", 0);
                    questManager.Event($"Rescue {npcName}", 1);
                }

                StartCoroutine(Disappear());
            }
            else
            {
                talking = true;
                uiManager.StartSpeak(npcName, message);
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                if (this is QuestFetch)
                    questManager.AddQuest($"Bring {npcName} {GetComponent<QuestFetch>().item}");
                else if (this is QuestDefeat)
                    questManager.AddQuest($"Save {npcName} from the {GetComponent<QuestDefeat>().enemy}");
                else if (this is QuestRescue)
                    questManager.AddQuest($"Rescue {npcName}");
                uiManager.StopSpeak();
                talking = false;
            }
        }
    }

    IEnumerator Disappear()
    {
        for (int i=0; i < 3000; i++)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            transform.Translate(Vector3.right * (Time.deltaTime * 2));
            yield return null;
        }
        gameObject.SetActive(false);
    }
    
}