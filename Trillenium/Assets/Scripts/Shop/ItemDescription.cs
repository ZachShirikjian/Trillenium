using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To access image component.
using TMPro; // We use TMPro in order to modify the description text.

public class ItemDescription : MonoBehaviour
{
    #region Variables
    [SerializeField] private PublicMethods methods;

    [SerializeField] private ItemSelection itemSelect;
    [SerializeField] private SwordMovement sword;
    
    private GameObject note;
    private GameObject noteShadow;
    private TextMeshProUGUI descText;

    [SerializeField] private Sprite[] noteSprites = new Sprite[8];
    private int spriteIndex = 2; // Starts us with the first frame of the unfolding animation.

    private float localStartY;
    private float localOffsetY = 1f;
    private float offsetScale = 17.5f;

    private float theta = 0f;
    private float rotZ = 0f;

    private bool animActive = false; // Is animation active?.

    #region Shadow-related Variables
    private float shadowStartX; // Shadow's starting local X position.
    private float shadowStartY; // Shadow's starting local Y position.

    private float shadowPosX; // Shadow's current local X position.
    private float shadowPosY; // Shadow's current local Y position.

    private float shadowSpeed = 2.1f; // Movement speed of shadow.
    private const float shadowAcc = 0.08f; // Acceleration of shadow's movement speed.

    private bool shadowActive = false; // Is shadow border movement active?
    #endregion
    #endregion

    void Start()
    {
        note = this.transform.GetChild(1).gameObject;
        noteShadow = this.transform.GetChild(0).gameObject;
        descText = this.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        localStartY = note.transform.localPosition.y;

        descText.text = ""; // Assign empty string.

        // Set shadow to inactive.
        noteShadow.gameObject.SetActive(false);

        // ADD TEMP GRAPHICS FOR PROMPT!!!
    }

    void FixedUpdate()
    {
        // Once sword reaches its stopping point, we start the note's animation and set the note sprite to the non-blurred version of the fully folded sprite.
        if (sword.transform.localPosition.x >= sword.targetX && !animActive)
        {
            animActive = true;
            spriteIndex = 3; // Non-blurred and fully folded.
        }

        // If animation is allowed to play.
        if (animActive)
        {
            // Once sword's shadow border animation begins, we begin the note's shadow border animation.
            if (sword.shadowActive && !shadowActive) // Will only play once.
            {
                InitializeShadow();
            }

            // If we're not using the main note sprite, then continue with animation.
            if (spriteIndex != 0)
            {
                // If local offset hasn't reached 0f continue to decrement.
                if (localOffsetY > 0f)
                {
                    localOffsetY -= Time.fixedDeltaTime * 3f; // Decrements offset.

                    // While offset scale is greater than 0.5f, decrement the scale for an interpolated look.
                    if (offsetScale > 0.5f)
                    {
                        offsetScale -= Time.fixedDeltaTime * 40f; // Decrement offset scaling.
                    }
                    else
                    {
                        offsetScale = 0.5f; // Keep offset scaling from going below 0.5f.
                    }
                }
                else // Now that we've reached the starting local position, we start unfolding the note while shaking it.
                {
                    localOffsetY = 0f; // Keep local offset from going below 0f.

                    theta += Time.fixedDeltaTime * 60f; // Increment theta so we can use a cosine function to shake the note.

                    // If theta is 360 degrees or higher, modulo wrap and increment sprite index.
                    if (theta >= Mathf.Deg2Rad * 360f)
                    {
                        theta %= Mathf.Deg2Rad * 360f; // Modulo wrapping.
                        spriteIndex++;
                    }

                    rotZ = Mathf.Cos(theta); // Save output of cosine function.

                    // Once we've reached the sprite index length, set the index to 0 and reset our rotation to 0f.
                    if (spriteIndex >= noteSprites.Length)
                    {
                        spriteIndex = 0;
                        rotZ = 0f;
                    }
                }
            }
        }

        // Animate moving shadow border if shadow border is allowed to move and if the sprite being used is the fully unfolded sprite.
        if (shadowActive && spriteIndex == 0)
        {
            ShadowMovement();
        }

        note.transform.localPosition = new Vector3(note.transform.localPosition.x, localStartY + (localOffsetY * offsetScale), 0f);
        note.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ * 2f);
        note.GetComponent<Image>().sprite = noteSprites[spriteIndex];
        note.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f - localOffsetY);

        if (spriteIndex == 0)
        {
            // Assign description text the current selected item's correspondering item description unless either items haven't faded in or all items have been purchased, in which case, display no text at all.
            if (!itemSelect.itemsActive || itemSelect.itemRow < 0)
            {
                if (!itemSelect.itemsActive)
                {
                    descText.text = itemSelect.items[itemSelect.itemRow, itemSelect.itemCol].GetComponent<ShopItem>().itemDescription;
                    descText.color = new Color(1f, 1f, 1f, itemSelect.itemsAlpha);
                }
                else
                {
                    descText.text = ""; // Assign empty string whenever there is no item selected (only in effect AFTER items have faded into view).
                }
            }
            else
            {
                // Assigns current selected item's description string.
                descText.text = itemSelect.items[itemSelect.itemRow, itemSelect.itemCol].GetComponent<ShopItem>().itemDescription;
                descText.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    #region Methods
    // Initialize all data needed for shadow movement to begin.
    private void InitializeShadow()
    {
        // Set shadow to active.
        noteShadow.gameObject.SetActive(true);
        
        // Update shadow sprite.
        noteShadow.GetComponent<Image>().sprite = noteSprites[1]; // Shadow border sprite.

        // Initialize shadow position data and allow for shadow movement to begin.
        shadowStartX = noteShadow.transform.localPosition.x;
        shadowStartY = noteShadow.transform.localPosition.y;

        shadowPosX = shadowStartX;
        shadowPosY = shadowStartY;

        shadowActive = true;
    }

    // Code for moving sword's shadow.
    private void ShadowMovement()
    {
        // Decrement shadow speed until it reaches 0f; also moves shadow relative to speed and time between current frame and last frame.
        if (shadowSpeed > 0f)
        {
            shadowSpeed -= shadowAcc / 2f; // Because of delay in fixed update method, we decrement in two parts; before and after movement.

            shadowPosX += shadowSpeed * Time.fixedDeltaTime; // Move horizontally.
            shadowPosY -= shadowSpeed * Time.fixedDeltaTime; // Move vertically.

            shadowSpeed -= shadowAcc / 2f;
        }
        else
        {
            shadowSpeed = 0f; // Ensure shadow speed never goes below 0f.
            shadowActive = false; // No need to run code that isn't needed anymore.
        }

        // Update shadow position.
        noteShadow.transform.localPosition = new Vector3(methods.DecimalsRounded(shadowPosX), methods.DecimalsRounded(shadowPosY), 0f);
    }
    #endregion
}
