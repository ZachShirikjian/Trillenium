using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    //VARIABLES//
    public bool isPaused = false; //set to true if the game is paused, but false by default

    //REFERENCES//
    public GameObject npcDialogue;
    public GameObject pauseMenu; //reference to PauseMenu that opens up once isPaused = true
    public GameObject controlsMenu;
    public GameObject controlsButton; 
    public GameObject closeControlsButton;
    public GameObject resumeButton;

    public List<GameObject> playerParty = new List<GameObject>();
    private GameObject sylvia;

    public InputActionAsset controls;
    public InputActionReference closeMenu;

    //DIALOGUE REFERENCE//
    
    //The current lines of dialogue which is being spoken.
        public TextMeshProUGUI dialogueText;
        public Dialogue afraidToBattleDialogue;

        //The Header for the Dialogue speaker (Sylvia in this case).
        public TextMeshProUGUI speaker;

        //Reference to the Portrait Image of the current character that's speaking
        public GameObject portraitImage;
        //Reference to the DialogueBox animator to animate the DialogueBox UI during the cutscene.
        public Animator dialogueAnim;

        //Reference to the DialogueBox
        public GameObject dialogueBox;

        //Reference to Continue Button 
        public GameObject continueButton;

    //Reference to SFXSource//
    public AudioSource sfxSource;

    //Reference to Dialogue AudioClip//
    public AudioSource dialogueSource;

        //Reference to AudioManager//
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {   
        npcDialogue.SetActive(false);
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);

        sylvia = GameObject.Find("Sylvia");
        playerParty.Add(sylvia);

    //FOR CONTROLS PANEL//
        OnDisable(); //Disables backspace from being pressed until controls OR settings menu is open
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //FOR ALLOWING BACKSPACE TO BE PRESSED DURING CONTROLS MENU
    //FOR ENABLING INTERACT INPUT
    private void OnEnable()
    {
        closeMenu.action.performed += CloseMenu;
        closeMenu.action.Enable();
    }

    private void OnDisable()
    {
        closeMenu.action.performed -= CloseMenu;
        closeMenu.action.Disable();
    }

    // //LOADING SCREEN FOR THE BATTLE
    // public void LoadingScreeen()
    // {

    // }

    // IEnumerator LoadSceneAsync()
    // {
    //     //AsyncOperation runs in the background
    //     AsyncOperation operation = SceneManager.LoadSceneAsync("TestBattle");

    // }


    //FOR FIRST AREA ONLY//
    //Indicates to players they can't fight the first enemy alone and must talk to Vahan.
    public void NoSoloBattle()
    {
        continueButton.SetActive(false);
        npcDialogue.SetActive(true);
        dialogueBox.SetActive(true);
        dialogueAnim.SetTrigger("NewDialogue"); //Play the initial DialogueBox animation, which switches to its Idle state after it appears.
        portraitImage.GetComponent<Image>().sprite = afraidToBattleDialogue.speakerPortait;
       //Initalize the Trigger so the Portrait slides in for every time a different speaker says something
         portraitImage.GetComponent<Animator>().SetTrigger("New");
        sfxSource.PlayOneShot(audioManager.newDialogue);
        dialogueText.text = afraidToBattleDialogue.speakerText;
        dialogueSource.PlayOneShot(afraidToBattleDialogue.audioClip);
        speaker.text = "Sylvia";
        sylvia.GetComponent<PlayerMovement>().enabled = false; //DISABLE MOVEMENT during the no solo battle dialogue prompt
       
        Invoke("CloseDialogue", 5f);
    }

    public void CloseDialogue()
    {
        Debug.Log("");
            // currentImage.sprite = cutsceneBG[curPlace];
            dialogueText.text = "";
            dialogueAnim.SetBool("EndDialogue", true);
            speaker.text = "";
            Debug.Log("END DIALOGUE");
            continueButton.SetActive(false);

            //Enables player movement once dialogue is completed 

            portraitImage.GetComponent<Animator>().Play("End");
            portraitImage.GetComponent<Animator>().SetTrigger("New");
            sylvia.GetComponentInChildren<PlayerInteract>().currentlyInteracting = false;
            npcDialogue.SetActive(false);
            sylvia.GetComponent<PlayerMovement>().enabled = true; //RE-ENABLE MOVEMENT after the no solo battle dialogue prompt
    }

    //Called on the PlayerInput Script
    //Pauses the Game during the Movement and Battle scenes.
    public void Pause(InputAction.CallbackContext context)
    {
        if(isPaused == false)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton);
            Debug.Log("PauseGame");
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

        else if(isPaused == true)
        {
            Debug.Log("UnpauseGame");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }


    //PAUSE MENU UI BUTTONS//

    //RESUME GAME
        public void ResumeGame()
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }


    //QuitGame

    //TODO: Replace this with "Are You Sure You Want to Quit? All Unsaved Progress will be Lost." text
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }


//CONTROLS PANEL, SAME AS THE ONE IN THE TITLE SCREEN TO SHOW YOU CONTROLS!!!//
    public void OpenControls()
    {
        EventSystem.current.SetSelectedGameObject(closeControlsButton);
        controlsMenu.SetActive(true);
        OnEnable(); //For allowing backspace to close out of menus
    }

    // public void CloseControls()
    // {
    //     controlsMenu.SetActive(false);
    //     EventSystem.current.SetSelectedGameObject(controlsButton);
    // }

        public void CloseMenu(InputAction.CallbackContext context)
    {
        controlsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(controlsButton);
        OnDisable();
    }
}
