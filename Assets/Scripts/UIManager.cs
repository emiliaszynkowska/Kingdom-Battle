using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Objects
    public PlayerController playerController;
    public GameObject map;
    public GameObject info;
    public GameObject lives;
    public GameObject inventory;
    public GameObject items;
    public GameObject active;
    public GameObject options;
    public GameObject optionsInventory;
    public GameObject optionsShop;
    public GameObject minimap;
    public GameObject dialog;
    public Text npcName;
    public Text message;
    public GameObject menu;
    public GameObject level;
    public GameObject gameOver;
    public Text powerup;
    public Image fade;
    // Variables
    public int activeItem;
    // Assets
    public Sprite fullLife;
    public Sprite emptyLife;
    public Sprite key;
    public Sprite scroll;
    public Sprite wiggsBrew;
    public Sprite liquidLuck;
    public Sprite ogresStrength;
    public Sprite elixirofSpeed;

    // Player Information
    public void SetInfo(string player, string[] disciplines, int coins)
    {
        // Get objects
        Text playerText = info.transform.GetChild(0).gameObject.GetComponentInChildren<Text>();
        GameObject discipline2 = info.transform.GetChild(2).gameObject;
        GameObject discipline3 = info.transform.GetChild(3).gameObject;
        Text disciplineText1 = info.transform.GetChild(1).gameObject.GetComponentInChildren<Text>();
        Text disciplineText2 = info.transform.GetChild(2).gameObject.GetComponentInChildren<Text>();
        Text disciplineText3 = info.transform.GetChild(3).gameObject.GetComponentInChildren<Text>();
        Text moneyText = info.transform.GetChild(4).gameObject.GetComponentInChildren<Text>();
        GameObject money = info.transform.GetChild(4).gameObject;
        
        // Name
        playerText.text = player;
        // Coins
        moneyText.text = coins.ToString();
        // Discipline 3
        if (disciplines[2] != null)
        {
            discipline3.SetActive(true);
            disciplineText3.text = disciplines[2];
            SetMoneyPosition(money, -180);
        }
        else
        {
            discipline3.SetActive(false);
            SetMoneyPosition(money, -120);
        }
        // Discipline 2
        if (disciplines[1] != null)
        {
            discipline2.SetActive(true);
            disciplineText2.text = disciplines[1];
            SetMoneyPosition(money, -120);
        }
        else
        {
            discipline2.SetActive(false);
            SetMoneyPosition(money, -60);
        }
        // Discipline 1
        disciplineText1.text = disciplines[0];
    }

    void SetMoneyPosition(GameObject money, int y)
    {
        var pos = money.transform.localPosition;
        pos.y = y;
        money.transform.localPosition = pos;
    }
    
    // Lives 
    public void SetLivesActive(int n)
    {
        for (int i = 0; i < 8; i++)
        {
            if (i < n)
                lives.transform.GetChild(i).gameObject.SetActive(true);
            else 
                lives.transform.GetChild(i).gameObject.SetActive(false);
        }  
    }
    
    public void SetLives(int n)
    {
        for (int i = 0; i < 8; i++)
        {
            if (i < n)
                lives.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = fullLife;
            else
                lives.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = emptyLife;
        }
    }

    // Inventory
    public void Inventory()
    {
        PauseGame();
        minimap.SetActive(false);
        options.SetActive(false);
        inventory.SetActive(true);
        ChangeItem(0);
    }

    public void ExitInventory()
    {
        ResumeGame();
        inventory.SetActive(false);
        options.SetActive(true);
        minimap.SetActive(true);
    }
    
    public bool IsInventory()
    {
        return inventory.activeSelf;
    }

    public void AddItem(string item, int index)
    {
        switch (item)
        {
          case "Key":
              items.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = key;
              break;
          case "Scroll":
              items.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = scroll;
              break;
          case "Wigg's Brew":
              items.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = wiggsBrew;
              break;
          case "Liquid Luck":
              items.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = liquidLuck;
              break;
          case "Ogre's Strength":
              items.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = ogresStrength;
              break;
          case "Elixir of Speed":
              items.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = elixirofSpeed;
              break;
        }
        items.transform.GetChild(index).gameObject.SetActive(true);
    }
    
    public void ChangeItem(int index)
    {
        activeItem = index;
        active.transform.position = items.transform.GetChild(activeItem).position;
    }
    
    public void UseItem()
    {
        for (int i=activeItem; i < 7; i++)
        {
            Image next = items.transform.GetChild(i + 1).gameObject.GetComponent<Image>();
            items.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = next.sprite;
            next.sprite = null;
        }
    }

    public void DisableItem(int index)
    {
        items.transform.GetChild(index).gameObject.SetActive(false);
    }

    public void Powerup(string text, Color color)
    {
        powerup.text = text;
        powerup.color = color;
    }

    public void ClearInventory()
    {
        for (int i = 0; i < 8; i++)
        {
            items.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = null;
            items.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Options
    public IEnumerator Interact()
    {
        options.transform.GetChild(0).GetComponent<Image>().CrossFadeAlpha(0.25f, 0, true);
        yield return new WaitForSeconds(0.2f);
        options.transform.GetChild(0).GetComponent<Image>().CrossFadeAlpha(1, 0, true);
    }

    public void Quests()
    {
        
    }

    public void Menu()
    {
        PauseGame();
        menu.SetActive(true);
    }

    public void ExitMenu()
    {
        ResumeGame();
        menu.SetActive(false);
    }

    public bool IsMenu()
    {
        return menu.activeSelf;
    }

    public void Shop()
    {
        options.SetActive(false);
        optionsShop.SetActive(true);
    }

    public IEnumerator ExitShop()
    {
        yield return new WaitForSeconds(0.1f);
        options.SetActive(true);
        optionsShop.SetActive(false);
    }
    
    public void StartSpeak(string npc, string text)
    {
        npcName.text = npc;
        message.text = text;
        dialog.SetActive(true);
    }

    public void StopSpeak()
    {
        dialog.SetActive(false);
    }

    public IEnumerator GameOver()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(0.5f);
        gameOver.SetActive(true);
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(0.5f);
    }

    public bool IsGameOver()
    {
        return gameOver.activeSelf;
    }

    public void Level(int n)
    {
        PauseGame();
        level.transform.GetChild(1).GetComponent<Text>().text = "Level " + n.ToString();
        menu.SetActive(false);
        gameOver.SetActive(false);
        inventory.SetActive(false);
        level.SetActive(true);
    }

    public void LevelStart()
    {
        level.SetActive(false);
        ResumeGame();
        fade.CrossFadeAlpha(0.8f, 0, true);
        StartCoroutine(FadeOut());
    }

    public bool IsLevel()
    {
        return level.activeSelf;
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    
    public void Restart()
    {
        
    }

    public void Exit()
    {
        StartCoroutine(ExitFade());
    }

    public IEnumerator ExitFade()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Title");
    }

    public IEnumerator FadeIn()
    {
        fade.CrossFadeAlpha(1.0f, 0.5f, true);
        yield return new WaitForSeconds(0.5f);
    }
    
    public IEnumerator FadeOut()
    {
        fade.CrossFadeAlpha(0.0f, 0.5f, true);
        yield return new WaitForSeconds(0.5f);
    }

}
