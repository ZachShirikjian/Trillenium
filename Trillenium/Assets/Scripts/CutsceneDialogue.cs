using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//The script used for the Cutscene which plays after beating all the Levels.
//Displays Dialogue and Background images, which changes after clicking the ContinueButton.
public class CutsceneDialogue : MonoBehaviour
{
    //VARIABLES//

    //The current point in the cutscene.
    public int curPlace;

    //The current lines of dialogue which is being spoken.
    public TextMeshProUGUI currentDialogue;

    //The Header for the Dialogue speaker (Zort).
    public TextMeshProUGUI speaker;

    //The current Background Image which is being displayed.
    public Image currentImage; 

    //REFERENCES//

    //List of all the Background Images used for the Cutscene.
    public Sprite[] cutsceneBG; 

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

    //Reference to AudioManager//
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        curPlace = 0;
        currentImage.sprite = cutsceneBG[0];
        currentDialogue.text = dialogue[curPlace].speakerText;
        dialogueAnim.SetTrigger("NewDialogue"); //Play the initial DialogueBox animation, which switches to its Idle state after it appears.
        dialogueBox.SetActive(true);
        sfxSource.PlayOneShot(audioManager.newDialogue);

        //Initalize the Trigger so the Portrait slides in for every time a different speaker says something
                portraitImage.GetComponent<Animator>().SetTrigger("New");
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
        if(curPlace < cutsceneBG.Length)
        {
            currentImage.sprite = cutsceneBG[curPlace];
            currentDialogue.text = dialogue[curPlace].speakerText;
            speaker.text = dialogue[curPlace].personSpeaking;
            portraitImage.GetComponent<Image>().sprite = dialogue[curPlace].speakerPortait;

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
        else if(curPlace == cutsceneBG.Length)
        {
            dialogueBox.SetActive(false);
            currentImage.sprite = cutsceneBG[curPlace];
            currentDialogue.text = "";
            dialogueAnim.SetBool("EndDialogue",true);
            speaker.text = "";
        }

        //After seeing the "To Be Continued in Trillenium", clicking the Continue Button again returns you back to the Title Screen.
        else if(curPlace > cutsceneBG.Length -1)
        {
            Debug.Log("END INTRO CUTSCENE");
        }
    }
}
