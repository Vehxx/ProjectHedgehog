using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarTenderHandler : MonoBehaviour
{
    public Vector2 movement = new Vector2();
    public Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void FixedUpdate()
    {
        GetInput();
        MoveCharacter(movement);
    }

    private void GetInput() {
    
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    public void MoveCharacter(Vector2 movementVector)
    {
        movementVector.Normalize();
        // move the RigidBody2D instead of moving the Transform
        rb2D.velocity = movementVector * BarTender.speed;
    }

  
}
