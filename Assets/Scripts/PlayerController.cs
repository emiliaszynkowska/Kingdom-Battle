using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement
    private Vector2 movement;
    public float speed = 0.1f;
    // Combat
    public int attack;
    public int health;
    public int livesActive;
    // Booleans
    private bool isAttacking;
    private bool isBlocking;
    private bool isHurt;
    private bool isBouncing;
    private bool ogreStrength;
    // Player Information
    public string playerName;
    public List<string> playerDisciplines = new List<string>();
    public List<string> playerTitles = new List<string>();
    public int playerCoins;
    // Variables
    public List<string> inventory = new List<string>();
    // Objects
    public Rigidbody2D body;
    public SpriteRenderer playerRenderer;
    public Animator playerAnimator;
    public Animator weaponAnimator;
    public GameObject weapon;
    public GameObject shield;
    public UIManager uiManager;
    public SoundManager soundManager;
    public QuestManager questManager;
    public GameObject lifeup;
    public GameObject shadow;
    public GameObject shine;
    public GameObject death;
    public ParticleSystem particles;
    public GameObject timers;
    public Collider2D attackCollider;
    public Collider2D spinAttackCollider;
    public Collider2D groundPoundCollider;
    public ContactFilter2D contactFilter;
    // Prefabs
    public RuntimeAnimatorController playerR;
    public RuntimeAnimatorController playerG;
    public RuntimeAnimatorController playerB;
    public RuntimeAnimatorController rustySword;
    public RuntimeAnimatorController jaggedBlade;
    public RuntimeAnimatorController warpedEdge;
    public RuntimeAnimatorController knightsSword;
    public RuntimeAnimatorController kingsbane;

    public void Start()
    {
        LoadData();
        Initialise();
    }

    public void Update()
    {
        // Check Inputs
        CheckCombat();
        CheckUI();

        // Calculate player movement
        movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        Flip();
        
        // Set animations
        playerAnimator.SetFloat("Horizontal",movement.x);
        playerAnimator.SetFloat("Speed",movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        if (!isBouncing)
            transform.Translate(movement.x * speed, movement.y * speed, 0, Space.Self);
    }

    public int GetCoins()
    {
        return playerCoins;
    }

    public void Spend(int coins)
    {
        playerCoins -= coins;
    }

    void Flip()
    {
        // Player
        if (Input.GetAxis("Horizontal") > 0)
            playerRenderer.flipX = false;
        else if (Input.GetAxis("Horizontal") < 0)
            playerRenderer.flipX = true;
        // Weapon & Shield
        if (Input.GetKey(KeyCode.D))
        {
            weapon.transform.localPosition = new Vector2(1, 0);
            weapon.GetComponent<SpriteRenderer>().flipX = false;
            shield.transform.localPosition = new Vector3(1, -0.3f, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            weapon.transform.localPosition = new Vector2(-1, 0);
            weapon.GetComponent<SpriteRenderer>().flipX = true;
            shield.transform.localPosition = new Vector3(-1, -0.3f, 0);
        }
    }
    
    public int GetAttack()
    {
        return attack;
    }
    
    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }
    
    public bool IsBouncing()
    {
        return isBouncing;
    }

    public bool IsHurt()
    {
        return isHurt;
    }
    
    void StopBounce()
    {
        isBouncing = false;
        body.velocity = new Vector2(0, 0);
    }

    public void StartBlock()
    {
        StartCoroutine(Block());
    }

    public void StartAttack()
    {
        StartCoroutine(Attack());
    }
    
    public void StartSpinAttack()
    {
        StartCoroutine(SpinAttack());
    }
    
    public void StartGroundPound()
    {
        StartCoroutine(GroundPound());
    }
    
    public List<string> GetInventory()
    {
        return inventory;
    }

    public void AddItem(string item)
    {
        inventory.Add(item);
        questManager.Event($"Find {item}", 0, true);
    }

    public void RemoveItem(string item)
    {
        inventory.Remove("Scroll");
    }
    
// ---------------------------------------------------------------------------------------------------------------------
// Combat
    
    IEnumerator TakeDamage(int damage)
    {
        if (health > 0 && !IsHurt() && !IsAttacking() && !IsBlocking())
        {
            isHurt = true;
            health -= damage;
            uiManager.SetLives(health);
            soundManager.PlaySound(soundManager.elfDamage);
            playerRenderer.color = new Color(0.8f, 0.22f, 0.2f);
            yield return new WaitForSeconds(1);
            playerRenderer.color = Color.white;
            isHurt = false;
        }
        if (health <= 0 && !uiManager.IsGameOver())
        {
            uiManager.PauseGame();
            soundManager.PlaySound(soundManager.elfDie);
            death.GetComponent<SpriteRenderer>().flipX = false;
            death.transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
            death.SetActive(true);
            playerRenderer.enabled = false;
            weapon.SetActive(false);
            yield return new WaitForSecondsRealtime(1);
            uiManager.GameOver();
        }
    }

    IEnumerator TakeKnockback(Vector3 source)
    {
        if (!isBlocking)
        {
            isBouncing = true;
            Vector3 force = transform.position - source;
            body.AddForce((force.normalized) * 3, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.5f);
            isBouncing = false;
            StopBounce();
        }
    }

    IEnumerator Block()
    {
        if (timers.transform.GetChild(0).gameObject.GetComponent<TimerController>().CanAttack())
        {
            // Start Blocking
            isBlocking = true;
            // Animate Block
            soundManager.PlaySound(soundManager.block);
            weapon.SetActive(false);
            shield.SetActive(true);
            yield return new WaitForSeconds(2);
            // Stop Blocking
            shield.SetActive(false);
            weapon.SetActive(true);
            isBlocking = false;
            ScoreManager.AddDefensive(1);
            timers.transform.GetChild(0).gameObject.GetComponent<TimerController>().Reset();
        }
    }

    IEnumerator Attack()
    {
        if (timers.transform.GetChild(1).gameObject.GetComponent<TimerController>().CanAttack())
        {
            // Start Attacking
            isAttacking = true;
            // Animate Attack
            soundManager.PlaySound(soundManager.swordAttack);
            shield.SetActive(false);
            weapon.SetActive(true);
            weaponAnimator.SetTrigger("Attack");
            // Damage Enemies
            Collider2D[] results = new Collider2D[5];
            attackCollider.OverlapCollider(contactFilter, results);
            foreach (Collider2D col in results)
            {
                if (col != null)
                {
                    EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        StartCoroutine("TakeKnockback", enemy.transform.position);
                        enemy.StartCoroutine("TakeDamage", attack);
                        if (IsOgreStrength())
                            enemy.StartCoroutine("PoisonDamage");
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
            // Stop Attacking
            isAttacking = false;
            timers.transform.GetChild(1).gameObject.GetComponent<TimerController>().Reset();
        }
    }
    
    IEnumerator SpinAttack()
    {
        if (timers.transform.GetChild(2).gameObject.GetComponent<TimerController>().CanAttack())
        {
            // Start Attacking
            isAttacking = true;
            // Animate Spin Attack
            soundManager.PlaySound(soundManager.spinAttack);
            shield.SetActive(false);
            weapon.SetActive(true);
            weaponAnimator.SetTrigger("Spin");
            // Damage Enemies
            Collider2D[] results = new Collider2D[5];
            spinAttackCollider.OverlapCollider(contactFilter, results);
            foreach (Collider2D col in results)
            {
                if (col != null)
                {
                    EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        StartCoroutine("TakeKnockback", enemy.transform.position);
                        enemy.StartCoroutine("TakeDamage", attack * 2);
                        if (IsOgreStrength())
                            enemy.StartCoroutine("PoisonDamage");
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
            // Stop Attacking
            isAttacking = false;
            timers.transform.GetChild(2).gameObject.GetComponent<TimerController>().Reset();
            questManager.Event("Spin Attack", "Use", true);
        }
    }
    
    IEnumerator GroundPound()
    {
        if (timers.transform.GetChild(3).gameObject.GetComponent<TimerController>().CanAttack())
        {
            // Start Attacking
            isAttacking = true;
            // Animate Jump
            soundManager.PlaySound(soundManager.jump);
            shield.SetActive(false);
            weapon.SetActive(true);
            playerAnimator.SetTrigger("Jump");
            yield return new WaitForSeconds(0.5f);
            shadow.SetActive(true);
            // Animate Ground Pound
            soundManager.PlaySound(soundManager.groundPound);
            Instantiate(particles, transform);
            // Damage Enemies
            Collider2D[] results = new Collider2D[8];
            groundPoundCollider.OverlapCollider(contactFilter, results);
            foreach (Collider2D col in results)
            {
                if (col != null)
                {
                    EnemyController enemy = col.gameObject.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.StartCoroutine("TakeDamage", attack * 3);
                        if (IsOgreStrength())
                            enemy.StartCoroutine("PoisonDamage");
                    }
                }
            }
            // Stop Attacking
            yield return new WaitForSeconds(1);
            shadow.SetActive(false);
            isAttacking = false;
            timers.transform.GetChild(3).gameObject.GetComponent<TimerController>().Reset();
            questManager.Event("Ground Pound", "Use", true);
        }
    }
    
// ---------------------------------------------------------------------------------------------------------------------
// Input

    void CheckCombat()
    {
        // Block
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartBlock();
            timers.transform.GetChild(0).GetComponent<TimerController>().Reset();
        }
        // Attack
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartAttack();
            timers.transform.GetChild(1).GetComponent<TimerController>().Reset();
        }
        // Spin Attack
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartSpinAttack();
            timers.transform.GetChild(2).GetComponent<TimerController>().Reset();
        }
        // Ground Pound
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGroundPound();
            timers.transform.GetChild(3).GetComponent<TimerController>().Reset();
        }
    }
        
    void CheckUI()
    {
        // Interact
        if (Input.GetKeyDown(KeyCode.E) && !uiManager.IsInventory() && Time.timeScale != 0)
        {
            StartCoroutine(uiManager.Interact(0, 0, 0));
            Collider2D[] results = new Collider2D[8];
            spinAttackCollider.OverlapCollider(contactFilter, results);
            foreach (Collider2D col in results)
            {
                if (col != null && col.CompareTag("NPC"))
                {
                    if (col.name.Contains("Wizard"))
                    {
                        StartCoroutine(col.GetComponent<Quest>().Talk());
                        questManager.Event($"Talk to {col.GetComponent<Quest>().npcName}", 0, true);
                    }
                    else if (col.name.Equals("Merchant"))
                    {
                        col.GetComponent<MerchantController>().Talk();
                        questManager.Event("Talk to Merchant", 0, true);
                    }
                    else if (col.name.Equals("Wigg"))
                    {
                        StartCoroutine(col.GetComponent<WiggController>().Talk());
                        questManager.Event("Talk to Wigg", 0, false);
                    }
                }
                else if (col != null && col.CompareTag("Chest") && inventory.Contains("Key"))
                {
                    var index = inventory.IndexOf("Key");
                    if (UseItem(index))
                    {
                        uiManager.activeItem = index;
                        uiManager.UseItem();
                        uiManager.DisableItem(inventory.Count);
                    }
                }
                else if (col != null && col.CompareTag("Door") && inventory.Contains("Boss Key"))
                {
                    var index = inventory.IndexOf("Boss Key");
                    if (UseItem(index))
                    {
                        uiManager.activeItem = index;
                        uiManager.UseItem();
                        uiManager.DisableItem(inventory.Count);
                    }
                }
                else if (col != null && col.CompareTag("Door") && inventory.Contains("Key"))
                {
                    var index = inventory.IndexOf("Key");
                    if (UseItem(index))
                    {
                        uiManager.activeItem = index;
                        uiManager.UseItem();
                        uiManager.DisableItem(inventory.Count);
                    }
                }
                else if (col != null && col.CompareTag("Door") && col.GetComponent<DoorController>().special)
                {
                    col.GetComponent<DoorController>().Boss();
                }
            }
        }
        
        // Inventory
        if (Input.GetKeyDown(KeyCode.I) && !uiManager.IsInventory() && uiManager.options.activeSelf && Time.timeScale != 0)
        {
            StartCoroutine(uiManager.Interact(1, 0, 0));
            StartCoroutine(uiManager.Interact(1, 1, 0));
            uiManager.Inventory();
        }
        else if (Input.GetKeyDown(KeyCode.I) && uiManager.IsInventory())
        {
            StartCoroutine(uiManager.Interact(1, 1, 0));
            StartCoroutine(uiManager.Interact(1, 0, 0));
            uiManager.ExitInventory();
        }
        else if (Input.GetKeyDown(KeyCode.D) && uiManager.IsInventory() && inventory.Count - 1 > uiManager.activeItem)
        {
            StartCoroutine(uiManager.Interact(2, 1, 1));
            if (uiManager.activeItem >= 5 && playerDisciplines[1].Equals("Collection"))
            {
                uiManager.activeItem++;
                uiManager.ChangeItem(uiManager.activeItem);
            }
            else if (uiManager.activeItem < 5)
            {
                uiManager.activeItem++;
                uiManager.ChangeItem(uiManager.activeItem);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) && uiManager.IsInventory() && uiManager.activeItem > 0)
        {
            StartCoroutine(uiManager.Interact(3, 1, 1));
            uiManager.activeItem--;
            uiManager.ChangeItem(uiManager.activeItem);
        }
        else if (Input.GetKeyDown(KeyCode.E) && uiManager.IsInventory())
        {
            StartCoroutine(uiManager.Interact(0, 1, 0));
            if (UseItem(uiManager.activeItem))
            {
                uiManager.UseItem();
                uiManager.DisableItem(inventory.Count);
            }
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A)) && uiManager.IsInventory())
        {
            StartCoroutine(uiManager.Error());
        }

        // Quests
        if (Input.GetKeyDown(KeyCode.Q) && !uiManager.IsQuests() && Time.timeScale != 0)
        {
            StartCoroutine(uiManager.Interact(2, 0, 0));
            uiManager.Quests();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && uiManager.IsQuests())
        {
            StartCoroutine(uiManager.Interact(2, 0, 0));
            uiManager.ExitQuests();
        }
        
        // Menu
        else if (Input.GetKeyDown(KeyCode.Escape) && !uiManager.IsMenu() && Time.timeScale != 0)
        {
            StartCoroutine(uiManager.Interact(3, 0, 0));
            uiManager.Menu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && uiManager.IsMenu())
        {
            StartCoroutine(uiManager.Interact(3, 0, 0));
            uiManager.ExitMenu();
        }
    }

