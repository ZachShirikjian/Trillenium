using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMovement : MonoBehaviour
{
    // This script handles the sword UI's movements, rotations, speed, sprites, child objects (like the shadow), etc.

    #region Variables
    #region Reference-related Variables
    // We connect this script to the script we use for housing public methods.
    [SerializeField] private PublicMethods methods;

    [SerializeField] private ItemSelection items;

    [SerializeField] private GameObject childSprite; // Sword sprite itself.
    [SerializeField] private GameObject childShadow; // Shadow sprite.

    [SerializeField] private Sprite[] spritesheet = new Sprite[6]; // Sword spritesheet.
    #endregion

    #region Movement-based Variables
    private float posX; // X position of sword.
    private float posY; // Y position of sword.

    private float rotZ; // Z-axis local rotation of sword.

    private const float startX = -24.69f; // Starting X position.
    private const float startY = 3.94f; // Starting Y position.

    private float targetX; // Target X position/X position limit.

    private float childPosX; // This variable specifically refers to the X position of the child object that renders the sword sprite (the parent object that has this script just holds all of the related objects).

    
    private float speed; // How fast the sword moves.
    private float speedDecrement; // How much we want to decrement the speed by at the end of the movement.

    private float movThetaY; // Theta for Sin we use to represent vertical sword UI movement.
    #endregion

    #region Rotation-related Variables
    private float rotationSpeed = 45f; // Speed of rotation.
    private float rotationDecrement = 0.025f; // How much we lower rotation speed by.
    private const float rotationDecrementAcc = 0.0005f; // How much we lower rotation speed decrement by (probably could've used a better name, but this is for more smooth and precise movement when it comes to slowing down).
    private float rotationDirection = 1; // Which rotation are we rotating in?

    // Crap variable naming on my part, but picture bounds placed upon a line that represents the rotation speed; each time the speed goes down enough to reach one, we decrement our sprite for our blur effect.
    private float rotBounds; // Current rotation speed bound.
    private const float startBounds = 2f; // Rotation speed bound we started with for future referencing when the other variable changes.
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

    // Once the sword has reached target X position, we use this check to ensure the sword doesn't go back to the first phase of movement, even if the sword is moved back.
    private bool shadowCheck = false;
    #endregion

    private int spriteIndex = 0; // Spritesheet index to determine which sprite to render.

    // Buffer before the animation begins.
    private float buffer = 0.46f; // Sword anim: 1.78f; Medkit anim: 1.32f; 1.78f - 1.32f = 0.46f.
    #endregion

    #region Body
    void Start()
    {
        childShadow.GetComponent<SpriteRenderer>().sprite = null; // No shadow sprite is rendered until needed.

        // Set positions to starting position.
        targetX = startX + 21f;
        
        posX = startX;
        posY = startY;

        childPosX = 2;

        speed = 90f;

        rotZ = 0.04f; // Offets a very minor mistake I made earlier (rotation of parent object and its sprite aren't 0, but cancel each other out, meaning the sword will end its animation at a rotation of 0).

        rotBounds = startBounds; // Initialize our rotation speed bounds.

        // Take user speed and divide it by half (multiply it by two).
        speedDecrement = methods.DecimalsRounded(speed / 0.5f); // The reason this works is because we will be multiplying by fixed delta time later on, which, by default, is set to 0.02.
    }

    void FixedUpdate() // We use fixed update here because we're dealing with physics and trying to keep it consistent across differing frame rates.
    {
        if (shadowActive) // Start shadow movements once we give it the "okay" to do so.
        {
            ShadowMovement(); // STEP 5.
        }
        else if (!shadowCheck)
        {
            if (buffer > 0f) // If buffer is greater than 0, decrement until it reaches 0, then allow our sword to start moving. // STEP 0.
            {
                Buffer();
            }
            else // Buffer period has ended; now begin movement.
            {
                MoveSword(); // STEP 1.

                if (rotBounds > 0f) // Final rotation speed bound hasn't been crossed yet/blur sprites still active (jiggle movement is still going). // STEP 2.
                {
                    if (rotationDirection > 0)
                    {
                        rotZ -= rotationSpeed; // Rotate clockwise.
                    }
                    else
                    {
                        rotZ += rotationSpeed; // Rotate counter-clockwise.
                    }

                    if (this.transform.localPosition.x >= targetX) // Have we reached our position destination (sword in non-existent wall)? // STEP 3 (loop).
                    {
                        JiggleMovementHandler();
                    }
                }
                else // Jiggle movement has stopped.
                {
                    InitializeShadow(); // STEP 4.
                }
            }
        }

        if (shadowCheck) // Has the shadow border finished moving? // STEP 6.
        {
            // Increment both scale thetas and clamp them to 0-360.
            ShadowScaleAnimation(ref shadowScaleThetaX);
            ShadowScaleAnimation(ref shadowScaleThetaY);

            // Shadow stretching animation is only active when cursor is on the same row.
            if ((items.itemIndex >= 2 && items.itemIndex < 4) || items.itemIndex < 0)
            {
                // Only do if not on sword row.
                shadowScaleThetaX = 0f;
                shadowScaleThetaY = Mathf.Deg2Rad * 180f;

                // Slowly increment movement theta.
                movThetaY += Mathf.Deg2Rad * 1f;
            }
            else
            {
                // Increment movement theta.
                movThetaY += Mathf.Deg2Rad * 4f;
            }

            // Clamp movThetaY to 0 - 360.
            while (movThetaY >= Mathf.Deg2Rad * 360f)
            {
                movThetaY -= Mathf.Deg2Rad * 360f;
            }
        }

        // Kind of like our late update where we pass our position and rotation data to the actual object.
        UpdateObject(); // STEP 7.
    }
    #endregion

    #region Methods
    // If buffer is greater than 0, decrement until it reaches 0, then allow our sword to start moving.
    private void Buffer()
    {
        buffer -= Time.deltaTime;
        buffer = methods.DecimalsRounded(buffer);

        if (buffer < 0f)
        {
            buffer = 0f;
        }
    }
    
    // Moves our sword.
    private void MoveSword()
    {
        // Handles sword's horizontal movement.
        if (this.transform.localPosition.x < targetX)
        {
            posX += speed * Time.fixedDeltaTime; // Move sword with no change in speed.
        }
    }

    // Handles rotation speed bounds.
    private void RotBoundHandler()
    {
        // This method is entirely for the sword jiggle animation.
        
        if (rotBounds > (startBounds / 3f) * 2f) // Greater than 2/3 of bounds (3/3).
        {
            spriteIndex = 1;
        }
        else if (rotBounds > (startBounds / 3f)) // Greater than 1/3 of bounds and less than 3/3 of bounds (2/3).
        {
            spriteIndex = 2;
        }
        else // Not greater than 2/3 of bounds (1/3 or below).
        {
            spriteIndex = 3;
        }
    }

    // Confine rotation to 0-360 in order to simplify and easily translate data values.
    private void ConfineTo360Degrees()
    {
        if (rotZ >= 360f)
        {
            rotZ -= 360f; // This is better than setting it to 0 since it's more precise and we can't always expect the rotation to perfectly equal 360 each time.
        }
    }

    // Handles the direction the sword is going in during the jiggle animation.
    private void JiggleDirectionHandler()
    {
        if (rotZ < 360f)
        {
            if (rotZ >= rotBounds)
            {
                rotZ = rotBounds;
                rotationDirection = 1f; // Now we rotate clockwise.
            }
            else if (rotZ <= -rotBounds)
            {
                rotZ = -rotBounds;
                rotationDirection = -1f; // Now we rotate counter-clockwise.
            }
        }
    }

    // Method is called once, but modifies some variable values to prepare for jiggle movement.
    private void JiggleMovementPrep()
    {
        if (childPosX == 2) // Child's X position is changed in here, so this method is only called once after the sword hits the non-existent wall.
        {
            targetX = startX + 21f + 7f;
            posX = targetX;
            rotationSpeed = 10f;
            childSprite.transform.position = new Vector3(methods.DecimalsRounded(posX), 3.94f, 0f);
            childPosX = -5f;
        }
    }

    // Handles sword's jiggle movement in its entirety.
    private void JiggleMovementHandler()
    {
        JiggleMovementPrep();

        RotBoundHandler();

        ConfineTo360Degrees();

        JiggleDirectionHandler();

        rotBounds -= rotationDecrement;
        rotationDecrement += rotationDecrementAcc;
    }

    // Initialize all data needed for shadow movement to begin.
    private void InitializeShadow()
    {
        rotBounds = 0f; // Hard set rotation speed bounds to 0.
        spriteIndex = 4; // Hard set sprite to un-blurred sprite.

        // Update shadow sprite.
        childShadow.GetComponent<SpriteRenderer>().sprite = spritesheet[spritesheet.Length - 1]; // Shadow is consistently the sprite in the spritesheet for all spritesheets containing a shadow.

        // Initialize shadow position data and allow for shadow movement to begin.
        shadowStartX = childShadow.transform.localPosition.x;
        shadowStartY = childShadow.transform.localPosition.y;

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
        else if (!shadowCheck)
        {
            shadowSpeed = 0f; // Ensure shadow speed never goes below 0f.
            shadowActive = false; // No need to run code that isn't needed anymore.

            shadowCheck = true;
        }

        // Update shadow position.
        childShadow.transform.localPosition = new Vector3(methods.DecimalsRounded(shadowPosX), methods.DecimalsRounded(shadowPosY), 0f);
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

        x = (Mathf.Sin(x) * trigScale) * -1f; // Set negative counterpart of highest possible output to act as an offset (we start from 0, so we offset by 1 so that highest case results in default position of sword UI).

        x += Mathf.Sin(movThetaY) * trigScale; // Add Sin of our movement theta for movement and multiply it by a scale value to control how far it moves.

        return x; // Return total value.
    }

    // Kind of like our late update where we pass our position and rotation data to the actual object.
    private void UpdateObject()
    {
        float shadowScaler = 0.17f;

        if (!shadowCheck) // Object updates before shadow border finishes its movement.
        {
            // Sets position of object using our variables.
            this.transform.localPosition = new Vector3(methods.DecimalsRounded(posX), startY, 0f);

            // Sets rotation of object using our variables.
            if (this.transform.localPosition.x >= startX + 21f)
            {

                this.transform.localRotation = Quaternion.Euler(0f, 0f, methods.DecimalsRounded(rotZ));
            }
            else
            {
                this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                childSprite.transform.localRotation = Quaternion.Euler(0f, 0f, methods.DecimalsRounded(rotZ + 135f));
            }

            // Sword is only visible when buffer is done.
            if (buffer > 0f)
            {
                childSprite.GetComponent<SpriteRenderer>().sprite = null;
            }
            else
            {
                childSprite.GetComponent<SpriteRenderer>().sprite = spritesheet[spriteIndex];
            }
        }
        else // If shadow border movement has finished.
        {
            // Overwrite previous position with version that handles our late animation vertical movement loop.
            this.transform.localPosition = new Vector3(methods.DecimalsRounded(posX), methods.DecimalsRounded(posY + ReturnTrigMovement()), 0f); // Uses our trig movement.

            // Stretch animation for our shadow object.
            childShadow.transform.localScale = new Vector3(methods.DecimalsRounded(Mathf.Sin(shadowScaleThetaX) * shadowScaler + 1f), methods.DecimalsRounded(Mathf.Sin(shadowScaleThetaY) * shadowScaler + 1f), 1f);
        }
    }
    #endregion
}
