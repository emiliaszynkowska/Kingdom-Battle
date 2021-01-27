using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DenController : MonoBehaviour
{
    public GameObject chest;
    public GameObject spikes;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public bool complete;
    public bool active;
    
    void Update()
    {
        if (active && enemy1 == null && enemy2 == null && enemy3 == null)
        {
            complete = true;
            spikes.SetActive(false);
        }
    }
    
}
