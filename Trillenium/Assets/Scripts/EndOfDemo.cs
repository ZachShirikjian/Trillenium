using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//USED AS A DISCLAIMER WHEN STARTING A NEW GAME//
public class EndOfDemo : MonoBehaviour
{
    public GameObject quitButton;
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(quitButton);
    }
    //TEMP method, quits out of the game on the button press
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
