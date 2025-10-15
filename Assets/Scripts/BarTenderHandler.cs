using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Globalization;

public class BarTenderHandler : MonoBehaviour
{
    public Vector2 movement = new Vector2();
    public Rigidbody2D rb2D;
    public GameObject panel;
    public List<Ingredient> inventory = new List<Ingredient>();

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
        if (Mathf.Abs(movement.x) + Mathf.Abs(movement.y) > 0) {
            panel.SetActive(false);
        }
    }

    public void MoveCharacter(Vector2 movementVector)
    {
        movementVector.Normalize();
        // move the RigidBody2D instead of moving the Transform
        rb2D.linearVelocity = movementVector * BarTender.speed;
    }


    public string MakeTwoLetterSignature()
    {
        if (inventory == null) return "";

        var culture = CultureInfo.CurrentCulture;
        var sb = new StringBuilder();

        foreach (var ing in inventory
                 .Where(i => i != null && !string.IsNullOrWhiteSpace(i.name))
                 .OrderBy(i => i.name, StringComparer.CurrentCultureIgnoreCase))
        {
            var n = ing.name;

            // Take first TWO LETTERS (skip digits/punctuation/spaces)
            var letters = n.Where(char.IsLetter).Take(2).ToArray();
            if (letters.Length == 0) continue;

            sb.Append(char.ToUpper(letters[0], culture));
            if (letters.Length > 1)
                sb.Append(char.ToLower(letters[1], culture));
        }

        return sb.ToString();
    }

  
}
