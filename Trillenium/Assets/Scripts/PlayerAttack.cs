using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//Script is modified from 2D Melee in Unity Tutorial by Muddy Wolf 
//https://www.youtube.com/watch?v=giJKCl-GVrU&t=32s&ab_channel=MuddyWolf

public class PlayerAttack : MonoBehaviour
{

    //VARIABLES//
    [SerializeField]private Animator anim; //reference to Player's animation 

    [SerializeField]private float meleeSpeed; //speed of Player's attack

    [SerializeField]private int damage; //damage of Player's attack (MAKE SURE TO SET THIS TO ATTACK STAT IN THEUNITSTATS SCRIPT!)

    float timeUntilMelee; //cooldown before next melee attack happens 
    bool canAttack = true; //only true when timeUntilMelee is <= 0 
    bool attacking = false; //ensures OnTriggerEnter ONLY gets called when attack motion is being done 

    //FOR NEW INPUT SYSTEM//
    public InputActionAsset controls;

    public InputActionReference attackButton;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        damage = GetComponent<TheUnitStats>().attack; //ensures attack in TheUnitStats is set to damage of your attack
    }

    // Update is called once per frame
    void Update()
    {
        //Once cooldown is over & J key is pressed 
        //Set the Animation Trigger to Attack to play attacking animation 
        if(timeUntilMelee <= 0)
        {
            canAttack = true;
            attacking = false;
            OnEnable();
        }

        //Subtract Time.deltaTime from timeUntilMelee to reset cooldown
        else
        {
            canAttack = false;
            timeUntilMelee -= Time.deltaTime; 
            OnDisable();
        }
    }

       //FOR ENABLING INTERACT INPUT
    public void OnEnable()
    {
        Debug.Log("CAN ATTACK");
        attackButton.action.performed += Attack;
        attackButton.action.Enable();
    }

    //FOR DISABLING INTERACT INPUT//
    public void OnDisable()
    {
        Debug.Log("COOLING DOWN");
        attackButton.action.performed -= Attack;
        attackButton.action.Disable();
    }

    //Performs a melee attack at the dungeon enemies in the direction player's currently facing 
    private void Attack(InputAction.CallbackContext context)
    {
            Debug.Log("ATTACKING");
            attacking = true;
            anim.SetTrigger("Attack");
            timeUntilMelee = meleeSpeed; //Reset the cooldown 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If Enemy is inside Attack Collider & player is attacking them
        //Damage the Enemy 
        if(other.tag == "Enemy" && attacking == true)
        {
            Debug.Log("Enemy taking damage");
            other.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}
