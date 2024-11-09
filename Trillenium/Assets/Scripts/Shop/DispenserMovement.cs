using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserMovement : MonoBehaviour
{
    // This script handles the movement of the dispensers.

    #region Variables
    #region Reference-related Variables
    // We connect this script to the script we use for housing public methods.
    [SerializeField] private PublicMethods methods;

    // Let's link this script to the script we use for moving our tickets so that we can tell it when to start moving.
    [SerializeField] private TicketMovement ticket;
    #endregion

    #region Movement-based Variables.
    private float posY; // Y Position of group of tickets.
    private float startY; // The Y position we want our group of tickets
    private float distanceLimit = 1f; // How far of a distance can the tickets travel before stopping?

    private float speed = 30f; // How fast the dispenser moves upward.
    private float speedDecrement; // How much we want to decrement the speed by at the end of the movement.
    #endregion

    // The user can choose to add a buffer at the beginning of the animation; used for delaying movements between multiple dispensers.
    [SerializeField] private float buffer; // (0-2).

    public bool mainMovDone; // Are we done with the main dispenser movement?
    #endregion

    #region Body
    void Start()
    {
        // Initialize starting Y position.
        startY = this.transform.position.y;

        // Set Y position to starting Y position.
        posY = startY;

        // Take user speed and divide it by half (multiply it by two).
        speedDecrement = methods.DecimalsRounded(speed / 0.5f); // The reason this works is because we will be multiplying by fixed delta time later on, which, by default, is set to 0.02.

        buffer *= 6f; // We do this to make setting the buffer easier for the user (again, 0-2).

        mainMovDone = false; // Main movement is not done by default.
    }

    void FixedUpdate()
    {
        if (!mainMovDone) // If main movement is not done, continue main movement.
        {
            // If buffer is greater than 0, decrement until it reaches 0, then allow our dispenser to start moving with a custom method.
            // Note: I definitely could've improved the versatility of the buffer by using delta time, however, it works well like this and would be pointless to update.
            if (buffer > 0)
            {
                buffer -= 1;
            }
            else
            {
                MoveDispenser();
            }

            // Sets position of object using our variables.
            this.transform.position = new Vector3(this.transform.position.x, methods.DecimalsRounded(posY), 0f);
        }
    }
    #endregion

    #region Methods
    // This method is responsible for moving our dispensers and controlling their speed.
    void MoveDispenser()
    {
        // If we've reached the distance limit, then decrement speed until we reach 0 for a smooth interpolated effect.
        if (this.transform.position.y >= startY + distanceLimit)
        {
            // Until our speed reaches 0 or lower, let's decrement our speed.
            if (speed > 0)
            {
                // Because of math and how framerate works in relation, we need to split up the deacceleration of our speed by half and apply it both before and after our position update.
                // This rule only applies to speeds that change over time.
                // The fixed update method technically has a 1-frame delay, which I believe is why the order of these three lines is set up this way.
                speed -= (speedDecrement / 2) * Time.fixedDeltaTime; // Lower our speed.
                posY += speed * Time.fixedDeltaTime; // Move tickets.
                speed -= (speedDecrement / 2) * Time.fixedDeltaTime; // Lower our speed again with deacceleration rule.
            }
            else
            {
                // Now that the dispenser is no longer moving, the ticket animation is allowed to play out.
                if (!ticket.ticketsMoving)
                {
                    mainMovDone = true; // Main movement is now done.
                    ticket.ticketsMoving = true; // With the main movemnt just finishing up, we can now begin the ticket movement.
                }
            }

            // Now that we've done the calculation, we check if speed has accidentally gone below 0 before feeding value to Y position.
            if (speed < 0)
            {
                speed = 0;
            }
        }
        else
        {
            posY += speed * Time.fixedDeltaTime; // Move dispenser with no change in speed.
        }
    }
    #endregion
}
