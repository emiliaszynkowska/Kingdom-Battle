using System;
using System.Collections;
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
    private bool IsMoving = true;
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
    private SpriteRenderer enemyRenderer;
    // Prefabs
    public GameObject projectilePrefab;
    public GameObject knightPrefab;
    public GameObject goblinPrefab;

    public int GetAttack()
    {
        return attack;
    }
    
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
    }

    private void FixedUpdate()
    {
        // Move Enemy
        Flip();
        if (IsMoving)
            body.MovePosition(Vector2.MoveTowards(body.position, target, speed * Time.fixedDeltaTime));
        if (health >= 0)
            healthText.text = health.ToString();
        else
            healthText.text = "0";
    }

    void Flip()
    {
        if (target.x - transform.position.x > 0)
            enemyRenderer.flipX = false;
        else if (transform.position.x - target.x > 0)
            enemyRenderer.flipX = true;
    }

    public IEnumerator TakeDamage(int playerAttack)
    {
        if ((Time.time - SavedTime) > DelayTime)
        {
            SavedTime = Time.time;
            // Take Damage
            health -= playerAttack;
            enemyRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            enemyRenderer.color = Color.white;
            if (health <= 0)
            {
                Destroy(healthText);
                gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator PoisonDamage()
    {
        enemyRenderer.color = Color.green;
        yield return new WaitForSeconds(3);
        enemyRenderer.color = Color.white;
    }

    IEnumerator MagicAttack()
    {
        // Randomise Start Time
        yield return new WaitForSeconds(Random.Range(0, 3));
        while (true)
        {
            // Magic Attack
            if (Mathf.Abs(player.transform.position.magnitude - transform.position.magnitude) < 3)
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
            if (Mathf.Abs(player.transform.position.magnitude - transform.position.magnitude) < 5)
            {
                // Wait
                IsMoving = false;
                yield return new WaitForSeconds(1);
                // Jump
                animator.SetTrigger("Jump");
                IsMoving = true;
                yield return new WaitForSeconds(1);
                // Animate
                IsMoving = false;
                Instantiate(particles, transform);
                yield return new WaitForSeconds(1);
                // Reset
                IsMoving = true;
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
            if (Mathf.Abs(player.transform.position.magnitude - transform.position.magnitude) < 3)
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
