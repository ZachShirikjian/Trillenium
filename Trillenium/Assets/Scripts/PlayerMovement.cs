using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    //VARIABLES//
    private float horizontal; //references horizontal movement
    private float vertical; //references vertical movement
    private float speed = 5f; //speed of character;

    //REFERNCES//
    private Rigidbody2D rb2d; 
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //FixedUpdate used for physics calculations
    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(horizontal * speed,vertical * speed);
    }

//context.performed used for checking if button is pressed 
//context.cancelled used for checking if button is released     
    //NEW INPUT SYSTEM
    //Moves the Player with WASD or Keyboard
    //References the InputAction when an action is triggererd
    public void Move(InputAction.CallbackContext context)
    {
        //Detects movement based on D-Pad or WASD input for both horizontal AND vertical
        // movementInput = context.ReadValue<Vector2>();


        horizontal = context.ReadValue<Vector2>().x;

        //Reads the vertical movement of keyboard/gamepad for up/down movement
        vertical = context.ReadValue<Vector2>().y;
    }
}
