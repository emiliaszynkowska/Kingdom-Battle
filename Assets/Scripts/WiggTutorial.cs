using System;
using System.Collections;
using UnityEngine;

public class WiggTutorial : MonoBehaviour
{
    public GameObject player;
    public PlayerTutorial playerTutorial;
    public TutorialManager tutorialManager;
    public DungeonManager dungeonManager;
    public QuestManager questManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public string npcName = "Wigg";
    public bool talking;
    public bool active;

    public void FixedUpdate()
    {
        if (player.transform.position.x - transform.position.x >= 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else
            GetComponent<SpriteRenderer>().flipX = true;
    }
    
    public IEnumerator Talk()
    {
        if (!talking)
        {
            talking = true;
            if (!active)
            {
                yield return uiManager.Speak(npcName, "An elf? How did you get here?");
                yield return uiManager.Speak(npcName, "...");
                yield return uiManager.Speak(npcName, "You tried to challenge the Evil King? You must be very brave.");
                yield return uiManager.Speak(npcName,
                    "Welcome to the Dungeon Dimension. Most of us wizards escaped here after King Eldar took over the kingdom.");
                yield return uiManager.Speak(npcName, "What is your name, elf?");
                yield return uiManager.NameInput();
                yield return uiManager.Speak(npcName, $"It's nice to meet you, {playerTutorial.playerName}.");
                uiManager.info.SetActive(true);
                uiManager.SetName(playerTutorial.playerName);
                yield return uiManager.Speak(npcName,
                    "You can use the player information on the top right to view your status.");
                uiManager.difficulty.transform.parent.gameObject.SetActive(true);
                yield return uiManager.Speak(npcName,
                    "The difficulty bar to the left shows how many monsters are nearby. I think we're safe for now.");
                uiManager.lives.SetActive(true);
                yield return uiManager.Speak(npcName, "And remember to keep an eye on your health too.");
                yield return uiManager.Speak(npcName, "Now, where is your weapon?");
                yield return uiManager.Speak(npcName, "...");
                yield return uiManager.Speak(npcName,
                    "You don't have one? Well, I'm sure we can find you a sword somewhere around here.");
                uiManager.minimap.SetActive(true);
                yield return uiManager.Speak(npcName, "Here, use this map to help you find your way.");
                uiManager.options.SetActive(true);
                yield return uiManager.Speak(npcName, "One more thing! You see the menu in the bottom left corner?");
                yield return uiManager.Speak(npcName, "You can use the Interact button to use items, objects, and characters.");
                yield return uiManager.Speak(npcName, "The Inventory button will show you what you're carrying. You can use items you've picked up from here.");
                yield return uiManager.Speak(npcName, "The Quests button will show you what to do next. If you get stuck, try completing all your quests. It might open up a new path.");
                yield return uiManager.Speak(npcName, "The Menu button will let you pause time. You can also restart or exit the dungeon from here.");
                questManager.AddMainQuest("Find a weapon");
                active = true;
                while (transform.position.x > -2)
                {
                    transform.position = new Vector3(transform.position.x - 0.05f, transform.position.y, 0);
                    yield return new WaitForSecondsRealtime(0.05f);
                }
            }
            talking = false;
        }
    }

    public IEnumerator FindWeapons()
    {
        yield return Appear(new Vector3(80, 27), new Vector3(70, 27), true);
        talking = true;
        yield return uiManager.Speak(npcName, "Ah, an old sword and shield! These should do nicely.");
        tutorialManager.DisableWeapons();
        yield return uiManager.Upgrade("Rusty Sword");
        yield return uiManager.Upgrade("Shield");
        yield return uiManager.Speak(npcName, "You should practice before you continue.");
        questManager.AddMainQuest("Use 3 attacks                 0/3");
        playerTutorial.SetPrompt("Use left-click to attack\nUse left-shift to block\n");
        yield return new WaitForSecondsRealtime(3);
        questManager.AddMainQuest("Use 3 blocks                   0/3");
        talking = false;
        yield return new WaitUntil(() => questManager.complete == 3);
        playerTutorial.SetPrompt("");
        dungeonManager.PlaceItem("Key", new Vector3(68, 28));
        soundManager.PlaySound(soundManager.complete);
        talking = true;
        yield return uiManager.Speak(npcName, "Well done, you are skilled with the sword and shield.");
        yield return uiManager.Speak(npcName, "Here, use this key to open the door.");
        playerTutorial.SetPrompt("Use E to open doors");
        talking = false;
    }

    public IEnumerator FindChests()
    {
        yield return Appear(new Vector3(-5, 47), new Vector3(-0.5f, 47), false);
        talking = true;
        yield return uiManager.Speak(npcName, "It looks like you've found some chests.");
        yield return uiManager.Speak(npcName, "In this dimension, there are four potions you can find. One lies in each chest.");
        yield return uiManager.Speak(npcName, "Here, use a key to open each chest.");
        questManager.AddMainQuest("Open 4 chests                  0/4");
        dungeonManager.PlaceItem("Key", new Vector3(1, 48.5f));
        dungeonManager.PlaceItem("Key", new Vector3(5, 48.5f));
        dungeonManager.PlaceItem("Key", new Vector3(1, 44.5f));
        dungeonManager.PlaceItem("Key", new Vector3(5, 44.5f));
        playerTutorial.SetPrompt("Use E to open chests");
        talking = false;
        yield return new WaitUntil(() => questManager.complete == 5);
        yield return new WaitForSecondsRealtime(0.5f);
        yield return new WaitWhile(() => talking);
        GameObject.Find("Objects").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Objects").transform.GetChild(1).gameObject.SetActive(false);
        soundManager.PlaySound(soundManager.complete);
        talking = true;
        yield return uiManager.Speak(npcName, "Use these potions wisely.");
        talking = false;
    }
    
    public IEnumerator FindMonster()
    {
        yield return Appear(new Vector3(70, 58, 0), new Vector3(78, 58, 0), false);
        talking = true;
        yield return uiManager.Speak(npcName, "Aha! It's a monster. Use your sword to defeat it!");
        questManager.AddMainQuest("Defeat the monster");
        talking = false;
        yield return new WaitUntil(() => questManager.complete == 4);
        dungeonManager.PlaceItem("Key", new Vector3(80, 63));
        soundManager.PlaySound(soundManager.complete);
        talking = true;
        yield return uiManager.Speak(npcName, "Very good. I knew you could do it.");
        yield return uiManager.Speak(npcName, "Here, use this key to open the door.");
        talking = false;
    }
    
    public IEnumerator FindMonsters()
    {
        yield return Appear(new Vector3(70, 58, 0), new Vector3(78, 58, 0), false);
        talking = true;
        yield return uiManager.Speak(npcName, $"More monsters? {playerTutorial.playerName}, defeat those weaklings.");
        questManager.AddMainQuest("Defeat the monsters");
        talking = false;
        yield return new WaitUntil(() => questManager.complete == 6);
        dungeonManager.PlaceItem("Key", new Vector3(-23, 82));
        soundManager.PlaySound(soundManager.complete);
        talking = true;
        yield return uiManager.Speak(npcName, "Excellent. Your skill is improving.");
        yield return uiManager.Speak(npcName, "Here, use this key to open the door.");
        playerTutorial.SetPrompt("Use E to open doors");
        talking = false;
    }
    
    public IEnumerator FindSpecial()
    {
        yield return Appear(new Vector3(15, 89, 0), new Vector3(21, 89, 0), false);
        talking = true;
        yield return uiManager.Speak(npcName, "Congratulations, you've reached the end of this dungeon."); 
        yield return uiManager.Speak(npcName, "You are ready to continue this journey on your own.");
        yield return uiManager.Speak(npcName, "Beyond this place you will find many more powerful monsters and challenges.");
        yield return uiManager.Speak(npcName, "If you venture far enough, you will be given a chance to escape this dimension and return to Asteron.");
        yield return uiManager.Speak(npcName, "Promise me that when that happens you will not fail.");
        yield return uiManager.Speak(npcName, "I will be watching over you from afar. Farewell, young one.");
        talking = false;
    }

    public IEnumerator WiggsBrew()
    {
        questManager.Event("chest", "Open", false);
        yield return uiManager.Speak(npcName, "You've found Wigg's Brew. I invented this potion to heal wounds and restore health.");
    }
    
    public IEnumerator LiquidLuck()
    {
        questManager.Event("chest", "Open", false);
        yield return uiManager.Speak(npcName, "You've found Liquid Luck. This potion increases your combat skills when fighting enemies.");
    }
    public IEnumerator OgresStrength()
    {
        questManager.Event("chest", "Open", false);
        yield return uiManager.Speak(npcName, "You've found Ogre's Strength. Taken from ogres, this toxic substance poisons enemies.");
    }
    public IEnumerator ElixirOfSpeed()
    {
        questManager.Event("chest", "Open", false);
        yield return uiManager.Speak(npcName, "You've found Elixir of Speed. This elixir makes you a lot faster.");
    }
    
    IEnumerator Appear(Vector3 start, Vector3 end, bool dir)
    {
        uiManager.PauseGame();
        transform.position = start;
        if (dir)
        {
            while (transform.position.x > end.x)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                transform.Translate(Vector3.left * (Time.unscaledDeltaTime * 5));
                yield return null;
            } 
        }
        else
        {
            while (transform.position.x < end.x)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                transform.Translate(Vector3.right * (Time.unscaledDeltaTime * 5));
                yield return null;
            } 
        }
        uiManager.ResumeGame();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active && other is BoxCollider2D && other.name != null && other.name.Equals("Player"))
        {
            StartCoroutine(Talk());
        }
    }
}