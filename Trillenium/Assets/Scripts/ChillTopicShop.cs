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
    public bool buyingItem = false;

    //REFERENCES//
    public GameObject shopUI; 
    private GameManager gm;
    private GameObject sylvia;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI dialogueText;

    public AudioSource dialogueSource;
    public List<Dialogue> lizzyDialogue = new List<Dialogue>(); //holds all of Lizzy's dialogue that plays during cutscene

    public GameObject curSelectedButton;

    public GameObject purchaseConfirmation;

    //INPUT//
    public InputActionAsset controls;
    public InputActionReference buyItem;
    public InputActionReference cancelPurchase;

    //SHOP ITEM LIST//
   // public List<ShopItem> shopItems = new List<ShopItem>();

    // Start is called before the first frame update
    void Start()
    {
        //TODO: Set current currency to currency in UnitStats or GameManager (which is saved between scenes)

        //TODO: Set the name, text, and cost of each item in the buttons via script instead of via scene  
        purchaseConfirmation.SetActive(false);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        sylvia = GameObject.Find("Sylvia");
        shopUI.SetActive(false);
        OnDisable();
    }


    // Update is called once per frame
    void Update()
    {
      //Sets currently hovered over button to be one you're selecting for purchase
        curSelectedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log(curSelectedButton);
    }

    //FOR ALLOWING INPUT FOR SHOP MENU UI// (CHANGE LATER?)
    private void OnEnable()
    {
      buyItem.action.performed += BuyItem;
      cancelPurchase.action.performed += CancelPurchase;
      //  closeMenu.action.performed += CloseMenu;
      //  closeMenu.action.Enable();
    }

    private void OnDisable()
    {
      buyItem.action.performed -= BuyItem;
      cancelPurchase.action.performed -= CancelPurchase;
      //  closeMenu.action.performed -= CloseMenu;
      //  closeMenu.action.Disable();
    }

    //BEGIN SHOPPING SCRIPT//
    public void OpenShop()
    {
        Debug.Log("OPENING SHOP");
        //Ensures first dialogue line is first one in the list (changes each day)
        dialogueText.text = lizzyDialogue[0].speakerText;
        currencyText.text = currency.ToString();
        buyingItem = false;

        gm.musicSource.Stop();
        gm.musicSource.clip = gm.audioManager.shopTheme;
        gm.musicSource.Play();
        shopUI.SetActive(true);

        //Temporarily Disable Player Movement 
        sylvia.GetComponent<PlayerMovement>().enabled = false; //DISABLE MOVEMENT DURING SHOPPING

        EventSystem.current.SetSelectedGameObject(shopUI.transform.GetChild(0).GetChild(0).gameObject);

    }

  //CLOSES THE SHOP ONCE YOU'RE ALL DONE SHOPPING
    public void CloseShop()
    {
        Debug.Log("CLOSING SHOP");
        gm.musicSource.Stop();
        dialogueText.text = lizzyDialogue[5].speakerText;
        shopUI.SetActive(false);
        sylvia.GetComponent<PlayerMovement>().enabled = true; //RE-ENABLE MOVEMENT AFTER EXITTING THE SHOP
        this.enabled = false;
    }

    //METHOD FOR CONFIRMING A PURCHASE (put for the submit event on each button)
    public void ConfirmPurchase()
    {
      if(currency >= curSelectedButton.GetComponentInChildren<ShopItem>().itemCost)
      {
        Debug.Log("Do you want to purchase this?");
        OnEnable();
        //1 = Checking to see if you want to purchase
        //2 = Purchase is made, Lizzy thanks you 
        //4 = Not Enough Money 
        //5 = Leaving Shop
        dialogueText.text = lizzyDialogue[1].speakerText;
        buyingItem = true;
        purchaseConfirmation.SetActive(true);
      }
      else
      {
          Debug.Log("You're out of funds!");
          dialogueText.text = lizzyDialogue[4].speakerText;
          buyingItem = false;
      }

    }

    public void BuyItem(InputAction.CallbackContext context)
    {

      //If you pressed Enter once when buying an item
      //Play the Thanks for Buying Dialogue audio clip & show text
      //Subtract item's cost from your current currency
      //Update the currency UI 
      //Remove the button AND object from the scene
      //Reset the shop after 1 second
        if(buyingItem = true)
        {
          Debug.Log("Thanks for buying!");
          dialogueText.text = lizzyDialogue[2].speakerText;
          currency -= curSelectedButton.GetComponentInChildren<ShopItem>().itemCost; 
          currencyText.text = currency.ToString();
          curSelectedButton.GetComponent<Button>().interactable = false; //prevents button from being interacted with again

          //TODO: REMOVE ITEM FROM THE SHOP DISPLAY ITSELF 

          Invoke("ResetShop", 1f); //Prevents accidentally buying too quickly 
        }
    }

  //RESETS DIALOGUE AFTER MAKING PURCHASE OR CANCELING PURCHASE
    public void ResetShop()
    {
       EventSystem.current.SetSelectedGameObject(shopUI.transform.GetChild(0).GetChild(0).gameObject);
       dialogueText.text = lizzyDialogue[5].speakerText;
       OnDisable();
    }

    //METHOD FOR CANCELING A PURCHASE
    public void CancelPurchase(InputAction.CallbackContext context)
    {
      Debug.Log("CANCELING PURCHASE");
      purchaseConfirmation.SetActive(false);
      dialogueText.text = lizzyDialogue[3].speakerText;
             EventSystem.current.SetSelectedGameObject(shopUI.transform.GetChild(0).GetChild(0).gameObject);
      //Invoke("ResetShop", 1f);
    }
}
