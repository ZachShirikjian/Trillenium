using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

//The "GameManager" of the Chill Topic Shop Scene
public class ChillTopicShop : MonoBehaviour
{

    //The script used for Chill Topic, Lizzy's shop which players interact with @ start of each dungeon
    //When an item gets bought, reduce amount from current currency players have,
    //And remove it from inventory 

    //VARIABLES//
    public int currency; 
    public bool buyingItem = false;
    public bool shopOpen = false; //prevents curSelectedButton from being set to continue button
    //public string shopName; //name of shop for PlayerInteract script
    public string sceneToLoad; //The Scene we want to load once we leave the Chill Topic scene. Allows this script to be used for the Chill Topic stand in multiple scenes.
    
    //REFERENCES//

    //Reference to the TransitionController for exiting out of the Chill Topic shop when pressing Backspace.
    public TransitionController transitionController;

    //References Item Selection script for selecting & handling UI of items 
    //public ItemSelection itemSelectionScript; Commented out by Cerulean.

    //public GameObject shopUI; 
    private GameManager gm;
    //private GameObject sylvia;
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
    public ItemSelection shopMenu; //reference to list of objects/buttons to spawn in ; Changed from game object to item selection script by Cerulean.

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
        //sylvia = GameObject.Find("Sylvia");
        audioManager = sfxSource.GetComponent<AudioManager>();
       // shopUI.SetActive(false);
        //shopMenu.SetActive(false);
        OnDisable();

        //EventSystem.current.SetSelectedGameObject(shopMenu.transform.GetChild(itemSelectionScript.itemIndex).gameObject); Commented out by Cerulean.
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        Debug.Log("CLEAR");

        curSelectedButton = shopMenu.transform.GetChild(0).gameObject; // Grab first item; added by Cerulean.

        Debug.Log("OPENING SHOP");
        purchaseConfirmation.SetActive(false);
        shopOpen = true;

