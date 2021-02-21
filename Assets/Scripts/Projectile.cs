using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject player;
    private Rigidbody2D body;

    void Start()
    {
        player = GameObject.Find("Player");
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 target = player.transform.position - transform.position;
        body.AddForce(target.normalized * (Time.deltaTime * 2), ForceMode2D.Impulse);
    }

}
