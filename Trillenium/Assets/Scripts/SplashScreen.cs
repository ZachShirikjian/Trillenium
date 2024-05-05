using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//LOAD THE TITLE SCREEN AFTER THE SPLASH SCREEN PLAYS (CHANGE VALUE LATER IF MORE LOGOS ARE ADDED!)
public class SplashScreen : MonoBehaviour
{
    GameObject[] logoAnimator;

  //  public GameObject OwlsNestLogo;
    // Start is called before the first frame update
    void Start()
    {
        logoAnimator = GameObject.FindGameObjectsWithTag("Logos");
        
        //OwlsNestLogo.SetActive(false);
        //17 seconds total time to load all the logos, disclaimer, and content warning

        //Invoke("LoadTitleScreen", 19f); 
    }

    //Popup ON logo (& add others if needed later) then loads title screen after splash screen ends
 //   void LoadLogo()
  //  {
 //       OwlsNestLogo.SetActive(true);
 //      Invoke("LoadTitleScreen", 4f); //increase this if more logos are added
 //   }

    void Update()
    {
        if (Input.GetButton("Submit"))
        {
            Debug.Log("Working");
            foreach (GameObject logo in logoAnimator)
            {
                logo.GetComponent<Animator>().speed = 5f;
                if (logo.GetComponent<Alert>() != null)
                {
                    if (logo.GetComponent<Alert>().signalMessage == "LogosDone")
                    {
                        Invoke("LoadTitleScreen", .1f);
                    }
                }
            }
        }

        //If no button's been pressed, automatically load the Title Screen after 19 seconds
        else if(!Input.GetButton("Submit"))
        {
            foreach (GameObject logo in logoAnimator)
            {
                logo.GetComponent<Animator>().speed = 1f;
                if (logo.GetComponent<Alert>() != null)
                {
                    if (logo.GetComponent<Alert>().signalMessage == "LogosDone")
                    {
                        Invoke("LoadTitleScreen", .1f);
                    }
                }
            }
        }
    }

    void LoadTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
