using System;
using System.Collections;
using UnityEngine;

public class WiggTutorial : MonoBehaviour
{
    public GameObject player;
    public PlayerTutorial playerTutorial;
    public TutorialManager tutorialManager;
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
                uiManager.lives.SetActive(true);
                yield return uiManager.Speak(npcName, "And keep an eye on your health on the top left.");
                yield return uiManager.Speak(npcName, "Now, where is your weapon?");
                yield return uiManager.Speak(npcName, "...");
                yield return uiManager.Speak(npcName,
                    "You don't have one? Well, I'm sure we can find you a sword somewhere around here.");
                uiManager.minimap.SetActive(true);
                yield return uiManager.Speak(npcName, "Here, use this map to help you find your way.");
                uiManager.options.SetActive(true);
                yield return uiManager.Speak(npcName, "One more thing! You see the menu in the bottom left corner?");
                yield return uiManager.Speak(npcName, "You can use the Interact button to talk to me.");
                yield return uiManager.Speak(npcName, "The Inventory button will show you what you're carrying.");
                yield return uiManager.Speak(npcName, "The Quests button will show you any tasks you need to do.");
                yield return uiManager.Speak(npcName, "The Menu button will let you pause time in this dimension.");
                yield return uiManager.Speak(npcName, "Come and talk to me if you get lost.");
                questManager.AddMainQuest("Find a weapon");
                while (transform.position.x > -2)
                {
                    transform.position = new Vector3(transform.position.x - 0.05f, transform.position.y, 0);
                    yield return new WaitForSecondsRealtime(0.05f);
                }
                active = true;
            }
            else 
            {
                if (questManager.complete == 0)
                    uiManager.Speak(npcName, "Can you see a sword anywhere?");
                else if (questManager.complete == 1)
                    uiManager.Speak(npcName, "Well, why don't you give them a try?");
            }
            talking = false;
        }
    }

    public IEnumerator FindWeapons()
    {
        talking = true;
        transform.SetPositionAndRotation(new Vector3(10, 19, 0), Quaternion.Euler(0, 0, 180));
        yield return uiManager.Speak(npcName, "Ah, an old sword and shield! These should do nicely.");
        tutorialManager.DisableWeapons();
        yield return uiManager.Upgrade("Rusty Sword");
        playerTutorial.weapon.SetActive(true);
        uiManager.timers.SetActive(true);
        uiManager.SetAttacks(1);
        yield return uiManager.Speak(npcName, "Try using your attack with the left mouse button, and block with shift.");
        questManager.AddMainQuest("Use 3 Sword Attacks        0/3");
        yield return new WaitForSecondsRealtime(2);
        questManager.AddMainQuest("Use 3 Blocks        0/3");
        talking = false;
        yield return new WaitUntil(() => questManager.complete == 3);
        GameObject.Find("Items").transform.GetChild(0).gameObject.SetActive(true);
        talking = true;
        yield return uiManager.Speak(npcName, "Well done, you are skilled with the sword and shield.");
        yield return uiManager.Speak(npcName, "You can use this key to open the door.");
        talking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active)
        {
            StartCoroutine(Talk());
        }
    }
}