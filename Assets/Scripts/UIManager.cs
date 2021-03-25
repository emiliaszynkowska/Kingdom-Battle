using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Objects
    public PlayerController playerController;
    public PlayerTutorial playerTutorial;
    public SoundManager soundManager;
    public Camera mainCamera;
    public GameObject map;
    public GameObject info;
    public GameObject lives;
    public GameObject timers;
    public GameObject inventory;
    public GameObject items;
    public GameObject active;
    public GameObject inactive1;
    public GameObject inactive2;
    public GameObject description;
    public GameObject options;
    public GameObject optionsInventory;
    public GameObject optionsShop;
    public GameObject minimap;
    public GameObject dialog;
    public Text npcName;
    public Text message;
    public Text prompt;
    public GameObject nameInput;
    public InputField inputField;
    public GameObject menu;
    public GameObject quests;
    public GameObject complete;
    public GameObject scores;
    public GameObject disciplines;
    public GameObject level;
    public GameObject next;
    public GameObject gameOver;
    public Text powerup;
    public Text notification;
    public GameObject upgrade;
    public GameObject invFull;
    public Image difficulty;
    public Image equipment;
    public Image background;
    public Image fade;
    // Variables
    public bool isPowerup;
    public bool isError;
    public int levelNum;
    public bool bossFight;
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

    private Dictionary<string,string> descriptions = new Dictionary<string, string>()
    {
        {"Key", "Opens doors and chests."},
        {"Wigg's Brew", "Restores some health."},
        {"Liquid Luck", "Increases your ability in combat."},
        {"Ogre's Strength", "Gradually poisons enemies over time."},
        {"Elixir of Speed", "Increases your movement speed."},
        {"Boss Key", "A large gold key engraved with a monster head."},
        {"Scroll", "A mysterious item filled with knowledge."}
    };
        
    public void Start()
    {
        if (!bossFight)
        {
            ScoreManager.Reset();
            ScoreManager.active = true;
        }
        ResetCamera();
        StartCoroutine(FadeText());
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

    public void SetName(string n)
    {
        info.transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = n;
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
                timers.transform.GetChild(1).gameObject.SetActive(true);
                timers.transform.GetChild(2).gameObject.SetActive(false);
                timers.transform.GetChild(3).gameObject.SetActive(false);
                timers.transform.GetChild(0).localPosition = new Vector3(-150, 25, 0);
                timers.transform.GetChild(1).localPosition = new Vector3(150, 50, 0);
                break;
            case 2: 
                timers.transform.GetChild(0).gameObject.SetActive(true);
                timers.transform.GetChild(1).gameObject.SetActive(true);
                timers.transform.GetChild(2).gameObject.SetActive(true);
                timers.transform.GetChild(3).gameObject.SetActive(false);
                timers.transform.GetChild(0).localPosition = new Vector3(-300, 25, 0);
                timers.transform.GetChild(1).localPosition = new Vector3(0, 50, 0);
                timers.transform.GetChild(2).localPosition = new Vector3(300, 50, 0);
                break;
            case 3: 
                timers.transform.GetChild(0).gameObject.SetActive(true);
                timers.transform.GetChild(1).gameObject.SetActive(true);
                timers.transform.GetChild(2).gameObject.SetActive(true);
                timers.transform.GetChild(3).gameObject.SetActive(true);
                timers.transform.GetChild(0).localPosition = new Vector3(-500, 25, 0);
                timers.transform.GetChild(1).localPosition = new Vector3(-220, 50, 0);
                timers.transform.GetChild(2).localPosition = new Vector3(90, 50, 0);
                timers.transform.GetChild(3).localPosition = new Vector3(410, 50, 0);
                break;
        }
    }

    public void SetDifficulty(float d)
    {
        difficulty.fillAmount = d/100;
    }

    // Inventory
    public void Inventory()
    {
        PauseGame();
        soundManager.PauseMusic();
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

        if (playerTutorial != null)
        {
            inactive1.SetActive(false);
            inactive2.SetActive(false);
        }
    }

    public void ExitInventory()
    {
        ResumeGame();
        soundManager.ResumeMusic();
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
        soundManager.PlaySound(soundManager.changeItem);
        activeItem = index;
        active.transform.position = items.transform.GetChild(activeItem).position;
        if (playerController != null)
        {
            if (playerController.GetInventory().Count > 0)
            {
                ChangeDescription(playerController.GetInventory()[activeItem]);
                description.SetActive(true);
            }
            else
                description.SetActive(false);
        }
        else if (playerTutorial != null)
        {
            if (playerTutorial.GetInventory().Count > 0)
            {
                ChangeDescription(playerController.GetInventory()[activeItem]);
                description.SetActive(true);
            }
            else
                description.SetActive(false);
        }
    }

    public void ChangeDescription(string item)
    {
        description.transform.GetChild(0).GetComponent<Text>().text = item;
        description.transform.GetChild(1).GetComponent<Text>().text = descriptions[item];
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

    public void RemoveItem(int index)
    {
        activeItem = index;
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

    public void InvFull(Vector3 pos)
    {
        StartCoroutine(Error());
        invFull.SetActive(true);
        invFull.transform.localPosition = new Vector3(0, pos.y + 40, 0);
    }

    public void ExitInvFull()
    {
        invFull.SetActive(false);
    }

    public bool IsError()
    {
        return isError;
    }

    public IEnumerator Error()
    {
        if (!IsError())
        {
            isError = true;
            soundManager.PlaySound(soundManager.error);
            yield return new WaitForSecondsRealtime(1);
            isError = false;
        }
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
        soundManager.PlaySound(soundManager.complete);
        upgrade.SetActive(true);
        upgrade.GetComponent<Text>().text = "You got the " + e + "!";
        switch (e)
        {
          case("Blue Robe"):
              equipment.sprite = blueRobe;
              break;
          case("Red Robe"):
              equipment.sprite = redRobe;
              break;
          case("Rusty Sword"):
              equipment.sprite = rustySword;
              break;
          case("Jagged Blade"):
              equipment.sprite = jaggedBlade;
              break;
          case("Warped Edge"):
              equipment.sprite = warpedEdge;
              break;
          case("Knight's Sword"):
              equipment.sprite = knightsSword;
              break;
          case("Kingsbane"):
              equipment.sprite = kingsbane;
              break;
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        upgrade.SetActive(false);
        switch (e)
        {
            case("Blue Robe"):
                playerController.playerAnimator.runtimeAnimatorController =
                    Instantiate(playerController.playerB, playerController.transform);
                break;
            case("Red Robe"):
                playerController.playerAnimator.runtimeAnimatorController =
                    Instantiate(playerController.playerR, playerController.transform);
                break;
            case("Jagged Blade"):
                playerController.weaponAnimator.runtimeAnimatorController = Instantiate(playerController.jaggedBlade,
                    playerController.weapon.transform);
                break;
            case("Warped Edge"):
                playerController.weaponAnimator.runtimeAnimatorController = Instantiate(playerController.warpedEdge,
                    playerController.weapon.transform);
                break;
            case("Knight's Sword"):
                playerController.weaponAnimator.runtimeAnimatorController = Instantiate(playerController.knightsSword,
                    playerController.weapon.transform);
                break;
            case("Kingsbane"):
                playerController.weaponAnimator.runtimeAnimatorController = Instantiate(playerController.kingsbane,
                    playerController.weapon.transform);
                break;
        }
        ResumeGame();
        yield return null;
    }

    // Options
    public IEnumerator Interact(int index, int option, int sound)
    {
        if (sound == 0)
            soundManager.PlaySound(soundManager.clickButton);
        else
            soundManager.PlaySound(soundManager.changeItem);
        switch (option)
        {
            // Options
            case(0):
                this.options.transform.GetChild(index).GetComponent<Image>().CrossFadeAlpha(0.25f, 0, true);
                yield return new WaitForSecondsRealtime(0.2f);
                this.options.transform.GetChild(index).GetComponent<Image>().CrossFadeAlpha(1, 0, true);
                break;
            // Inventory
            case(1):
                optionsInventory.transform.GetChild(index).GetComponent<Image>().CrossFadeAlpha(0.25f, 0, true);
                yield return new WaitForSecondsRealtime(0.2f);
                optionsInventory.transform.GetChild(index).GetComponent<Image>().CrossFadeAlpha(1, 0, true);
                break;
            // Shop
            case(2):
                optionsShop.transform.GetChild(index).GetComponent<Image>().CrossFadeAlpha(0.25f, 0, true);
                yield return new WaitForSecondsRealtime(0.2f);
                optionsShop.transform.GetChild(index).GetComponent<Image>().CrossFadeAlpha(1, 0, true);
                break;
        }
    }

    public IEnumerator Notification(string s, Color c)
    {
        notification.color = c;
        notification.text = s;
        yield return new WaitForSeconds(3);
        notification.text = "";
    }

    public void Quests()
    {
        PauseGame();
        soundManager.PauseMusic();
        quests.SetActive(true);
    }

    public void ExitQuests()
    {
        ResumeGame();
        soundManager.ResumeMusic();
        quests.SetActive(false);
    }

    public bool IsQuests()
    {
        return quests.activeSelf;
    }

    public void Menu()
    {
        PauseGame();
        soundManager.PauseMusic();
        menu.SetActive(true);
    }

    public void ExitMenu()
    {
        ResumeGame();
        soundManager.ResumeMusic();
        menu.SetActive(false);
    }

    public bool IsMenu()
    {
        return menu.activeSelf;
    }

    public void Shop()
    {
        soundManager.PauseMusic();
        soundManager.PlayMusic(soundManager.shopMusic);
        options.SetActive(false);
        optionsShop.SetActive(true);
    }

    public IEnumerator ExitShop()
    {
        yield return new WaitForSeconds(0.1f);
        soundManager.PlayMusic(soundManager.dungeonMusic);
        options.SetActive(true);
        optionsShop.SetActive(false);
    }

    public IEnumerator Speak(string npc, string text)
    {
        PauseGame();
        npcName.text = npc;
        message.text = text;
        dialog.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        dialog.SetActive(false);
        ResumeGame();
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

    public IEnumerator NameInput()
    {
        PauseGame();
        nameInput.SetActive(true);
        inputField.ActivateInputField();
        yield return new WaitUntil(() => !inputField.text.Equals(""));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        playerTutorial.playerName = inputField.text;
        nameInput.SetActive(false);
        ResumeGame();
    }
    
    public IEnumerator FadeText()
    {
        while (true)
        {
            prompt.CrossFadeColor(new Color(1, 1, 1, 0), 1, true, true);
            yield return new WaitForSecondsRealtime(1);
            prompt.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        soundManager.PlayMusic(soundManager.gameOverMusic);
    }

    public bool IsGameOver()
    {
        return gameOver.activeSelf;
    }

    public void Level(int n, int d)
    {
        ResetCamera();
        levelNum = n;
        PauseGame();
        level.transform.GetChild(1).GetComponent<Text>().text = "Level " + n.ToString();
        menu.SetActive(false);
        gameOver.SetActive(false);
        inventory.SetActive(false);
        level.SetActive(true);
    }
    
    public void Level(string s)
    {
        ResetCamera();
        if (levelNum == 10)
            StartCoroutine(ExitFade());
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
        ResetCamera();
        soundManager.ResumeMusic();
        soundManager.PlaySound(soundManager.clickButton);
        if (bossFight)
            soundManager.PlayMusic(soundManager.bossMusic);
        else
        {
            soundManager.PlayMusic(soundManager.dungeonMusic);
        }
        level.SetActive(false);
        ResumeGame();
        fade.CrossFadeAlpha(0.8f, 0, true);
        StartCoroutine(FadeOut());
    }

    public void LevelNext()
    {
        next.SetActive(true);
    }

    public void LevelComplete()
    {
        PauseGame(); 
        complete.SetActive(true);
        soundManager.PauseMusic();
        soundManager.PlayMusic(soundManager.victoryMusic);
    }

    public bool IsLevel()
    {
        return level.activeSelf;
    }

    public void Boss()
    {
        playerController.SaveData(true, false);
        StartCoroutine(FadeIn());
        SceneManager.LoadScene("Main");
    }

    public void ResetCamera()
    {
        mainCamera.orthographicSize = 7;
    }

    public void StartScores()
    {
        StartCoroutine(Scores(ScoreManager.GetScores()));
    }

    public IEnumerator Scores(int[] s)
    {
        complete.SetActive(false);
        scores.SetActive(true);
        GameObject aggressive = scores.transform.GetChild(4).gameObject;
        GameObject defensive = scores.transform.GetChild(5).gameObject;
        GameObject exploration = scores.transform.GetChild(6).gameObject;
        GameObject collection = scores.transform.GetChild(7).gameObject;
        GameObject puzzlesolving = scores.transform.GetChild(8).gameObject;
        yield return new WaitForSecondsRealtime(1);
        soundManager.PlaySound(soundManager.item);
        aggressive.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[0].ToString());
        yield return new WaitForSecondsRealtime(0.5f);
        soundManager.PlaySound(soundManager.item);
        defensive.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[1].ToString());
        yield return new WaitForSecondsRealtime(0.5f);
        soundManager.PlaySound(soundManager.item);
        exploration.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[2].ToString());
        yield return new WaitForSecondsRealtime(0.5f);
        soundManager.PlaySound(soundManager.item);
        collection.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[3].ToString());
        yield return new WaitForSecondsRealtime(0.5f);
        soundManager.PlaySound(soundManager.item);
        puzzlesolving.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(s[4].ToString());
        scores.transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        soundManager.PlaySound(soundManager.clickButton);
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
                string titleAggressive = ScoreManager.TitleAggressive(PlayerData.Difficulty);
                primary.GetComponent<Image>().sprite = spriteAggressive;
                primary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                primary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;
                primary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titleAggressive);
                primary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+1 Attack");
                d.Add("Aggressive");
                t.Add(titleAggressive);
                break;
            case "Defensive":
                string titleDefensive = ScoreManager.TitleDefensive(PlayerData.Difficulty);
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
                string titleExploration = ScoreManager.TitleExploration(PlayerData.Difficulty);
                secondary.GetComponent<Image>().sprite = spriteExploration;
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.8f, 0);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(0.1f, 0.8f, 0);
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titleExploration);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+50% Map Vision");
                d.Add("Exploration");
                t.Add(titleExploration);
                break;
            case "Collection":
                string titleCollection = ScoreManager.TitleCollection(PlayerData.Difficulty);
                secondary.GetComponent<Image>().sprite = spriteCollection;
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 0.75f, 0);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1, 0.75f, 0);
                secondary.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(titleCollection);
                secondary.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("+2 Inventory");
                d.Add("Collection");
                t.Add(titleCollection);
                break;
            case "Puzzle Solving":
                string titlePuzzleSolving = ScoreManager.TitlePuzzleSolving(PlayerData.Difficulty);
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
        soundManager.PlaySound(soundManager.complete);
        primary.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        soundManager.PlaySound(soundManager.complete);
        secondary.SetActive(true);
        if (levelNum > 10)
        {
            yield return new WaitForSecondsRealtime(1);
            soundManager.PlaySound(soundManager.complete);
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
        soundManager.PlaySound(soundManager.clickButton);
        disciplines.SetActive(false);
        PlayerData.Wins += 1;
        playerController.SaveData(d, t, false);
        LevelNext();
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
        soundManager.PlaySound(soundManager.clickButton);
        SceneManager.LoadScene("Main");
    }
    
    public void Restart()
    {
        soundManager.PlaySound(soundManager.clickButton);
        playerController.health = 0;
        if (bossFight)
            playerController.SaveData(true, false);
        else
            playerController.SaveData(false, false);
        SceneManager.LoadScene("Main");
    }

    public void Exit()
    {
        soundManager.PlaySound(soundManager.clickButton);
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
