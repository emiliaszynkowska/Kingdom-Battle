using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KingController : MonoBehaviour
{
    // Variables
    public int health;
    public int attack;
    public int speed;
    private int stage = 1;
    private float SavedTime;
    private float DelayTime = 0.5f;
    private bool isHurt;
    private bool isMoving;
    private bool isPoisoned;
    private bool isAttacking;
    public int groundPoundTime;
    public int magicTime;
    public int spawnTime;
    public int attackTime;
    public int circleTime;
    
    // Objects
    public DungeonManager dungeonManager;
    public BattleManager battleManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    private Text healthText;
    private Rigidbody2D body;
    private Animator animator;
    private GameObject player;
    private Vector2 target;
    private Dungeon dungeon;
    private SpriteRenderer kingRenderer;
    private Animator weaponAnimator;
    private GameObject weapon;
    private GameObject shadow;
    private GameObject shield;
    public GameObject enemies;
    public GameObject NPCs;
    public ParticleSystem particles;
    public Collider2D groundPoundCollider;
    // Prefabs
    public GameObject projectilePrefab;
    public GameObject projectilesPrefab1;
    public GameObject projectilesPrefab2;
    
    void Start()
    {
        isMoving = true;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        weapon = transform.GetChild(1).gameObject;
        shadow = transform.GetChild(2).gameObject;
        shield = transform.GetChild(3).gameObject;
        weapon.SetActive(true);
        weaponAnimator = weapon.GetComponent<Animator>(); 
        kingRenderer = GetComponent<SpriteRenderer>();kingRenderer = GetComponent<SpriteRenderer>();
        healthText = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        healthText.text = health.ToString();
        StartCoroutine(MagicAttack());
        StartCoroutine(GroundPound());
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SwordAttack());
    }

    void Update()
    {
        // Calculate enemy movement
        target = player.transform.position;
        // Set enemy colours
        if (isPoisoned)
            kingRenderer.color = Color.green;
        else if (isHurt)
            kingRenderer.color = Color.red;
        else
            kingRenderer.color = Color.white;
    }

    private void FixedUpdate()
    {
        // Move Enemy
        Flip();
        if (isMoving)
            body.MovePosition(Vector2.MoveTowards(body.position, target, speed / 10 * Time.fixedDeltaTime));
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
        {
            kingRenderer.flipX = false;
            weapon.transform.localPosition = new Vector2(1, 0.8f);
            weapon.GetComponent<SpriteRenderer>().flipX = false;
        }
           
        else if (transform.position.x - target.x > 0)
        {
            kingRenderer.flipX = true;
            weapon.transform.localPosition = new Vector2(-1, 0.8f);
            weapon.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    IEnumerator Die()
    {
        if (stage == 1)
        {
            uiManager.PauseGame();
            soundManager.StopMusic();
            animator.SetTrigger("Death");
            transform.rotation = Quaternion.Euler(0, 0, -90);
            weapon.SetActive(false);
            isMoving = false;
            health = 0;
            // Destroy Enemies
            for (int i = 0; i < enemies.transform.childCount; i++)
            {
                Destroy(enemies.transform.GetChild(i).gameObject);
            }
            yield return uiManager.Speak("King Eldar", "Ugh... Curse you...");
            yield return uiManager.Speak("King Eldar", "How are you still alive... You're better than I thought.");
            yield return uiManager.Speak("King Eldar", "It seems I must bring out some bigger monsters.");
            yield return uiManager.Speak("King Eldar", "Guards, destroy them!");
            // Call Guards
            soundManager.PlaySound(soundManager.magicSpell);
            StartCoroutine(Appear(NPCs.transform.GetChild(1), new Vector3(-15, 8), new Vector3(-5, 8), false));
            StartCoroutine(Appear(NPCs.transform.GetChild(2), new Vector3(-15, 0), new Vector3(-5, 0), false));
            StartCoroutine(Appear(NPCs.transform.GetChild(3), new Vector3(15, 8), new Vector3(5, 8), true));
            yield return Appear(NPCs.transform.GetChild(4), new Vector3(15, 0), new Vector3(5, 0), true);
            for (int j = 1; j < 5; j++)
            {
                NPCs.transform.GetChild(j).GetComponent<EnemyController>().enabled = true;
                NPCs.transform.GetChild(j).gameObject.layer = 10;
            }
            uiManager.ResumeGame();
            // Battle
            yield return new WaitUntil(() => NPCs.transform.childCount == 1);
            uiManager.PauseGame();
            animator.SetTrigger("Idle");
            transform.rotation = Quaternion.Euler(0, 0, 0);
            yield return uiManager.Speak("King Eldar",
                "Enough! You think you can take down my best guards with such tenacity?");
            yield return uiManager.Speak("King Eldar",
                "Useless monsters! I should have eradicated you the instance you entered my castle.");
            yield return uiManager.Speak("King Eldar",
                "You're going to die like all those who challenged me before you.");
            stage = 2;
            health = 300;
            isMoving = true;
            weapon.SetActive(true);
            uiManager.ResumeGame();
            StartCoroutine(CircleAttack());
            soundManager.PlayMusic(soundManager.battleMusic);
        }
        else if (stage == 2)
        {
            uiManager.PauseGame();
            soundManager.StopMusic();
            weapon.SetActive(false);
            kingRenderer.enabled = false;
            // Destroy Enemies
            for (int i = 0; i < enemies.transform.childCount; i++)
            {
                Destroy(enemies.transform.GetChild(i).gameObject);
            }
            yield return uiManager.Upgrade("Crown");
            StartCoroutine(battleManager.Win());
        }
    }
    
    public IEnumerator TakeDamage(int playerAttack)
    {
        if (isMoving && !isPoisoned && !isHurt && !isAttacking && !shield.activeSelf && (Time.time - SavedTime) > DelayTime)
        {
            // Take Damage
            isHurt = true;
            SavedTime = Time.time;
            soundManager.PlaySound(soundManager.elfDamage);
            kingRenderer.color = new Color(0.8f, 0.22f, 0.2f);
            health -= playerAttack;
            if (health <= 0)
                StartCoroutine(Die());
            yield return new WaitForSeconds(1);
            kingRenderer.color = Color.white;
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
            if (isMoving)
            {
                // Magic Attack
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
            if (isMoving)
            {
                // Wait
                yield return new WaitForSeconds(1);
                // Jump
                soundManager.PlaySound(soundManager.jump);
                animator.SetTrigger("Jump");
                yield return new WaitForSeconds(0.5f);
                shadow.SetActive(true);
                // Animate
                soundManager.PlaySound(soundManager.groundPound);
                Instantiate(particles, transform);
                groundPoundCollider.enabled = true;
                yield return new WaitForSeconds(0.1f);
                groundPoundCollider.enabled = false;
                yield return new WaitForSeconds(1);
                // Reset
                shadow.SetActive(false);
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
            if (isMoving)
            {
                soundManager.PlaySound(soundManager.magicSpell);
                dungeonManager.PlaceEnemyHard(dungeon, false);
            }
            yield return new WaitForSeconds(spawnTime);
        }
    }
    
    IEnumerator SwordAttack()
    {
        while (true)
        {
            if (isMoving)
            {
                // Start Attacking
                isAttacking = true;
                // Animate Attack
                soundManager.PlaySound(soundManager.swordAttack);
                weapon.SetActive(true);
                weaponAnimator.SetTrigger("Attack");
                yield return new WaitForSeconds(1);
                // Stop Attacking
                isAttacking = false;
            }
            yield return new WaitForSeconds(attackTime);
        }
    }

    IEnumerator CircleAttack()
    {
        // Randomise Start Time
        yield return new WaitForSeconds(Random.Range(0, 3));
        while (true)
        {
            // Circle Attack
            isMoving = false;
            soundManager.PlaySound(soundManager.block);
            shield.SetActive(true);
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 5; i++)
            {
                soundManager.PlaySound(soundManager.magicSpell);
                for (int j = 0; j < 10; j++)
                {
                    Instantiate(projectilesPrefab1, transform.position, transform.rotation, transform);
                    yield return new WaitForSeconds(0.05f);
                }
                soundManager.PlaySound(soundManager.magicSpell);
                for (int k = 0; k < 10; k++)
                {
                    Instantiate(projectilesPrefab2, transform.position, transform.rotation, transform);
                    yield return new WaitForSeconds(0.05f);
                }
            }
            isMoving = true;
            shield.SetActive(false);
            yield return new WaitForSeconds(circleTime);
        }
    }
    
    IEnumerator Appear(Transform guard, Vector3 start, Vector3 end, bool dir)
    {
        guard.position = start;
        if (dir)
        {
            while (guard.position.x > end.x)
            {
                guard.GetComponent<SpriteRenderer>().flipX = true;
                guard.Translate(Vector3.left * (Time.unscaledDeltaTime * 3));
                yield return null;
            } 
        }
        else
        {
            while (guard.position.x < end.x)
            {
                guard.GetComponent<SpriteRenderer>().flipX = false;
                guard.Translate(Vector3.right * (Time.unscaledDeltaTime * 3));
                yield return null;
            } 
        }
    }
}
