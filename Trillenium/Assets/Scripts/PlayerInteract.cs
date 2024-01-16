using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PlayerInteract : MonoBehaviour
{

    //VARIABLES//
    public bool canInteract = false; //set to true if player is near interactable
    public GameObject curObject = null; //current interactable object

    //REFERENCES//
   private GameManager gm; //reference to GameManager
   public NPCDialogue npcScript;

    void Start()
    {
       gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Check if an interactable, enemy, or NPC is in player's interact range and displays interact prompt
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "NPC")
        {
            canInteract = true;
            curObject = other.gameObject;
            Debug.Log("CAN SPEAK");
        }

        else if(other.tag == "Enemy")
        {
            canInteract = true;
            curObject = other.gameObject;
            Debug.Log("CAN FIGHT");
        }
    }

    //If player exits range of interactable object, set canInteract to be false
    public void OnTriggerExit2D(Collider2D other)
    {
        canInteract = false;
        curObject = null;
        Debug.Log("OUT OF RANGE");
    }

    //Interacts with an NPC, called with INTERACT button (A on Xbox, Space on PC)
    //Called on the Player Input methods in Inspector of Player GameObject
    //Calls BeginDialogue() method in NPCDialogue canvas script, using dialogue that's on every individual NPC itself
    public void Interact(InputAction.CallbackContext context)
    {
        if(canInteract && curObject.tag == "NPC" && gm.isPaused == false)
        {
            Debug.Log("INTERACTING");
            // npcScript.enabled = true;
             npcScript.BeginDialogue();            
        }

        if(canInteract && curObject.tag == "Enemy" && gm.isPaused == false)
        {
            Debug.Log("ENEMY BATTLE ENGAGE!");
            SceneManager.LoadScene("TestBattle");
        }
    }
}
