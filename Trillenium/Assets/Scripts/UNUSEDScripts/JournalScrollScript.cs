using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
public class JournalScrollScript : MonoBehaviour
{
    //Moves the scrollbar by holding the Analog Stick or Up/Down Arrow keys by -1 and 1 multiplier.
    //Adjusts the value of the RectTransform of the TextArea
    // Start is called before the first frame update

    //VARIABLES//
    public float vertical;

    //REFERENCES//
    public RectTransform textArea;

    public Scrollbar scrollBar;

    public InputActionAsset controls;
    public InputActionReference navigate;
    void Start()
    {
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
        navigate.action.performed += Scroll;
        navigate.action.Enable();
    }

    private void OnDisable()
    {
        navigate.action.performed -= Scroll;
        navigate.action.Disable();
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        //Read the Vertical axis from the Gamepad/Keyboard
        vertical = context.ReadValue<Vector2>().y;

        //Move the textArea's anchoredPosition based on the vertical value
        textArea.anchoredPosition = new Vector3(0, 1, 0);

        //Adjust the scrollbar's value so it's UI matches the player's input speed
        scrollBar.value = vertical;

    }
}
