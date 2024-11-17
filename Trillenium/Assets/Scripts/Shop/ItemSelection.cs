using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // Use input system.
using TMPro; // We use TMPro in order to modify the text representing the price.

public class ItemSelection : MonoBehaviour
{
    #region Variables
    #region References
    [SerializeField] private PublicMethods methods; // Public methods.
    public ChillTopicShop chillTopicScript;
    
    public GameObject buyItemButton; //The Purchase Confirm Button
    #endregion

    #region Cursor
    public GameObject cursor; // Cursor object.

    private float cursorX, cursorY, cursorTheta = 0f; // Cursor's position variables and theta (for trig functions).

    private float cursorSpeed = 0.3f; // Cursor's movement speed.
    private float cursorBounds = 0.25f; // Length of bounds cursor can move within.
    #endregion

    #region Items
    public Button[,] items = new Button[2, 6]; // 2D array of item buttons.

    // Serve as the item index (read by particle spawner).
    public int itemRow = 0; // Item index for rows (top row and bottom row).
    public int itemCol = 0; // Item index for columns.

    private float itemScale, itemTheta = 0f; // Scale of items and item theta (for trig functions).

    private float itemSpeed = 0.1f; // Item's scale speed.
    private float itemBounds = 0.25f; // Length of bounds items can scale within.

    public float itemsAlpha = 0f; // Alpha value for items (for animation showing the items fading into view).

    public bool itemsActive = false; // Can the player select an item (also read by particle spawner)?

    private bool alphaCheck = false; // Checks if alpha value has been set to 1f on all items.
    #endregion

    #region Highlight
    private GameObject highlight; // Highlight object.

    private float highlightAlpha = 1f; // Alpha value for highlight transparency.
    #endregion

    #region Input-related
    private InputAction moveCursor; // Our cursor movement inputs.

    private float inputCounter = 0f; // A counter that essentially acts as a delay between the user's initial input and "press and hold"-esque control.
    private const float inputCounterLimit = 0.2f; // Limit for counter.
    private int lastInput = -1; // Tracks last input.
    #endregion

    #region Misc.
    [SerializeField] private CardMovement card;

    private float tagMovScale = 1.75f; // Scales the movement of the price tag during the fade animation.
    #endregion
    #endregion

    void Awake()
    {
        // Assign inputs and what methods to call when pressed.
        moveCursor = chillTopicScript.navigate; // Assign navigation inputs.
    }
    
    void OnEnable()
    {
        moveCursor.performed += CursorController;
    }

    void OnDisable()
    {
        moveCursor.performed -= CursorController;
    }


    void Start()
    {
        // Initialize both thetas such that trig functions will start in the middle of their expected bounds.
        cursorTheta = itemTheta = Mathf.Deg2Rad * 180f;

        cursor.SetActive(false); // Cursor object is inactive.

        InitializeButtonValues(); // Initialize values for all item buttons in array.

        moveCursor.performed += CursorController; // Subscribe inputs' performed action phase to cursor controller so that whenever I press said inputs, cursor controller is called.
    }

    void Update()
    {
        //ReturnItemNames(); Debug method.
        
        // Once items are active, allow control over the cursor and increment thetas for cursor and highlighted item (allows for their respective movements and scaling).
        if (itemsActive && itemCol >= 0) // itemCol becomes -1 after all items are purchased, so don't run this code if that's the case.
        {
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
        // Use trig functions to calculate both the scaling and transparency of the items and the item highlight repsectively.
        // We use Sin() and Cos() throughout this script because both functions oscillate between -1 and 1 at a non-linear speed, which happens to give us the type of movement we want.
        itemScale = highlightAlpha = Mathf.Abs(Mathf.Sin(itemTheta));
        itemScale *= itemBounds; // Multiply by respective bounds to clamp the scaling to our liking.
    }

    void LateUpdate()
    {
        // Assign our values to the objects and their respective components.
        AssignObjectValues();
    }

    #region Methods
    // Cycle through all items to change their respective highlights alpha values to 0f, then set only the currently selected item's highlight to the item highlight object to be modified.
    private void AssignHighlight()
    {
        // Use a nested for loop to cycle through all of the elements of our 2D array of items.
        for (int i = 0; i < items.GetLength(0); i++) // It is assumed that there is an object for each row, so using the set length of rows is appropriate here.
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++) // Rather than using the set length for columns, we actually check how many items are in the row parent object and use that as our length.
            {
                items[i, j].gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Make completely transparent.
            }
        }

