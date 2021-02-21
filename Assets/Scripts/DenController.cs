using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DenController : MonoBehaviour
{
    public SoundManager soundManager;
    public GameObject chest;
    public GameObject spikes;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public bool complete;
    public bool active;

    void Start()
    {
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
        StartCoroutine(PlaySounds());
    }

    void FixedUpdate()
    {
        if (active && !complete && (enemy1 == null || enemy1.name.Equals("Respawn")) && (enemy2 == null || enemy2.name.Equals("Respawn")) && (enemy3 == null || enemy3.name.Equals("Respawn")))
        {
            active = false;
            complete = true;
            spikes.SetActive(false);
            soundManager.PlaySound(soundManager.complete);
            ScoreManager.AddPuzzleSolving(3);
        }
    }

    IEnumerator PlaySounds()
    {
        while (!complete)
        {
            soundManager.PlaySound(soundManager.monster);
            yield return new WaitForSeconds(3);
        }
    }
    
}
