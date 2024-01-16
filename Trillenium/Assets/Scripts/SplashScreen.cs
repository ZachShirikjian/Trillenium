using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//LOAD THE TITLE SCREEN AFTER THE SPLASH SCREEN PLAYS (CHANGE VALUE LATER IF MORE LOGOS ARE ADDED!)
public class SplashScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadTitleScreen", 6f);
    }

    //Loads Title Screen after the ZS logo and sting plays
    void LoadTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
