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

    // Start is called before the first frame update
    void Start()
    {
        playerMove = GameObject.Find("Sylvia").GetComponent<PlayerMovement>();
        interactScript = GameObject.Find("Sylvia").transform.GetChild(0).GetComponent<PlayerInteract>();
        playerMove.enabled = false;
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
        portraitImage.GetComponent<Animator>().SetTrigger("New");
        continueButton.SetActive(true);

        //Ensures continue button is automatically selected object so it can be pressed with gamepad/keyboard button
        EventSystem.current.SetSelectedGameObject(continueButton);

    }

    // Update is called once per frame
    void Update()
    {
        
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
            interactScript.enabled = true;
            interactScript.currentlyInteracting = false;

            portraitImage.GetComponent<Animator>().Play("End");
            portraitImage.GetComponent<Animator>().SetTrigger("New");
        }
    }
}