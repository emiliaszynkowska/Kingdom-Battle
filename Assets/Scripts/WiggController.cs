using System.Collections;
using UnityEngine;

public class WiggController : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    public DungeonGenerator dungeonGenerator;
    public DungeonManager dungeonManager;
    public QuestManager questManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public string npcName = "Wigg";
    public bool talking;
    public bool active;
    
    public void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        dungeonGenerator = GameObject.Find("Map").GetComponent<DungeonGenerator>();
        dungeonManager = dungeonGenerator.dungeonManager;
        questManager = GameObject.Find("UI").GetComponent<QuestManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
    }
    
    public void FixedUpdate()
    {
        if (player.transform.position.x - transform.position.x >= 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else
            GetComponent<SpriteRenderer>().flipX = true;
    }
    
    public IEnumerator Talk()
    {
        // Give Quest
        if (!talking && !active)
        {
            talking = true;
            uiManager.PauseGame();
            switch (PlayerData.Level)
            {
                // Spin Attack
                case 3:
                    yield return uiManager.Speak(npcName, "Hello, elf. It seems you have become stronger since the last time we met.");
                    yield return uiManager.Speak(npcName, "However, I have found a way for you to become more powerful.");
                    yield return uiManager.Speak(npcName, "There is a hidden scroll somewhere in this dungeon. It is said to contain ancient sword techniques, sealed away for centuries.");
                    yield return uiManager.Speak(npcName, "You must find this scroll and bring it to me.");
                    questManager.AddMainQuest("Find the scroll");
                    dungeonManager.PlaceItem("Scroll", dungeonManager.RandomPosition(dungeonGenerator.RandomDungeon().GetRoom(), false));
                    break;
                // Blue Robe
                case 5:
                    yield return uiManager.Speak(npcName, "Ah, we meet again. How is your journey?");
                    yield return uiManager.Speak(npcName, "I have something that may help you. But first, you must prove your skill.");
                    yield return  uiManager.Speak(npcName, "If you can defeat 5 monsters I will grant you this power.");
                    questManager.AddMainQuest("Defeat 5 monsters             0/5");
                    break;
                // Ground Pound
                case 6:
                    yield return uiManager.Speak(npcName, "Greetings, young adventurer!");
                    yield return uiManager.Speak(npcName, "I have watched you advance through these dungeons. Your skill is impressive, but I fear the monsters ahead may be too strong.");
                    yield return uiManager.Speak(npcName, "I have located another scroll in this area. I sense its magic...");
                    yield return uiManager.Speak(npcName, "Please, find the scroll and return to me.");
                    questManager.AddMainQuest("Find the scroll");
                    dungeonManager.PlaceItem("Scroll", dungeonManager.RandomPosition(dungeonGenerator.RandomDungeon().GetRoom(), false));
                    break;
                // Red Robe
                case 8:
                    yield return uiManager.Speak(npcName, "Hello again, elf. I commend you for getting this far.");
                    yield return uiManager.Speak(npcName, "I have one final quest for you. Do this for me and I will award you this final power.");
                    yield return uiManager.Speak(npcName, "When four potions come together in unison, an incredible power is made.");
                    yield return uiManager.Speak(npcName, "You must find all four potions and bring them to me.");
                    questManager.AddMainQuest("Collect all four potions");
                    break;
                // Kingsbane
                case 9:
                    yield return uiManager.Speak(npcName, $"{playerController.playerName}, we meet again for the last time.");
                    yield return uiManager.Speak(npcName, "The time has almost come for you to return to Asteron, and your journey will only become tougher.");
                    yield return uiManager.Speak(npcName, "You need not prove your strength to me this time, I know you are ready.");
                    yield return uiManager.Speak(npcName, "Before you go, I want you to have this gift. It is an ancient heirloom passed down through generations of wizards.");
                    yield return uiManager.Speak(npcName, "Only this sword can vanqish the evil king. Take it with you and bring peace to all of Asteron.");
                    yield return uiManager.Upgrade("Kingsbane");
                    StartCoroutine(Disappear());
                    break;
            }
            talking = false;
            active = true;
            uiManager.ResumeGame();
        }
        // Quest Complete
        else if (active)
        {
            talking = true;
            uiManager.PauseGame();
            if (PlayerData.Level == 3 && playerController.GetInventory().Contains("Scroll"))
            {
                yield return uiManager.Speak(npcName, "Well done, you found the scroll.");
                questManager.Event("Return to Wigg", 0, false);
                var index = playerController.GetInventory().IndexOf("Scroll");
                playerController.RemoveItem("Scroll");
                uiManager.activeItem = index;
                uiManager.UseItem();
                uiManager.DisableItem(playerController.GetInventory().Count);
                yield return uiManager.Speak(npcName, "This scroll speaks of an ancient technique, the Spin Attack...");
                yield return uiManager.FadeIn();
                soundManager.PlaySound(soundManager.powerup);
                yield return new WaitForSecondsRealtime(1);
                uiManager.SetAttacks(2);
                yield return uiManager.FadeOut();
                yield return uiManager.Speak(npcName, "You have learned the Spin Attack. Good luck.");
                questManager.AddMainQuest("Use 3 Spin Attacks         0/3");
                StartCoroutine(Disappear());
            }
            else if (PlayerData.Level == 5 && questManager.GetQuests().Contains("Return to Wigg"))
            {
                yield return uiManager.Speak(npcName, "Well done, you have proved your skill. In return, I will give you this power.");
                yield return uiManager.Speak(npcName, "The blue robe will grant you more health. Use it wisely.");
                yield return uiManager.Upgrade("Blue Robe");
                StartCoroutine(Disappear());
            }
            else if (PlayerData.Level == 6 && playerController.GetInventory().Contains("Scroll"))
            {
                yield return uiManager.Speak(npcName, "Well done, you found the scroll.");
                questManager.Event("Return to Wigg", 0, false);
                uiManager.RemoveItem(playerController.GetInventory().IndexOf("Scroll"));
                playerController.RemoveItem("Scroll");
                yield return uiManager.Speak(npcName, "This scroll speaks of an ancient technique, the Ground Pound...");
                yield return uiManager.FadeIn();
                soundManager.PlaySound(soundManager.powerup);
                yield return new WaitForSecondsRealtime(1);
                uiManager.SetAttacks(3);
                yield return uiManager.FadeOut();
                yield return uiManager.Speak(npcName, "You have learned the Ground Pound. Good luck.");
                questManager.AddMainQuest("Use 3 Ground Pounds         0/3");
                StartCoroutine(Disappear());
            }
            else if (PlayerData.Level == 8 && questManager.GetQuests().Contains("Return to Wigg"))
            {
                yield return uiManager.Speak(npcName, "Well done, you have proved your skill. In return, I will give you this power.");
                yield return uiManager.Speak(npcName, "The red robe will grant you more health. Use it wisely.");
                yield return uiManager.Upgrade("Red Robe");
                StartCoroutine(Disappear());
            }
            // Quest Incomplete
            else
            {
                if (PlayerData.Level == 3 | PlayerData.Level == 6)
                    yield return uiManager.Speak(npcName, "Return when you have found the scroll.");
                else if (PlayerData.Level == 5)
                    yield return uiManager.Speak(npcName, "Return when you have defeated 5 monsters.");
                else if (PlayerData.Level == 8)
                    yield return uiManager.Speak(npcName, "Return when you have found all four potions.");
            }
            uiManager.ResumeGame();
            talking = false;
            yield return new WaitForSeconds(3);
            questManager.CheckQuests();
        }
    }
        
    IEnumerator Disappear()
    {
        for (int i=0; i < 3000; i++)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            transform.Translate(Vector3.right * (Time.deltaTime * 2));
            yield return null;
        }
        gameObject.SetActive(false);
    }
}