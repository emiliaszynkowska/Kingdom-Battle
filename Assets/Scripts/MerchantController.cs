using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MerchantController : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    public QuestManager questManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public GameObject items;
    public GameObject active;
    public int activeItem;
    public string npcName = "Merchant";
    public string message;
    public bool talking;
    public bool shopping;
    public Sprite key;
    public Sprite wiggsBrew;
    public Sprite liquidLuck;
    public Sprite ogresStrength;
    public Sprite elixirofSpeed;

    List<string> messages = new List<string>()
    {
        "Welcome to my shop.", 
        "Greetings, traveller. Need any items?",
        "So you're interested in my merchandise?", 
        "I've got plenty of tools and potions.",
        "Take a look at my merchandise."
    };

    private List<string> merchandise = new List<string>()
    {
        "Key",
        "Wigg's Brew",
        "Liquid Luck",
        "Ogre's Strength",
        "Elixir of Speed"
    };

    private Dictionary<string,int> prices = new Dictionary<string, int>()
    {
        {"Key", 10},
        {"Wigg's Brew", 50},
        {"Liquid Luck", 25},
        {"Ogre's Strength", 25},
        {"Elixir of Speed", 20}
    };

    public void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        questManager = GameObject.Find("UI").GetComponent<QuestManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
        SetItems();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && active.activeSelf && !shopping)
        {
            StartCoroutine(uiManager.Interact(0, 2, 0));
            Buy();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && active.activeSelf)
        {
            StartCoroutine(uiManager.Interact(1, 2, 0));
            ExitShop();
        }
        else if (Input.GetKeyDown(KeyCode.D) && active.activeSelf && activeItem < 5)
        {
            StartCoroutine(uiManager.Interact(2, 2, 1));
            activeItem++;
            active.transform.position = items.transform.GetChild(activeItem).position;
            Price();
        }
        else if (Input.GetKeyDown(KeyCode.A) && active.activeSelf && activeItem > 0)
        {
            StartCoroutine(uiManager.Interact(3, 2, 1));
            activeItem--;
            active.transform.position = items.transform.GetChild(activeItem).position;
            Price();
        }
    }

    public void Talk()
    {
        if (!talking && !shopping)
        {
            StartCoroutine(DoTalk());
        }
        else if (!talking)
        {
            talking = true;
            Shop();
        }
    }

    IEnumerator DoTalk()
    {
        talking = true;
        message = messages[Random.Range(0, 5)];
        uiManager.StartSpeak(npcName, message);
        yield return null;
        Shop();
    }
    
    // Shop 
    public void Shop()
    {
        uiManager.PauseGame();
        active.SetActive(true);
        uiManager.Shop();
    }

    public void ExitShop()
    {
        active.SetActive(false);
        uiManager.StopSpeak();
        StartCoroutine(uiManager.ExitShop());
        uiManager.ResumeGame();
        talking = false;
    }

    public void DisableItem()
    {
        items.transform.GetChild(activeItem).gameObject.SetActive(false);
    }

    public void Price()
    {
        var itemName = items.transform.GetChild(activeItem).name;
        var price = prices[itemName];
        var buyMessage = "That " + itemName + " costs " + price + " coins.";
        uiManager.StartSpeak(npcName, buyMessage);
    }

    public void Buy()
    {
        shopping = true;
        var itemName = items.transform.GetChild(activeItem).name;
        var price = prices[itemName];
        if (playerController.GetCoins() >= price && items.transform.GetChild(activeItem).gameObject.activeSelf)
        {
            soundManager.PlaySound(soundManager.buyItem);
            playerController.Spend(price);
            uiManager.AddItem(itemName, playerController.GetInventory().Count);
            playerController.AddItem(itemName);
            questManager.Event($"Buy {itemName}", 0, true);
            DisableItem();
            uiManager.StartSpeak(npcName, "Here you go.");
        }
        else if (playerController.GetCoins() < price && items.transform.GetChild(activeItem).gameObject.activeSelf)
        {
            soundManager.PlaySound(soundManager.error);
            uiManager.StartSpeak(npcName, "You don't have enough coins.");
        }
        shopping = false;
    }

    public void SetItems()
    {
        var itemsList = merchandise.OrderBy(x => Guid.NewGuid()).ToList();
        itemsList.Add(merchandise[Random.Range(0,5)]);
        for (int i=0; i<items.transform.childCount; i++)
        {
            GameObject item = items.transform.GetChild(i).gameObject;
            var itemName = itemsList[i];
            item.name = itemName;
            switch (itemName)
            {
                case "Key":
                    item.GetComponent<Image>().sprite = key;
                    break;
                case "Wigg's Brew":
                    item.GetComponent<Image>().sprite = wiggsBrew;
                    break;
                case "Liquid Luck":
                    item.GetComponent<Image>().sprite = liquidLuck;
                    break;
                case "Ogre's Strength":
                    item.GetComponent<Image>().sprite = ogresStrength;
                    break;
                case "Elixir of Speed":
                    item.GetComponent<Image>().sprite = elixirofSpeed;
                    break;
            }
        }
    }

}