// ---------------------------------------------------------------------------------------------------------------------
// Items

    public bool UseItem(int index)
    {
        questManager.Event($"Use {inventory[index]}", 0, true);
        switch (inventory[index])
        {
            // Key
            case "Key":
                Collider2D[] results1 = new Collider2D[8];
                spinAttackCollider.OverlapCollider(contactFilter, results1);
                foreach (Collider2D col in results1)
                {
                    if (col != null && col.CompareTag("Chest") && inventory.Contains("Key"))
                    {
                        if (col.GetComponent<ChestController>().Open())
                        {
                            inventory.RemoveAt(index);
                            questManager.Event("Open a chest", 0, true);
                            questManager.Event("chest", "Open", true);
                            return true;
                        }
                    }
                    else if (col != null && col.CompareTag("Door") && inventory.Contains("Key"))
                    {
                        if (!col.GetComponent<DoorController>().boss && !col.GetComponent<DoorController>().special && col.GetComponent<DoorController>().Open())
                        {
                            inventory.RemoveAt(index);
                            questManager.Event("Open a door", 0, true);
                            questManager.Event("door", "Open", true);
                            return true;
                        }
                        else if (col.GetComponent<DoorController>().special)
                        {
                            if (col.GetComponent<DoorController>().Boss())
                            {
                                inventory.RemoveAt(index);
                                return true;
                            }
                        }
                    }
                }
                return false;
            // Boss Key
            case "Boss Key":
                Collider2D[] results2 = new Collider2D[8];
                spinAttackCollider.OverlapCollider(contactFilter, results2);
                foreach (Collider2D col in results2)
                {
                    if (col != null && col.CompareTag("Door") && inventory.Contains("Boss Key"))
                    {
                        if (col.GetComponent<DoorController>().boss && col.GetComponent<DoorController>().Open())
                        {
                            inventory.RemoveAt(index);
                            return true;
                        }
                    }
                }
                return false;
            // Potion
            case "Wigg's Brew":
                StartCoroutine(WiggsBrew());
                if (health < livesActive)
                {
                    health++;
                    uiManager.SetLives(health);
                }
                inventory.RemoveAt(index);
                uiManager.ExitInventory();
                return true;
            case "Liquid Luck":
                StartCoroutine(LiquidLuck());
                inventory.RemoveAt(index);
                uiManager.ExitInventory();
                return true;
            case "Ogre's Strength":
                StartCoroutine(OgreStrength());
                inventory.RemoveAt(index);
                uiManager.ExitInventory();
                return true;
            case "Elixir of Speed":
                StartCoroutine(ElixirOfSpeed());
                inventory.RemoveAt(index);
                uiManager.ExitInventory();
                return true;
        }
        return false;
    }
    
    // Wigg's Brew - increase health by 1 and glow red 
    IEnumerator WiggsBrew()
    {
        ScoreManager.AddDefensive(5);
        soundManager.PlaySound(soundManager.lifeup);
        questManager.Event("Restore some health", 0, true);
        uiManager.Powerup("Wigg's Brew: +1 Health", Color.red);
        lifeup.SetActive(true);
        yield return new WaitForSeconds(3);
        uiManager.Powerup("", Color.white);
        lifeup.SetActive(false);
    }

    // Liquid Luck - increase attack by 1 and glow yellow for 5 seconds
    IEnumerator LiquidLuck()
    {
        ScoreManager.AddAggressive(1);
        soundManager.PlaySound(soundManager.powerup);
        questManager.Event("Gain an attack boost", 0, true);
        uiManager.Powerup("Liquid Luck: +1 Attack", new Color(1,0.9f,0));
        GetComponent<SpriteRenderer>().color = new Color(1,0.9f,0);
        shine.SetActive(true);
        attack++;
        yield return new WaitForSeconds(10);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = Color.white;
        shine.SetActive(false);
        attack--;
    }

    // Ogre's Strength - apply poison damage to damaged enemies for 5 seconds
    IEnumerator OgreStrength()
    {
        ScoreManager.AddAggressive(1);
        soundManager.PlaySound(soundManager.powerup);
        uiManager.Powerup("Ogre's Strength: +Poison Damage", new Color(0, 0.75f, 0));
        GetComponent<SpriteRenderer>().color = new Color(0, 0.75f, 0);
        ogreStrength = true;
        yield return new WaitForSeconds(10);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = Color.white;
        ogreStrength = false;
    }

    // Elixir of Speed - increase speed for 10 seconds
    IEnumerator ElixirOfSpeed()
    {
        ScoreManager.AddExploration(1);
        soundManager.PlaySound(soundManager.powerup);
        questManager.Event("Gain a speed boost", 0, true);
        uiManager.Powerup("Elixir of Speed: +5 Speed", new Color(0,0.4f,1));
        GetComponent<SpriteRenderer>().color = new Color(0,0.4f,1);
        speed = 0.15f;
        yield return new WaitForSeconds(15);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = Color.white;
        speed = 0.1f;
    }

    public bool IsOgreStrength()
    {
        return ogreStrength;
    }
    
