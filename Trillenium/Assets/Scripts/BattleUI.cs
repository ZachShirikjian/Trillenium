using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleUI : MonoBehaviour
{

    //VARIABLES//
    public bool alreadyAttacked = false; //set to true if current party member has already attacked, if false they can attack

    //REFERENCES//
    public GameObject attackButton;
    public GameObject player;
    private Animator playerAnim;
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(attackButton);
        playerAnim = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Reference to Attack button in the scene
    //Attacks an Enemy
    public void Attack()
    {
        if(alreadyAttacked == false)
        {

            Debug.Log("ATTACK ENEMY");
            //Add in option for selecting enemy to target

            //CHANGE LATER//
            //Plays attacking animation after the attack button is selected
            playerAnim.SetTrigger("Attacking");
            alreadyAttacked = true;
        }
        else if(alreadyAttacked == true)
        {
            Debug.Log("Already Attacked");
        }

    }
}
