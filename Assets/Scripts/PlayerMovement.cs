using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D body;
    private Vector2 movement;
    private float speed = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        animator.SetFloat("Horizontal",movement.x);
        animator.SetFloat("Speed",movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + movement * speed * Time.fixedDeltaTime);
    }
}
