using System.Collections.Generic;
using UnityEngine;

public class QuestDefeat : Quest
{
    private string enemy;
    public GameObject obj;
    
    void Awake()
    {
        List<string> enemies = new List<string>() {"Imp", "Goblin", "Chort", "Orc", "Necromancer"};
        List<string> messages = new List<string>()
        {
            "I'm terrified of the * over there. Please defeat it!", 
            "I hate *s. If you see one, kill it and come back to me.",
            "I saw this * earlier. I'll move if you can defeat it."
        };
        enemy = enemies[Random.Range(0, 5)];
        message = messages[Random.Range(0, 3)];
        message = message.Replace("*", enemy);
    }
    
    void Update()
    {
        if (!obj.activeSelf)
            complete = true;
    }

}