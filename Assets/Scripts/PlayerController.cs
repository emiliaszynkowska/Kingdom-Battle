using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : MonoBehaviour
{
    // Movement
    private Vector2 movement;
    private float speed = 0.1f;
    // Combat
    private int attack = 1;
    private int health = 5;
    private int livesActive = 3;
    private float lastHurtTime = -10;
    private float hurtTimeOut = 1;
    // Booleans
    private bool isAttacking;
    private bool isHurt;
    private bool isBouncing;
    private bool ogreStrength;
    // Player Information
    private string playerName = "Player";
    private string[] playerDisciplines = new string[3];
    private int playerCoins;
    // Variables
    private List<string> inventory = new List<string>();
    // Objects
    private Rigidbody2D body;
    private SpriteRenderer playerRenderer;
    private Animator playerAnimator;
    private Animator weaponAnimator;
    private GameObject weapon;
    private UIManager uiManager;
    private GameObject lifeup;
    private GameObject shadow;
    public ParticleSystem particles;
    public GameObject timers;
    private Collider2D attackCollider;
    private Collider2D spinAttackCollider;
    private Collider2D groundPoundCollider;
    public ContactFilter2D contactFilter;

    void Start()
    {
        // Get object references
        body = GetComponent<Rigidbody2D>();
        weapon = transform.GetChild(0).gameObject;
        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        weaponAnimator = weapon.GetComponent<Animator>();
        shadow = transform.GetChild(4).gameObject;
        lifeup = transform.GetChild(5).gameObject;
        attackCollider = weapon.GetComponents<CircleCollider2D>()[0];
        spinAttackCollider = weapon.GetComponents<CircleCollider2D>()[1];
        groundPoundCollider = weapon.GetComponents<CircleCollider2D>()[2];
        // Initialise UI components
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        livesActive = health;
        uiManager.SetLivesActive(health);
        uiManager.SetLives(health);
        playerDisciplines = new [] {"Fledgling", null, null};
        uiManager.SetInfo(playerName, playerDisciplines, playerCoins);
        uiManager.ClearInventory();
    }

    void Update()
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
    
    IEnumerator TakeDamage()
    {
        if (!IsAttacking() && !IsBouncing() && Time.time < lastHurtTime + hurtTimeOut)
        {
            Flip();
            lastHurtTime = Time.time;
            isHurt = true;
            playerRenderer.color = new Color(0.8f, 0.3f, 0.4f, 1);
            yield return new WaitForSeconds(1);
            playerRenderer.color = Color.white;
            isHurt = false;
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
            Flip();
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
            Flip();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.otherCollider is BoxCollider2D && !isHurt)
        {
            // Bounce off the enemy
            StartCoroutine("TakeKnockback", collision.transform.position);
            Invoke("StopBounceHorizontal", 0.5f);
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            if (enemyController != null && enemyController.health > 0)
            {
                // Take damage
                if (health > 0)
                {
                    int enemyAttack = collision.gameObject.GetComponent<EnemyController>().GetAttack();
                    health -= enemyAttack;
                    uiManager.SetLives(health);
                    StartCoroutine("TakeDamage");
                }
                else
                {
                    // Game over
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            other.gameObject.SetActive(false);
            // Check item
            switch (other.name)
            {
                // Coin
                case "Coin":
                    playerCoins++;
                    uiManager.SetInfo(playerName, playerDisciplines, playerCoins);
                    break;
                // Key
                case "Key":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Key", inventory.Count);
                        inventory.Add("Key");
                    }

                    break;
                // Scroll
                case "Scroll":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Scroll", inventory.Count);
                        inventory.Add("Scroll");
                    }

                    break;
                // Potion
                case "Wigg's Brew":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Wigg's Brew", inventory.Count);
                        inventory.Add("Wigg's Brew");
                    }

                    break;
                case "Liquid Luck":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Liquid Luck", inventory.Count);
                        inventory.Add("Liquid Luck");
                    }

                    break;
                case "Ogre's Strength":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Ogre's Strength", inventory.Count);
                        inventory.Add("Ogre's Strength");
                    }

                    break;
                case "Elixir of Speed":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Elixir of Speed", inventory.Count);
                        inventory.Add("Elixir of Speed");
                    }

                    break;
            }
        }

        if (other.gameObject.name.Equals("Spike") && GetComponent<BoxCollider2D>().IsTouching(other) && !isHurt)
        {
            // Bounce off the spikes
            body.AddForce(Vector2.up * 100);
            isBouncing = true;
            Invoke("StopBounceVertical", 0.2f);
            // Take damage
            if (health > 0)
            {
                health--;
                uiManager.SetLives(health);
                StartCoroutine("TakeDamage");
            }
            else
            {
                // Game over
            }
        }

        if (other.gameObject.CompareTag("Projectile") && GetComponent<BoxCollider2D>().IsTouching(other) && !isHurt)
        {
            Destroy(other);
            // Take damage
            if (health > 0)
            {
                health--;
                uiManager.SetLives(health);
                StartCoroutine(TakeDamage());
            }
            else
            {
                // Game Over
            }
        }
    }

    void CheckUI()
    {
        // Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] results = new Collider2D[8];
            groundPoundCollider.OverlapCollider(contactFilter, results);
            foreach (Collider2D col in results)
            {
                if (col != null && col.CompareTag("NPC"))
                {
                    StartCoroutine(uiManager.Interact());
                    StartCoroutine(col.GetComponent<QuestFetch>().Talk());
                }
            }
        }
        
        // Inventory
        if (Input.GetKeyDown(KeyCode.I) && !uiManager.IsInventory())
            uiManager.Inventory();
        else if (Input.GetKeyDown(KeyCode.A) && uiManager.IsInventory() && uiManager.activeItem > 0)
        {
            uiManager.activeItem--;
            uiManager.ChangeItem(uiManager.activeItem);
        }
        else if (Input.GetKeyDown(KeyCode.D) && uiManager.IsInventory() && inventory.Count - 1 > uiManager.activeItem)
        {
            uiManager.activeItem++;
            uiManager.ChangeItem(uiManager.activeItem);
        }
        else if (Input.GetKeyDown(KeyCode.E) && uiManager.IsInventory())
        {
            UseItem(inventory[uiManager.activeItem]);
            inventory.RemoveAt(uiManager.activeItem);
            uiManager.UseItem();
            uiManager.DisableItem(inventory.Count);
        }
        else if (Input.GetKeyDown(KeyCode.I) && uiManager.IsInventory())
            uiManager.ExitInventory();
        
        // Quests
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
        }
        
        // Menu
        else if (Input.GetKeyDown(KeyCode.Escape) && !uiManager.IsMenu())
        {
            uiManager.Menu();
        }
    }

    void UseItem(string item)
    {
        switch (item)
        {
            // Key
                case "Key":
                    break;
                // Scroll
                case "Scroll":
                    break;
                // Potion
                case "Wigg's Brew":
                    StartCoroutine("WiggsBrew");
                    if (health < livesActive)
                    {
                        health++;
                        uiManager.SetLives(health);
                    }
                    uiManager.ExitInventory();
                    break;
                case "Liquid Luck":
                    StartCoroutine("LiquidLuck");
                    uiManager.ExitInventory();
                    break;
                case "Ogre's Strength":
                    StartCoroutine("OgreStrength");
                    uiManager.ExitInventory();
                    break;
                case "Elixir of Speed":
                    StartCoroutine("ElixirOfSpeed");
                    uiManager.ExitInventory();
                    break;
        }
    }
    
    // Wigg's Brew - increase health by 1 and glow red 
    IEnumerator WiggsBrew()
    {
        uiManager.Powerup("Wigg's Brew: +1 Health", Color.red);
        lifeup.SetActive(true);
        yield return new WaitForSeconds(2);
        uiManager.Powerup("", Color.white);
        lifeup.SetActive(false);
    }

    // Liquid Luck - increase attack by 1 and glow yellow for 5 seconds
    IEnumerator LiquidLuck()
    {
        uiManager.Powerup("Liquid Luck: +1 Attack", new Color(1,0.9f,0,1));
        var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(1,0.9f,0,1);
        attack++;
        yield return new WaitForSeconds(5);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = color;
        attack--;
    }

    // Ogre's Strength - apply poison damage to damaged enemies for 5 seconds
    IEnumerator OgreStrength()
    {
        uiManager.Powerup("Ogre's Strength: +Poison Damage", new Color(0, 0.75f, 0, 1));
        var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(0, 0.75f, 0, 1);
        ogreStrength = true;
        yield return new WaitForSeconds(5);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = color;
        ogreStrength = false;
    }

    // Elixir of Speed - increase speed for 10 seconds
    IEnumerator ElixirOfSpeed()
    {
        uiManager.Powerup("Elixir of Speed: +5 Speed", new Color(0,0.4f,1,1));
        GetComponent<SpriteRenderer>().color = new Color(0,0.4f,1,1);
        speed *= 1.5f;
        yield return new WaitForSeconds(10);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = Color.white;
        speed /= 1.5f;
    }

    public bool IsOgreStrength()
    {
        return ogreStrength;
    }

    public List<string> GetInventory()
    {
        return inventory;
    }
}
