using System;
using System.Collections;
using UnityEngine;

public class WiggTutorial : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    public TutorialManager TutorialManager;
    public QuestManager QuestManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public string npcName = "Wigg";
    public bool talking;
    public bool active = false;

    public void FixedUpdate()
    {
        if (player.transform.position.x - transform.position.x >= 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else
            GetComponent<SpriteRenderer>().flipX = true;
    }
    
    public IEnumerator Talk()
    {
        if (!talking && !active)
        {
            talking = true;
            StartCoroutine(uiManager.FadeText());
            yield return uiManager.Speak(npcName, "An elf? How did you get here?");
            yield return uiManager.Speak(npcName, "...");
            yield return uiManager.Speak(npcName, "You tried to challenge the Evil King? You must be very brave.");
            yield return uiManager.Speak(npcName, "Welcome to the Dungeon Dimension. Most of us wizards escaped here after King Eldar came to rule.");
            yield return uiManager.Speak(npcName, "What is your name, elf?");
            yield return uiManager.NameInput();
            yield return uiManager.Speak(npcName, $"It's nice to meet you, {playerController.playerName}.");
            uiManager.info.SetActive(true);
            uiManager.SetName(playerController.playerName);
            yield return uiManager.Speak(npcName, "You can use the player information on the top right to view your status.");
            uiManager.lives.SetActive(true);
            yield return uiManager.Speak(npcName, "And keep an eye on your lives on the top left.");
            yield return uiManager.Speak(npcName, "Now, where is your weapon?");
            yield return uiManager.Speak(npcName, "...");
            yield return uiManager.Speak(npcName, "You don't have one? Well, I'm sure we can find you a sword somewhere around here.");
            uiManager.minimap.SetActive(true);
            yield return uiManager.Speak(npcName, "Use this map to find your way around.");
            uiManager.menu.SetActive(true);
            yield return uiManager.Speak(npcName, "One more thing! You see the menu in the bottom left corner?");
            yield return uiManager.Speak(npcName, "You can use the Interact button to talk to me.");
            yield return uiManager.Speak(npcName, "The Inventory button will show you what you're carrying.");
            yield return uiManager.Speak(npcName, "The Quests button will show you any tasks you need to do");
            yield return uiManager.Speak(npcName, "The Menu button will let you pause time in this dimension.");
            yield return uiManager.Speak(npcName, "Come and talk to me if you get lost.");
            QuestManager.AddMainQuest("Find a weapon");
            active = true;
            talking = false;
        }
        else if (!talking && active)
        {
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
        {
            StartCoroutine(Talk());
        }
    }
}