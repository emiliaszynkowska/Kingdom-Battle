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
        message2 = "Hello young elf, where did you come from?";
        message3 = $"Thank you for killing those monsters. Here, take this {reward}.";
    }
    
    void Update()
    {
        if (!complete && (obj1 == null || obj1.name.Equals("Respawn")) && (obj2 == null || obj2.name.Equals("Respawn")) && (obj3 == null || obj3.name.Equals("Respawn")))
        {
            alert.SetActive(false);
            complete = true;
            ScoreManager.AddDefensive(3);
        }
    }
    
}