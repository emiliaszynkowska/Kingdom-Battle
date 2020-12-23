﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : MonoBehaviour
{
    // Movement
    private Vector2 movement;
    private float speed = 0.1f;
    // Combat
    private int attack = 1;
    private int health = 3;
    private int livesActive = 3;
    // Booleans
    private bool isAttacking = false;
    private bool isHurt = false;
    private bool isBouncing = false;
    private bool ogreStrength = false;
    // Player Information
    private string playerName = "Player";
    private string[] playerDisciplines = new string[3];
    private int playerCoins = 0;
    // Variables
    private List<string> inventory = new List<string>();
    // Objects
    private Rigidbody2D body;
    private SpriteRenderer playerRenderer;
    private Animator playerAnimator;
    private Animator weaponAnimator;
    private GameObject weapon;
    private UIManager uiManager;
    private GameObject lifeup;
    private GameObject shadow;
    public ParticleSystem particles;

    void Start()
    {
        // Get object references
        body = GetComponent<Rigidbody2D>();
        weapon = transform.GetChild(0).gameObject;
        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        weaponAnimator = weapon.GetComponent<Animator>();
        lifeup = transform.GetChild(3).gameObject;
        shadow = transform.GetChild(1).gameObject;
        // Initialise UI components
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        livesActive = health;
        uiManager.SetLivesActive(health);
        uiManager.SetLives(health);
        playerDisciplines = new string [] {"Fledgling", null, null};
        uiManager.SetInfo(playerName, playerDisciplines, playerCoins);
        uiManager.ClearInventory();
        uiManager.SetInteract(false);
        
        inventory.Add("Wigg's Brew");
        inventory.Add("Liquid Luck");
        inventory.Add("Ogre's Strength");
        inventory.Add("Elixir of Speed");
        uiManager.AddItem("Wigg's Brew",0);
        uiManager.AddItem("Liquid Luck",1);
        uiManager.AddItem("Ogre's Strength",2);
        uiManager.AddItem("Elixir of Speed",3);
    }

    void Update()
    {
        // Calculate player movement
        movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        Flip();
        
        // Set animations
        playerAnimator.SetFloat("Horizontal",movement.x);
        playerAnimator.SetFloat("Speed",movement.sqrMagnitude);
        
        // Move the weapon
        if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
        {
            weapon.transform.localPosition = new Vector2(0.9f, -0.2f);
            weapon.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
        {
            weapon.transform.localPosition = new Vector2(-0.9f,-0.2f);
            weapon.GetComponent<SpriteRenderer>().flipX = true;
        }
        
        // Play attack animations
        // Attack
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Attack();
        // Spin Attack
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SpinAttack();
        // Ground Pound
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine("GroundPound");

        // Check UI Inputs
        CheckUI();
    }

    private void FixedUpdate()
    {
        if (!isBouncing)
            transform.Translate(movement.x * speed, movement.y * speed, 0, Space.Self);
    }

    void Flip()
    {
        if (Input.GetAxis("Horizontal") > 0)
            playerRenderer.flipX = false;
        else if (Input.GetAxis("Horizontal") < 0)
            playerRenderer.flipX = true;
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
        Flip();
        isHurt = true;
        playerAnimator.SetTrigger("Damage");
        yield return new WaitForSeconds(1);
        isHurt = false;
    }

    void Attack()
    {
        Flip();
        isAttacking = true;
        weaponAnimator.SetTrigger("Attack");
        isAttacking = false;
    }
    
    void SpinAttack()
    {
        Flip();
        isAttacking = true;
        weaponAnimator.SetTrigger("Spin");
        isAttacking = false;
    }
    
    IEnumerator GroundPound()
    {
        isAttacking = true;
        playerAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(0.5f);
        shadow.SetActive(true);
        Instantiate(particles, transform);
        yield return new WaitForSeconds(1);
        shadow.SetActive(false);
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
            // Bounce off the enemy
            body.AddForce(collision.contacts[0].normal * 75);
            isBouncing = true;
            Invoke("StopBounceHorizontal", 0.5f);
            // Take damage
            if (health > 0)
            {
                int enemyAttack = collision.gameObject.GetComponent<EnemyController>().GetAttack();
                health -= enemyAttack;
                uiManager.SetLives(health);
                StartCoroutine("TakeDamage");
            }
            else
            {
                // Game over
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && GetComponent<BoxCollider2D>().IsTouching(other))
        {
            other.gameObject.SetActive(false);

            // Check item
            switch (other.gameObject.name)
            {
                // Coin
                case "Coin":
                    playerCoins++;
                    uiManager.SetInfo(playerName, playerDisciplines, playerCoins);
                    break;
                // Key
                case "Key":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Key", inventory.Count);
                        inventory.Add("Key");
                    }

                    break;
                // Scroll
                case "Scroll":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Scroll", inventory.Count);
                        inventory.Add("Scroll");
                    }

                    break;
                // Potion
                case "Wigg's Brew":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Wigg's Brew", inventory.Count);
                        inventory.Add("Wigg's Brew");
                    }

                    break;
                case "Liquid Luck":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Liquid Luck", inventory.Count);
                        inventory.Add("Liquid Luck");
                    }

                    break;
                case "Ogre's Strength":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Ogre's Strength", inventory.Count);
                        inventory.Add("Ogre's Strength");
                    }

                    break;
                case "Elixir of Speed":
                    if (inventory.Count < 8)
                    {
                        other.gameObject.SetActive(false);
                        uiManager.AddItem("Elixir of Speed", inventory.Count);
                        inventory.Add("Elixir of Speed");
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
            // Take damage
            if (health > 0)
            {
                health--;
                uiManager.SetLives(health);
                StartCoroutine("TakeDamage");
            }
            else
            {
                // Game over
            }
        }
    }

    void CheckUI()
    {
        // Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            
        }
        
        // Inventory
        if (Input.GetKeyDown(KeyCode.I) && !uiManager.IsInventory())
            uiManager.Inventory();
        else if (Input.GetKeyDown(KeyCode.A) && uiManager.IsInventory() && uiManager.activeItem > 0)
        {
            uiManager.activeItem--;
            uiManager.ChangeItem(uiManager.activeItem);
        }
        else if (Input.GetKeyDown(KeyCode.D) && uiManager.IsInventory() && inventory.Count - 1 > uiManager.activeItem)
        {
            uiManager.activeItem++;
            uiManager.ChangeItem(uiManager.activeItem);
        }
        else if (Input.GetKeyDown(KeyCode.E) && uiManager.IsInventory())
        {
            UseItem(inventory[uiManager.activeItem]);
            inventory.RemoveAt(uiManager.activeItem);
            uiManager.UseItem();
            uiManager.DisableItem(inventory.Count);
        }
        else if (Input.GetKeyDown(KeyCode.I) && uiManager.IsInventory())
            uiManager.ExitInventory();
        
        // Quests
        if (Input.GetKeyDown(KeyCode.Q))
        {
            
        }
        
        // Menu
        else if (Input.GetKeyDown(KeyCode.Escape) && !uiManager.IsMenu())
        {
            uiManager.Menu();
        }
    }

    void UseItem(string item)
    {
        switch (item)
        {
            // Key
                case "Key":
                    break;
                // Scroll
                case "Scroll":
                    break;
                // Potion
                case "Wigg's Brew":
                    StartCoroutine("WiggsBrew");
                    if (health < livesActive)
                    {
                        health++;
                        uiManager.SetLives(health);
                    }
                    uiManager.ExitInventory();
                    break;
                case "Liquid Luck":
                    StartCoroutine("LiquidLuck");
                    uiManager.ExitInventory();
                    break;
                case "Ogre's Strength":
                    StartCoroutine("OgreStrength");
                    uiManager.ExitInventory();
                    break;
                case "Elixir of Speed":
                    StartCoroutine("ElixirOfSpeed");
                    uiManager.ExitInventory();
                    break;
        }
    }
    
    // Wigg's Brew - increase health by 1 and glow red 
    IEnumerator WiggsBrew()
    {
        uiManager.Powerup("Wigg's Brew: +1 Health", Color.red);
        lifeup.SetActive(true);
        yield return new WaitForSeconds(2);
        uiManager.Powerup("", Color.white);
        lifeup.SetActive(false);
    }

    // Liquid Luck - increase attack by 1 and glow yellow for 5 seconds
    IEnumerator LiquidLuck()
    {
        uiManager.Powerup("Liquid Luck: +1 Attack", new Color(1,0.9f,0,1));
        var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(1,0.9f,0,1);
        attack++;
        yield return new WaitForSeconds(5);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = color;
        attack--;
    }

    // Ogre's Strength - apply poison damage to damaged enemies for 5 seconds
    IEnumerator OgreStrength()
    {
        uiManager.Powerup("Ogre's Strength: +Poison Damage", new Color(0, 0.75f, 0, 1));
        var color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(0, 0.75f, 0, 1);
        ogreStrength = true;
        yield return new WaitForSeconds(5);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = color;
        ogreStrength = false;
    }

    // Elixir of Speed - increase speed for 10 seconds
    IEnumerator ElixirOfSpeed()
    {
        uiManager.Powerup("Elixir of Speed: +5 Speed", new Color(0,0.4f,1,1));
        GetComponent<SpriteRenderer>().color = new Color(0,0.4f,1,1);
        speed *= 1.5f;
        yield return new WaitForSeconds(10);
        uiManager.Powerup("", Color.white);
        GetComponent<SpriteRenderer>().color = Color.white;
        speed /= 1.5f;
    }

    public bool IsOgreStrength()
    {
        return ogreStrength;
    }
}