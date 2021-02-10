using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DenController : MonoBehaviour
{
    public GameObject chest;
    public GameObject spikes;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public bool complete;
    public bool active;
    
    void FixedUpdate()
    {
        if (active && !complete && (enemy1 == null || enemy1.name.Equals("Respawn")) && (enemy2 == null || enemy2.name.Equals("Respawn")) && (enemy3 == null || enemy3.name.Equals("Respawn")))
        {
            active = false;
            complete = true;
            spikes.SetActive(false);
            ScoreManager.AddPuzzleSolving(3);
        }
    }
    
}
