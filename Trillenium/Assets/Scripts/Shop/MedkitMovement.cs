using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedkitMovement : MonoBehaviour
{
    // This script handles the medical kit UI's movements, rotations, speed, sprites, child objects (like the shadow), etc.
    
    #region Variables
    #region Reference-related Variables
    // We connect this script to the script we use for housing public methods.
    [SerializeField] private PublicMethods methods;

    [SerializeField] private ItemSelection items;

    [SerializeField] private GameObject childSprite; // Medical kit sprite itself.
    [SerializeField] private GameObject childShadow; // Shadow sprite.

    [SerializeField] private Sprite[] spritesheet = new Sprite[6]; // Medical kit spritesheet.
    #endregion

    #region Movement-based Variables
    private float posY; // Y Position.

    private const float startX = -3.62f; // Starting X position.
    private const float startY = -19.65f; // Starting Y position.

    private float targetY; // The Y position we want to reach before we stop upward movement.

    
    private float speed; // How fast the medical kit moves.
    private float speedDecrement; // How much we want to decrement the speed by at the end of the movement.

    private float movThetaY; // Theta for Sin we use to represent vertical medical kit UI movement.
    #endregion

    #region Rotation-related Variables
    private float rotationSpeed; // Speed of rotation.
    private const float rotationSpeedStart = 24f; // Initial speed of rotation.
    private float rotationFraction; // Represents a fraction of the initial speed, which is compared to the current speed in order to determine which sprite is being rendered.
    private float rotationDecrement = 0.8f; // How much we lower rotation speed by.
    private const float rotationDecrementAcc = 0.015f; // How much we lower rotation speed decrement by (probably could've used a better name, but this is for more smooth and precise movement when it comes to slowing down).

    private float rotZ; // Current local rotation about the Z-axis.
    private int reversePhase = 0; // Essentially a phase variable that determines which way the medical kit is swinging (0-2).
    #endregion

    #region Shadow-related Variables
    private float shadowStartX; // Shadow's starting X position.
    private float shadowStartY; // Shadow's starting Y position.

    private float shadowPosX; // Shadow's current X position.
    private float shadowPosY; // Shadow's current Y position.

    private float shadowSpeed = 1.65f; // Movement speed of shadow.
    private const float shadowAcc = 0.08f; // Acceleration of shadow's movement speed.

    private bool shadowActive = false; // Can we run the shadow movement method?

    // Theta for Sin and Cos we use to represent scaling; start half a rotation from each other so that when X stretches outward, Y stretches inward and vice versa.
    private float shadowScaleThetaX = 0f;
    private float shadowScaleThetaY = Mathf.Deg2Rad * 180f;

    // Once the medical kit has reached target X position, we use this check to ensure the medical kit doesn't go back to the first phase of movement, even if the medical kit is moved back.
    private bool shadowCheck = false;
    #endregion

    private int spriteIndex = 1; // Spritesheet index to determine which sprite to render.
    #endregion

    #region Body
    void Start()
    {
        // Set positions to starting position.
        targetY = startY + 18f;

        posY = startY;

        speed = 120f;

        // Initial rotation to ensure that medical kit ends with a rotation of 0f.
        rotZ = 94.98f; // -76.29f + 168.78f + 2.49f.

        rotationSpeed = rotationSpeedStart;
        rotationFraction = rotationSpeedStart - (rotationSpeedStart / 3f);

        // Take user speed and divide it by half (multiply it by two).
        speedDecrement = methods.DecimalsRounded(speed / 0.5f); // The reason this works is because we will be multiplying by fixed delta time later on, which, by default, is set to 0.02.

        childShadow.GetComponent<SpriteRenderer>().sprite = null; // Set shadow sprite to null.
    }

    void FixedUpdate()
    {
        if (shadowActive)
        {
            ShadowMovement();
        }
        else if (!shadowCheck)
        {
            // Medical kit is thrown into the air and lands on a non-existent hook, but will then continue to spin and eventually swing to a hault after the target position has been reached.
            if (this.transform.localPosition.y >= targetY && this.transform.localPosition.y < targetY + 2f && speed < 0f)
            {
                PerfectRotation();
            }
            else // Medical kit is thrown into the air and lands on non-existent hook.
            {
                MoveMedkit();

                PerfectRotation();
            }
        }

        if (shadowCheck) // Has the shadow border finished moving? // STEP 6.
        {
            // Increment both scale thetas and clamp them to 0-360.
            ShadowScaleAnimation(ref shadowScaleThetaX);
            ShadowScaleAnimation(ref shadowScaleThetaY);

            // Shadow stretching animation is only active when cursor is on the same row.
            if (items.itemRow == 1)
            {
                // Increment movement theta.
                movThetaY += Mathf.Deg2Rad * 4f;
            }
            else
            {
                // Only do if not on medical kit row.
                shadowScaleThetaX = 0f;
                shadowScaleThetaY = Mathf.Deg2Rad * 180f;

                // Slowly increment movement theta.
                movThetaY += Mathf.Deg2Rad * 1f;
            }

            // Clamp movThetaY to 0 - 360.
            while (movThetaY >= Mathf.Deg2Rad * 360f)
            {
                movThetaY -= Mathf.Deg2Rad * 360f;
            }
        }

        // Kind of like our late update where we pass our position and rotation data to the actual object.
        UpdateObject();
    }
    #endregion

    #region Methods
    // Moves medical kit.
    private void MoveMedkit()
    {
        // Note: Looking back, I realized that I forgot to use the previously used acceleration rule, however, because the animation is quick, it's pretty inconsequential.
        
        posY += speed * Time.fixedDeltaTime; // Move medical kit relative to speed and time between current frame and last frame.

        speed -= 7f; // Decrement speed.
    }

    private void PerfectRotation()
    {
        // Note: Looking back, I realized that I forgot to use the previously used acceleration rule, however, because the animation is quick, it's pretty inconsequential.

        rotZ += rotationSpeed; // Rotate relative to the rotation speed.

        #region Rotation and Swinging
        // Handles the base rotation as well as both swinging phases of the medical kit.
        switch (reversePhase)
        {
            case 0: // Base rotation (counter-clockwise) until rotation speed is less than or equal to -9f.

                rotationSpeed -= rotationDecrement; // Decrement

                // Decrement the decrement variable (speed) after speed is below 0f.
                if (rotationSpeed < 0f)
                {
                    rotationDecrement -= rotationDecrementAcc;
                }

                // Once rotation speed has reached -9f or below, reverse phase is 1.
                if (rotationSpeed <= -9f)
                {
                    reversePhase = 1;
                }
                break;
            case 1: // Swing (clockwise) until rotation speed is greater than or equal to 3f.

                // While rotation speed is less than 3f, increase rotation speed by rotation decrement for an inverse effect where the rotation speed is increasing (rotating clockwise) instead of decreasing (rotating counter-clockwise).
                if (rotationSpeed < 3f) // Rotation speed goes from -9f to 3f during this phase.
                {
                    rotationSpeed += rotationDecrement;
                }
                else
                {
                    reversePhase = 2; // At 3f or higher, reverse phase is 2.
                }

                // // Decrement the decrement variable (not speed) after speed is below 0f so that less is being added to the rotation speed.
                if (rotationSpeed < 0f)
                {
                    rotationDecrement -= rotationDecrementAcc;
                }
                break;
            case 2: // Swing (counter-clockwise) until rotation speed simply reaches 0f.
                if (rotationSpeed > 0f) // Rotation speed goes from 3f to 0f during this phase.
                {
                    rotationSpeed -= rotationDecrement; // Decrement rotation speed (counter-clockwise again).
                    rotationDecrement -= rotationDecrementAcc * 0.75f; // Decrement the decrement variable (decrement by a little less than before since this is the final swing).
                }
                else
                {
                    rotationSpeed = 0f; // Keep rotation speed from going below 0f.
                    rotZ = 0f; // Keep rotation from going below 0f.

                    shadowPosX = childShadow.transform.localPosition.x; // Save shadow's current local X position.
                    shadowPosY = childShadow.transform.localPosition.y; // Save shadow's current local Y position.
                    shadowActive = true; // Enable shadow movement.

                    reversePhase = -1; // Reverse phase is now no longer in use.
                }
                break;
            default:
                break;
        }
        #endregion

        SpriteHandler();
    }

    // This method handles which sprite to render since they're all based on how fast the medical kit is rotating.
    private void SpriteHandler()
    {
        // As our rotation speed approaches 0f, we iterate through most of the spritesheet for a motion blur effect.
        if (rotationSpeed <= rotationFraction)
        {
            // Increment sprite index.
            if (spriteIndex < spritesheet.Length - 3)
            {
                spriteIndex++;
            }

            if (rotationFraction > 0f)
            {
                rotationFraction -= (rotationSpeedStart / 3f); // Update rotation fraction by dividing the initial rotation speed by 3f until the rotation fraction reaches 0f.
            }
            else
            {
                rotationFraction = 0f; // Ensures that the rotation fraction never goes below 0f.
                spriteIndex = spritesheet.Length - 2; // Ensures that the sprite index doesn't go out of bounds, which also means the sprite only updates on the initial rotation, not the swings.
            }
        }
    }

    // Movement medical kit's shadow.
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
        else if (!shadowCheck)
        {
            shadowSpeed = 0f; // Ensure shadow speed never goes below 0f.
            shadowActive = false; // No need to run code that isn't needed anymore.

            shadowCheck = true;
        }

        // Update shadow local position.
        childShadow.transform.localPosition = new Vector3(methods.DecimalsRounded(shadowPosX), methods.DecimalsRounded(shadowPosY), 0f);

        // Update shadow sprite.
        childShadow.GetComponent<SpriteRenderer>().sprite = spritesheet[spritesheet.Length - 1]; // Shadow is consistently the sprite in the spritesheet for all spritesheets containing a shadow.
    }

    // Increment both scale thetas and clamp them to 0-360.
    private void ShadowScaleAnimation(ref float shadowScaleTheta)
    {
        shadowScaleTheta += Mathf.Deg2Rad * 12f;

        while (shadowScaleTheta >= Mathf.Deg2Rad * 360f)
        {
            shadowScaleTheta -= Mathf.Deg2Rad * 360f;
        }
    }

    // Trigonemetry calculations for late movement.
    private float ReturnTrigMovement()
    {
        float trigScale = 0.25f;

        float x = Mathf.Deg2Rad * 360f; // Set to highest possible theta.

        x = (Mathf.Cos(x) * trigScale) * -1f; // Set negative counterpart of highest possible output to act as an offset (we start from 0, so we offset by 1 so that highest case results in default position of sword UI).

        x += Mathf.Cos(movThetaY) * trigScale; // Add Sin of our movement theta for movement and multiply it by a scale value to control how far it moves.

        return x; // Return total value.
    }

    // Kind of like our late update where we pass our position and rotation data to the actual object.
    private void UpdateObject()
    {
        float shadowScaler = 0.17f;
        
        if (!shadowCheck) // Object updates before shadow border finishes its movement.
        {
            // Sets position of object using our variables.
            this.transform.localPosition = new Vector3(startX, methods.DecimalsRounded(posY), 0f);

            // Sets rotation of object using our variables.
            this.transform.localRotation = Quaternion.Euler(0f, 0f, methods.DecimalsRounded(rotZ));

            // Update rendered sprite.
            childSprite.GetComponent<SpriteRenderer>().sprite = spritesheet[spriteIndex];
        }
        else // If shadow border movement has finished.
        {
            // Overwrite previous position with version that handles our late animation vertical movement loop.
            this.transform.localPosition = new Vector3(startX, methods.DecimalsRounded(posY + ReturnTrigMovement()), 0f); // Uses our trig movement.

            // Stretch animation for our shadow object.
            childShadow.transform.localScale = new Vector3(methods.DecimalsRounded(Mathf.Sin(shadowScaleThetaX) * shadowScaler + 1f), methods.DecimalsRounded(Mathf.Sin(shadowScaleThetaY) * shadowScaler + 1f), 1f);
        }
    }
    #endregion
}
