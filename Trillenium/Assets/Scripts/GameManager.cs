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
    public bool secondMenuOpen = false; //set to true if you open any of the secondary menus (Item, Journal, System, etc.)
    public bool thirdMenuOpen = false; //only set to true if you open up a third menu (Controls, Settings, etc)

    //REFERENCES//
    public GameObject loadingScreen; //reference to loading screen transition
    public GameObject npcDialogue;
    public GameObject pauseMenu; //reference to PauseMenu that opens up once isPaused = true
    public GameObject itemsButton;

     //PAUSE MENU REFS//
    public GameObject itemMenu;
    public GameObject statsMenu;
    public GameObject journalMenu;
    public GameObject systemMenu;
    public GameObject controlsMenu;

    public GameObject itemButton;
    public GameObject statsButton;
    public GameObject journalButton;
    public GameObject systemButton;
    public GameObject controlsButton; 
    public GameObject closeControlsButton;
    //Reference to current menu being displayed//
    public GameObject currentMenu;


    public List<GameObject> playerParty = new List<GameObject>();
    private GameObject sylvia;

    public InputActionAsset controls;
    public InputActionReference closeMenu;

    private ChillTopicShop shopRef;

    //Reference to the PauseMenuUI script, which gets enabled when the Pause Menu is active
    public PauseMenuUI pauseUI;

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
        sfxSource = GameObject.Find("SFXSource").GetComponent<AudioSource>();
        audioManager = sfxSource.GetComponent<AudioManager>();
        isPaused = false;
        pauseUI = GameObject.Find("Canvas").GetComponent<PauseMenuUI>();
        pauseUI.enabled = false;
        secondMenuOpen = false;
        thirdMenuOpen = false;
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
        systemMenu.SetActive(false);
        journalMenu.SetActive(false);

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
            //CHANGE THIS TO ITEMS AFTER IDGA CLARK DEMO
            EventSystem.current.SetSelectedGameObject(journalButton);
            Debug.Log("PauseGame");
            pauseMenu.SetActive(true);
            currentMenu = pauseMenu;
            Time.timeScale = 0f;
            isPaused = true;
            pauseUI.enabled = true;
        }

        else if(isPaused == true)
        {
            Debug.Log("UnpauseGame");
            pauseMenu.SetActive(false);
            pauseUI.enabled = false;
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    //PAUSE MENU UI BUTTONS//

    //When you select an option in the Pause Menu
    //Set currentMenu to the menu that's displayed on screen
    //When pressing backspace, you disable the currentMenu that's being played
    //To close back to the menu you're currently on 

    //ITEMS//
    public void OpenItems()
    {
        //TODO: SET SELECTED BUTTON TO FIRST ITEM MENU OPTION SO IT CAN BE SCROLLED DOWN, USED, ETC.
        EventSystem.current.SetSelectedGameObject(null);
        itemMenu.SetActive(true);
        currentMenu = itemMenu;
        secondMenuOpen = true;
        OnEnable();
    }

    //STATS//
    public void OpenStats()
    {
        //TODO: SET SELECTED BUTTON TO FIRST ITEM MENU OPTION SO IT CAN BE SCROLLED DOWN, USED, ETC.
        EventSystem.current.SetSelectedGameObject(null);
        statsMenu.SetActive(true);
        currentMenu = statsMenu;
        secondMenuOpen = true;
        OnEnable();
    }

    //JOURNAL//
    public void OpenJournal()
    {
        //TODO: SET SELECTED BUTTON TO NAVIGATE BETWEEN DIFFERENT JOURNAL ENTRY DATES
        EventSystem.current.SetSelectedGameObject(null);
        journalMenu.SetActive(true);
        currentMenu = journalMenu;
        secondMenuOpen = true;
        OnEnable();
    }

//OPENS SYSTEM MENU
//INCLUDES: CONTROLS, SETTINGS, AND RETURN TO TITLE
public void OpenSystem()
{
    systemMenu.SetActive(true);
    currentMenu = systemMenu;
    EventSystem.current.SetSelectedGameObject(controlsButton);
    secondMenuOpen = true;
    OnEnable();

}
//CONTROLS PANEL, SAME AS THE ONE IN THE TITLE SCREEN TO SHOW YOU CONTROLS!!!//
    public void OpenControls()
    {
        //TODO: CHANGE THIS TO SELECT KEYBOARD BUTTON, SWITCH BETWEEN KEYBOARD AND GAMEPAD INPUT
       // EventSystem.current.SetSelectedGameObject(closeControlsButton);
               EventSystem.current.SetSelectedGameObject(null);

        currentMenu = controlsMenu;
        controlsMenu.SetActive(true);
        systemMenu.SetActive(false);
        thirdMenuOpen = true;
        OnEnable(); //For allowing backspace to close out of menus
    }

    // public void CloseControls()
    // {
    //     controlsMenu.SetActive(false);
    //     EventSystem.current.SetSelectedGameObject(controlsButton);
    // }

    //TODO: Replace this with "Are You Sure You Want to Quit? All Unsaved Progress will be Lost." text
    public void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }


    public void CloseMenu(InputAction.CallbackContext context)
    {
        //Ensures this only runs when menu is paused
        if(isPaused == true)
        {
            //If you're on the main Pause menu (not in Controls, Items, System, etc.)
            if(secondMenuOpen == false && thirdMenuOpen == false)
            {
                Debug.Log("UnpauseGame");
                sfxSource.PlayOneShot(audioManager.uiClose);
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                isPaused = false;
            }

            //If closing out of 3rd menu (controls, settings, etc)
            else if(thirdMenuOpen == true)
            {
                Debug.Log("Close 3rd Menu");
                sfxSource.PlayOneShot(audioManager.uiCancel);

                //Brings you back to correct 2nd menu & makes selected button controls button
                if(currentMenu == controlsMenu)
                {
                    Debug.Log("CLOSE CONTROLS");
                     //disables current menu & sets current menu to one that's on screen
                     currentMenu.SetActive(false);
                     systemMenu.SetActive(true);
                     EventSystem.current.SetSelectedGameObject(controlsButton);
                     currentMenu = systemMenu;
                     thirdMenuOpen = false;
                }

               // else if(currentMenu == settingsMenu)
              //  {
            //
            //    }

                //ENSURES TO UPDATE THE PAUSE MENU UI BASED ON THE CURRENT BUTTON SELECTED
                pauseUI.optionText.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonName;
                pauseUI.optionDesc.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonDescription;
            }

            //If closing out of 2nd Menu (Items, Journal, System, etc)
            //Disable the 2nd menu and 
            //Return to Main Menu
            else if(secondMenuOpen == true)
            {
                Debug.Log("Close 2nd Menu");
                sfxSource.PlayOneShot(audioManager.uiCancel);

                if(currentMenu == itemMenu)
                {
                    Debug.Log("CLOSE STATS");
                    //TODO: Add animation of sylvia looking up after menu is closed
                    currentMenu.SetActive(false);
                    pauseMenu.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(itemsButton);
                    currentMenu = pauseMenu;
                    secondMenuOpen = false;
                }

                else if(currentMenu == statsMenu)
                {
                    Debug.Log("CLOSE STATS");
                    //TODO: Add animation of sylvia looking up after menu is closed
                    currentMenu.SetActive(false);
                    pauseMenu.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(statsButton);
                    currentMenu = pauseMenu;
                    secondMenuOpen = false;
                }

                else if(currentMenu == journalMenu)
                {
                    Debug.Log("CLOSE JOURNAL");
                    //TODO: Add animation of sylvia closing her book after the menu is being closed
                    currentMenu.SetActive(false);
                    pauseMenu.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(journalButton);
                    currentMenu = pauseMenu;
                    secondMenuOpen = false;
                }

                else if(currentMenu == systemMenu)
                {
                    currentMenu.SetActive(false);
                    pauseMenu.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(systemButton);
                    //disables current menu & sets current menu to one that's on screen
                    currentMenu = pauseMenu;
                    secondMenuOpen = false;
                }

                 //ENSURES TO UPDATE THE PAUSE MENU UI BASED ON THE CURRENT BUTTON SELECTED
                pauseUI.optionText.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonName;
                pauseUI.optionDesc.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonDescription;
            }
            
        }

    }
}
