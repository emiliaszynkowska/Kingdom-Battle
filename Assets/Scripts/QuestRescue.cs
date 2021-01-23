using System.Collections.Generic;
using UnityEngine;

public class QuestRescue : Quest
{
    public GameObject alert;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;

    void Awake()
    {
        List<string> messages = new List<string>()
        {
            "Please help me, I'm being attacked!", 
            "Someone, defeat these monsters!",
            "Rescue me from these monsters!",
            "Young elf, please defeat these monsters!"
        };
        message = messages[Random.Range(0, 4)];
    }
    
    void Update()
    {
        if (!obj1.activeSelf && !obj2.activeSelf && !obj3.activeSelf)
        {
            alert.SetActive(false);
            active = true;
            complete = true;
        }
    }
    
}