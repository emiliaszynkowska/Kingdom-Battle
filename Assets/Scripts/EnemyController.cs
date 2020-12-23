using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Variables
    public int health;
    public int attack;
    public int speed;
    float SavedTime;
    float DelayTime = 0.5f;
    // Objects
    private Text healthText;
    private Rigidbody2D body;
    private GameObject player;
    private Vector2 target;
    private SpriteRenderer enemyRenderer;

    public int GetAttack()
    {
        return attack;
    }
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        enemyRenderer = GetComponent<SpriteRenderer>();
        healthText = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        healthText.text = health.ToString();
    }

    void Update()
    {
        // Calculate enemy movement
        target = player.GetComponent<Rigidbody2D>().position;
    }

    private void FixedUpdate()
    {
        // Move the enemy
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

}
