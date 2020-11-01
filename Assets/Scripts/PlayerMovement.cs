using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    // Variables
    private Vector2 movement;
    private float speed = 10;
    //private int weaponType = 1;
    private int health = 1;
    private int attack = 1;
    private bool isAttacking = false;
    private bool isHurt = false;
    private string playerName = null;
    private int level = 0;
    private int coins = 0;
    private int inventorySlots = 3;
    private int inventory = 0;
    // Objects
    private Rigidbody2D body;
    private Animator playerAnimator;
    private Animator weaponAnimator;
    private GameObject weapon;
    private GameObject info;
    private GameObject lives;
    private GameObject inv;
    // Assets
    public Sprite fullLife;
    public Sprite emptyLife;

    public int GetAttack()
    {
        return attack;
    }
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        weapon = GameObject.FindWithTag("Weapon");
        playerAnimator = GetComponent<Animator>();
        weaponAnimator = weapon.GetComponent<Animator>();
        info = GameObject.FindGameObjectsWithTag("UI")[1];
        lives = GameObject.FindGameObjectsWithTag("UI")[2];
        inv = GameObject.FindGameObjectsWithTag("UI")[3];
        for (int i = 0; i < health; i++)
        {
            Transform child = lives.transform.GetChild(i);
            child.gameObject.SetActive(true);
            child.GetComponent<SpriteRenderer>().sprite = fullLife;
        }

        info.GetComponent<Text>().text = playerName + "\nLevel " + level + "\n" + coins;
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
        {
            StartCoroutine(AttackLeft());
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(movement.x / speed, movement.y / speed, 0, Space.Self);
        //body.MovePosition(body.position + movement * (speed * Time.fixedDeltaTime)); old movement method
    }

    private void UpdateText(string t)
    {
        info.GetComponent<Text>().text = t;
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
        weapon.transform.localPosition = Vector3.Slerp(weapon.transform.localPosition, Vector2.down + Vector2.left, 0.1f);
        yield return new WaitForSeconds(1);
        weapon.transform.localPosition = new Vector2(0.9f, -0.2f);
        isAttacking = false;
    }

    IEnumerator AttackLeft()
    {
        isAttacking = true;
        weaponAnimator.SetTrigger("AttackLeft");
        weapon.transform.localPosition = Vector3.Slerp(weapon.transform.localPosition, Vector2.down + Vector2.right, 0.1f);
        yield return new WaitForSeconds(1);
        weapon.transform.localPosition = new Vector2(-0.9f, -0.2f);
        isAttacking = false;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.otherCollider is BoxCollider2D && !isHurt)
        {
            StartCoroutine(TakeDamage());
            if (health > 0)
            {
                int enemyAttack = collision.gameObject.GetComponent<EnemyMovement>().GetAttack();
                for (int i = 0; i < enemyAttack; i++)
                {
                    health--;
                    lives.transform.GetChild(health).gameObject.GetComponent<SpriteRenderer>().sprite = emptyLife;
                }
            }
            else
            {
                // Game Over
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && other.gameObject.name.Contains("Coin") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            other.gameObject.SetActive(false);
            coins++;
            UpdateText(playerName + "\nLevel " + level + "\n" + coins);
        }
        else if (other.gameObject.CompareTag("Item") && other.gameObject.name.Contains("Key") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            if (inventory < inventorySlots)
            {
                other.gameObject.SetActive(false);
                inv.transform.GetChild(inventory).gameObject.SetActive(true);
                inventory++;
            }
        }
            else if (other.gameObject.CompareTag("Spikes") && GetComponent<BoxCollider2D>().IsTouching(other) && !isHurt)
        {
            StartCoroutine(TakeDamage());
            transform.Translate(0,0.4f,0,Space.Self);
            if (health > 0)
            {
                health--;
                lives.transform.GetChild(health).gameObject.GetComponent<SpriteRenderer>().sprite = emptyLife;
            }
            else
            {
                // Game Over
            }
        }
    }
}
