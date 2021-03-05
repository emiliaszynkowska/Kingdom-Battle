using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Variables
    public int health;
    public int attack;
    public int speed;
    private float SavedTime;
    private float DelayTime = 0.5f;
    private bool isMoving = true;
    private bool isHurt;
    private bool isPoisoned;
    public bool boss;
    public bool den;
    public bool groundPoundAttack;
    public bool magicAttack;
    public bool spawnEnemies;
    public int difficulty;
    public int groundPoundTime;
    public int magicTime;
    public int spawnTime;
    // Objects
    public QuestManager questManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    private Text healthText;
    private Rigidbody2D body;
    private Animator animator;
    private GameObject player;
    private Vector2 target;
    public GameObject enemies;
    public Dungeon dungeon;
    public Text text;
    public ParticleSystem particles;
    public Collider2D groundPoundCollider;
    private SpriteRenderer enemyRenderer;
    // Prefabs
    public GameObject projectilePrefab;
    public GameObject knightPrefab;
    public GameObject goblinPrefab;
    
    private List<string> upgrades = new List<string>();
    private List<string> drops = new List<string>()
    {
        "Coin", "Coin", "Coin", "Coins", "Coins", "Coins", 
        "Key", "Key", "Wigg's Brew", "Liquid Luck", "Ogre's Strength", "Elixir of Speed"
    };

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        enemyRenderer = GetComponent<SpriteRenderer>();
        healthText = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        healthText.text = health.ToString();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        questManager = GameObject.Find("UI").GetComponent<QuestManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
        enemies = GameObject.Find("Enemies");
        if (groundPoundAttack)
            StartCoroutine(GroundPound());
        if (magicAttack)
            StartCoroutine(MagicAttack());
        if (spawnEnemies)
            StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        // Calculate enemy movement
        target = player.transform.position;
        // Set enemy colours
        if (isPoisoned)
            enemyRenderer.color = Color.green;
        else if (isHurt)
            enemyRenderer.color = Color.red;
        else
            enemyRenderer.color = Color.white;
    }

    private void FixedUpdate()
    {
        // Move Enemy
        Flip();
        if (boss)
        {
            isMoving = true;
            body.MovePosition(Vector2.MoveTowards(body.position, target, speed / 10 * Time.fixedDeltaTime));
        }
        else if (Mathf.Abs(player.transform.position.magnitude - transform.position.magnitude) < 5)
        {
            isMoving = true;
            body.MovePosition(Vector2.MoveTowards(body.position, target, speed / 10 * Time.fixedDeltaTime));
        }
        else
            isMoving = false;
        if (health >= 0)
            healthText.text = health.ToString();
        else
            healthText.text = "0";
    }

    public int GetAttack()
    {
        return attack;
    }

    public void SetBoss()
    {
        boss = true;
    }

    public void SetDifficulty(int d)
    {
        difficulty = d;
    }

    public void SetUpgrade(string u)
    {
        upgrades.Add(u);
    }

    public void SetDungeon(Dungeon d)
    {
        dungeon = d;
    }

    void Flip()
    {
        if (target.x - transform.position.x > 0)
            enemyRenderer.flipX = false;
        else if (transform.position.x - target.x > 0)
            enemyRenderer.flipX = true;
    }

    IEnumerator Die()
    {
        // Prepare for Respawn
        name = "Respawn";
        soundManager.PlaySound(soundManager.monsterDie);
        questManager.Event($"Defeat 1 {name}                        0/1", 0);
        questManager.Event(name, "Defeat");
        yield return new WaitForSeconds(0.5f);
        // Disable Enemy
        enemyRenderer.enabled = false;
        magicAttack = false;
        groundPoundAttack = false;
        spawnEnemies = false;
        healthText.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        // Add Aggressive Score
        ScoreManager.AddAggressive(1);
        DungeonGenerator dungeonGenerator = GameObject.Find("Map").GetComponent<DungeonGenerator>();
        DungeonManager dungeonManager = dungeonGenerator.dungeonManager;
        if (boss)
        {
            soundManager.PauseMusic();
            soundManager.PlayMusic(soundManager.victoryMusic);
            dungeonManager.PlaceItem("Boss Key", transform.position);
            // Create Upgrades
            if (upgrades.Count > 0)
            {
                foreach (string upgrade in upgrades)
                {
                    yield return uiManager.Upgrade(upgrade);
                }
            }
            // Destroy Enemies
            for (int i = 0; i < enemies.transform.childCount; i++)
            {
                Destroy(enemies.transform.GetChild(i).gameObject);
            }
            // Save Data
            player.GetComponent<PlayerController>().SaveData(false, true);
        }
        else
        {
            // Create Drops
            dungeonManager.PlaceItem(drops[Random.Range(0, 12)], transform.position);
            // Respawn Enemy
            if (dungeon != null && !den)
            {
                yield return new WaitForSeconds(10);
                dungeonManager.PlaceEnemy(dungeon, dungeonGenerator.difficulty);
            }
        }
        // Destroy Enemy
        Destroy(gameObject);
    }

    public IEnumerator TakeDamage(int playerAttack)
    {
        if (!isPoisoned && (Time.time - SavedTime) > DelayTime)
        {
            SavedTime = Time.time;
            // Take Damage
            soundManager.PlaySound(soundManager.monsterDamage);
            isHurt = true;
            health -= playerAttack;
            if (health <= 0 && !name.Equals("Respawn"))
                StartCoroutine(Die());
            yield return new WaitForSeconds(0.5f);
            isHurt = false;
        }
    }

    public IEnumerator PoisonDamage()
    {
        questManager.Event("Poison an enemy", 0);
        isPoisoned = true;
        yield return new WaitForSeconds(1);
        health -= 1;
        if (health <= 0)
            StartCoroutine(Die());
        yield return new WaitForSeconds(1);
        health -= 1;
        if (health <= 0)
            StartCoroutine(Die());
        yield return new WaitForSeconds(1);
        health -= 1;
        if (health <= 0)
            StartCoroutine(Die());
        isPoisoned = false;
    }

    IEnumerator MagicAttack()
    {
        // Randomise Start Time
        yield return new WaitForSeconds(Random.Range(0, 3));
        while (true)
        {
            // Magic Attack
            if (magicAttack && boss)
            {
                soundManager.PlaySound(soundManager.magicAttack);
                GameObject p = Instantiate(projectilePrefab, transform.position, transform.rotation);
                p.transform.SetParent(transform);
                p.tag = "Projectile";
            }
            else if (magicAttack && Mathf.Abs(player.transform.position.magnitude - transform.position.magnitude) < 2)
            {
                soundManager.PlaySound(soundManager.magicAttack);
                GameObject p = Instantiate(projectilePrefab, transform.position, transform.rotation);
                p.transform.SetParent(transform);
                p.tag = "Projectile";
            }
            yield return new WaitForSeconds(magicTime);
        }
    }
    
    IEnumerator GroundPound()
    {
        // Randomise Start Time
        yield return new WaitForSeconds(Random.Range(0, 3));
        // Ground Pound
        while (true)
        {
            if (groundPoundAttack && isMoving)
            {
                // Wait
                isMoving = false;
                yield return new WaitForSeconds(1);
                // Jump
                soundManager.PlaySound(soundManager.jump);
                animator.SetTrigger("Jump");
                isMoving = true;
                yield return new WaitForSeconds(1);
                // Animate
                isMoving = false;
                soundManager.PlaySound(soundManager.groundPound);
                Instantiate(particles, transform);
                groundPoundCollider.enabled = true;
                yield return new WaitForSeconds(0.1f);
                groundPoundCollider.enabled = false;
                yield return new WaitForSeconds(1);
                // Reset
                isMoving = true;
            }
            yield return new WaitForSeconds(groundPoundTime);
        }
    }
    
    IEnumerator SpawnEnemies()
    {
        // Randomise Start Time
        yield return new WaitForSeconds(Random.Range(0, 3));
        // Spawn Enemies 
        while (true)
        {
            if (spawnEnemies && isMoving)
            {
                soundManager.PlaySound(soundManager.monsterDamage);
                if (name.Equals("Elite Knight") || name.Equals("Royal Guardian"))
                {
                    GameObject knight = Instantiate(knightPrefab, transform.position, transform.rotation, enemies.transform);
                    knight.GetComponent<EnemyController>().health = (difficulty <= 10 ? 1 : difficulty <= 25 ? 2 : difficulty <= 50 ? 3 : difficulty <= 75 ? 4 : 5);
                    knight.GetComponent<EnemyController>().attack = (difficulty <= 25 ? 1 : difficulty <= 50 ? 2 : 3);
                }
                else if (name.Equals("Troll"))
                {
                    GameObject goblin = Instantiate(goblinPrefab, transform.position, transform.rotation, enemies.transform);
                    goblin.GetComponent<EnemyController>().health = (difficulty <= 10 ? 1 : difficulty <= 25 ? 2 : difficulty <= 50 ? 3 : difficulty <= 75 ? 4 : 5);
                    goblin.GetComponent<EnemyController>().attack = (difficulty <= 25 ? 1 : difficulty <= 50 ? 2 : 3);
                }
            }
            yield return new WaitForSeconds(spawnTime);
        }
    }

}