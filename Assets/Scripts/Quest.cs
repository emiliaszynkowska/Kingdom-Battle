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
    public string message3;
    public string reward;
    public bool complete;
    public bool talking;
    public bool talkedTo;
    
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
                if (!talkedTo)
                    yield return uiManager.Speak(npcName, message2);
                yield return uiManager.Speak(npcName, message3);
                soundManager.PlaySound(soundManager.complete);
                ScoreManager.AddPuzzleSolving(5);
                if (this is QuestFetch)
                {
                    questManager.AddSideQuest($"Bring {npcName} {GetComponent<QuestFetch>().item}");
                    questManager.Event("Complete a fetch quest", 0, true);
                    questManager.Event($"Bring {npcName} {GetComponent<QuestFetch>().item}", 1, true);
                }
                else if (this is QuestDefeat)
                {
                    questManager.AddSideQuest($"Save {npcName} from the {GetComponent<QuestDefeat>().enemy}");
                    questManager.Event("Complete a defeat quest", 0, true);
                    questManager.Event($"Save {npcName} from the {GetComponent<QuestDefeat>().enemy}", 1, true);
                }
                else if (this is QuestRescue)
                {
                    questManager.AddSideQuest($"Rescue {npcName}");
                    questManager.Event("Complete a rescue quest", 0, true);
                    questManager.Event($"Rescue {npcName}", 1, true);
                }

                StartCoroutine(Disappear());
            }
            else
            {
                talking = true;
                talkedTo = true;
                yield return uiManager.Speak(npcName, message);
                if (this is QuestFetch)
                    questManager.AddSideQuest($"Bring {npcName} {GetComponent<QuestFetch>().item}");
                else if (this is QuestDefeat)
                    questManager.AddSideQuest($"Save {npcName} from the {GetComponent<QuestDefeat>().enemy}");
                else if (this is QuestRescue)
                    questManager.AddSideQuest($"Rescue {npcName}");
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