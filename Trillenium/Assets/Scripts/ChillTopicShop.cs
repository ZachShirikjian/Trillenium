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
    public AudioSource sfxSource;
    private AudioManager audioManager;
    public List<Dialogue> lizzyDialogue = new List<Dialogue>(); //holds all of Lizzy's dialogue that plays during cutscene

    public GameObject curSelectedButton;
    public GameObject purchaseConfirmation;
    public GameObject buyItemButton;
   // public GameObject cancelItemButton;
    public GameObject shopMenu; //reference to list of objects/buttons to spawn in 

    //INPUT//
    public InputActionAsset controls;
    public InputActionReference navigate;
   // public InputActionReference buyItem;
    public InputActionReference cancelPurchase;
    public InputActionReference closeShop; 

    // Start is called before the first frame update
    void Start()
    {
        //TODO: Set current currency to currency in UnitStats or GameManager (which is saved between scenes)

        //TODO: Set the name, text, and cost of each item in the buttons via script instead of via scene  
        shopOpen = false;

        purchaseConfirmation.SetActive(false);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        sylvia = GameObject.Find("Sylvia");
        audioManager = sfxSource.GetComponent<AudioManager>();
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
    //  buyItem.action.performed += BuyItem;
     // cancelPurchase.action.performed += CancelPurchase;

      Debug.Log("Enable");
      navigate.action.performed -= UpdateSelectedItem;
      //closeShop.action.performed -= CloseShop;

    }

    private void OnDisable()
    {
      //buyItem.action.performed -= BuyItem;
      //cancelPurchase.action.performed -= CancelPurchase;

      //This is enabled during normal menu selection, NOT when an item is selected to be purchased
      Debug.Log("Disable");
      navigate.action.performed += UpdateSelectedItem;
      closeShop.action.performed += CloseShop;
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
        purchaseConfirmation.SetActive(false);
        shopUI.SetActive(true); //entire UI parent of the Shop UI
        shopMenu.SetActive(false); //just the option buttons for buying stuff in the Shop UI 

                //Temporarily Disable Player Movement
                gm.isPaused = true;
       // sylvia.GetComponent<PlayerMovement>().canMove = false; //DISABLE MOVEMENT DURING SHOPPING
        //sylvia.GetComponent<PlayerMovement>().enabled = false; //DISABLE MOVEMENT DURING SHOPPING
        shopOpen = true;

        //Ensures first dialogue line is first one in the list (changes each day)
        dialogueText.text = lizzyDialogue[0].speakerText;
        currencyText.text = currency.ToString();
        buyingItem = false;

        gm.musicSource.Stop();
        gm.musicSource.clip = gm.audioManager.shopTheme;
        gm.musicSource.Play();

        Invoke("EnableShopMenu", 1f);
  
    }


    //UPDATES CURSELECTEDITEM BASED ON ONE THAT'S IN THE CURSELECTEDGAMEOBJECT
    public void CloseShop(InputAction.CallbackContext context)
    {
      //Sets currently hovered over button to be one you're selecting for purchase
      if(shopOpen == true && buyingItem == false)
      {
          curSelectedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log("CLOSING SHOP");
        sfxSource.PlayOneShot(audioManager.uiClose);
        gm.musicSource.Stop();
        dialogueText.text = lizzyDialogue[6].speakerText; //index 6 = leavingshop dialogue
        Invoke("ResetMovement", 2f);
          Debug.Log(curSelectedButton);
      }

      else if(buyingItem == true)
      {
        Debug.Log("CANCELING PURCHASE");
        sfxSource.PlayOneShot(audioManager.uiCancel);
        purchaseConfirmation.SetActive(false);
        dialogueText.text = lizzyDialogue[3].speakerText;
        //EventSystem.current.SetSelectedGameObject(null);
        buyingItem = false;
        Invoke("ResetShop", 1f);
      }
    }

  public void ResetMovement()
  {
      shopUI.SetActive(false);
      gm.isPaused = false;
      //sylvia.GetComponent<PlayerMovement>().enabled = true; //RE-ENABLE MOVEMENT AFTER EXITTING THE SHOP
      //sylvia.GetComponent<PlayerMovement>().canMove = true; //DISABLE MOVEMENT DURING SHOPPING
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
         EventSystem.current.SetSelectedGameObject(buyItemButton);
      }
      else
      {
          Debug.Log("You're out of funds!");
          dialogueText.text = lizzyDialogue[4].speakerText;
          buyingItem = false;
      }

    }

    //FOR PRESSING ENTER IN SHOP MENU WHEN YOU HOVER OVER THE PURCHASE BUTTON
    public void BuyItem()
    {

      //If you pressed Enter once when buying an item AND the PurchaseButton is being hovered over
      //Play the Thanks for Buying Dialogue audio clip & show text
      //Subtract item's cost from your current currency
      //Update the currency UI 
      //Remove the button AND object from the scene
      //Reset the shop after 1 second
      if(buyingItem == true)
      {
          Debug.Log("Thanks for buying!");
          dialogueText.text = lizzyDialogue[2].speakerText;
          sfxSource.PlayOneShot(audioManager.buyItem);
          currency -= curSelectedButton.GetComponentInChildren<ShopItem>().itemCost; 
          currencyText.text = currency.ToString();
          curSelectedButton.GetComponentInChildren<ShopItem>().itemSprite.SetActive(false); //prevents button from being interacted with again
          curSelectedButton.GetComponent<Button>().interactable = false; //prevents button from being interacted with again
          purchaseConfirmation.SetActive(false);
          EventSystem.current.SetSelectedGameObject(null);
          Invoke("ResetShop", 0.5f); //Prevents accidentally buying too quickly 
        }
      }

        //METHOD FOR CANCELING A PURCHASE (MAKE SURE THIS IS EXACTLY THE SAME AS IN THE BUYITEM() METHOD!)
    public void CancelPurchase()
    {
      if(buyingItem == true)
      {
        Debug.Log("CANCELING PURCHASE");
        sfxSource.PlayOneShot(audioManager.uiCancel);
        purchaseConfirmation.SetActive(false);
        dialogueText.text = lizzyDialogue[3].speakerText;
        //EventSystem.current.SetSelectedGameObject(null);
        buyingItem = false;
        Invoke("ResetShop", 1f);
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
}
