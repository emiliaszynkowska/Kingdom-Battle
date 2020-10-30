using System.Collections;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody2D body;
    private Vector2 movement;
    private float speed = 5;
    public GameObject weapon;
    private bool weaponDirection; //true = ->, false = <- 
    
    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        weapon.transform.rotation = Quaternion.Euler(0, 0, 45);
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        playerAnimator.SetFloat("Horizontal",movement.x);
        playerAnimator.SetFloat("Speed",movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + movement * speed * Time.fixedDeltaTime);
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Right"))
        {
            if (!weaponDirection)
                weapon.transform.localPosition = new Vector3(-0.65f, -0.45f, 0);
            weapon.transform.rotation = Quaternion.Euler(0, 0, 45);
            weaponDirection = true;
        }
        else if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Left"))
        {
            if (weaponDirection)
                weapon.transform.localPosition = new Vector3(0.65f, -0.45f, 0);
            weapon.transform.rotation = Quaternion.Euler(0, 180, 45);
            weaponDirection = false;
        }
    }
    
}
