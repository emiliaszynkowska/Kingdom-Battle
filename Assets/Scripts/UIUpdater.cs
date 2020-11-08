using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    // Objects
    public GameObject info;
    public GameObject lives;
    public GameObject inventory;
    public GameObject controls;
    // Assets
    public Sprite fullLife;
    public Sprite emptyLife;
    public Sprite key;
    public Sprite scroll;
    public Sprite potionRed;
    public Sprite potionYellow;
    public Sprite potionGreen;
    public Sprite potionBlue;

    // Player Information 
    public void SetInfo(string player, int level, int money)
    {
        Text playerText = (Text) info.transform.GetChild(2).gameObject.GetComponent<Text>();
        Text levelText = (Text) info.transform.GetChild(3).gameObject.GetComponent<Text>();
        Text moneyText = (Text) info.transform.GetChild(4).gameObject.GetComponent<Text>();
        playerText.text = player;
        levelText.text = "Level " + level.ToString();
        moneyText.text = money.ToString();
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
    public void AddItem(string item, int index)
    {
        switch (item)
        {
          case "key":
              inventory.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = key;
              break;
          case "scroll":
              inventory.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = scroll;
              break;
          case "potionRed":
              inventory.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = potionRed;
              break;
          case "potionYellow":
              inventory.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = potionYellow;
              break;
          case "potionGreen":
              inventory.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = potionGreen;
              break;
          case "potionBlue":
              inventory.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = potionBlue;
              break;
        }
        inventory.transform.GetChild(index).gameObject.SetActive(true);
    }

    public void RemoveItem(int index)
    {
        inventory.transform.GetChild(index).gameObject.SetActive(false);
    }

    public void ClearInventory()
    {
        for (int i = 0; i < 8; i++)
        {
            inventory.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = null;
            inventory.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    // Controls
    public void SetInteract(bool canInteract)
    {
        Color color = controls.GetComponentInChildren<Image>().color;
        if (canInteract)
            color.a = 1.0f;
        else
        {
            color.a = 0.5f;
        }
        controls.GetComponentInChildren<Image>().color = color;
    }
    
}
