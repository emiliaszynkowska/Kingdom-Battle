using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Variables
    public int health;
    public int attack;
    public int speed;
    float SavedTime = 0;
    float DelayTime = 0.5f;
    // Objects
    private Text healthText;
    private Rigidbody2D body;
    private GameObject player;
    private Vector2 target;

    public int GetAttack()
    {
        return attack;
    }
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
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
        healthText.text = health.ToString();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Take Damage
        if ((Time.time - SavedTime) > DelayTime)
        {
            SavedTime = Time.time;

            if (other.gameObject.CompareTag("Weapon") &&
                other.gameObject.GetComponentInParent<PlayerController>().IsAttacking())
            {
                int playerAttack = other.gameObject.GetComponentInParent<PlayerController>().GetAttack();
                health -= playerAttack;
                if (health <= 0)
                {
                    Destroy(healthText);
                    gameObject.SetActive(false);
                }
                // Take Poison Damage
                if (other.gameObject.GetComponentInParent<PlayerController>().IsOgreStrength())
                    StartCoroutine("PoisonDamage");
            }
        }
    }

    IEnumerator PoisonDamage()
    {
        var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = Color.green;
        
        yield return new WaitForSeconds(3);
        GetComponent<SpriteRenderer>().color = color;
    }

}
