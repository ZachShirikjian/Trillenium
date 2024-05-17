using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
//USED AS A DISCLAIMER WHEN STARTING A NEW GAME//
public class Disclaimer : MonoBehaviour
{
    //VARIABLES//
    public bool beganGame = false;

    //REFERENCES//

    //The current point in the cutscene.
    public int curPlace;

    //The current lines of dialogue which is being spoken.
    public TextMeshProUGUI currentDialogue;

    //The Header for the Dialogue speaker (Zort).
    public TextMeshProUGUI speaker;

    //List of all the Dialogue spoken for the Cutscene. 
    public Dialogue[] dialogue;

    //Reference to the DialogueBox animator to animate the DialogueBox UI during the cutscene.
    public Animator dialogueAnim;

    //Reference to the DialogueBox
    public GameObject dialogueBox;

    //Reference to ContinueButton for advancing the Dialogue//
    public GameObject continueButton;

    //FOR THE YES/NO PROMPT AT THE END OF THE DISCLAIMER
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject blackSquare;

    public InputActionAsset controls;

    public InputActionReference skipIntro;

    //Reference to Dialogue AudioClip//
    public AudioSource dialogueSource;
    private AudioSource sfxSource;
    private AudioManager audioManager;

    //Fades screen to black after 15 seconds of reading the disclaimer message
    void Start()
    {
        beganGame = false;
        sfxSource = GameObject.Find("SFXSource").GetComponent<AudioSource>();
        audioManager = sfxSource.GetComponent<AudioManager>();
        yesButton.SetActive(false);
        noButton.SetActive(false);

        curPlace = 0;
        currentDialogue.text = dialogue[curPlace].speakerText;
        speaker.text = dialogue[curPlace].personSpeaking;
        dialogueAnim.SetTrigger("NewDialogue"); //Play the initial DialogueBox animation, which switches to its Idle state after it appears.
        dialogueBox.SetActive(true);
        sfxSource.PlayOneShot(audioManager.newDialogue);
        dialogueSource.PlayOneShot(dialogue[curPlace].audioClip);

        continueButton.SetActive(true);

        //Ensures continue button is automatically selected object so it can be pressed with gamepad/keyboard button
        EventSystem.current.SetSelectedGameObject(continueButton);

       // Invoke("PromptPlayer", 36f); //Prompt the player after the Disclaimer narration plays.
       // Invoke("FadeToBlack", 15f);
        OnEnable();
    }

    //This method gets called every time the ContinueButton is clicked in the cutscene. 
    public void ContinueDialogue()
    {
        //Play DialogueBox animation (eg Persona)
        //When clicking the Continue button, move to the next place in the cutsceneImage array and continue the dialogue
        curPlace++;
        if(curPlace < 6)
        {
            currentDialogue.text = dialogue[curPlace].speakerText;
            speaker.text = dialogue[curPlace].personSpeaking;
            dialogueSource.Stop();
            dialogueSource.PlayOneShot(dialogue[curPlace].audioClip);
            sfxSource.PlayOneShot(audioManager.continueDialogue);
        }

        //Once you reach the 2nd to last point in the cutscene, disable the DialogueBox and Speaker
        //To indicate that the Cutscene has ended.
        else if(curPlace >= 6)
        {
            //dialogueBox.SetActive(false);
            //dialogueSource.Stop();
            //currentDialogue.text = "";
            //dialogueAnim.SetBool("EndDialogue",true);
            //speaker.text = "";
            Debug.Log("END CUTSCENE");
            continueButton.SetActive(false);
            
            //After the Disclaimer plays, prompt the player if they'd like to continue or not.
            PromptPlayer(); 
        }
    }

    //Prompts player to proceed with disclaimer or return back to title 
    void PromptPlayer()
    {
        sfxSource.PlayOneShot(audioManager.promptPlayer);
        yesButton.SetActive(true);
        noButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(yesButton);
    }

    //If YES selected, play Yes dialogue and then call FadeToBlack 
    public void ChooseYes()
    {
        //There are 6 lines of dialogue in Disclaimer
        //0-5 are Disclaimer
        //6 is YES 
        //7 is NO
        curPlace = 6; 
        yesButton.SetActive(false);
        noButton.SetActive(false);
        beganGame = true;
        currentDialogue.text = dialogue[curPlace].speakerText;
        speaker.text = dialogue[curPlace].personSpeaking;
        dialogueSource.Stop();
        dialogueSource.PlayOneShot(dialogue[curPlace].audioClip);
        sfxSource.PlayOneShot(audioManager.continueDialogue);
        Invoke("FadeToBlack", 4f);
    }

    //If NO selected, play No dialogue, call FadeToBlack and return to the Title screen
    public void ChooseNo()
    {
        //There are 6 lines of dialogue in Disclaimer
        //0-5 are Disclaimer
        //6 is YES 
        //7 is NO
        curPlace = 7; 
        yesButton.SetActive(false);
        noButton.SetActive(false);
        beganGame = false;
        currentDialogue.text = dialogue[curPlace].speakerText;
        speaker.text = dialogue[curPlace].personSpeaking;
        dialogueSource.Stop();
        dialogueSource.PlayOneShot(dialogue[curPlace].audioClip);
        sfxSource.PlayOneShot(audioManager.continueDialogue);
        Invoke("FadeToBlack", 4f);
    }
    //Fades screen to black after reading message
    void FadeToBlack()
    {
        blackSquare.SetActive(true);
        blackSquare.GetComponent<Animator>().Play("FadeToBlack");
        Invoke("LoadCutscene", 2f);
    }

    //Depending on option players picked after disclaimer message plays,
    //Load the proper scene
    void LoadCutscene()
    {
        //If player chose YES, load the Intro Cutscene
        if(beganGame == true)
        {
            SceneManager.LoadScene("IntroCutscene");
        }

        //If player chose NO, return to the Title screen
        else if(beganGame == false)
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }

    //If Enter is pressed during disclaimer, instantly load 1st cutscene and skip disclaimer
    public void SkipDisclaimer(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("IntroCutscene");
    }


    private void OnEnable()
    {
        skipIntro.action.performed += SkipDisclaimer;
        skipIntro.action.Enable();
    }

    private void OnDisable()
    {
        Debug.Log("DISABLE INPUT");
        skipIntro.action.performed -= SkipDisclaimer;
        skipIntro.action.Disable();
    }
}