        highlight = items[itemRow, itemCol].transform.GetChild(0).GetChild(1).gameObject; // Currently selected item's respective highlight becomes the item highlight object to be modified.
    }

    // Return cost as a string.
    private string CostStringUpdate(int cost)
    {
        string costStr = cost.ToString(); // Save cost to variable as a string.

        /* For adding zeros when less than 1000.
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

    // Initialize our array of item buttons and assign starting values for certain components.
    private void InitializeButtonValues()
    {
        // This is where we assign our item buttons to our 2D array of items.
        for (int i = 0; i < items.GetLength(0); i++) // Index of i is responsible for rows; we use hard-set limit because it is assumed items parent object already contains sufficient number of row objects.
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++) // Index of j is responsible for columns; we get number of children in row and use that as limit.
            {
                items[i, j] = this.transform.GetChild(i).GetChild(j).GetComponent<Button>();
            }
        }
        
        // Use for loop to iterate through each button in 2D array.
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++)
            {
                items[i, j].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Make mask invisible.
                items[i, j].transform.GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Make lighting/highlight invisible.

                // Turn masking off for each items' main image so that we can see the fade animation (if on, mask needs to be visible for masked image to be visible).
                items[i, j].transform.GetChild(0).GetChild(0).GetComponent<Image>().maskable = false;

                // Set money text to the updated money string.
                items[i, j].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = CostStringUpdate(items[i, j].GetComponent<ShopItem>().itemCost);

                // Make price tag in its entirety invisible by default.
                items[i, j].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Set alpha value for tag image to 0f.
                items[i, j].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f); // Set alpha value for price text to 0f.
            }
        }

        SetItem(); // Will assign item highlight/lighting, set the cursor position, and set the item in the event system.
    }

    // We use a nested for loop to set the masks of each item to fully opaque and essentially "enable" them.
    // "Enable", while technically a little misleading, gets the point across and serves the same purpose here.
    private void EnableMasks()
    {
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++)
            {
                // Now that items are fully faded into view, make mask fully visible.
                items[i, j].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // Make mask visible.

                // Mask is fully visible, so now we can turn masking on for the main images.
                items[i, j].transform.GetChild(0).GetChild(0).GetComponent<Image>().maskable = true;
            }
        }
    }
    
    // Check if all of our items are fully opaque so we can stop calling this method as well as some others.
    private void CheckAlphaValues()
    {
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++)
            {
                // If item's main image's alpha is not 1f, then return.
                if (items[i, j].transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a != 1f)
                {
                    break; // Break from inner loop.
                }

                // The only way we can know if all items passed alpha check is if this is the last avaiable index.
                if (i == items.GetLength(0) - 1 && j == this.transform.GetChild(i).childCount - 1)
                {
                    EnableMasks();

                    // Assign highlight to highlight object of parent object that identifies with item index (item 0 by default).
                    highlight = items[itemRow, itemCol].transform.GetChild(0).GetChild(1).gameObject;

                    // Set highlight alpha to items alpha value.
                    highlight.GetComponent<Image>().color = new Color(1f, 1f, 1f, highlightAlpha * 0.65f);

                    alphaCheck = true; // If so, we set alpha check to true.
                }
            }
        }
    }
    
    // Sets alpha value for items; for fading them into view.
    private void FadeAnim()
    {
        // Offset we use for moving the price tags.
        float localXOffset = (1f - itemsAlpha) * tagMovScale; // Invert alpha value, then scale amount.
        
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++)
            {
                // Updates the alpha value of the item's main image.
                items[i, j].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, itemsAlpha);


                // Sync alpha values for price tag (tag image and price text) up with item fading in.
                items[i, j].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, itemsAlpha); // Set alpha value for tag image.
                items[i, j].transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, itemsAlpha); // Set alpha value for price text.

                // Have price tag move into starting position (local position is 0f), in sync with fade in.
                items[i, j].transform.GetChild(1).GetChild(0).localPosition = new Vector3(localXOffset, 0f, 0f);
            }
        }

        // Check if all of our items are fully opaque so we can stop calling this method as well as some others.
        CheckAlphaValues();
    }
    
    // Will return the cursor offset from the currently selected item.
    private GameObject GetCurrentItem()
    {
        return this.transform.GetChild(itemRow).GetChild(itemCol).gameObject;
    }

    // Set cursor position depending on current item index.
    private void SetCursorPosition()
    {
        // Cursor position is based on selected item's position.
        cursorX = methods.DecimalsRounded(GetCurrentItem().transform.position.x - GetCurrentItem().GetComponent<ShopItem>().cursorOffset); // X position of current item, minus its cursor offset.
        cursorY = GetCurrentItem().transform.position.y; // Y position of current item.
    }

    // Loop through array of items and reset all of their scales to default.
    private void ResetItemScaling()
    {
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++)
            {
                items[i, j].transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    // Return string depending on user input to filter out unavailable inputs.
    private string GetInput(Vector2 inputVector)
    {
        // Works with Vector2 and is based on UI Navigate's input (only one direction at a time).
        if (inputVector.y > 0) // Checks if last is false, and so on and so forth.
            return "Up";
        if (inputVector.x > 0)
            return "Right";
        if (inputVector.y < 0)
            return "Down";
        if (inputVector.x < 0)
            return "Left";

        // If none of the above mentioned keys were pressed, return KeyCode.None (checks last; if no available input is pressed).
        return "";
    }

    // UNUSED: Used to allow "press & hold" functionality after a small delay, however, I removed it because it didn't seem necessary and messes with the BGM due to it inputting every single frame.
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

    // Skip over any inactive items while moving cursor horizontally.
    private void SkipInactiveHorizontal(int modulo, int increment)
    {
        // Iterate through columns of same row until we find an active item (cursor is disabled once all items are purchased).
        while (!items[itemRow, itemCol].gameObject.activeSelf)
        {
            itemCol += increment; // Typically takes either 1 or -1.
            itemCol = ((itemCol % modulo) + modulo) % modulo; // Formula for modulo wrapping working with negative values.
        }
    }

    // Skip over any inactive items while moving cursor vertically.
    private void SkipInactiveVertical(int modulo, int increment)
    {
        int startingCol = itemCol; // Track our starting column.

        // Because we could end up in a scenario where we lose access to moving to the next row (depends on which items are available and placed where),
        // we must check every column of each row we end up on until we either find an active item object or loop back to where we started.
        while (!items[itemRow, itemCol].gameObject.activeSelf) // Continue until we find an active item object.
        {
            // Cycle through every column of current row.
            while (!items[itemRow, itemCol].gameObject.activeSelf)
            {
                itemCol++; // Iterate to the right.
                itemCol %= this.transform.GetChild(itemRow).childCount; // Modulo wrapping.
                
                // If we loop back to our starting column, that means there are no active item objects on this row, so we break out of the inner loop and continue with the outer loop.
                if (itemCol == startingCol)
                {
                    break;
                }
            }

            // If the item object we're currently on is inactive, then increment row and modulo wrap it.
            if (!items[itemRow, itemCol].gameObject.activeSelf)
            {
                itemRow += increment; // Increment to either the left or right.
                itemRow = ((itemRow % modulo) + modulo) % modulo; // Formula for modulo wrapping when working with negative values.
            }
            else // If the item object we're currently on is active, break outer loop (active item and can still be item we started with).
            {
                break;
            }
        }
    }

    // Handles updating a lot of values whenever a new item has been selected.
    public void SetItem()
    {
        GameObject currentItem = items[itemRow, itemCol].gameObject; // The item currently being hovered over.

        AssignHighlight(); // Reassign which item has highlight/lighting effect (current selected item).
        ResetItemScaling(); // Reset item scaling.
        SetCursorPosition(); // Set cursor position relative to current selected item.

        chillTopicScript.curSelectedButton = currentItem; // Important to note that curSelectedButton is a GAME OBJECT and NOT a button.
        EventSystem.current.SetSelectedGameObject(currentItem); // Set selected object to current selected item.
    }

    // Main controller for cursor.
    private void CursorController(InputAction.CallbackContext context)
    {
        int currentChildCount = this.transform.GetChild(itemRow).childCount; // Number of items in current row.

        if (!chillTopicScript.buyingItem && itemsActive) // Only allow item selection when purchase prompt is NOT up and player is selecting an item.
        {
            Debug.Log(itemsActive);
            
            //Control manager for selecting an item.
            switch (GetInput(context.ReadValue<Vector2>())) // Filter out unavailable inputs by returning specific strings, then assign further insctuctions based on returned string.
            {
                case "Up": // Move up.
                    if (itemRow > 0)
                    {
                        itemRow--; // Move to row directly above.
                        SkipInactiveVertical(items.GetLength(0), -1); // Skip over any inactive item objects.
                        SetItem(); // Update item values.
                    }
                    break;
                case "Right": // Move right.
                    if (itemCol < currentChildCount - 1)
                    {
                        itemCol++; // Move to column directly to the right.
                        SkipInactiveHorizontal(currentChildCount, 1); // Skip over any inactive item objects.
                        SetItem(); // Update item values.
                    }
                    break;
                case "Down": // Move down.
                    if (itemRow < items.GetLength(0) - 1)
                    {
                        itemRow++; // Move to row directly below.
                        SkipInactiveVertical(items.GetLength(0), 1); // Skip over any inactive item objects.
                        SetItem(); // Update item values.
                    }
                    break;
                case "Left": // Move left.
                    if (itemCol > 0)
                    {
                        itemCol--; // Move to column directly to the left.
                        SkipInactiveHorizontal(currentChildCount, -1); // Skip over any inactive item objects.
                        SetItem(); // Update item values.
                    }
                    break;
                default:
                    break;
            }
        }
    }

    // Assign our values to the objects and their respective components.
    void AssignObjectValues()
    {
        // If items aren't totally opaque, then update alpha value.
        if (!alphaCheck)
        {
            FadeAnim();
        }

        // If item row index isn't a negative value, then assign values to these objects and their components.
        if (itemRow >= 0)
        {
            cursor.transform.position = new Vector3(cursorX - (Mathf.Cos(cursorTheta) * cursorBounds), cursorY, 0f); // Position of cursor.

            items[itemRow, itemCol].transform.localScale = highlight.transform.localScale = new Vector3(itemScale + 1f, itemScale + 1f, 1f); // Scale of selected item.

            highlight.transform.position = new Vector3(items[itemRow, itemCol].transform.position.x, items[itemRow, itemCol].transform.position.y, 0f); // Position of item highlight.

            highlight.GetComponent<Image>().color = new Color(1f, 1f, 1f, highlightAlpha * 0.65f); // Transparency of highlight sprite.
        }
        else // If row is -1, then all items have been purchased, which means we no longer need the above updates and can set these to null.
        {
            EventSystem.current.SetSelectedGameObject(null); // Set selected object null if all items have been purchased.

            // If not in purchase prompt, set current selected button null.
            if (!chillTopicScript.buyingItem)
            {
                chillTopicScript.curSelectedButton = null;
            }
        }
    }

    // Debug method that will give us the name of every entry in our 2D array of items.
    private void ReturnItemNames()
    {
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).childCount; j++)
            {
                if (items[i, j] != null)
                {
                    Debug.Log(i + ", " + j + ": " + items[i, j].gameObject.name);
                }
                else
                {
                    Debug.Log(i + ", " + j + ": No button.");
                }
            }
        }
    }
    #endregion
}
