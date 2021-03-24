using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMinimal : MonoBehaviour
{
    // Movement
    private Vector2 movement;
    public float speed = 0.1f;
    public bool active = true;
    // Objects
    public Rigidbody2D body;
    public SpriteRenderer playerRenderer;
    public Animator playerAnimator;
    public GameObject prompt;

    public void Start()
    {
        StartCoroutine(WaitForInput());
    }

    public IEnumerator WaitForInput()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D));
        prompt.SetActive(false);
    }

    public void Update()
    {
        // Calculate player movement
        movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        Flip();
        
        // Set animations
        playerAnimator.SetFloat("Horizontal",movement.x);
        playerAnimator.SetFloat("Speed",movement.sqrMagnitude);
    }
    
    private void FixedUpdate()
    {
        if (active)
            transform.Translate(movement.x * speed, movement.y * speed, 0, Space.Self);
    }

    void Flip()
    {
        // Player
        if (Input.GetAxis("Horizontal") > 0)
            playerRenderer.flipX = false;
        else if (Input.GetAxis("Horizontal") < 0)
            playerRenderer.flipX = true;
    }
}