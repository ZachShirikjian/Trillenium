using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    //VARIABLES//
    public int lastKey; // Keeps track of the way the player is facing. Directions are numbered 0-3 and are based on clockwise rotation (up, right, down, left) for easier readability.
    public float horizontal; //references horizontal movement
    public float vertical; //references vertical movement
    private float speed = 5f; //speed of character;
    public bool isMoving = false; //set to true if input is being made
    private Vector2 input; //reference to the input vector which normalizes to prevent awkward movement diagonally

    //REFERENCES//
    private Rigidbody2D rb2d; 
    private Animator anim;
    private GameManager gm;

    //Reference to Vahan/Petros Party Memebrs
    public GameObject vahan;

    //REFERENCE to Main Player's Melee Attack Collider 
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); 
        anim = GetComponent<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        lastKey = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(gm.isPaused == false)
        {
            anim.SetFloat("Horizontal", horizontal);
            anim.SetFloat("Vertical", vertical);
            Animate();
        }

    }

    //FixedUpdate used for physics calculations
    void FixedUpdate()
    {
        rb2d.velocity = input * speed;
    }

//context.performed used for checking if button is pressed 
//context.cancelled used for checking if button is released     
    //NEW INPUT SYSTEM
    //Moves the Player with WASD or Keyboard
    //References the InputAction when an action is triggererd
    public void Move(InputAction.CallbackContext context)
    {
        //ONLY MOVE WHEN GAME ISN'T PAUSED
        if(gm.isPaused == false)
        {
            //Detects movement based on D-Pad or WASD input for both horizontal AND vertical
            // movementInput = context.ReadValue<Vector2>();

            horizontal = context.ReadValue<Vector2>().x;

            //Reads the vertical movement of keyboard/gamepad for up/down movement
            vertical = context.ReadValue<Vector2>().y;

            input = new Vector2(horizontal,vertical);
            input.Normalize(); //normalizes the input so it doesn't move awkwardly in diagonal directions 

            if(horizontal >= 1 && vertical == 0)
            {
                lastKey = 1;
            }

            else if(horizontal <= -1 && vertical == 0)
            {
                lastKey = 3;
            }

            else if(horizontal == 0 && vertical >= 1)
            {
                lastKey = 0;
            }

            else if(horizontal == 0 && vertical <= -1)
            {
                lastKey = 2;
            }

            else if(horizontal == 0 && vertical == 0)
            {
                lastKey = -1;
            }
        
            //SET POSITION OF VAHAN TO BE BASED ON INPUT VALUES OF PLAYER CHARACTER
            //vahan.transform.position = new Vector2(horizontal, vertical);
        }
    }

    //Animates the Player 
    //CODE TAKEN FROM https://www.youtube.com/watch?v=vcDaQy3wBeU&t=0s 
    public void Animate()
    {
        //If the player is moving at all in ANY direction, set isMoving to be true
        if(input.magnitude > 0.1f || input.magnitude < -0.1f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
            anim.SetBool("isMoving", false);
        }

        if(isMoving)
        {
            anim.SetFloat("Horizontal", horizontal);
            anim.SetFloat("Vertical", vertical);
            anim.SetBool("isMoving", true);
        }
    }

    //FOR DUNGEON 
    //If you're in a dungeon, perform melee attack on enemies
}
