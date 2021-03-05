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
    public bool active;
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
                    questManager.Event("Complete a fetch quest");
                else if (this is QuestDefeat)
                    questManager.Event("Complete a defeat quest");
                else if (this is QuestRescue)
                    questManager.Event("Complete a rescue quest");
                StartCoroutine(Disappear());
            }
            else
            {
                talking = true;
                uiManager.StartSpeak(npcName, message);
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                uiManager.StopSpeak();
                talking = false;
                active = true;
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