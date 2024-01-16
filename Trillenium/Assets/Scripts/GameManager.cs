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
    // Start is called before the first frame update
    void Start()
    {   
        npcDialogue.SetActive(false);
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    public void CloseControls()
    {
        controlsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(controlsButton);
    }
}
