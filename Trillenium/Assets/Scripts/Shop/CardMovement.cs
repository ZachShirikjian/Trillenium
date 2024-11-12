using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // We use TMPro in order to modify the text on the tickets.

public class CardMovement : MonoBehaviour
{
    #region Variables
    #region Reference-related Variables
    // We connect this script to the script we use for housing public methods.
    [SerializeField] private PublicMethods methods;

    // Connects this script to money data, however, it is currently connected to a temporary one that generates random numbers.
    [SerializeField] private DataExample moneyPull;

    private string moneyUpdated; // String that updates value of money data with extra zeros, depending on how many digits the original numeric value is.

    [SerializeField] private GameObject childSprite;
    [SerializeField] private GameObject childShadow;
    [SerializeField] private GameObject childText;

    [SerializeField] private Sprite[] spritesheet = new Sprite[2];
    #endregion

    #region Movement-based Variables
    // Position.
    private float posX; // X Position of UI element.
    private float posY; // Y Position of UI element.

    private float rotZ;

    private float startX; // The X position we want our UI element.
    private float startY; // The Y position we want our UI element.

    private float targetX;
    private float targetY;

    private float speed; // How fast the UI element moves.
    private float speedDecrement; // How much we want to decrement the speed by at the end of the movement.
    #endregion

    #region Rotation-related Variables
    private float rotationSpeed = 16f;
    private float rotationDecrement = 0.8f;
    private float rotationDecrementAcc = 0.03f;
    #endregion

    #region Shadow Variables.
    private float shadowStartX;
    private float shadowStartY;

    private float shadowPosX;
    private float shadowPosY;

    private float shadowSpeed = 1.65f; // Speed.
    private const float shadowAcc = 0.08f; // Acceleration.

    public bool shadowActive = false; // Can we run the shadow movement method (public because ItemSelection relies on its value)?

    // Once the card has reached target X position, we use this check to ensure the card doesn't go back to the first phase of movement, even if the card is moved back.
    private bool shadowCheck = false;
    #endregion

    // Buffer before the animation begins.
    private float buffer = 1.22f; // Medkit anim: 1.32f; Card anim: 0.58f; 1.32f - 0.58f = 0.74f.
    #endregion

    #region Body
    void Start()
    {
        // Initialize stuff like speed, positions, and rotations.
        startX = 27.6f; // 15f + 12.6f.
        startY = 8f;

        // Set positions to starting position.
        targetX = methods.DecimalsRounded(startX - 12.6f);

        posX = startX;
        posY = startY;

        speed = 60f;

        rotZ = -185.4f;

        // Take user speed and divide it by half (multiply it by two).
        speedDecrement = methods.DecimalsRounded(speed / 0.5f); // The reason this works is because we will be multiplying by fixed delta time later on, which, by default, is set to 0.02.

        childShadow.GetComponent<SpriteRenderer>().sprite = null; // Set sprite to null for now.

        MoneyStringUpdate(); // Update text on card.
    }

    void FixedUpdate()
    {
        if (shadowActive)
        {
            ShadowMovement();
        }
        else if (!shadowCheck)
        {
            if (speed > 0f)
            {
                // If buffer is greater than 0, decrement until it reaches 0, then allow our dispenser to start moving with a custom method.
                if (buffer > 0)
                {
                    Buffer();
                }
                else
                {
                    MoveCard();

                    PerfectRotation();
                }
            }
            else
            {
                speed = 0f;

                PerfectRotation();
            }
        }

        if (!shadowCheck) // It saves processing power if we don't update movement values when we know that the movement values aren't being updated.
        {
            MassUpdate(); // Updates a bunch of stuff.
        }
    }
    #endregion

    #region Methods
    // Moves our card.
    private void MoveCard()
    {
        posX -= speed * Time.fixedDeltaTime; // Move horizontally, relative to speed.

        // Decrement speed until speed reaches 0.
        if (speed > 0f)
        {
            speed -= 3f;
        }
        else
        {
            speed = 0f;
        }
    }

