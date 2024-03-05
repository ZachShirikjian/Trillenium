using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
//USED AS A DISCLAIMER WHEN STARTING A NEW GAME//
public class Disclaimer : MonoBehaviour
{

    //REFERENCES//
    public GameObject blackSquare;

    public InputActionAsset controls;

    public InputActionReference skipIntro;

    //Fades screen to black after 15 seconds of reading the disclaimer message
    void Start()
    {
        Invoke("FadeToBlack", 15f);
        OnEnable();
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
        SceneManager.LoadScene("IntroCutscene");
    }

    //If Enter is pressed during disclaimer, instantly load 1st cutscene and skip disclaimer
    public void SkipDisclaimer(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("IntroCutscene");
    }


    private void OnEnable()
    {
        skipIntro.action.performed += SkipDisclaimer;
        skipIntro.action.Enable();
    }

    private void OnDisable()
    {
        Debug.Log("DISABLE INPUT");
        skipIntro.action.performed -= SkipDisclaimer;
        skipIntro.action.Disable();
    }
}
