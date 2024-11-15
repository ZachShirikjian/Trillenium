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

        // Automatically set the prompt to inactive if we are not buying an item; added by Cerulean.
        if (!buyingItem)
        {
            purchaseConfirmation.SetActive(false);
        }
    }

    // After we make a purchase, the current selected item, so this method takes care of taking us to the concept of "the next available item" by cycling through our 2D array of items.
    // If there are no more active item objects left, we disable the cursor and prepare to transition back into our overworld scene.
    public void CheckItemActivity()
    {
        // Keep track of the row and column that the purchased item was on for loop-checking purposes.
        int startingRow = shopMenu.itemRow;
        int startingCol = shopMenu.itemCol;

        // Assign our modulo values for easy access.
        int moduloRow = shopMenu.transform.GetChild(shopMenu.itemCol).childCount; // Based on number of children current row has.
        int moduloCol = shopMenu.items.GetLength(0); // Based on set length of rows.

        curSelectedButton.gameObject.SetActive(false); // Current item object becomes inactive to denote that it has vbeen purchased.

        // If the current item is inactive, check the next one.
        while (!shopMenu.items[shopMenu.itemRow, shopMenu.itemCol].gameObject.activeSelf)
        {
            shopMenu.itemCol++; // Increment to the right.
            shopMenu.itemCol %= moduloCol; // Modulo wrapping.

            // If we have reached our starting column, then all item objects in current row are disabled, so increment row check its columns.
            if (shopMenu.itemCol == startingCol)
            {
                shopMenu.itemRow++; // Increment to the bottom (rows go from top to bottom).
                shopMenu.itemRow %= moduloRow; // Modulo wrapping.

                // If we're passed the length and still haven't found an active item, then all items are inactive, meaning there are no more items left to purchase.
                if (shopMenu.itemRow == startingRow)
                {
                    // Set item row and column indexes to -1 to signify that there are no more items available for purchase, then disable the cursor and manually exit the while loop.
                    Debug.Log("Soul Phrase");
                    shopMenu.cursor.gameObject.SetActive(false); // Disable cursor.
                    shopMenu.itemRow = shopMenu.itemCol = -1;
                    buyingItem = false; // Prompt is down.
                    Invoke("CallCloseShop", 3.5f); // Leave shop for player after delay for audio clip has concluded.
                    return; // Exit method entirely, skipping SetItem() in order to avoid an out-of-bounds error.
                }
            }
        }

        shopMenu.SetItem(); // Update values for when new item is selected.
    }

    //FOR ALLOWING INPUT FOR SHOP MENU UI// (CHANGE LATER?)
    private void OnEnable()
    {
    //  buyItem.action.performed += BuyItem;
     // cancelPurchase.action.performed += CancelPurchase;

      Debug.Log("Enable");
      //navigate.action.performed -= UpdateSelectedItem; // Taken care of in item selection script; Cerulean.
      //closeShop.action.performed -= CloseShop;

    }

    private void OnDisable()
    {
      //buyItem.action.performed -= BuyItem;
      //cancelPurchase.action.performed -= CancelPurchase;

      //This is enabled during normal menu selection, NOT when an item is selected to be purchased
      Debug.Log("Disable");
      //navigate.action.performed += UpdateSelectedItem; // Taken care of in item selection script; Cerulean.
      closeShop.action.performed += CloseShop;
    }

    // UNUSED: Temporary method to ensure that the player has pressed ONLY the return/enter key; comment and method added by Cerulean.
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
    public void UpdateSelectedItem(InputAction.CallbackContext context) // Taken care of in item selection script; Cerulean.
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
        if (shopMenu.itemsActive && shopMenu.itemRow >= 0) // Items had to have faded into view and there must be items available for purchase.
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
                EventSystem.current.SetSelectedGameObject(buyItemButton);
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
        if (buyingItem == true && shopMenu.itemsActive)
        {
            // Only do a thing if there are items available to be purchased.
            if (shopMenu.itemRow >= 0)
            {
                Debug.Log("Thanks for buying!");
                dialogueSource.Stop();
                dialogueText.text = lizzyDialogue[4].speakerText;
                dialogueSource.PlayOneShot(lizzyDialogue[4].audioClip);
                sfxSource.PlayOneShot(audioManager.buyItem);
                currency -= curSelectedButton.GetComponentInChildren<ShopItem>().itemCost;
                currencyText.text = currency.ToString();
                //urSelectedButton.GetComponentInChildren<ShopItem>().itemSprite.SetActive(false); //prevents button from being interacted with again
                CheckItemActivity(); // Instead of just setting the item object to inactive here, I placed the whole method that already does that and repositions the selected item and cursor instead; added by Cerulean.
                                     //curSelectedButton.GetComponent<Button>().interactable = false; //prevents button from being interacted with again
                purchaseConfirmation.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                Invoke("ResetShop", 3f); //Prevents accidentally buying too quickly 
            }
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
        EventSystem.current.SetSelectedGameObject(curSelectedButton); // Sets current object in event system to currently selected item when shop is reset, however, do keep in mind that the SFX play whenever this is done; added by Cerulean.
        OnDisable();
    }
}
