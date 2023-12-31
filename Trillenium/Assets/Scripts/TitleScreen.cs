using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class TitleScreen : MonoBehaviour
{

    //REFERENCES//
    public GameObject newGameButton;

    void Start()
    {
        //FIX THIS LATER//
        
        //Makes first selected button NEWGAME by default
        EventSystem.current.SetSelectedGameObject(newGameButton);
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
}
