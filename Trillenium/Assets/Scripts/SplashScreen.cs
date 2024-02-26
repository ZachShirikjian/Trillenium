using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//LOAD THE TITLE SCREEN AFTER THE SPLASH SCREEN PLAYS (CHANGE VALUE LATER IF MORE LOGOS ARE ADDED!)
public class SplashScreen : MonoBehaviour
{
    public GameObject OwlsNestLogo;
    // Start is called before the first frame update
    void Start()
    {
        OwlsNestLogo.SetActive(false);
        Invoke("LoadLogo", 5f);
    }

    //Popup ON logo (& add others if needed later) then loads title screen after splash screen ends
    void LoadLogo()
    {
        OwlsNestLogo.SetActive(true);
        Invoke("LoadTitleScreen", 3f); //increase this if more logos are added
    }

    void LoadTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
