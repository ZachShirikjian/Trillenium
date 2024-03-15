using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class TitleScreen : MonoBehaviour
{

    //VARIABLES//

    //REFERENCES//
    public GameObject menuButtons;
    public GameObject newGameButton;
    public GameObject controlsButton;
  //  public GameObject closeControls;
    public GameObject controlsPanel;

    
    public InputActionAsset controls;
    public InputActionReference closeMenu;

    public AudioSource sfxSource;
    public AudioManager audioManager;

    void Start()
    {
       // menuButtons.SetActive(false);
        sfxSource = GameObject.Find("SFXSource").GetComponent<AudioSource>();
        audioManager = sfxSource.GetComponent<AudioManager>();

        //TO DO: PRESS ANY BUTTON BEFORE THE MENU BUTTONS APPEAR//
        
        //Makes first selected button NEWGAME by default
        EventSystem.current.SetSelectedGameObject(newGameButton);
        controlsPanel.SetActive(false);
        OnDisable(); //Disables backspace from being pressed until controls OR settings menu is open
    }

    //Enable input for closing out of controls menu using the Backspace button (B on Xbox controller)
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

    //TEMP METHOD//

    //Loads the Disclaimer of the game.
    //REPLACE THIS WITH LOADING SAVE FILES LATER!!!
    //Put on the New Game button in the Title Screen
    public void NewGame()
    {
        SceneManager.LoadScene("Disclaimer");
    }


    //Quits out of the game in the Title Screen.
    //Can be done both in the Editor (Play Mode) and in Builds.
    public void QuitGame()
    {
        //Exits Play Mode if QUIT is clicked during Play Mode (Editor).
        # if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;


        //Closes the Game if QUIT is clicked in a Build of the game.
        #endif
        Application.Quit();
    }

    //Put on the Controls button in the Main Menu
    //Opens up the Controls Panel 
    public void OpenControls()
    {
        Debug.Log("OPENING CONTROLS");
        controlsPanel.SetActive(true);

        //TODO CHANGE THIS TO KEYBOARD TAB SO IT DISPLAYS CONTROLLER INPUTS
        EventSystem.current.SetSelectedGameObject(null);
        OnEnable();
    }

    //Close the controls panel if backspace is pressed (like how most games handle exitting out of menus)
    public void CloseMenu(InputAction.CallbackContext context)
    {
        controlsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(controlsButton);
        sfxSource.PlayOneShot(audioManager.uiClose);
        OnDisable();
    }

}
