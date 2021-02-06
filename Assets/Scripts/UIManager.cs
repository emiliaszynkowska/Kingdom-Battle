using System;
using System.Collections;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Objects
    public PlayerController playerController;
    public GameObject map;
    public GameObject info;
    public GameObject lives;
    public GameObject timers;
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
    public GameObject scores;
    public GameObject disciplines;
    public GameObject level;
    public GameObject gameOver;
    public Text powerup;
    public Image background;
    public Image fade;
    // Variables
    public int time;
    public bool isPowerup;
    public int levelNum;
    public int difficulty;
    public int activeItem;
    // Assets
    public Sprite fullLife;
    public Sprite emptyLife;
    public Sprite key;
    public Sprite bossKey;
    public Sprite scroll;
    public Sprite wiggsBrew;
    public Sprite liquidLuck;
    public Sprite ogresStrength;
    public Sprite elixirofSpeed;
    public Sprite spriteAggressive;
    public Sprite spriteDefensive;
    public Sprite spriteExploration;
    public Sprite spriteCollection;
    public Sprite spritePuzzleSolving;

    private void Start()
    {
        StartCoroutine(Timer());
    }

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

    public void SetAttacks(int n)
    {
        switch (n)
        {
            case 1:
                timers.transform.GetChild(0).gameObject.SetActive(true);
                timers.transform.GetChild(1).gameObject.SetActive(false);
                timers.transform.GetChild(2).gameObject.SetActive(false);
                timers.transform.GetChild(0).localPosition = new Vector3(0, 50, 0);
                break;
            case 2: 
                timers.transform.GetChild(0).gameObject.SetActive(true);
                timers.transform.GetChild(1).gameObject.SetActive(true);
                timers.transform.GetChild(2).gameObject.SetActive(false);
                timers.transform.GetChild(0).localPosition = new Vector3(-175, 50, 0);
                timers.transform.GetChild(1).localPosition = new Vector3(175, 50, 0);
                break;
            case 3: 
                timers.transform.GetChild(0).gameObject.SetActive(true);
                timers.transform.GetChild(1).gameObject.SetActive(true);
                timers.transform.GetChild(2).gameObject.SetActive(true);
                timers.transform.GetChild(0).localPosition = new Vector3(-350, 50, 0);
                timers.transform.GetChild(1).localPosition = new Vector3(0, 50, 0);
                break;
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
          case "Boss Key":
              items.transform.GetChild(index).gameObject.GetComponent<Image>().sprite = bossKey;
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
        powerup.fontSize = 50;
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

    public void Level(int n, int d)
    {
        difficulty = d;
        PauseGame();
        level.transform.GetChild(1).GetComponent<Text>().text = "Level " + n.ToString();
        menu.SetActive(false);
        gameOver.SetActive(false);
        inventory.SetActive(false);
        level.SetActive(true);
    }
    
    public void Level(string s)
    {
        PauseGame();
        Text text = level.transform.GetChild(1).GetComponent<Text>();
        text.text = String.Format("Boss: {0}", s);
        switch (s)
        {
            case "Elite Knight":
                text.color = new Color(0.9f, 0, 0, 1);
                break;
            case "Royal Guardian":
                text.color = Color.yellow;
                break;
            case "Troll":
                text.color = new Color(0, 0.8f, 0, 1);
                break;
            case "Golem":
                text.color = new Color(0.85f, 0.45f, 0.25f, 1);
                break;
        }
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

    public IEnumerator Scores(int[] s)
    { 
        PauseGame(); 
        scores.SetActive(true);
        GameObject aggressive = scores.transform.GetChild(4).gameObject;
        GameObject defensive = scores.transform.GetChild(5).gameObject;
        GameObject exploration = scores.transform.GetChild(6).gameObject;
        GameObject collection = scores.transform.GetChild(7).gameObject;
        GameObject puzzlesolving = scores.transform.GetChild(8).gameObject;
        yield return new WaitForSecondsRealtime(1);
        aggressive.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[0].ToString());
        yield return new WaitForSecondsRealtime(1);
        defensive.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[1].ToString());
        yield return new WaitForSecondsRealtime(1);
        exploration.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[2].ToString());
        yield return new WaitForSecondsRealtime(1);
        collection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[3].ToString());
        yield return new WaitForSecondsRealtime(1);
        puzzlesolving.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[4].ToString());
        scores.transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        scores.SetActive(false);
        StartCoroutine(Disciplines());
    }

    public IEnumerator Disciplines()
    {
        disciplines.SetActive(true);
        GameObject primary = disciplines.transform.GetChild(3).gameObject;
        GameObject secondary = disciplines.transform.GetChild(4).gameObject;
        GameObject tertiary = disciplines.transform.GetChild(5).gameObject;
        var strPrimary = ScoreManager.DisciplinePrimary();
        var strSecondary = ScoreManager.DisciplineSecondary();
        switch (strPrimary)
        {
            case "Aggressive":
                primary.GetComponent<Image>().sprite = spriteAggressive;
                primary.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                primary.GetComponentInChildren<TextMeshProUGUI>().SetText(ScoreManager.TitleAggressive(difficulty));
                break;
            case "Defensive":
                primary.GetComponent<Image>().sprite = spriteDefensive;
                primary.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0, 0.7f, 1);
                primary.GetComponentInChildren<TextMeshProUGUI>().SetText(ScoreManager.TitleDefensive(difficulty));
                break;
        }
        switch (strSecondary)
        {
            case "Exploration":
                secondary.GetComponent<Image>().sprite = spriteExploration;
                secondary.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.1f, 0.8f, 0);
                secondary.GetComponentInChildren<TextMeshProUGUI>().SetText(ScoreManager.TitleExploration(difficulty));
                break;
            case "Collection":
                secondary.GetComponent<Image>().sprite = spriteDefensive;
                secondary.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1, 0.75f, 0);
                secondary.GetComponentInChildren<TextMeshProUGUI>().SetText(ScoreManager.TitleCollection(difficulty));
                break;
            case "PuzzleSolving":
                secondary.GetComponent<Image>().sprite = spritePuzzleSolving;
                secondary.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.7f, 0.2f, 1);
                secondary.GetComponentInChildren<TextMeshProUGUI>().SetText(ScoreManager.TitlePuzzleSolving(difficulty));
                break;
        }
        tertiary.GetComponent<TextMeshProUGUI>().color = new Color(1, 0.55f, 0); 
        tertiary.GetComponent<TextMeshProUGUI>().SetText(ScoreManager.TitleTertiary());
        yield return new WaitForSecondsRealtime(1);
        primary.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        secondary.SetActive(true);
        if (levelNum > 7)
        {
            yield return new WaitForSecondsRealtime(1);
            tertiary.SetActive(true);
        }
        disciplines.transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        disciplines.SetActive(false);
    }

    IEnumerator Timer()
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            if (powerup.color == Color.white && time >= 10)
            {
                powerup.text = "0:" + time;
                powerup.fontSize = 150;
            }
            else if (powerup.color == Color.white && time < 10)
            {
                powerup.text = "0:0" + time;
                powerup.fontSize = 150;
            }
        }
        StartCoroutine(Scores(ScoreManager.GetScores()));
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
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene("Title");
    }

    public IEnumerator FadeIn()
    {
        fade.CrossFadeAlpha(1.0f, 0.5f, true);
        yield return new WaitForSecondsRealtime(0.5f);
    }
    
    public IEnumerator FadeOut()
    {
        fade.CrossFadeAlpha(0.0f, 0.5f, true);
        yield return new WaitForSecondsRealtime(0.5f);
    }

}
