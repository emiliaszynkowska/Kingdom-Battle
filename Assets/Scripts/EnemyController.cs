using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    // Objects
    private Text healthText;
    private Rigidbody2D body;
    private Animator animator;
    private GameObject player;
    private Vector2 target;
    public Text text;
    public GameObject projectile;
    public ParticleSystem particles;
    private SpriteRenderer enemyRenderer;

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
        else if (magicAttack)
            StartCoroutine(MagicAttack());
    }

    void Update()
    {
        // Calculate enemy movement
        target = player.GetComponent<Rigidbody2D>().position;
    }

    private void FixedUpdate()
    {
        // Move Enemy
        if (IsMoving)
            body.MovePosition(Vector2.MoveTowards(body.position, target, speed * Time.fixedDeltaTime));
        if (health >= 0)
            healthText.text = health.ToString();
        else
            healthText.text = "0";
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
        while (true)
        {
            // Attack Every 3 Seconds
            GameObject p = Instantiate(projectile, transform.position, transform.rotation);
            p.transform.SetParent(transform);
            yield return new WaitForSeconds(3);
        }
    }
    
    IEnumerator GroundPound()
    {
        // Randomise Start Time
        yield return new WaitForSeconds(Random.Range(0, 3));
        // Attack Every 3 Seconds
        while (true)
        {
            // Wait
            IsMoving = false;
            text.enabled = false;
            yield return new WaitForSeconds(1);
            // Jump
            animator.SetTrigger("Jump");
            IsMoving = true;
            yield return new WaitForSeconds(1);
            // Animate
            IsMoving = false;
            text.enabled = true;
            Instantiate(particles, transform);
            yield return new WaitForSeconds(1);
            // Reset
            IsMoving = true;
            yield return new WaitForSeconds(3);
        }
    }

}
