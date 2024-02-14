using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PlayerInteract : MonoBehaviour
{

    //VARIABLES//
    public bool currentlyInteracting = false; //if player's currently interacting with NPC, set this to be true
    public bool canInteract = false; //set to true if player is near interactable
    public GameObject curObject = null; //current interactable object

    //REFERENCES//
   private GameManager gm; //reference to GameManager
  // private GameObject shopUI; //reference to Chill Topic (Lizzy's Shop)
  //TODO: ADD REFERENCE TO SHOP UI SCRIPT, WHICH GETS ENABLED WHEN PRESSING INTERACT AT LIZZY'S SHOP
   public NPCDialogue npcScript;

   //FOR NEW INPUT SYSTEM//
    public InputActionAsset controls;

    public InputActionReference interactButton;

    void Start()
    {
       gm = GameObject.Find("GameManager").GetComponent<GameManager>();
       //shopUI = GameObject.Find("ShopUI");
       //shopUI.SetActive(false);
       OnEnable();
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

        else if(other.tag == "Door")
        {
            canInteract = true;
            curObject = other.gameObject;
            Debug.Log("CAN ENTER DOOR");
        }
    }

    //If player exits range of interactable object, set canInteract to be false
    public void OnTriggerExit2D(Collider2D other)
    {
        currentlyInteracting = false;
        canInteract = false;
        curObject = null;
        Debug.Log("OUT OF RANGE");
    }

    //FOR ENABLING INTERACT INPUT
    private void OnEnable()
    {
        interactButton.action.performed += Interact;
        interactButton.action.Enable();
    }

    //FOR DISABLING INTERACT INPUT//
    private void OnDisable()
    {
        Debug.Log("DISABLE INPUT");
        interactButton.action.performed -= Interact;
        interactButton.action.Disable();
    }


    //Interacts with an NPC, called with INTERACT button (A on Xbox, Space on PC)
    //Called on the Player Input methods in Inspector of Player GameObject
    //Calls BeginDialogue() method in NPCDialogue canvas script, using dialogue that's on every individual NPC itself
    public void Interact(InputAction.CallbackContext context)
    {
        if(currentlyInteracting == false)
        {
            if(interactButton.action.triggered)
            {

                //FOR DOORS//
                if(canInteract && curObject.tag == "Door" && gm.isPaused == false)
                {
                    Debug.Log("OPENING DOOR");
                    currentlyInteracting = true;
                }


                //FOR NPCS//
                if(canInteract && curObject.tag == "NPC" && gm.isPaused == false)
                {
                    Debug.Log("INTERACTING");
                    // npcScript.enabled = true;
                    npcScript.BeginDialogue();        
                    currentlyInteracting = true;    
                }

                //FOR LIZZY//
                if(canInteract && curObject.tag == "Shop" && gm.isPaused == false)
                {
                    Debug.Log("ENTERING SHOP");
                    currentlyInteracting = true;
                    //shopUI.SetActive(true);
                }
                    
                //TO PREVENT BATTLE FROM STARTING BEFORE TALKING TO VAHAN, CHECK FOR LENGTH OF PARTY MEMBERS ARRAY 
                if(canInteract && curObject.tag == "Enemy" && gm.isPaused == false)
                {
                        if(gm.playerParty.Count > 1) 
                            {
                                if(canInteract && curObject.tag == "Enemy" && gm.isPaused == false)
                                {
                                    Debug.Log("ENEMY BATTLE ENGAGE!");

                                    //gm.LoadingScreen();
                                    SceneManager.LoadScene("TestBattle");
                                }
                            }
                            else if(gm.playerParty.Count == 1)
                            {
                                gm.NoSoloBattle();
                                Debug.Log("CAN'T FIGHT ALONE");
                                currentlyInteracting = true;
                            }
                        }
            }
        }

 
    }
}
