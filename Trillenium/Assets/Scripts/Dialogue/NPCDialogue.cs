using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
public class NPCDialogue : MonoBehaviour
{
    //VARIABLES//

    //The current point in the cutscene.
    public int curPlace;

    //The current lines of dialogue which is being spoken.
    public TextMeshProUGUI currentDialogue;

    //The Header for the Dialogue speaker (Zort).
    public TextMeshProUGUI speaker;

    //REFERENCES//

    // //List of all the Dialogue spoken for the Cutscene. 
    // public Dialogue[] npcDialogue;

    private GameManager gm;

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

    //Reference to NPC script//
    public NPC npcRef;

    //Reference to PlayerMovement script 
    private PlayerMovement playerMove;

    //Reference to PlayerInteract script//
    private PlayerInteract interactScript;

    //Reference to PlayerParty GameObject
    public GameObject playerParty;
    // Start is called before the first frame update
    void Start()
    {
        playerMove = GameObject.Find("OverworldSylvia").GetComponent<PlayerMovement>();
        interactScript = GameObject.Find("OverworldSylvia").transform.GetChild(0).GetComponent<PlayerInteract>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    //To prevent dialogue boxes from closing too fast
    //Make ContinueButton appear 0.5s after interacting with an NPC 
    public void CloseBoxDelay()
    {
         continueButton.SetActive(true);
        //Ensures continue button is automatically selected object so it can be pressed with gamepad/keyboard button
        EventSystem.current.SetSelectedGameObject(continueButton);
    }

    //Call this method every time players speak to a new NPC OR Interactable in the PlayerInteract script when pressing Interact button
    public void BeginDialogue()
    {
         if (interactScript.curObject.tag == "NPC" || interactScript.curObject.tag == "Interactable")
         {
             npcRef = interactScript.curObject.GetComponent<NPC>();
         }

         //Invoke("StartDialogue", 0.25f);
         StartDialogue();
         playerMove.enabled = false;

         //DUNCAN'S CODE FOR PREVENTING MOVEMENT SLIDING 
		 //playerMove.stopAllMovement();
		 //playerMove.canMove = false;
		  

        //ADD DELAY TO PREVENT SPAMMING AND PREVENTING FIRST DIALOGUE FROM APPEARING
        //if(npcRef.alreadySpokenTo == false)
       // {
        //

        //}

        //IF ALREADY SPOKEN TOO, PREVENT DIALOGUE FROM STARTING FROM BEGINNING

       // else if(npcRef.alreadySpokenTo == true)
       // {
       //     Debug.Log("You already talked to this NPC!");
       //}

      
    }

    public void StartDialogue()
    {
            //Prevents pausing during dialogue
            gm.inCutscene = true;
            Debug.Log("BEGIN DIALOGUE");
            curPlace = 0;
            Debug.Log(curPlace);

            //Disable the Portrait UI if the NPC is not a special NPC
            if(npcRef.specialNPC == true)
            {
                portraitImage.SetActive(true);
                portraitImage.GetComponent<Image>().sprite = npcRef.dialogue[curPlace].speakerPortait;
            }
            
            else if(npcRef.specialNPC == false)
            {
                portraitImage.SetActive(false);
            }

            currentDialogue.text = npcRef.dialogue[curPlace].speakerText;
            speaker.text = npcRef.dialogue[curPlace].personSpeaking;
            dialogueAnim.SetTrigger("NewDialogue"); //Play the initial DialogueBox animation, which switches to its Idle state after it appears.
            dialogueBox.SetActive(true);
            sfxSource.PlayOneShot(audioManager.newDialogue);
            dialogueSource.PlayOneShot(npcRef.dialogue[curPlace].audioClip);

        //Initalize the Trigger so the Portrait slides in for every time a different speaker says something
            portraitImage.GetComponent<Animator>().SetTrigger("New");

            Invoke("CloseBoxDelay", 0.5f);
    }

    // //This method gets called every time the ContinueButton is clicked when players are talking with the NPCs. 
     public void ContinueDialogue()
    {
        //Play DialogueBox animation (eg Persona)
        //When clicking the Continue button, move to the next place in the cutsceneImage array and continue the dialogue
        Debug.Log(curPlace);
        curPlace++;
        if (curPlace < npcRef.dialogue.Length)
        {
            currentDialogue.text = npcRef.dialogue[curPlace].speakerText;
            speaker.text = npcRef.dialogue[curPlace].personSpeaking;
            portraitImage.GetComponent<Image>().sprite = npcRef.dialogue[curPlace].speakerPortait;
            dialogueSource.Stop();
            dialogueSource.PlayOneShot(npcRef.dialogue[curPlace].audioClip);
            //If a different person is speaking in the Cutscene,
           // Play the New Dialogue SFX to indicate a different person is speaking (like in Persona 5)
           // And play the SlideIn animation for the character portrait 
                 if(npcRef.dialogue[curPlace].newPersonSpeaking == true)
                 {
                    portraitImage.GetComponent<Animator>().Play("SlideIn");
                     portraitImage.GetComponent<Animator>().SetTrigger("New");
                     sfxSource.PlayOneShot(audioManager.newDialogue);
                     Debug.Log("IF NEW PERSON IS SPEAKING, PLAY PORTRAIT SLIDE IN ANIMATION");
                 }

            //If the current speaker is continuing their dialogue, play the normal continueDialogue SFX
                 else if(npcRef.dialogue[curPlace].newPersonSpeaking == false)
                 {
                     sfxSource.PlayOneShot(audioManager.continueDialogue);
                 }

        }

        //Once you reach the 2nd to last point in the cutscene, disable the DialogueBox and Speaker
        //To indicate that the Cutscene has ended.
        else if (curPlace >= npcRef.dialogue.Length)
        {

            //Ensures NPC dialogue doesn't repeat again
            //npcRef.alreadySpokenTo = true;
            //npcRef.helpIcon.SetActive(false);

            dialogueSource.Stop();
            dialogueBox.SetActive(false);
            // currentImage.sprite = cutsceneBG[curPlace];
            currentDialogue.text = "";
            dialogueAnim.SetBool("EndDialogue", true);
            speaker.text = "";
            Debug.Log("END DIALOGUE");
            continueButton.SetActive(false);

            //Enables player movement once dialogue is completed 
            playerMove.enabled = true;
			//playerMove.canMove = true;
            interactScript.currentlyInteracting = false;

            portraitImage.GetComponent<Animator>().Play("End");
            portraitImage.GetComponent<Animator>().SetTrigger("New");

            // //If NPC is Vahan or Petros, adds them to Player Party List 
            //Necessary for fighting first battle 

            //Load the first battle scene 
            
            if(interactScript != null)
            {
                if(interactScript.curObject.name == "VahanNPC")
                {
                    gm.playerParty.Add(interactScript.curObject);
                    gm.LoadBattleScene("FirstBattle");
                }
            }

            else if(interactScript == null)
            {
                Debug.Log("Remove this later.");
            }

            gm.inCutscene = false;
            // {
            //     Debug.Log("Vahan has joined the party!");
            //     interactScript.curObject.transform.parent = playerParty.transform;
            //     interactScript.curObject.transform.position = new Vector2(0,-2);

            //     //Ensures HelpIcon is disabled if an NPC joins your party//
            //     interactScript.curObject.transform.GetChild(0).gameObject.SetActive(false);

            //     //ENABLE ANIMATIONS//
            //     interactScript.curObject.GetComponent<Animator>().enabled = true;
            // }
        }
    }
}
