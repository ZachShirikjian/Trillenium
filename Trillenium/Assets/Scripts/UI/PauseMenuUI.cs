using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
public class PauseMenuUI : MonoBehaviour
{
    //Moves the scrollbar by holding the Analog Stick or Up/Down Arrow keys by -1 and 1 multiplier.
    //Adjusts the value of the RectTransform of the TextArea
    // Start is called before the first frame update

    //VARIABLES//
    public float vertical;
    public float anchoredPos = 10;
    public float maxPos = 970; //Max position the TextArea anchoredPos can be at.
    public float minPos = -1; //Min position the TextArea anchoredPos can be at.

    //REFERENCES//
    public RectTransform textArea;

    public Scrollbar scrollBar;

    //REFERENCES//
    public TextMeshProUGUI optionText;
    public TextMeshProUGUI optionDesc;
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

    //FOR ALLOWING BACKSPACE TO BE PRESSED DURING CONTROLS MENU.
    //Disable the Scrolling method from being called in the Journal menu.
    private void OnEnable()
    {
        navigate.action.performed += UpdatePauseMenuUI;
        navigate.action.performed -= Scroll;
        navigate.action.Enable();
    }

    //Enable scrolling method from being called while the PauseMenuUI is being disabled.
    private void OnDisable()
    {
        navigate.action.performed -= UpdatePauseMenuUI;
        navigate.action.performed += Scroll;
        //navigate.action.Disable();
    }

    //Updates the UI text in the Pause menu to display current menu & description of what it does 
    //Gets the Name & description from the PauseMenuButton script attached to each Button GameObject

    //If not in a 2nd or 3rd menu to prevent a null ref error from occurring
    public void UpdatePauseMenuUI(InputAction.CallbackContext context)
    {

        if (gm.isPaused == true && gm.secondMenuOpen == false)
        {
            Debug.Log("UPDATE PAUSE MENU UI");
            OnEnable();
            optionText.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonName;
            optionDesc.text = EventSystem.current.currentSelectedGameObject.GetComponent<PauseMenuButton>().buttonDescription;
        }

        else if(gm.secondMenuOpen == true)
        {
            Debug.Log("ALLOW JOURNAL UI SCROLLING");
            OnDisable();
        }
    }

    //When closing a secondary or tertiary menu
    //Resume allowing UpdatePauseMenuUI to happen 
    public void ReenablePauseUIUpdate()
    {
        OnEnable();
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        if (textArea == null) // Fixed Null Bug -Duncan
            return;

        //Read the Vertical axis from the Gamepad/Keyboard
        vertical = context.ReadValue<Vector2>().y;

        //If you haven't reached the bottom yet

        if (textArea.anchoredPosition.y <= maxPos)
        {
            //Scroll down the text box if inputting down
            if (vertical < 0)
            {
                Debug.Log("Max value reached");
                textArea.anchoredPosition = new Vector3(0, anchoredPos + vertical, 0);
                anchoredPos += 10;

                //Adjust the scrollbar's value so it's UI matches the player's input speed (% of reaching the bottom)
                scrollBar.value = (vertical + anchoredPos) / maxPos;

            }
        }

        if(textArea.anchoredPosition.y >= minPos)
        {
            //Scroll up the text box if inputting up 
            if (vertical > 0)
            {
                textArea.anchoredPosition = new Vector3(0, textArea.anchoredPosition.y - 10, 0);
                anchoredPos -= 10;

                //Adjust the scrollbar's value so it's UI matches the player's input speed (% of reaching the bottom)
                scrollBar.value = (vertical + anchoredPos) / maxPos;
            }
        }




    }
}
