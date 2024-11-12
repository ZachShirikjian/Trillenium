using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // We use TMPro in order to modify the text representing the price.

public class ItemSelection : MonoBehaviour
{
    //REFERENCES//
    [SerializeField] public ChillTopicShop chillTopicScript;
    #region Variables
    #region Cursor
    public GameObject cursor; // Cursor object.

    private float cursorX, cursorY, cursorTheta = 0f; // Cursor's position variables and theta (for trig functions).

    private float cursorSpeed = 0.3f; // Cursor's movement speed.
    private float cursorBounds = 0.25f; // Length of bounds cursor can move within.
    #endregion

    #region Items
    public Button[] items; // Array of item objects.

    public int itemIndex = 0; // Item index (read by particle spawner).

    private float itemScale, itemTheta = 0f; // Scale of items and item theta (for trig functions).

    private float itemSpeed = 0.1f; // Item's scale speed.
    private float itemBounds = 0.25f; // Length of bounds items can scale within.

    private float itemsAlpha = 0f; // Alpha value for items (for animation showing the items fading into view).

    private bool alphaCheck = false; // Checks if alpha value has been set to 1f on all items.
    #endregion

    #region Lighting
    private GameObject lighting; // Lighting object.

    private float lightingAlpha = 1f; // Alpha value for lighting transparency.
    #endregion

    #region Input-related
    private float inputCounter = 0f; // A counter that essentially acts as a delay between the user's initial input and "press and hold"-esque control.
    private const float inputCounterLimit = 0.2f; // Limit for counter.
    private int lastInput = -1; // Tracks last input.
    #endregion

    public bool itemsActive = false; // Can the player select an item (also read by particle spawner)?

    [SerializeField] private CardMovement card;

    private float tagMovScale = 1.75f; // Scales the movement of the price tag during the fade animation.
    #endregion

    void Start()
    {
        // Initialize both thetas such that trig functions will start in the middle of their expected bounds.
        cursorTheta = itemTheta = Mathf.Deg2Rad * 180f;

        cursor.SetActive(false); // Cursor object is inactive.

        InitializeButtonValues(); // Initialize values for all item buttons in array.
        
        SetCursorPosition(); // Set cursor potion.
    }

    void Update()
    {
        // Once items are active, allow control over the cursor and increment thetas for cursor and highlighted item (allows for their respective movements and scaling).
        if (itemsActive && itemIndex >= 0)
        {
            CursorController(); // Listen for user inputs.

            // Increment both thetas by their respective speeds (use delta time in case of frame rate issues).
            cursorTheta += cursorSpeed * (Time.deltaTime * 60f);
            itemTheta += itemSpeed * (Time.deltaTime * 60f);

            // Wrap both thetas to a limit of 0-360 using modulo arithmetic for simplicity and for better performance.
            cursorTheta %= Mathf.Deg2Rad * 360f;
            itemTheta %= Mathf.Deg2Rad * 360f;
        }

        // If the card's shadow border is active and items are not fully faded in, increment item alpha price tag movement scale value until items are fully faded in, then set items to active.
        if (card.shadowActive && !itemsActive)
        {
            if (itemsAlpha < 1f)
            {
                itemsAlpha += Time.deltaTime * 3f; // Increment alpha.

                if (tagMovScale > 0.5f) // Decrement scale until it gets to a small enough value (gives slight interpolated look to price tag movement).
                {
                    tagMovScale -= Time.deltaTime * 4f;
                }
                else
                {
                    tagMovScale = 0.5f;
                }
            }
            else
            {
                itemsAlpha = 1f;

                itemsActive = true; // Items are now active.

                cursor.SetActive(true); // Cursor is now active.
            }
        }
        // Use trig functions to calculate both the scaling and transparency of the items and the item lighting repsectively.
        // We use Sin() and Cos() throughout this script because both functions oscillate between -1 and 1 at a non-linear speed, which happens to give us the type of movement we want.
        itemScale = lightingAlpha = Mathf.Abs(Mathf.Sin(itemTheta));
        itemScale *= itemBounds; // Multiply by respective bounds to clamp the scaling to our liking.
    }

    void LateUpdate()
    {
        if (itemIndex >= 0)
        {
            AssignObjectValues(); // Assign our values to the objects and their respective components.
        }
        else // Is item index is -1, all items have been purchased, so we set the selected object to null.
        {
            EventSystem.current.SetSelectedGameObject(null); // Set selected object null if all items have been purchased.

            if (chillTopicScript.buyingItem == false) // If not in purchase prompt, set current selected button null.
            {
                chillTopicScript.curSelectedButton = null;
            }
        }
    }

