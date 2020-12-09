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
    public GameObject minimap;
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
    public void SetInfo(string player, string[] disciplines, int coins)
    {
        Text playerText = (Text) info.transform.GetChild(0).gameObject.GetComponentInChildren<Text>();
        Text disciplineText1 = (Text) info.transform.GetChild(1).gameObject.GetComponentInChildren<Text>();
        Text disciplineText2 = (Text) info.transform.GetChild(2).gameObject.GetComponentInChildren<Text>();
        Text disciplineText3 = (Text) info.transform.GetChild(3).gameObject.GetComponentInChildren<Text>();
        Text moneyText = (Text) info.transform.GetChild(4).gameObject.GetComponentInChildren<Text>();
        playerText.text = player;
        moneyText.text = coins.ToString();
        disciplineText1.text = disciplines[0];
        if (disciplines.Length > 1)
        {
            disciplineText2.gameObject.SetActive(true);
            disciplineText2.text = disciplines[1];
        }
        else
        {
            disciplineText2.gameObject.SetActive(false);
        }
        if (disciplines.Length > 2)
        {
            disciplineText3.gameObject.SetActive(true);
            disciplineText3.text = disciplines[2];
        }
        else
        {
            disciplineText3.gameObject.SetActive(false);
        }
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
