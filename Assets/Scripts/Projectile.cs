using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D body;
    private Vector2 target;
    public int direction;

    void Start()
    {
        player = GameObject.Find("Player");
        body = GetComponent<Rigidbody2D>();
        target = player.transform.position - transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other is TilemapCollider2D)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        switch (direction)
        {
            // Target
            case 0:
                body.AddForce(target.normalized * (Time.deltaTime * 2), ForceMode2D.Impulse);
                break;
            // E
            case 1:
                body.AddForce(Vector2.right * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
            // W
            case 2:
                body.AddForce(Vector2.left * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
            // N
            case 3:
                body.AddForce(Vector2.up * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
            // S
            case 4:
                body.AddForce(Vector2.down * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
            // NE
            case 5:
                body.AddForce((Vector2.right + Vector2.up) * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
            // NW
            case 6:
                body.AddForce((Vector2.left + Vector2.up) * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
            // SE
            case 7:
                body.AddForce((Vector2.right + Vector2.down) * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
            // SW
            case 8:
                body.AddForce((Vector2.left + Vector2.down) * (Time.deltaTime * 10), ForceMode2D.Impulse);
                break;
        }
    }
}
