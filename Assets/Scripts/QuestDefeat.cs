using System.Collections.Generic;
using UnityEngine;

public class QuestDefeat : Quest
{
    public string enemy;
    public GameObject obj;
    
    void Awake()
    {
        List<string> messages = new List<string>()
        {
            "I'm terrified of that monster over there. Please defeat it!", 
            "If you see a monster in this room, kill it and come back to me.",
            "I saw a monster earlier. I'll move if you can defeat it."
        };
        message = messages[Random.Range(0, 3)];
        message2 = "Thank you for killing that monster. Here, take this *.";
        message2 = message2.Replace("*", reward);
    }
    
    void Update()
    {
        if (!complete && (obj == null || obj.name.Equals("Respawn")))
        {
            complete = true;
            ScoreManager.AddAggressive(3);
        }
    }

}