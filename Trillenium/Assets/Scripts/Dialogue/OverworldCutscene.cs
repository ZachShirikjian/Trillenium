using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//USED AT THE BEGINNING OF OVERWORLD SCENES WHERE PLAYER CONTROL RESUMES AFTER CUTSCENE ENDS
public class OverworldCutscene : MonoBehaviour
{
    //VARIABLES//

    //The current point in the cutscene.
    public int curPlace;

    //The current lines of dialogue which is being spoken.
    public TextMeshProUGUI currentDialogue;

    //The Header for the Dialogue speaker (Zort).
    public TextMeshProUGUI speaker;

    //REFERENCES//
    //GameManager Reference//
    private GameManager gm;

    //List of all the Dialogue spoken for the Cutscene. 
    public Dialogue[] dialogue;

    //Reference to the Portrait Image of the current character that's speaking
    public GameObject portraitImage;
    //Reference to the DialogueBox animator to animate the DialogueBox UI during the cutscene.
    public Animator dialogueAnim;

    //Reference to the DialogueBox
    public GameObject dialogueBox;

    //Reference to SFXSource//
    public AudioSource sfxSource;

    //Reference to Dialogue AudioClip//
    public AudioSource dialogueSource;

    //Reference to AudioManager//
    public AudioManager audioManager;

    //Reference to ContinueButton//
    public GameObject continueButton;

     //Reference to PlayerMovement script 
    private PlayerMovement playerMove;

    //Reference to PlayerInteract script//
    private PlayerInteract interactScript;

    //INPUT//
    public InputActionAsset controls;
   // public InputActionReference skipDialogue;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.inCutscene = true;
        playerMove = GameObject.Find("OverworldSylvia").GetComponent<PlayerMovement>();
        interactScript = GameObject.Find("OverworldSylvia").transform.GetChild(0).GetComponent<PlayerInteract>();
        playerMove.enabled = false;
		//playerMove.canMove = false;
        interactScript.enabled = false;

        curPlace = 0;
        portraitImage.GetComponent<Image>().sprite = dialogue[curPlace].speakerPortait;
        currentDialogue.text = dialogue[curPlace].speakerText;
        speaker.text = dialogue[curPlace].personSpeaking;
        dialogueAnim.SetTrigger("NewDialogue"); //Play the initial DialogueBox animation, which switches to its Idle state after it appears.
        dialogueBox.SetActive(true);
        sfxSource.PlayOneShot(audioManager.newDialogue);
        dialogueSource.PlayOneShot(dialogue[curPlace].audioClip);

        //Initalize the Trigger so the Portrait slides in for every time a different speaker says something
        portraitImage.SetActive(true);
        portraitImage.GetComponent<Animator>().SetTrigger("New");
        continueButton.SetActive(true);
        //Ensures continue button is automatically selected object so it can be pressed with gamepad/keyboard button
        EventSystem.current.SetSelectedGameObject(continueButton);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //FOR ENABLING INTERACT INPUT
    public void OnEnable()
    {
        //skipDialogue.action.performed += Skip;
        //skipDialogue.action.Enable();
    }

    //FOR DISABLING INTERACT INPUT//
    public void OnDisable()
    {
        Debug.Log("DISABLE CUTSCENE INPUT");
        //skipDialogue.action.performed -= Skip;
        //skipDialogue.action.Disable();
    }


    //This method gets called every time the ContinueButton is clicked in the cutscene. 
    public void ContinueDialogue()
    {
        //Play DialogueBox animation (eg Persona)
        //When clicking the Continue button, move to the next place in the cutsceneImage array and continue the dialogue
        curPlace++;
        if(curPlace < dialogue.Length)
        {
            currentDialogue.text = dialogue[curPlace].speakerText;
            speaker.text = dialogue[curPlace].personSpeaking;
            portraitImage.GetComponent<Image>().sprite = dialogue[curPlace].speakerPortait;
            dialogueSource.Stop();
            dialogueSource.PlayOneShot(dialogue[curPlace].audioClip);
            //If a different person is speaking in the Cutscene,
            //Play the New Dialogue SFX to indicate a different person is speaking (like in Persona 5)
            //And play the SlideIn animation for the character portrait 
                if(dialogue[curPlace].newPersonSpeaking == true)
                {
                    portraitImage.GetComponent<Animator>().Play("SlideIn");
                    portraitImage.GetComponent<Animator>().SetTrigger("New");
                    sfxSource.PlayOneShot(audioManager.newDialogue);
                    Debug.Log("IF NEW PERSON IS SPEAKING, PLAY PORTRAIT SLIDE IN ANIMATION");
                }

                //If the current speaker is continuing their dialogue, play the normal continueDialogue SFX
                else if(dialogue[curPlace].newPersonSpeaking == false)
                {
                    sfxSource.PlayOneShot(audioManager.continueDialogue);
                }

        }

        //Once you reach the 2nd to last point in the cutscene, disable the DialogueBox and Speaker
        //To indicate that the Cutscene has ended.
        //Set the currentObjective text to the text in the Dialogue's Objective variable.
        else if(curPlace >= dialogue.Length)
        {
            dialogueBox.SetActive(false);
            dialogueSource.Stop();
            // currentImage.sprite = cutsceneBG[curPlace];
            currentDialogue.text = "";
            dialogueAnim.SetBool("EndDialogue",true);
            speaker.text = "";
            Debug.Log("END CUTSCENE");
            continueButton.SetActive(false);

            //Enables player movement once dialogue is completed 
            playerMove.enabled = true;
			//playerMove.canMove = false;
            interactScript.enabled = true;
            interactScript.currentlyInteracting = false;
            interactScript.OnEnable();

            portraitImage.GetComponent<Animator>().Play("End");
            portraitImage.GetComponent<Animator>().SetTrigger("New");

            //IF THIS CUTSCENE HAS AN OBJECTIVE
            //MAKE THE OBJECTIVE MARKER UI APPEAR 
            //ADDED CURPLACE -1 TO PREVENT INDEXOUTOFBOUNDSARRAY

            //OTHERWISE, LEAVE IT BE.

            Debug.Log("TEST");
            gm.inCutscene = false;
            if (dialogue[curPlace -1].currentObjective != null)
            {
                gm.ChangeObjective(dialogue[curPlace -1].currentObjective);
            }

        }
    }

    //By pressing SHIFT on the keyboard, skips all dialogue in a dialogue sequence
    //public void Skip(InputAction.CallbackContext context)
   /* {
            Debug.Log("SKIP DIALOGUE");
            curPlace = dialogue.Length;
            dialogueBox.SetActive(false);
            dialogueSource.Stop();
            // currentImage.sprite = cutsceneBG[curPlace];
            currentDialogue.text = "";
            dialogueAnim.SetBool("EndDialogue",true);
            speaker.text = "";
            Debug.Log("END CUTSCENE");
            continueButton.SetActive(false);

            //Enables player movement once dialogue is completed 
            playerMove.enabled = true;
            interactScript.enabled = true;
            interactScript.currentlyInteracting = false;
            interactScript.OnEnable();

            portraitImage.GetComponent<Animator>().Play("End");
            portraitImage.GetComponent<Animator>().SetTrigger("New");

    //       gm.inCutscene = false;
    //}
   */ 
}