// ---------------------------------------------------------------------------------------------------------------------
// Triggers and Collisions
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.otherCollider is BoxCollider2D)
        {
            // Bounce off the enemy
            StartCoroutine(TakeKnockback(collision.transform.position));
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.health > 0)
            {
                // Take damage
                int enemyAttack = collision.gameObject.GetComponent<EnemyController>().GetAttack();
                StartCoroutine(TakeDamage(enemyAttack));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            if (!other.gameObject.name.Equals("Coin"))
            {
                ScoreManager.AddCollection(1);
                if ((inventory.Count == 6 && !playerDisciplines.Contains("Collection")) ||
                    (inventory.Count == 8 && playerDisciplines.Contains("Collection")))
                    uiManager.InvFull(other.transform.position);
            }
            // Check item
            switch (other.name)
            {
                // Coin
                case "Coin":
                    soundManager.PlaySound(soundManager.coin);
                    Destroy(other.gameObject);
                    playerCoins++;
                    uiManager.SetCoins(playerCoins);
                    questManager.Event("coin", "Collect", true);
                    break;
                // Scroll
                case "Scroll":
                    if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines.Contains("Collection")))
                    {
                        soundManager.PlaySound(soundManager.item);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Scroll", inventory.Count);
                        AddItem("Scroll");
                        if (questManager.Event("Find the scroll", 0, false))
                            questManager.AddMainQuest("Return to Wigg");
                    }
                    break;
                // Key
                case "Key":
                    if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines.Contains("Collection")))
                    {
                        soundManager.PlaySound(soundManager.item);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Key", inventory.Count);
                        AddItem("Key");
                    }
                    break;
                // Boss Key
                case "Boss Key":
                    if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines.Contains("Collection")))
                    {
                        soundManager.PlaySound(soundManager.win);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Boss Key", inventory.Count);
                        AddItem("Boss Key");
                    }
                    else
                    {
                        uiManager.RemoveItem(inventory.Count - 1);
                        inventory.RemoveAt(inventory.Count - 1);
                        soundManager.PlaySound(soundManager.win);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Boss Key", inventory.Count);
                        AddItem("Boss Key");
                    }
                    break;
                // Potion
                case "Wigg's Brew":
                    if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines.Contains("Collection")))
                    {
                        soundManager.PlaySound(soundManager.item);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Wigg's Brew", inventory.Count);
                        AddItem("Wigg's Brew");
                    }
                    break;
                case "Liquid Luck":
                    if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines.Contains("Collection")))
                    {
                        soundManager.PlaySound(soundManager.item);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Liquid Luck", inventory.Count);
                        AddItem("Liquid Luck");
                    }
                    break;
                case "Ogre's Strength":
                    if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines.Contains("Collection")))
                    {
                        soundManager.PlaySound(soundManager.item);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Ogre's Strength", inventory.Count);
                        AddItem("Ogre's Strength");
                    }
                    break;
                case "Elixir of Speed":
                    if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines.Contains("Collection")))
                    {
                        soundManager.PlaySound(soundManager.item);
                        Destroy(other.gameObject);
                        uiManager.AddItem("Elixir of Speed", inventory.Count);
                        AddItem("Elixir of Speed");
                    }
                    break;
            }
        }
        
        else if (other.gameObject.CompareTag("Enemy") && other is CircleCollider2D && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            // Take damage
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.health > 0)
            {
                int enemyAttack = other.gameObject.GetComponent<EnemyController>().GetAttack();
                StartCoroutine(TakeDamage(enemyAttack));
            }
        }

        else if (other.gameObject.name.Equals("Spikes") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            // Take damage
            if (!IsBouncing())
                StartCoroutine(TakeDamage(1));
            // Bounce off the spikes
            body.AddForce(Vector2.up * 100);
            isBouncing = true;
            Invoke("StopBounce", 0.2f);
        }

        else if (other.gameObject.CompareTag("Projectile") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            Destroy(other);
            // Take damage
            StartCoroutine(TakeDamage(1));
        }
        
        else if (other.gameObject.CompareTag("Collider") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            ScoreManager.AddExploration(1);
            questManager.Event("room", "Explore", true);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && GetComponent<BoxCollider2D>().IsTouching(other) && !other.gameObject.name.Equals("Coin") && 
            ((inventory.Count == 6 && !playerDisciplines.Contains("Collection")) || (inventory.Count == 8 && playerDisciplines.Contains("Collection"))))
            uiManager.InvFull(other.transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        uiManager.ExitInvFull();
    }

    // ---------------------------------------------------------------------------------------------------------------------
