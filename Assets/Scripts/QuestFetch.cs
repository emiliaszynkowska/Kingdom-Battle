using System.Collections.Generic;
using UnityEngine;

public class QuestFetch : Quest
{
    public string item;
    
    void Awake()
    {
        List<string> items = new List<string>() {"Key", "Wigg's Brew", "Liquid Luck", "Ogre's Strength", "Elixir of Speed"};
        item = items[Random.Range(0, 5)];
        List<string> messages = new List<string>()
        {
            $"I've lost my {item}. Can you find it for me?", 
            $"I'm looking for {item}. If you find one, please bring it to me.",
            $"I'm stuck here and I can't escape without {item}. Please help me!",
            $"I really need {item}. I won't move until I find one."
        };
        List<string> messagesAlt = new List<string>()
        {
            $"I've lost my {item}. Oh, you've already got one?",
            $"I'm looking for {item}. Oh, you've already got one?",
            $"I'm stuck here and I can't escape without {item}. Oh, you've already got one?",
            $"I really need {item}. Oh, you've already got one?"
        };
        message = messages[Random.Range(0, 4)];
        message = message.Replace("Key", "a Key");
        message2 = messagesAlt[Random.Range(0, 4)];
        message3 = $"Thank you for bringing me {item}. Here, take this {reward}.";
    }

    void Update()
    {
        if (!complete && playerController.GetInventory().Contains(item))
            complete = true;
    }
    
}
