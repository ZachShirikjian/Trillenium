using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PauseMenuUI : MonoBehaviour
{

    //REFERENCES//
    private TextMeshProUGUI optionText;
    private TextMeshProUGUI optionDesc;
    private GameManager gm;

    public InputActionAsset controls;
    public InputActionReference navigate;
    // Start is called before the first frame update
    void Start()
    {
        optionText = GameObject.Find("SelectedMenuText").GetComponent<TextMeshProUGUI>();
        optionDesc = GameObject.Find("SelectedMenuDesc").GetComponent<TextMeshProUGUI>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        OnEnable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        //FOR ALLOWING BACKSPACE TO BE PRESSED DURING CONTROLS MENU
    //FOR ENABLING INTERACT INPUT
    private void OnEnable()
    {
        navigate.action.performed += UpdatePauseMenuUI;
        navigate.action.Enable();
    }

    private void OnDisable()
    {
        navigate.action.performed -= UpdatePauseMenuUI;
        navigate.action.Disable();
    }

    //Updates the UI text in the Pause menu to display current menu & description of what it does 
    //Gets the Name & description from the PauseMenuButton script attached to each Button GameObject
    public void UpdatePauseMenuUI(InputAction.CallbackContext context)
    {
        Debug.Log("UPDATE PAUSE MENU UI");
        if(gm.isPaused == true)
        {
            optionText.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonName;
            optionDesc.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonDescription;
        }
    }
}
