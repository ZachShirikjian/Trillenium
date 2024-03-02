using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class ChillTopicShop : MonoBehaviour
{

    //The script used for Chill Topic, Lizzy's shop which players interact with @ start of each dungeon
    //When an item gets bought, reduce amount from current currency players have,
    //And remove it from inventory 

    //VARIABLES//
    public int currency; 

    //REFERENCES//
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI dialogueText;

    public AudioSource dialogueSource;
    public List<Dialogue> lizzyDialogue = new List<Dialogue>(); //holds all of Lizzy's dialogue that plays during cutscene

    // Start is called before the first frame update
    void Start()
    {
        //Ensures first dialogue line is first one in the list (changes each day)
        dialogueText.text = lizzyDialogue[0].speakerText;
        currencyText.text = currency.ToString();
        //TODO: Set current currency to currency in UnitStats or GameManager (which is saved between scenes)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //FOR ALLOWING INPUT FOR SHOP MENU UI// (CHANGE LATER?)
    private void OnEnable()
    {
      //  closeMenu.action.performed += CloseMenu;
      //  closeMenu.action.Enable();
    }

    private void OnDisable()
    {
      //  closeMenu.action.performed -= CloseMenu;
      //  closeMenu.action.Disable();
    }

    //METHOD FOR CONFIRMING A PURCHASE (put for the submit event on each button)
    //public void ConfirmPurchase(InputAction.CallbackContext context)
    //{
//
   // }

    public void CancelPurchase()
    {

    }
    //METHOD FOR CANCELING A PURCHASE

    //METHOD FOR LEAVING SHOP
}
