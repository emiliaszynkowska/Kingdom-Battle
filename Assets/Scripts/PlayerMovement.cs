using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    // Movement
    private Vector2 movement;
    private float speed = 10;
    // Combat
    private int health = 3;
    private int attack = 1;
    // Booleans
    private bool isAttacking = false;
    private bool isHurt = false;
    private bool isBouncing = false;
    // Player Information
    private string playerName = "Player";
    private int playerLevel = 1;
    private int playerMoney = 0;
    // Lists
    private List<string> inventory = new List<string>();
    // Objects
    private Rigidbody2D body;
    private Animator playerAnimator;
    private Animator weaponAnimator;
    private GameObject weapon;
    private UIUpdater uiUpdater;

    void Start()
    {
        // Get object references
        body = GetComponent<Rigidbody2D>();
        weapon = GameObject.Find("Weapon");
        playerAnimator = GetComponent<Animator>();
        weaponAnimator = weapon.GetComponent<Animator>();
        // Initialise UI components
        uiUpdater = GameObject.Find("UI").GetComponent<UIUpdater>();
        uiUpdater.SetLivesActive(3);
        uiUpdater.SetLives(3);
        uiUpdater.SetInfo(playerName,playerLevel,playerMoney);
        uiUpdater.ClearInventory();
        uiUpdater.SetInteract(false);
    }

    void Update()
    {
        // Calculate player movement
        movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        // Set animations
        playerAnimator.SetFloat("Horizontal",movement.x);
        playerAnimator.SetFloat("Speed",movement.sqrMagnitude);
        // Move the weapon
        if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
        {
            weapon.transform.localPosition = new Vector2(0.9f, -0.2f);
            weaponAnimator.SetTrigger("IdleRight");
        }
        else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
        {
            weapon.transform.localPosition = new Vector2(-0.9f,-0.2f);
            weaponAnimator.SetTrigger("IdleLeft");
        }
        // Play attack animations
        if (Input.GetButtonDown("Fire1") && (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdleRight") ||
                                             playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRunRight") ||
                                             playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("DamageRight")))
            StartCoroutine(AttackRight());
        else if (Input.GetButtonDown("Fire1") &&
                 (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdleLeft") ||
                  playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRunLeft") ||
                  playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("DamageLeft")))
            StartCoroutine(AttackLeft());
        }

    private void FixedUpdate()
    {
        if (!isBouncing)
            transform.Translate(movement.x / speed, movement.y / speed, 0, Space.Self);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.otherCollider is BoxCollider2D && !isHurt)
        {
            // Bounce off the enemy
            body.AddForce(collision.contacts[0].normal * 75);
            isBouncing = true;
            Invoke("StopBounceHorizontal", 0.5f);
            // Take Damage
            if (health > 0)
            {
                int enemyAttack = collision.gameObject.GetComponent<EnemyMovement>().GetAttack();
                health -= enemyAttack;
                uiUpdater.SetLives(health);
                StartCoroutine("TakeDamage");
            }
            else
            {
                // Game Over
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            other.gameObject.SetActive(false);

            // Check what the item is
            switch (other.gameObject.name)
            {
                // Coin
                case "Coin":
                    playerMoney++;
                    uiUpdater.SetInfo(playerName, playerLevel, playerMoney);
                    break;
                // Key
                case "Key":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiUpdater.AddItem("key", inventory.Count);
                        inventory.Add("key");
                    }

                    break;
                // Scroll
                case "Scroll":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiUpdater.AddItem("scroll", inventory.Count);
                        inventory.Add("scroll");
                    }

                    break;
                // Potion
                case "Potion Red":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiUpdater.AddItem("potionRed", inventory.Count);
                        inventory.Add("potionRed");
                    }

                    break;
                case "Potion Yellow":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiUpdater.AddItem("potionYellow", inventory.Count);
                        inventory.Add("potionYellow");
                    }

                    break;
                case "Potion Green":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiUpdater.AddItem("potionGreen", inventory.Count);
                        inventory.Add("potionGreen");
                    }

                    break;
                case "Potion Blue":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiUpdater.AddItem("potionBlue", inventory.Count);
                        inventory.Add("potionBlue");
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
            // Take Damage
            if (health > 0)
            {
                health--;
                uiUpdater.SetLives(health);
                StartCoroutine("TakeDamage");
            }
            else
            {
                // Game Over
            }
        }
    }

    public int GetAttack()
    {
        return attack;
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
    
    IEnumerator TakeDamage()
    {
        isHurt = true;
        playerAnimator.SetTrigger("Damage");
        yield return new WaitForSeconds(1);
        isHurt = false;
    }

    IEnumerator AttackRight()
    {
        isAttacking = true;
        weaponAnimator.SetTrigger("AttackRight");
        yield return new WaitForSeconds(1.2f);
        isAttacking = false;
    }

    IEnumerator AttackLeft()
    {
        isAttacking = true;
        weaponAnimator.SetTrigger("AttackLeft");
        yield return new WaitForSeconds(1.2f);
        isAttacking = false;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}
