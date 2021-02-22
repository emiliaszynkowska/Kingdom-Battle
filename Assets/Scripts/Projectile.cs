using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    public GameObject player;
    private Rigidbody2D body;
    private Vector2 target;

    void Start()
    {
        player = GameObject.Find("Player");
        body = GetComponent<Rigidbody2D>();
        target = player.transform.position - transform.position;
    }

    void FixedUpdate()
    {
        body.AddForce(target.normalized * (Time.deltaTime * 2), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider is TilemapCollider2D)
            Destroy(this);
    }
}