// Player Data

    public void SaveData(bool boss, bool increment)
    {
        PlayerData.LivesActive = livesActive;
        PlayerData.Health = health;
        PlayerData.Name = playerName;
        PlayerData.Coins = playerCoins;
        PlayerData.Inventory = inventory;
        if (increment)
            PlayerData.Level = uiManager.levelNum + 1;
        else
            PlayerData.Level = uiManager.levelNum;
        PlayerData.Boss = boss;
        PlayerData.Disciplines = playerDisciplines;
        PlayerData.Titles = playerTitles;
    }
    
    public void SaveData(List<string> d, List<string> t, bool boss)
    {
        PlayerData.LivesActive = livesActive;
        PlayerData.Health = health;
        PlayerData.Name = playerName;
        PlayerData.Coins = playerCoins;
        PlayerData.Inventory = inventory;
        PlayerData.Level = uiManager.levelNum + 1;
        PlayerData.Boss = boss;
        PlayerData.Disciplines = d;
        PlayerData.Titles = t;
    }
    
    public void LoadData()
    {
        // Check if save data exists
        if (PlayerData.Name != null)
        {
            // Load save data
            if (PlayerData.Health != 0)
            {
                playerName = PlayerData.Name;
                var oldLivesActive = PlayerData.LivesActive;
                attack = (PlayerData.Level < 2) ? 1 : (PlayerData.Level < 5 ? 2 : (PlayerData.Level < 8 ? 3 : (PlayerData.Level < 11 ? 4 : 5)));
                livesActive = (PlayerData.Level < 5) ? 3 : (PlayerData.Level < 8 ? 6 : 9);
                health = PlayerData.Health + (livesActive - oldLivesActive);
                playerCoins = PlayerData.Coins;
                inventory = PlayerData.Inventory;
                playerDisciplines = PlayerData.Disciplines;
                playerTitles = PlayerData.Titles;
            }
            // If respawned
            else
            {
                playerName = PlayerData.Name;
                attack = (PlayerData.Level < 2) ? 1 : (PlayerData.Level < 5 ? 2 : (PlayerData.Level < 8 ? 3 : (PlayerData.Level < 11 ? 4 : 5)));
                livesActive = (PlayerData.Level < 5) ? 3 : (PlayerData.Level < 8 ? 6 : 9);
                health = livesActive;
                playerDisciplines = PlayerData.Disciplines;
                playerTitles = PlayerData.Titles;
            }
        }
        else
        {
            // Create save data
            playerName = "Player";
            health = 3;
            livesActive = 3;
            attack = 1;
            playerDisciplines = new List<string>() {"Initial", null, null};
            playerTitles = new List<string>() {ScoreManager.TitleInitial(), null, null};
        }
    }

    void Initialise()
    {
        // Initialise UI components
        uiManager.SetInfo(playerName, playerTitles, playerCoins);
        uiManager.SetAttacks(livesActive / 3);
        uiManager.SetLivesActive(livesActive);
        uiManager.SetLives(health);
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i <= 5)
                uiManager.AddItem(inventory[i], i);
            if (i > 5 && playerDisciplines.Contains("Collection"))
                uiManager.AddItem(inventory[i], i);
        }
        playerAnimator.runtimeAnimatorController = Instantiate((livesActive < 6 ? playerG : (livesActive < 9 ? playerB : playerR)));
        weaponAnimator.runtimeAnimatorController = Instantiate((attack == 1 ? rustySword : (attack == 2 ? jaggedBlade : (attack == 3 ? warpedEdge : (attack == 4 ? knightsSword : kingsbane)))), weapon.transform);
        // Apply discipline rewards
        if (playerDisciplines[1] != null)
        {
            if (playerDisciplines[0].Equals("Aggressive"))
            {
                attack += 1;
                uiManager.SetEffect(1, 40, uiManager.spriteAggressive);
            }
            else if (playerDisciplines[0].Equals("Defensive"))
            {
                livesActive += 2;
                health += 2;
                uiManager.SetLivesActive(livesActive);
                uiManager.SetLives(health);
                uiManager.SetEffect(1, 40, uiManager.blueLife);
            }

            if (playerDisciplines[1].Equals("Exploration"))
            {
                transform.GetChild(2).GetComponent<Camera>().orthographicSize *= 2;
                uiManager.SetEffect(2, 40, uiManager.spriteExploration);
            }
            else if (playerDisciplines[1].Equals("Collection"))
            {
                uiManager.SetEffect(2, 50, uiManager.key);
            }
            else if (playerDisciplines[1].Equals("Puzzle Solving"))
            {
                speed *= 1.3f;
                uiManager.SetEffect(2, 45, uiManager.elixirofSpeed);
            }
        }
    }
}