    // This method is responsible for rotating the card.
    private void PerfectRotation()
    {
        rotZ += rotationSpeed; // Rotate relative to rotation speed.

        // Decrement rotation speed until it reaches 0.
        if (rotationSpeed > 0f)
        {
            rotationSpeed -= rotationDecrement;

            if (rotationSpeed < 10f && rotationSpeed > 0f)
            {
                rotationDecrement -= rotationDecrementAcc;
            }

            if (rotationDecrement < 0f)
            {
                rotationDecrement = 0f;
            }
        }
        else
        {
            rotationSpeed = 0f; // Never let rotation speed go below 0.

            // Now that we're done rotating the card, the card is in place and we can enable the card's shadow to begin its movement.
            shadowPosX = childShadow.transform.position.x; // Save shadow's current X position.
            shadowPosY = childShadow.transform.position.y; // Save shadow's current Y position.
            shadowActive = true; // Enable shadow movement.
        }
    }

    // Code for moving the card's shadow into place.
    private void ShadowMovement()
    {
        // Decrement shadow speed until it reaches 0; shadow movement is also included.
        if (shadowSpeed > 0f)
        {
            shadowSpeed -= shadowAcc / 2f; // Because of delay in fixed update method, we decrement in two parts; before and after movement.

            shadowPosX += shadowSpeed * Time.fixedDeltaTime; // Horizontal movement.
            shadowPosY -= shadowSpeed * Time.fixedDeltaTime; // Vertical movement.

            shadowSpeed -= shadowAcc / 2f;
        }
        else if (!shadowCheck)
        {
            shadowSpeed = 0f; // Ensure shadow speed never goes below 0f.
            shadowActive = false; // No need to run code that isn't needed anymore.

            rotZ = this.transform.localRotation.z; // The purpose of rotZ changes here by saving the current local rotation.

            shadowCheck = true;
        }

        // Update shadow position.
        childShadow.transform.position = new Vector3(methods.DecimalsRounded(shadowPosX), methods.DecimalsRounded(shadowPosY), 0f);
    }

    private void MoneyStringUpdate()
    {
        moneyUpdated = moneyPull.moneyData.ToString(); // Save money data to variable as a string.

        /*
        // We compare money data to certain values to determine how many digits it has, then add zeros if there are less than four digits.
        if (moneyPull.moneyData < 1000) // Value is less than four digits.
        {
            moneyUpdated = "0" + moneyUpdated; // Add a zero.

            if (moneyPull.moneyData < 100) // Value is less than three digits.
            {
                moneyUpdated = "0" + moneyUpdated; // Add another zero.

                if (moneyPull.moneyData < 10) // Value is less than two digits.
                {
                    moneyUpdated = "0" + moneyUpdated; // Add one last zero.
                }
            }
        }
        */

        // Set money text to the updated money string.
        childText.GetComponent<TextMeshProUGUI>().text = moneyUpdated; // DON'T FORGET, THIS SCRIPT MUST USE TMPro IN ORDER FOR THIS LINE TO WORK (see top of script)!!!
    }

    // This method updates a bunch of stuff, including position, rotation, sprites, text, etc.
    private void MassUpdate()
    {
        // Sets position of object using our variables.
        this.transform.position = new Vector3(methods.DecimalsRounded(posX), methods.DecimalsRounded(posY), 0f);

        // Local rotation, which is relative to parent objects' rotations.
        this.transform.localRotation = Quaternion.Euler(0f, 0f, methods.DecimalsRounded(rotZ));

        // Sets main card object's sprite to the main card sprite.
        childSprite.GetComponent<SpriteRenderer>().sprite = spritesheet[0]; // Is first sprite in the spritesheet.

        // If shadow is active, then set card shadow object's sprite to the card shadow sprite; if shadow is not active, then set to null.
        if (shadowActive)
        {
            childShadow.GetComponent<SpriteRenderer>().sprite = spritesheet[spritesheet.Length - 1]; // I always have the shadow sprite as the last sprite in the spritesheet.
        }
        else
        {
            childShadow.GetComponent<SpriteRenderer>().sprite = null;
        }

        MoneyStringUpdate(); // Update text on card.
    }

    // Just a simple buffer for timing-related purposes.
    private void Buffer()
    {
        // Simple decrement and rounding.
        buffer -= Time.deltaTime;
        buffer = methods.DecimalsRounded(buffer);
    }
    #endregion
}
