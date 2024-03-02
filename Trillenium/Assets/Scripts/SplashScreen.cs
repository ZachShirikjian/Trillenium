using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//LOAD THE TITLE SCREEN AFTER THE SPLASH SCREEN PLAYS (CHANGE VALUE LATER IF MORE LOGOS ARE ADDED!)
public class SplashScreen : MonoBehaviour
{
  //  public GameObject OwlsNestLogo;
    // Start is called before the first frame update
    void Start()
    {
        //OwlsNestLogo.SetActive(false);
        Invoke("LoadTitleScreen", 11.5f); //Maybe adjust this later for Content Warning about AI, Drug Abuse, and Suicide.
    }

    //Popup ON logo (& add others if needed later) then loads title screen after splash screen ends
 //   void LoadLogo()
  //  {
 //       OwlsNestLogo.SetActive(true);
 //      Invoke("LoadTitleScreen", 4f); //increase this if more logos are added
 //   }

    void LoadTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
