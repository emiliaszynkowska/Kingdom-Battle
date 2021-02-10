using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
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
    public GameObject inactive1;
    public GameObject inactive2;
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
    public GameObject next;
    public GameObject gameOver;
    public Text powerup;
    public GameObject upgrade;
    public Text upgradeText;
    public Image equipment;
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
    public Sprite blueLife;
    public Sprite key;
    public Sprite bossKey;
    public Sprite scroll;
    public Sprite wiggsBrew;
    public Sprite liquidLuck;
    public Sprite ogresStrength;
    public Sprite elixirofSpeed;
    public Sprite rustySword;
    public Sprite jaggedBlade;
    public Sprite warpedEdge;
    public Sprite knightsSword;
    public Sprite kingsbane;
    public Sprite blueRobe;
    public Sprite redRobe;
    public Sprite spriteAggressive;
    public Sprite spriteDefensive;
    public Sprite spriteExploration;
    public Sprite spriteCollection;
    public Sprite spritePuzzleSolving;

    public void Start()
    {
        ScoreManager.Reset();
        StartCoroutine(Timer());
    }

    // Player Information
    public void SetInfo(string player, List<string> playerTitles, int coins)
    {
        // Get objects
        Text playerText = info.transform.GetChild(0).gameObject.GetComponentInChildren<Text>();
        GameObject title1 = info.transform.GetChild(1).gameObject;
        GameObject title2 = info.transform.GetChild(2).gameObject;
        GameObject title3 = info.transform.GetChild(3).gameObject;
        GameObject money = info.transform.GetChild(4).gameObject;
        
        // Name
        playerText.text = player;
        // Coins
        money.GetComponentInChildren<Text>().text = coins.ToString();
        // Discipline 1
        title1.GetComponentInChildren<Text>().text = playerTitles[0];
        // Discipline 2
        if (playerTitles[1] != null)
        {
            title2.SetActive(true);
            title2.GetComponentInChildren<Text>().text = playerTitles[1];
            SetMoneyPosition(money, -120);
        }
        else
        {
            title2.SetActive(false);
            SetMoneyPosition(money, -60);
        }
        // Discipline 3
        if (playerTitles[2] != null)
        {
            title3.SetActive(true);
            title3.GetComponentInChildren<Text>().text = playerTitles[2];
            SetMoneyPosition(money, -180);
        }
        else 
        {
            title3.SetActive(false);
        }
    }

    public void SetCoins(int coins)
    {
        info.transform.GetChild(4).GetComponentInChildren<Text>().text = coins.ToString();
    }

    void SetMoneyPosition(GameObject money, int y)
    {
        var pos = money.transform.localPosition;
        pos.y = y;
        money.transform.localPosition = pos;
    }

    public void SetEffect(int i, float size, Sprite sprite)
    {
        info.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
        info.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = sprite;
        info.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
    }

    // Lives 
    public void SetLivesActive(int n)
    {
        for (int i = 0; i < 11; i++)
        {
            if (i < n)
                lives.transform.GetChild(i).gameObject.SetActive(true);
            else 
                lives.transform.GetChild(i).gameObject.SetActive(false);
        }  
    }
    
    public void SetLives(int n)
    {
        for (int i = 0; i < 11; i++)
        {
            if (i < n && playerController.livesActive - i < 3 && playerController.playerDisciplines[0].Equals("Defensive"))
                lives.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = blueLife;
            else if (i < n)
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
        if (playerController.playerDisciplines[1] != null && playerController.playerDisciplines[1].Equals("Collection"))
        {
            inactive1.SetActive(false);
            inactive2.SetActive(false);
        }
        else
        {
            inactive1.SetActive(true);
            inactive2.SetActive(true);
        }
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
            Image j = items.transform.GetChild(i + 1).gameObject.GetComponent<Image>();
            items.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = j.sprite;
            j.sprite = null;
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

    public IEnumerator Upgrade(string e)
    {
        PauseGame();
        upgrade.SetActive(true);
        upgradeText.text = "You got the " + e + "!";
        equipment.sprite = (e.Equals("Blue Robe") ? blueRobe : (e.Equals("Red Robe") ? redRobe : (e.Equals("Rusty Sword")
                    ? rustySword : (e.Equals("Jagged Blade") ? jaggedBlade : (e.Equals("Warped Edge")
                            ? warpedEdge : (e.Equals("Knight's Sword") ? knightsSword : kingsbane))))));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        upgrade.SetActive(false);
        playerController.playerAnimator.runtimeAnimatorController = Instantiate((e.Equals("Blue Robe") ? playerController.playerB 
            : (e.Equals("Red Robe") ? playerController.playerR : playerController.playerG)), playerController.transform);
        playerController.weaponAnimator.runtimeAnimatorController = Instantiate((e.Equals("Rusty Sword") ? playerController.rustySword
                : (e.Equals("Jagged Blade") ? playerController.jaggedBlade : (e.Equals("Warped Edge") ? playerController.warpedEdge
                        : (e.Equals("Knight's Sword") ? playerController.knightsSword : (e.Equals("Kingsbane") ? playerController.kingsbane
                                : playerController.weaponAnimator.runtimeAnimatorController))))), playerController.weapon.transform);
        ResumeGame();
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

    public void GameOver()
    {
        gameOver.SetActive(true);
    }

    public bool IsGameOver()
    {
        return gameOver.activeSelf;
    }

    public void Level(int n, int d)
    {
        levelNum = n;
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

    public void LevelNext()
    {
        next.SetActive(true);
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
        List<string> t = new List<string>(3);
        List<string> d = new List<string>(3);
        GameObject primary = disciplines.transform.GetChild(4).gameObject;
        GameObject secondary = disciplines.transform.GetChild(5).gameObject;
        GameObject tertiary = disciplines.transform.GetChild(6).gameObject;
        var strPrimary = ScoreManager.DisciplinePrimary();
        var strSecondary = ScoreManager.DisciplineSecondary();
        switch (strPrimary)
        {
            case "Aggressive":
                string titleAggressive = ScoreManager.TitleAggressive(difficulty);
                primary.GetComponent<Image>().sprite = spriteAggressive;
                primary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                primary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;
                primary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titleAggressive);
                primary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+1 Attack");
                d.Add("Aggressive");
                t.Add(titleAggressive);
                break;
            case "Defensive":
                string titleDefensive = ScoreManager.TitleDefensive(difficulty);
                primary.GetComponent<Image>().sprite = spriteDefensive;
                primary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0.7f, 1);
                primary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0, 0.7f, 1);
                primary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titleDefensive);
                primary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+2 Health");
                d.Add("Defensive");
                t.Add(titleDefensive);
                break;
        }
        switch (strSecondary)
        {
            case "Exploration":
                string titleExploration = ScoreManager.TitleExploration(difficulty);
                secondary.GetComponent<Image>().sprite = spriteExploration;
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.8f, 0);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.8f, 0);
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titleExploration);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+50% Map Vision");
                d.Add("Exploration");
                t.Add(titleExploration);
                break;
            case "Collection":
                string titleCollection = ScoreManager.TitleCollection(difficulty);
                secondary.GetComponent<Image>().sprite = spriteCollection;
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 0.75f, 0);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1, 0.75f, 0);
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titleCollection);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+2 Inventory");
                d.Add("Collection");
                t.Add(titleCollection);
                break;
            case "Puzzle Solving":
                string titlePuzzleSolving = ScoreManager.TitlePuzzleSolving(difficulty);
                secondary.GetComponent<Image>().sprite = spritePuzzleSolving;
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.2f, 1);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.2f, 1);
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titlePuzzleSolving);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+3 Speed");
                d.Add("Puzzle Solving");
                t.Add(titlePuzzleSolving);
                break;
        }
        yield return new WaitForSecondsRealtime(1);
        primary.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        secondary.SetActive(true);
        if (levelNum > 10)
        {
            yield return new WaitForSecondsRealtime(1);
            tertiary.GetComponent<TextMeshProUGUI>().color = new Color(1, 0.55f, 0);
            string titleTertiary = ScoreManager.TitleTertiary();
            tertiary.GetComponent<TextMeshProUGUI>().SetText(titleTertiary);
            d.Add("Tertiary");
            t.Add(titleTertiary);
            tertiary.SetActive(true);
        }
        else
        {
            d.Add(null);
            t.Add(null);
        }
        disciplines.transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        disciplines.SetActive(false);
        playerController.SaveData(d, t);
        LevelNext();
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

    public void Next()
    {
        SceneManager.LoadScene("Main");
    }
    
    public void Restart()
    {
        playerController.health = 0;
        playerController.SaveData();
        SceneManager.LoadScene("Main");
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
