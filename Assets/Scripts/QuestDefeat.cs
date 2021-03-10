using System.Collections.Generic;
using UnityEngine;

public class QuestDefeat : Quest
{
    public string enemy;
    public GameObject obj;
    
    public void Set()
    {
        List<string> messages = new List<string>()
        {
            $"I'm terrified of that {enemy} over there. Please defeat it!", 
            "If you see a monster in this room, kill it and come back to me.",
            "I saw a monster earlier. I'll move if you can defeat it."
        };
        List<string> messagesAlt = new List<string>()
        {
            $"I'm terrified of that {enemy} over there. Oh, you defeated it?", 
            "I saw a monster in this room. Oh, you defeated it?",
            "I saw a monster earlier. Oh, you defeated it?"
        };
        message = messages[Random.Range(0, 3)];
        message2 = messagesAlt[Random.Range(0, 3)];
        message3 = $"Thank you for killing that {enemy}. Here, take this {reward}.";
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