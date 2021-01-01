﻿using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject player;
    private Rigidbody body;
    private Vector3 start;

    void Start()
    {
        player = GameObject.Find("Player");
        body = GetComponent<Rigidbody>();
        start = transform.position;
    }

    void FixedUpdate()
    {
        Vector2 target = player.transform.position - start; 
        body.AddForce(target / 50, ForceMode.Impulse);
    }

}
