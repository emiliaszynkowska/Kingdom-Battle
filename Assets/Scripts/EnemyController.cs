using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

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
    public bool groundPoundAttack;
    public bool magicAttack;
    public bool spawnEnemies;
    public int groundPoundTime;
    public int magicTime;
    public int spawnTime;
    // Objects
    private Text healthText;
    private Rigidbody2D body;
    private Animator animator;
    private GameObject player;
    private Vector2 target;
    public Text text;
    public ParticleSystem particles;
    public Collider2D groundPoundCollider;
    private SpriteRenderer enemyRenderer;
    public Dungeon dungeon;
    // Prefabs
    public GameObject projectilePrefab;
    public GameObject knightPrefab;
    public GameObject goblinPrefab;

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
        target = player.GetComponent<Rigidbody2D>().position;
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
        yield return new WaitForSeconds(0.5f);
        name = "Respawn";
        enemyRenderer.enabled = false;
        magicAttack = false;
        groundPoundAttack = false;
        spawnEnemies = false;
        healthText.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        ScoreManager.AddAggressive(1);
        DungeonGenerator dungeonGenerator = GameObject.Find("Map").GetComponent<DungeonGenerator>();
        DungeonManager dungeonManager = dungeonGenerator.dungeonManager;
        if (boss)
            dungeonManager.PlaceItem("Boss Key", transform.position);
        else
            dungeonManager.PlaceItem(drops[Random.Range(0, 12)], transform.position); 
        yield return new WaitForSeconds(10);
        dungeonManager.PlaceEnemy(dungeon, dungeonGenerator.difficulty);
        Destroy(gameObject);
    }

    public IEnumerator TakeDamage(int playerAttack)
    {
        if (!isPoisoned && (Time.time - SavedTime) > DelayTime)
        {
            SavedTime = Time.time;
            // Take Damage
            isHurt = true;
            health -= playerAttack;
            if (health <= 0)
                StartCoroutine(Die());
            yield return new WaitForSeconds(0.5f);
            isHurt = false;
        }
    }

    public IEnumerator PoisonDamage()
    {
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
                GameObject p = Instantiate(projectilePrefab, transform.position, transform.rotation);
                p.transform.SetParent(transform);
                p.tag = "Projectile";
            }
            else if (magicAttack && Mathf.Abs(player.transform.position.magnitude - transform.position.magnitude) < 3)
            {
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
                animator.SetTrigger("Jump");
                isMoving = true;
                yield return new WaitForSeconds(1);
                // Animate
                isMoving = false;
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
                if (name.Equals("Elite Knight") || name.Equals("Royal Guardian"))
                {
                    GameObject knight = Instantiate(knightPrefab, transform.position, transform.rotation);
                    knight.transform.SetParent(transform);
                }
                else if (name.Equals("Troll"))
                {
                    GameObject goblin = Instantiate(goblinPrefab, transform.position, transform.rotation);
                    goblin.transform.SetParent(transform);
                }
            }
            yield return new WaitForSeconds(spawnTime);
        }
    }

}
