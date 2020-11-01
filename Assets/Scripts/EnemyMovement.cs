using UnityEngine;
using UnityEngine.UI;

public class EnemyMovement : MonoBehaviour
{
    // Variables
    public int health;
    public int attack;
    public int speed;
    // Objects
    private Rigidbody2D body;
    private Text healthText;
    private GameObject player;
    private Vector2 target;

    public int GetAttack()
    {
        return attack;
    }
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        health = 3;
        attack = 1;
        speed = 1;
        healthText = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        healthText.rectTransform.position = new Vector2(transform.position.x, transform.position.y + 1);
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
        healthText.rectTransform.position = new Vector2(transform.position.x, transform.position.y + 1);
        healthText.text = health.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Weapon") && other.gameObject.GetComponentInParent<PlayerMovement>().IsAttacking())
        {
            int playerAttack = other.gameObject.GetComponentInParent<PlayerMovement>().GetAttack();
            health -= playerAttack;
            if (health <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
