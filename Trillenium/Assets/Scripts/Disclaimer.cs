using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//USED AS A DISCLAIMER WHEN STARTING A NEW GAME//
public class Disclaimer : MonoBehaviour
{

    //REFERENCES//
    public GameObject blackSquare;
    //Fades screen to black after 15 seconds of reading the disclaimer message
    void Start()
    {
        
        Invoke("FadeToBlack", 15f);
    }

    //Fades screen to black after reading message
    void FadeToBlack()
    {
        blackSquare.SetActive(true);
        blackSquare.GetComponent<Animator>().Play("FadeToBlack");
        Invoke("LoadCutscene", 2f);
    }

    //Loads 1st cutscene after disclaimer plays
    void LoadCutscene()
    {
        SceneManager.LoadScene("TestCutscene");
    }
}