    #region Methods
    private void AssignLighting()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        lighting = items[itemIndex].transform.GetChild(0).GetChild(1).gameObject;
    }

    private string CostStringUpdate(int cost)
    {
        string costStr = cost.ToString(); // Save cost to variable as a string.

        /*
        // We compare cost integer value to other integer values to determine how many digits it has, then add zeros if there are less than four digits.
        if (cost < 1000) // Value is less than four digits.
        {
            costStr = "0" + costStr; // Add a zero.

            if (cost < 100) // Value is less than three digits.
            {
                costStr = "0" + costStr; // Add another zero.

                if (cost < 10) // Value is less than two digits.
                {
                    costStr = "0" + costStr; // Add one last zero.
                }
            }
        }
        */

        return costStr;
    }

    private void InitializeButtonValues()
    {
        AssignLighting(); // Initialize lighting for each item button.

        // Use for loop to iterate through each button in array.
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Make mask invisible.
            items[i].transform.GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Make lighting/highlight invisible.

            // Turn masking off for each items' main image so that we can see the fade animation (if on, mask needs to be visible for masked image to be visible).
            items[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().maskable = false;

            // Set money text to the updated money string.
            items[i].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = CostStringUpdate(items[i].GetComponent<ShopItem>().itemCost);

            // Make price tag in its entirety invisible by default.
            items[i].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Set alpha value for tag image to 0f.
            items[i].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f); // Set alpha value for price text to 0f.
        }
    }

    // Sets alpha value for items; for fading them into view.
    private void FadeAnim()
    {
        // Offset we use for moving the price tags.
        float localXOffset = (1f - itemsAlpha) * tagMovScale; // Invert alpha value, then scale amount.
        
        for (int i = 0; i < items.Length; i++)
        {
            // Updates the alpha value of the item's main image.
            items[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, itemsAlpha);


            // Sync alpha values for price tag (tag image and price text) up with item fading in.
            items[i].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, itemsAlpha); // Set alpha value for tag image.
            items[i].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, itemsAlpha); // Set alpha value for price text.

            // Have price tag move into starting position (local position is 0f), in sync with fade in.
            items[i].transform.GetChild(1).GetChild(0).localPosition = new Vector3(localXOffset, 0f, 0f);
        }

        // Check if all of our items are fully opaque so we can stop calling this method.
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a != 1f) // If item's main image's alpha is not 1f, then return.
            {
                return;
            }

            if (i == items.Length - 1) // The only way we can know if all items passed alpha check is if this is the last avaiable index.
            {
                for (int j = 0; j < items.Length; j++)
                {
                    // Now that items are fully faded into view, make mask fully visible.
                    items[j].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // Make mask visible.

                    // Mask is fully visible, so now we can turn maasking on for the main images.
                    items[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().maskable = true;
                }
                
                // Assign lighting to lighting object of parent object that identifies with item index (item 0 by default).
                lighting = items[itemIndex].transform.GetChild(0).GetChild(1).gameObject;

                // Set lighting alpha to items alpha value.
                lighting.GetComponent<Image>().color = new Color(1f, 1f, 1f, lightingAlpha * 0.65f);

                alphaCheck = true; // If so, we set alpha check to true.
            }
        }
    }
    
    // Will assign cursor positions to each available input, depending on the current item.
    private int CursorPlacement(int item0, int item1, int item2, int item3)
    {
        switch (itemIndex)
        {
            case 0: // Top-left.
                return item0;
            case 1: // Top-right.
                return item1;
            case 2: // Bottom-left.
                return item2;
            default: // Bottom-right.
                return item3;
        }
    }

    // Set cursor position depending on current item index.
    private void SetCursorPosition()
    {
        // Cursor position is based on selected item's position.
        cursorX = items[itemIndex].transform.position.x;
        cursorY = items[itemIndex].transform.position.y;

        // Adjust horizontal position a little depending on which item is currently selected.
        switch (itemIndex)
        {
            case 1: // Top-right.
                cursorX -= 2.5f;
                break;
            case 2: // Bottom-left.
                cursorX -= 3f;
                break;
            default: // Top-left and bottom-right.
                cursorX -= 2f;
                break;
        }
    }

    // Loop through array of items and reset all of their scales to default.
    private void ResetItemScaling()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    // Return string depending on user input to filter out unavailable inputs.
    private string GetKeyPressed()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Return)) // Checks first.
            return "Select";
        */

        //IF the PURCHASE CONFIRM WINDOW IS NOT OPEN
        //Stop the cursor from moving.
        if (chillTopicScript.buyingItem == false)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // Checks if last is false, and so on and so forth.
                return "Up";
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                return "Right";
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                return "Down";
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                return "Left";
        }

        // If none of the above mentioned keys were pressed, return KeyCode.None (checks last; if no available input is pressed).
        return "";
    }

    private bool InputHoldDelay(int currentInput)
    {
        if (currentInput >= 0) // Directional input?
        {
            // If directional input, then allow for one input if this input is different from the last.
            if (lastInput != currentInput)
            {
                lastInput = currentInput;
                inputCounter = 0f;

                return true;
            }

            // If directional input is the same as the last, then increment counter by delta time and don't allow inputs to follow through.
            if (inputCounter < inputCounterLimit)
            {
                inputCounter += Time.deltaTime;

                if (inputCounter > inputCounterLimit)
                {
                    inputCounter = inputCounterLimit;
                }

                return false;
            }
            else // Counter has reached limit...
            {
                // Then only allow an input every (0.1 frames on 60FPS).
                if (inputCounter >= inputCounterLimit + (inputCounterLimit / 2f)) // Extends counter.
                {
                    inputCounter = inputCounterLimit; // Keeps counter above original limit if directional input is still being pressed.
                    return true;
                }

                inputCounter += Time.deltaTime;
                return false;
            }
        }
        else // No directional input, then reset values and do pretty much nothing.
        {
            lastInput = currentInput;
            inputCounter = 0f;

            return false;
        }
    }

    private void SkipInactiveItem(int increment)
    {
        while (!items[itemIndex].gameObject.activeSelf)
        {
            itemIndex += increment;
            itemIndex = ((itemIndex % items.Length) + items.Length) % items.Length; // Formula for modulo wrapping when value could be negative.
        }
    }

    // Main controller for cursor.
    private void CursorController()
    {
        GameObject currentItem; // The item currently being hovered over.
        
        if (Input.anyKey) // Listen for user input.
        {
            switch (GetKeyPressed()) // Filter out unavailable inputs by returning specific strings, then assign further insctuctions based on returned string.
            {
                case "Up":
                    if (InputHoldDelay(0)) // Only allow input to do something here if hold delay returns true.
                    {
                        itemIndex = CursorPlacement(2, 3, 0, 1); // Up and down have the exact same outputs.
                        SkipInactiveItem(1);
                        AssignLighting();
                    }
                    break;
                case "Right":
                    if (InputHoldDelay(1)) // Only allow input to do something here if hold delay returns true.
                    {
                        itemIndex = CursorPlacement(1, 0, 3, 2); // Left and right also have the exact same outputs.
                        SkipInactiveItem(1);
                        AssignLighting();
                    }
                    break;
                case "Down":
                    if (InputHoldDelay(2)) // Only allow input to do something here if hold delay returns true.
                    {
                        itemIndex = CursorPlacement(2, 3, 0, 1); // Up and down have the exact same outputs.
                        SkipInactiveItem(-1);
                        AssignLighting();
                    }
                    break;
                case "Left":
                    if (InputHoldDelay(3)) // Only allow input to do something here if hold delay returns true.
                    {
                        itemIndex = CursorPlacement(1, 0, 3, 2); // Left and right also have the exact same outputs.
                        SkipInactiveItem(-1);
                        AssignLighting();
                    }
                    break;
                default:
                    InputHoldDelay(-1); // Resets input hold values if no directional inputs are being pressed.
                    break;
            }

            ResetItemScaling(); // Reset item scaling.
            SetCursorPosition(); // Set the cursor's position.

            currentItem = items[itemIndex].gameObject;

            EventSystem.current.SetSelectedGameObject(currentItem); // Set selected object to current selected item.
            
            if (chillTopicScript.buyingItem == false) // If not in purchase prompt, set current selected button to current item.
            {
                chillTopicScript.curSelectedButton = currentItem; // Important to note that curSelectedButton is a GAME OBJECT and NOT a button.
            }
        }
        else
        {
            InputHoldDelay(-1); // Resets input hold values if no directional inputs are being pressed.
        }
    }

    // Assign our values to the objects and their respective components.
    void AssignObjectValues()
    {
        if (!alphaCheck) // If items aren't totally opaque, then update alpha value.
        {
            FadeAnim();
        }

        cursor.transform.position = new Vector3(cursorX - (Mathf.Cos(cursorTheta) * cursorBounds), cursorY, 0f); // Position of cursor.
        
        items[itemIndex].transform.localScale = lighting.transform.localScale = new Vector3(itemScale + 1f, itemScale + 1f, 1f); // Scale of selected item.

        lighting.transform.position = new Vector3(items[itemIndex].transform.position.x, items[itemIndex].transform.position.y, 0f); // Position of item lighting.

        lighting.GetComponent<Image>().color = new Color(1f, 1f, 1f, lightingAlpha * 0.65f); // Transparency of lighting sprite.
    }
    #endregion
}
