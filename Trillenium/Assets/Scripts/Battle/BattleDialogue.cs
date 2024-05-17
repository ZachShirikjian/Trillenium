using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
public class BattleDialogue : MonoBehaviour
{
    //VARIABLES//

    //The current point in the cutscene.
    public int curPlace;

    //The current lines of dialogue which is being spoken.
    public TextMeshProUGUI currentDialogue;

    //The Header for the Dialogue speaker (Zort).
    public TextMeshProUGUI speaker;

    //REFERENCES// 
    public BattleUI battleUI;
    // //List of all the Dialogue spoken for the Cutscene. 
    public Dialogue[] battleDialogue;
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

    //Reference to PlayerParty GameObject
   // public GameObject playerParty;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("BEGIN DIALOGUE");
        curPlace = 0;
        portraitImage.GetComponent<Image>().sprite = battleDialogue[curPlace].speakerPortait;
        currentDialogue.text = battleDialogue[curPlace].speakerText;
        speaker.text = battleDialogue[curPlace].personSpeaking;
        dialogueAnim.SetTrigger("NewDialogue"); //Play the initial DialogueBox animation, which switches to its Idle state after it appears.
        dialogueBox.SetActive(true);
        sfxSource.PlayOneShot(audioManager.newDialogue);
        dialogueSource.PlayOneShot(battleDialogue[curPlace].audioClip);

       //Initalize the Trigger so the Portrait slides in for every time a different speaker says something
         portraitImage.GetComponent<Animator>().SetTrigger("New");
         continueButton.SetActive(true);
       //Ensures continue button is automatically selected object so it can be pressed with gamepad/keyboard button
        EventSystem.current.SetSelectedGameObject(continueButton);
    }

    //Call this method every time continue button is pressed
     public void ContinueDialogue()
    {
        //Play DialogueBox animation (eg Persona)
        //When clicking the Continue button, move to the next place in the cutsceneImage array and continue the dialogue
        curPlace++;
        if (curPlace < battleDialogue.Length)
        {
            currentDialogue.text = battleDialogue[curPlace].speakerText;
            speaker.text = battleDialogue[curPlace].personSpeaking;
            portraitImage.GetComponent<Image>().sprite = battleDialogue[curPlace].speakerPortait;
            dialogueSource.Stop();
            dialogueSource.PlayOneShot(battleDialogue[curPlace].audioClip);
            //If a different person is speaking in the Cutscene,
           // Play the New Dialogue SFX to indicate a different person is speaking (like in Persona 5)
           // And play the SlideIn animation for the character portrait 
                 if(battleDialogue[curPlace].newPersonSpeaking == true)
                 {
                     portraitImage.GetComponent<Animator>().Play("SlideIn");
                     portraitImage.GetComponent<Animator>().SetTrigger("New");
                     sfxSource.PlayOneShot(audioManager.newDialogue);
                     Debug.Log("IF NEW PERSON IS SPEAKING, PLAY PORTRAIT SLIDE IN ANIMATION");
                 }

            //If the current speaker is continuing their dialogue, play the normal continueDialogue SFX
                 else if(battleDialogue[curPlace].newPersonSpeaking == false)
                 {
                     sfxSource.PlayOneShot(audioManager.continueDialogue);
                 }

        }

        //Once you reach the 2nd to last point in the cutscene, disable the DialogueBox and Speaker
        //To indicate that the Cutscene has ended.
        else if (curPlace >= battleDialogue.Length)
        {
            dialogueBox.SetActive(false);
            // currentImage.sprite = cutsceneBG[curPlace];
            currentDialogue.text = "";
            dialogueAnim.SetBool("EndDialogue", true);
            speaker.text = "";
            Debug.Log("END DIALOGUE");
            continueButton.SetActive(false);

            portraitImage.GetComponent<Animator>().Play("End");
            portraitImage.GetComponent<Animator>().SetTrigger("New");
            Invoke("DisableScript", 1f);
        }
    }

    //Sets current selected object to be Talent button to encourage Sylvia to activate talent 
    //Call ActivateTalent() in the BattleUI script and disable this script so there's no more dialogue for the rest of the battle 
    public void DisableScript()
    {
        battleUI.ActivateTalent();
        this.enabled = false; 
    }
}

