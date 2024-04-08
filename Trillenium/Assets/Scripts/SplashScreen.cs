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
        //17 seconds total time to load all the logos, disclaimer, and content warning
        Invoke("LoadTitleScreen", 19f); 
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
