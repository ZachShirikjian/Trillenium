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
    public bool inCutscene = false;
    public bool controlsMenuOpen = false; //set to true if you open controls menu in Pause menu

    //REFERENCES//
    public GameObject loadingScreen; //reference to loading screen transition
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

    private ChillTopicShop shopRef;

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

    //Reference to MusicSource//
    public AudioSource musicSource; 
    
    //Reference to SFXSource//
    public AudioSource sfxSource;

    //Reference to Dialogue AudioClip//
    public AudioSource dialogueSource;

        //Reference to AudioManager//
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {   
        isPaused = false;
        controlsMenuOpen = false;
        loadingScreen.SetActive(false);
        //TEMPORARY SOLUJTION
        //If GameManager GameObject doesn't have an OverworldCutsceneComponent attached to it (if it has no children)
        //Disable npcDialogue, overworld cutscene takes precedent
        if(transform.childCount == 0)
        {
            npcDialogue.SetActive(false);
        }

        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);

        sylvia = GameObject.Find("Sylvia");
        playerParty.Add(sylvia);

        //TODO: Get GameObject.FindWithTag("Player") and add it to the PlayerParty so when Vahan and Petros get added they g

        //For scenes with shop UI 
      //  shopRef = GameObject.Find("ChillTopicShop").GetComponent<ChillTopicShop>();
      //  if(shopRef != null)
      //  {
      //      Debug.Log("Disable Shop script");
      //      shopRef.enabled = false;
      //  }

        //FOR CONTROLS PANEL//
        OnEnable(); //Disables backspace from being pressed until controls OR settings menu is open (set to OnEnable as disabling this breaks backspace input elsewhere)
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

    //LOAD THE PROPER BATTLE SCENE WHEN INTERACTING WITH A BOSS
    //PARAMETER IS SCENENAME OF THE BOSS BATTLE SCENE 
    public void LoadBattleScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            yield return null;
        }
        if(operation.isDone)
        {
            loadingScreen.SetActive(false);
            yield return new WaitForSeconds(1f); //delay before new scene loads
        }
    }


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
    //Prevents Paused from being called during Dialogue sequences.
    public void Pause(InputAction.CallbackContext context)
    {
        if(isPaused == false && inCutscene == false)
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
        controlsMenuOpen = true;
        OnEnable(); //For allowing backspace to close out of menus
    }

    // public void CloseControls()
    // {
    //     controlsMenu.SetActive(false);
    //     EventSystem.current.SetSelectedGameObject(controlsButton);
    // }

    public void CloseMenu(InputAction.CallbackContext context)
    {
        //Ensures this only runs when menu is paused
        if(isPaused == true)
        {
            //If on the PauseMenu
            //Close the PauseMenu
            if(controlsMenuOpen == false)
            {
                Debug.Log("UnpauseGame");
                sfxSource.PlayOneShot(audioManager.uiClose);
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                isPaused = false;
            }

            //If on the controlsMenu (or a different menu for that matter)
            //close out of that current menu
            //play the cancel SFX 
            //and set curSelectedButton to be back on main one 
            else if(controlsMenuOpen == true)
            {
                controlsMenu.SetActive(false);
                EventSystem.current.SetSelectedGameObject(controlsButton);
                sfxSource.PlayOneShot(audioManager.uiCancel);
                controlsMenuOpen = false;
                //OnDisable();
            }

        }

    }
}
