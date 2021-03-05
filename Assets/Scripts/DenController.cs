using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DenController : MonoBehaviour
{
    public QuestManager questManager;
    public SoundManager soundManager;
    private GameObject player;
    public GameObject chest;
    public GameObject spikes;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public bool complete;
    public bool active;

    void Start()
    {
        questManager = GameObject.Find("UI").GetComponent<QuestManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
        player = GameObject.Find("Player");
        StartCoroutine(Growl());
    }

    void FixedUpdate()
    {
        if (active && !complete && (enemy1 == null || enemy1.name.Equals("Respawn")) && (enemy2 == null || enemy2.name.Equals("Respawn")) && (enemy3 == null || enemy3.name.Equals("Respawn")))
        {
            active = false;
            complete = true;
            spikes.SetActive(false);
            soundManager.PlaySound(soundManager.complete);
            questManager.Event("Infiltrate a monster den", 0);
            ScoreManager.AddPuzzleSolving(5);
        }
    }
    
    IEnumerator Growl()
    {
        while (active)
        {
            yield return new WaitForSeconds(3);
            if (Mathf.Abs(player.transform.position.magnitude - transform.position.magnitude) < 5 && Time.timeScale == 1)
                soundManager.PlaySound(soundManager.monsterDamage);
        }
    }

}
