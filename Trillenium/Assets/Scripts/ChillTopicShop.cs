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
    public bool shopOpen = false; //prevents curSelectedButton from being set to continue button 

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
    public GameObject shopMenu; //reference to list of objects/buttons to spawn in 

    //INPUT//
    public InputActionAsset controls;
    public InputActionReference navigate;
    public InputActionReference buyItem;
    public InputActionReference cancelPurchase;
    public InputActionReference closeupShop; 

    // Start is called before the first frame update
    void Start()
    {
        //TODO: Set current currency to currency in UnitStats or GameManager (which is saved between scenes)

        //TODO: Set the name, text, and cost of each item in the buttons via script instead of via scene  
        shopOpen = false;

        purchaseConfirmation.SetActive(false);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        sylvia = GameObject.Find("Sylvia");
        shopUI.SetActive(false);
        shopMenu.SetActive(false);
        OnDisable();
    }


    // Update is called once per frame
    void Update()
    {
    }

    //FOR ALLOWING INPUT FOR SHOP MENU UI// (CHANGE LATER?)
    private void OnEnable()
    {
      buyItem.action.performed += BuyItem;
      cancelPurchase.action.performed += CancelPurchase;

      navigate.action.performed -= UpdateSelectedItem;
      closeupShop.action.performed -= CloseShop;

    }

    private void OnDisable()
    {
      buyItem.action.performed -= BuyItem;
      cancelPurchase.action.performed -= CancelPurchase;

      //This is enabled during normal menu selection, NOT when an item is selected to be purchased
      navigate.action.performed += UpdateSelectedItem;
      closeupShop.action.performed += CloseShop;
    }

    //UPDATES CURSELECTEDITEM BASED ON ONE THAT'S IN THE CURSELECTEDGAMEOBJECT
    public void UpdateSelectedItem(InputAction.CallbackContext context)
    {
      //Sets currently hovered over button to be one you're selecting for purchase
      if(shopOpen == true)
      {
          curSelectedButton = EventSystem.current.currentSelectedGameObject;
          Debug.Log(curSelectedButton);
      }

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
        shopOpen = true;

        Invoke("EnableShopMenu", 0.5f);
  
    }

  //CLOSES THE SHOP ONCE YOU'RE ALL DONE SHOPPING
    public void CloseShop(InputAction.CallbackContext context)
    {
      Debug.Log("CLOSING SHOP");
        gm.musicSource.Stop();
        dialogueText.text = lizzyDialogue[5].speakerText;
        Invoke("ResetMovement", 2f);
      // if(buyingItem == false)
      // {

      // }
    }

  public void ResetMovement()
  {
      shopUI.SetActive(false);
      sylvia.GetComponent<PlayerMovement>().enabled = true; //RE-ENABLE MOVEMENT AFTER EXITTING THE SHOP
      shopOpen = false;
      this.enabled = false;
  }

  //Prevents you from mashing to buy items right when shop opens
  public void EnableShopMenu()
  {
    shopMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(shopUI.transform.GetChild(0).GetChild(0).gameObject);

  }

    //METHOD FOR CONFIRMING A PURCHASE (put for the submit event on each button)
    public void ConfirmPurchase()
    {
      if(shopOpen == true)
      {
          curSelectedButton = EventSystem.current.currentSelectedGameObject;
          Debug.Log(curSelectedButton);
      }
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

        //Prevents you from selecting other objects while buying
         EventSystem.current.SetSelectedGameObject(null);
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
          curSelectedButton.GetComponentInChildren<ShopItem>().itemSprite.SetActive(false); //prevents button from being interacted with again
          curSelectedButton.GetComponent<Button>().interactable = false; //prevents button from being interacted with again
          purchaseConfirmation.SetActive(false);
          Invoke("ResetShop", 0.5f); //Prevents accidentally buying too quickly 
        }
    }

  //RESETS DIALOGUE AFTER MAKING PURCHASE OR CANCELING PURCHASE
    public void ResetShop()
    {
       EventSystem.current.SetSelectedGameObject(shopUI.transform.GetChild(0).GetChild(0).gameObject);
       dialogueText.text = lizzyDialogue[5].speakerText;
      buyingItem = false;
       OnDisable();
    }

    //METHOD FOR CANCELING A PURCHASE
    public void CancelPurchase(InputAction.CallbackContext context)
    {
      if(buyingItem == true)
      {
        Debug.Log("CANCELING PURCHASE");
        purchaseConfirmation.SetActive(false);
        dialogueText.text = lizzyDialogue[3].speakerText;
        EventSystem.current.SetSelectedGameObject(shopUI.transform.GetChild(0).GetChild(0).gameObject);
        buyingItem = false;
        Invoke("ResetShop", 1f);
      }
      //Invoke("ResetShop", 1f);
    }
}
