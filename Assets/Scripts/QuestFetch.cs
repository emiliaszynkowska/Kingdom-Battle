using System.Collections.Generic;
using UnityEngine;

public class QuestFetch : Quest
{
    private string item;
    
    void Start()
    {
        List<string> items = new List<string>() {"Key", "Wigg's Brew", "Liquid Luck", "Ogre's Strength", "Elixir of Speed"};
        List<string> messages = new List<string>()
        {
            "I've lost my *. Can you find it for me?", 
            "I'm looking for *. If you find one, please bring it to me.",
            "I'm stuck here and I can't escape without *. Please help me!",
            "I really need *. I won't move until I find one."
        };
        item = items[Random.Range(0, 5)];
        message = messages[Random.Range(0, 4)];
        message = message.Replace("*", item);
        message = message.Replace("Key", "a Key");
    }

    void Update()
    {
        if (playerController.GetInventory().Contains(item))
            complete = true;
    }
    
}
