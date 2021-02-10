using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Assets.Scripts
{
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
        public UIManager uiManager;
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
            // Check UI Inputs
            CheckUI();
        
            // Calculate player movement
            movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
            Flip();
        
            // Set animations
            playerAnimator.SetFloat("Horizontal",movement.x);
            playerAnimator.SetFloat("Speed",movement.sqrMagnitude);
        
            // Move the weapon
            if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
            {
                weapon.transform.localPosition = new Vector2(0.9f, -0.2f);
                weapon.GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
            {
                weapon.transform.localPosition = new Vector2(-0.9f,-0.2f);
                weapon.GetComponent<SpriteRenderer>().flipX = true;
            }
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
            if (Input.GetAxis("Horizontal") > 0)
                playerRenderer.flipX = false;
            else if (Input.GetAxis("Horizontal") < 0)
                playerRenderer.flipX = true;
        }
    
        public int GetAttack()
        {
            return attack;
        }
    
        public bool IsAttacking()
        {
            return isAttacking;
        }
    
        public bool IsBouncing()
        {
            return isBouncing;
        }

        public bool IsHurt()
        {
            return isHurt;
        }
    
        void StopBounceHorizontal()
        {
            isBouncing = false;
            body.velocity = new Vector2(0, body.velocity.y);
        }
    
        void StopBounceVertical()
        {
            isBouncing = false;
            body.velocity = new Vector2(body.velocity.x, 0);
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
        }
    
// ---------------------------------------------------------------------------------------------------------------------
// Combat
    
        IEnumerator TakeDamage(int damage)
        {
            if (health > 1 && !IsHurt() && !IsAttacking())
            {
                isHurt = true;
                health -= damage;
                uiManager.SetLives(health);
                playerRenderer.color = new Color(0.8f, 0.22f, 0.2f);
                yield return new WaitForSeconds(1);
                playerRenderer.color = Color.white;
                isHurt = false;
                ScoreManager.AddDefensive(1);
            }
            else if (health <= 1 && !uiManager.IsGameOver())
            {
                uiManager.PauseGame();
                health -= damage;
                uiManager.SetLives(health);
                death.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.22f, 0.2f);
                death.GetComponent<SpriteRenderer>().flipY = playerRenderer.flipX;
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
            isBouncing = true;
            Vector3 force = transform.position - source;
            body.AddForce((force.normalized) * 3, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.5f);
            isBouncing = false;
            StopBounceHorizontal();
            StopBounceVertical();
        }

        IEnumerator Attack()
        {
            if (timers.transform.GetChild(0).gameObject.GetComponent<TimerController>().CanAttack())
            {
                // Start Attacking
                isAttacking = true;
                // Animate Attack
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
                timers.transform.GetChild(0).gameObject.GetComponent<TimerController>().Reset();
            }
        }
    
        IEnumerator SpinAttack()
        {
            if (timers.transform.GetChild(1).gameObject.GetComponent<TimerController>().CanAttack())
            {
                // Start Attacking
                isAttacking = true;
                // Animate Spin Attack
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
                timers.transform.GetChild(1).gameObject.GetComponent<TimerController>().Reset();
            }
        }
    
        IEnumerator GroundPound()
        {
            if (timers.transform.GetChild(2).gameObject.GetComponent<TimerController>().CanAttack())
            {
                // Start Attacking
                isAttacking = true;
                // Animate Jump
                playerAnimator.SetTrigger("Jump");
                yield return new WaitForSeconds(0.5f);
                shadow.SetActive(true);
                // Animate Ground Pound
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
                timers.transform.GetChild(2).gameObject.GetComponent<TimerController>().Reset();
            }
        }
    
// ---------------------------------------------------------------------------------------------------------------------
// UI Checking

        void CheckUI()
        {
            // Interact
            if (Input.GetKeyDown(KeyCode.E) && !uiManager.IsInventory() && Time.timeScale != 0)
            {
                Collider2D[] results = new Collider2D[8];
                groundPoundCollider.OverlapCollider(contactFilter, results);
                foreach (Collider2D col in results)
                {
                    if (col != null && col.CompareTag("NPC"))
                    {
                        StartCoroutine(uiManager.Interact());
                        if (col.name.Equals("Wizard"))
                            StartCoroutine(col.GetComponent<Quest>().Talk());
                        else if (col.name.Equals("Merchant"))
                            col.GetComponent<MerchantController>().Talk();
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
                }
            }
        
            // Inventory
            if (Input.GetKeyDown(KeyCode.I) && !uiManager.IsInventory() && uiManager.options.activeSelf && Time.timeScale != 0)
                uiManager.Inventory();
            else if (Input.GetKeyDown(KeyCode.A) && uiManager.IsInventory() && uiManager.activeItem > 0)
            {
                uiManager.activeItem--;
                uiManager.ChangeItem(uiManager.activeItem);
            }
            else if (Input.GetKeyDown(KeyCode.D) && uiManager.IsInventory() && inventory.Count - 1 > uiManager.activeItem)
            {
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
            else if (Input.GetKeyDown(KeyCode.E) && uiManager.IsInventory())
            {
                if (UseItem(uiManager.activeItem))
                {
                    uiManager.UseItem();
                    uiManager.DisableItem(inventory.Count);
                }
            }
            else if (Input.GetKeyDown(KeyCode.I) && uiManager.IsInventory())
                uiManager.ExitInventory();

            // Quests
            if (Input.GetKeyDown(KeyCode.Q))
            {
            
            }
        
            // Menu
            else if (Input.GetKeyDown(KeyCode.Escape) && !uiManager.IsMenu() && Time.timeScale != 0)
            {
                uiManager.Menu();
            }
        }

// ---------------------------------------------------------------------------------------------------------------------
// Items

        public bool UseItem(int index)
        {
            switch (inventory[index])
            {
                // Key
                case "Key":
                    Collider2D[] results1 = new Collider2D[8];
                    groundPoundCollider.OverlapCollider(contactFilter, results1);
                    foreach (Collider2D col in results1)
                    {
                        if (col != null && col.CompareTag("Chest") && inventory.Contains("Key"))
                        {
                            if (col.GetComponent<ChestController>().Open())
                            {
                                inventory.RemoveAt(index);
                                return true;
                            }
                        }
                        else if (col != null && col.CompareTag("Door") && inventory.Contains("Key"))
                        {
                            if (!col.GetComponent<DoorController>().boss && col.GetComponent<DoorController>().Open())
                            {
                                inventory.RemoveAt(index);
                                return true;
                            }
                        }
                    }
                    return false;
                // Boss Key
                case "Boss Key":
                    Collider2D[] results2 = new Collider2D[8];
                    groundPoundCollider.OverlapCollider(contactFilter, results2);
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
                // Scroll
                case "Scroll":
                    inventory.RemoveAt(index);
                    return true;
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
            uiManager.Powerup("Liquid Luck: +1 Attack", new Color(1,0.9f,0));
            var color = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(1,0.9f,0);
            shine.SetActive(true);
            attack++;
            yield return new WaitForSeconds(10);
            uiManager.Powerup("", Color.white);
            GetComponent<SpriteRenderer>().color = color;
            shine.SetActive(false);
            attack--;
        }

        // Ogre's Strength - apply poison damage to damaged enemies for 5 seconds
        IEnumerator OgreStrength()
        {
            ScoreManager.AddAggressive(1);
            uiManager.Powerup("Ogre's Strength: +Poison Damage", new Color(0, 0.75f, 0));
            var color = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(0, 0.75f, 0);
            ogreStrength = true;
            yield return new WaitForSeconds(10);
            uiManager.Powerup("", Color.white);
            GetComponent<SpriteRenderer>().color = color;
            ogreStrength = false;
        }

        // Elixir of Speed - increase speed for 10 seconds
        IEnumerator ElixirOfSpeed()
        {
            ScoreManager.AddExploration(1);
            uiManager.Powerup("Elixir of Speed: +5 Speed", new Color(0,0.4f,1));
            GetComponent<SpriteRenderer>().color = new Color(0,0.4f,1);
            speed *= 1.5f;
            yield return new WaitForSeconds(15);
            uiManager.Powerup("", Color.white);
            GetComponent<SpriteRenderer>().color = Color.white;
            speed /= 1.5f;
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
                StartCoroutine("TakeKnockback", collision.transform.position);
                Invoke("StopBounceHorizontal", 0.5f);
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
                    ScoreManager.AddCollection(1);
                if ((inventory.Count == 6 && !playerDisciplines[1].Equals("Collection")) || (inventory.Count == 8 && playerDisciplines[1].Equals("Collection")))
                    ScoreManager.AddCollection(1);
                // Check item
                switch (other.name)
                {
                    // Coin
                    case "Coin":
                        Destroy(other.gameObject);
                        playerCoins++;
                        uiManager.SetCoins(playerCoins);
                        break;
                    // Key
                    case "Key":
                        if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines[1].Equals("Collection")))
                        {
                            Destroy(other.gameObject);
                            uiManager.AddItem("Key", inventory.Count);
                            AddItem("Key");
                        }
                        break;
                    // Boss Key
                    case "Boss Key":
                        if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines[1].Equals("Collection")))
                        {
                            Destroy(other.gameObject);
                            uiManager.AddItem("Boss Key", inventory.Count);
                            AddItem("Boss Key");
                        }
                        break;
                    // Scroll
                    case "Scroll":
                        if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines[1].Equals("Collection")))
                        {
                            Destroy(other.gameObject);
                            uiManager.AddItem("Scroll", inventory.Count);
                            AddItem("Scroll");
                        }
                        break;
                    // Potion
                    case "Wigg's Brew":
                        if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines[1].Equals("Collection")))
                        {
                            Destroy(other.gameObject);
                            uiManager.AddItem("Wigg's Brew", inventory.Count);
                            AddItem("Wigg's Brew");
                        }
                        break;
                    case "Liquid Luck":
                        if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines[1].Equals("Collection")))
                        {
                            Destroy(other.gameObject);
                            uiManager.AddItem("Liquid Luck", inventory.Count);
                            AddItem("Liquid Luck");
                        }
                        break;
                    case "Ogre's Strength":
                        if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines[1].Equals("Collection")))
                        {
                            Destroy(other.gameObject);
                            uiManager.AddItem("Ogre's Strength", inventory.Count);
                            AddItem("Ogre's Strength");
                        }
                        break;
                    case "Elixir of Speed":
                        if (inventory.Count < 6 || (inventory.Count < 8 && playerDisciplines[1].Equals("Collection")))
                        {
                            Destroy(other.gameObject);
                            uiManager.AddItem("Elixir of Speed", inventory.Count);
                            AddItem("Elixir of Speed");
                        }
                        break;
                }
            }
        
            if (other.gameObject.CompareTag("Enemy") && other is CircleCollider2D)
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
                Invoke("StopBounceVertical", 0.2f);
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
                Destroy(other.gameObject);
            }
        }
    
// ---------------------------------------------------------------------------------------------------------------------
// Player Data

        public void SaveData()
        {
            PlayerData.LivesActive = livesActive;
            PlayerData.Health = health;
            PlayerData.Name = playerName;
            PlayerData.Coins = playerCoins;
            PlayerData.Level = uiManager.levelNum;
            PlayerData.Disciplines = playerDisciplines;
            PlayerData.Titles = playerTitles;
            // todo
            // upgrade UI
            // boss fights
        }
    
        public void SaveData(List<string> d, List<string> t)
        {
            PlayerData.LivesActive = livesActive;
            PlayerData.Health = health;
            PlayerData.Name = playerName;
            PlayerData.Coins = playerCoins;
            PlayerData.Inventory = inventory;
            PlayerData.Level = uiManager.levelNum + 1;
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
                    livesActive = (PlayerData.Level < 5) ? 3 : (PlayerData.Level < 8 ? 6 : 9);
                    health = livesActive;
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
                uiManager.AddItem(inventory[i], i);
            }

            playerAnimator.runtimeAnimatorController = Instantiate((health < 6 ? playerG : (health < 9 ? playerB : playerR)));
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
}
