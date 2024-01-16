using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class TitleScreen : MonoBehaviour
{

    //REFERENCES//
    public GameObject newGameButton;
    public GameObject controlsButton;
    public GameObject closeControls;
    public GameObject controlsPanel;

    void Start()
    {
        //FIX THIS LATER//
        
        //Makes first selected button NEWGAME by default
        EventSystem.current.SetSelectedGameObject(newGameButton);
        controlsPanel.SetActive(false);
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
        EventSystem.current.SetSelectedGameObject(closeControls);
    }

    //Put on the Close Button in the ControlsPanel
    //Closes the ControlsPanel
    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(controlsButton);
    }
}