        //Ensures first dialogue line is first one in the list (changes each day)
        dialogueText.text = lizzyDialogue[0].speakerText;
        dialogueSource.PlayOneShot(lizzyDialogue[0].audioClip);
        currencyText.text = currency.ToString();
        buyingItem = false;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
    }

    public void CheckItemActivity()
    {
        // This is a test to see if the code for disabling the purchased item works, but we use a custom method to make sure it only works if the player only inputs the return/enter key; added by Cerulean.
        if (IsOnlyReturnPressed() && shopMenu.itemsActive)
        {
            // Only do a thing if there are items available to be purchased.
            if (shopMenu.itemIndex >= 0)
            {
                curSelectedButton.gameObject.SetActive(false); // Current item object becomes inactive to denote that it has vbeen purchased.

                // In order to move the cursor onto the next available item, we start from 0, then iterate from there until we find an active item object.
                shopMenu.itemIndex = 0;

                // If the current item is inactive, check the next one.
                while (!shopMenu.items[shopMenu.itemIndex].gameObject.activeSelf)
                {
                    shopMenu.itemIndex++;

                    // If we're passed the length and still haven't found an active item, then all items are inactive, meaning there are no more items left to purchase.
                    if (shopMenu.itemIndex >= shopMenu.items.Length)
                    {
                        // Set item index to -1, disable the cursor, and manually exit the while loop.
                        Debug.Log("Soul Phrase");
                        shopMenu.itemIndex = -1;
                        shopMenu.cursor.gameObject.SetActive(false);
                        Invoke("CallCloseShop", 3.5f); //Automatically closes the shop after 1 second if all items have been purchased.
                        return; // Exit while loop.
                    }
                }

                shopMenu.AssignLighting();
            }
        }
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

    // Method to ensure that the player has pressed ONLY the return/enter key; comment and method added by Cerulean.
    private bool IsOnlyReturnPressed()
    {
        // Has the return/enter key been pressed as an input?
        bool returnPressed = Input.GetKeyDown(KeyCode.Return);

        // Have any keys that aren't the return/enter key been pressed as an input?
        bool otherInputPressed = Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Return);

        // If return/enter is pressed as an input and no other input has been pressed, return true.
        if (returnPressed && !otherInputPressed)
        {
            return true;
        }
        else // Otherwise, return false.
        {
            return false;
        }
    }

    //UPDATES CURSELECTEDITEM BASED ON ONE THAT'S IN THE CURSELECTEDGAMEOBJECT
    public void UpdateSelectedItem(InputAction.CallbackContext context)
    {
      //Sets currently hovered over button to be one you're selecting for purchase
      if(shopOpen == true && shopMenu.itemsActive)
      {
          curSelectedButton = EventSystem.current.currentSelectedGameObject;
          Debug.Log(curSelectedButton);
      }
    }

    //CLOSES THE SHOP AUTOMATICALLY WHEN ALL ITEMS ARE PURCHASED
    public void CallCloseShop()
    {
            curSelectedButton = EventSystem.current.currentSelectedGameObject;
            Debug.Log("CLOSING SHOP");
            sfxSource.PlayOneShot(audioManager.uiClose);
            //gm.musicSource.Stop();
            dialogueSource.Stop();
            dialogueText.text = lizzyDialogue[8].speakerText; //index 8 = leavingshop dialogue
            dialogueSource.PlayOneShot(lizzyDialogue[8].audioClip);

            //Calls TransitionController ExitShop() to exit out of the ChillTopic UI scene and load back to the original scene. 
            transitionController.ExitShop(sceneToLoad);
    }

    //UPDATES CURSELECTEDITEM BASED ON ONE THAT'S IN THE CURSELECTEDGAMEOBJECT
    public void CloseShop(InputAction.CallbackContext context)
    {
      //Sets currently hovered over button to be one you're selecting for purchase
      if(shopOpen == true && buyingItem == false && shopMenu.itemsActive)
      {
        curSelectedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log("CLOSING SHOP");
        sfxSource.PlayOneShot(audioManager.uiClose);
        //gm.musicSource.Stop();
        dialogueSource.Stop();
        dialogueText.text = lizzyDialogue[8].speakerText; //index 8 = leavingshop dialogue
        dialogueSource.PlayOneShot(lizzyDialogue[8].audioClip);

        //Calls TransitionController ExitShop() to exit out of the ChillTopic UI scene and load back to the original scene. 
        transitionController.ExitShop(sceneToLoad);
      }

      else if(buyingItem == true)
      {
        Debug.Log("CANCELING PURCHASE");
        sfxSource.PlayOneShot(audioManager.uiCancel);
        purchaseConfirmation.SetActive(false);
        dialogueSource.Stop();
        dialogueText.text = lizzyDialogue[5].speakerText;
        dialogueSource.PlayOneShot(lizzyDialogue[5].audioClip);
        //EventSystem.current.SetSelectedGameObject(null);
        buyingItem = false;
        Invoke("ResetShop", 1f);
      }
    }
    
    //METHOD FOR CONFIRMING A PURCHASE (put for the submit event on each button)
    public void ConfirmPurchase() // This is the method that brings up the menu that gives the player the option to purchase or cancel purchasing the current selected item; comment added by Cerulean.
    {
        if (shopMenu.itemsActive)
        {
            if (shopOpen == true)
            {
                curSelectedButton = EventSystem.current.currentSelectedGameObject;
                Debug.Log(curSelectedButton);
            }
            
            if (currency >= curSelectedButton.GetComponentInChildren<ShopItem>().itemCost)
            {
                Debug.Log("Do you want to purchase this?");
                OnEnable();
                //1-3  = Checking to see if you want to purchase (VARIATIONS)
                //4 = Purchase is made, Lizzy thanks you 
                //5 = Canceling purchase
                //6 = Not Enough Money 
                //7 = Do you want more items?
                //8 = Leaving Shop

                //Randomly pick 1 of the variations for seeing if you want to purchase item from Lizzy so it's not just the same line
                int randomVO = Random.Range(1, 3);
                dialogueSource.Stop();
                dialogueText.text = lizzyDialogue[randomVO].speakerText;
                dialogueSource.PlayOneShot(lizzyDialogue[randomVO].audioClip);
                buyingItem = true;
                purchaseConfirmation.SetActive(true);

                ////Prevents you from selecting other objects while buying
                //EventSystem.current.SetSelectedGameObject(buyItemButton);
            }
            else
            {
                Debug.Log("You're out of funds!");
                dialogueSource.Stop();
                dialogueText.text = lizzyDialogue[6].speakerText;
                dialogueSource.PlayOneShot(lizzyDialogue[6].audioClip);
                buyingItem = false;
            }
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
      if(buyingItem == true && shopMenu.itemsActive)
      {
          CheckItemActivity();
          Debug.Log("Thanks for buying!");
          dialogueSource.Stop();
          dialogueText.text = lizzyDialogue[4].speakerText;
          dialogueSource.PlayOneShot(lizzyDialogue[4].audioClip);
          sfxSource.PlayOneShot(audioManager.buyItem);
          currency -= curSelectedButton.GetComponentInChildren<ShopItem>().itemCost; 
          currencyText.text = currency.ToString();
            //urSelectedButton.GetComponentInChildren<ShopItem>().itemSprite.SetActive(false); //prevents button from being interacted with again
            curSelectedButton.gameObject.SetActive(false); // This is where the current selected button (it's actually a game object!!!) is disabled after purchasing; comment added by Cerulean.
           //curSelectedButton.GetComponent<Button>().interactable = false; //prevents button from being interacted with again
          purchaseConfirmation.SetActive(false);
          EventSystem.current.SetSelectedGameObject(null);
          Invoke("ResetShop", 3f); //Prevents accidentally buying too quickly 
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
        dialogueSource.Stop();
        dialogueText.text = lizzyDialogue[5].speakerText;
        dialogueSource.PlayOneShot(lizzyDialogue[5].audioClip);
        //EventSystem.current.SetSelectedGameObject(null);
        buyingItem = false;
        Invoke("ResetShop", 4f);
      }
    }

  //RESETS DIALOGUE AFTER MAKING PURCHASE OR CANCELING PURCHASE
    public void ResetShop()
    {
        //EventSystem.current.SetSelectedGameObject(shopMenu.transform.GetChild(itemSelectionScript.itemIndex).gameObject); Commented out by Cerulean.
        dialogueSource.Stop();
       dialogueText.text = lizzyDialogue[7].speakerText;
       dialogueSource.PlayOneShot(lizzyDialogue[7].audioClip);
       buyingItem = false;
       OnDisable();
    }
}
