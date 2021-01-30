using System.Collections.Generic;
using UnityEngine;

public class QuestDefeat : Quest
{
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
    }
    
    void Update()
    {
        if (active && !complete && (obj == null || obj.name.Equals("Respawn")))
        {
            complete = true;
            ScoreManager.AddAggressive(3);
        }
    }

}