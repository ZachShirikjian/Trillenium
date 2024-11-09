using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BannerMovement : MonoBehaviour
{
    // Moves base of the banner into place, then extends borders outward while fading the Chill Topic logo into view.

    #region Variables
    #region Reference-related Variables
    // We connect this script to the script we use for housing public methods.
    [SerializeField] private PublicMethods methods;

    // We connect this script to the main banner graphic so we can have control over it.
    [SerializeField] private GameObject bannerMain;

    [SerializeField] private GameObject[] bannerObjects = new GameObject[4];

    [SerializeField] private Sprite[] spritesheet = new Sprite[4];
    private int spriteIndex = 0;

    // Related to masked variant of our logo object.
    [SerializeField] private GameObject maskedLogoObject; // Main object (parent).
    private GameObject[] maskedLogoComponents = new GameObject[2]; // 0 is the mask, 1 is the outline of the logo.
    private GameObject[] frozenLogoComponents = new GameObject[3]; // 0 is the mask, 1 is the frozen variant of the logo, and 2 is the outline of the logo.
    private GameObject logoBg; // Background pattern for the logo.
    private GameObject whiteLight; // Light for logo transitioning.
    private GameObject topic; // Topic part of the logo.

    [SerializeField] private FrozenLightMovement frozenLight; // Script for light that runs across the frozen logo.
    #endregion

    #region Movement-based Variables
    private float posX;
    private float posY;

    private float startX;
    private float startY;

    private float speed = 0.5f; // Speed.
    private float speedDecrement = 0.0075f; // Deacceleration.

    private float maskedPosX; // X position of masked variant of Chill Topic logo.
    private float maskedPosY; // Y position of masked variant of Chill Topic logo.
    #endregion

    #region Border-related Variables
    private float borderPosX = 0;
    private float borderPosY = 0;

    private float borderSpeed = 1.65f; // Border speed.
    private const float borderDec = 0.08f; // Acceleration.

    private bool borderActive = false; // Can we run the border movement method?
    #endregion

    // Logo alpha value.
    private float logoColor = 0f;

    // The logo color will affect different componenets in different ways, depending on what phase the fade phase is on.
    private int fadePhase = 0;

    // Buffer before the animation plays.
    private float buffer = 0.1f;
    #endregion

    #region Body
    void Awake()
    {
        // Assign objects before start.
        InitialObjectAssignment();
    }
    
    void Start()
    {
        // Initialize positions.
        startX = -28.37f; // -11.45f - 16.92f.
        startY = 6.28f;

        posX = startX;
        posY = startY;

        // Set sprite of all four banner objects to null.
        for (int i = 0; i < 4; i++)
        {
            bannerObjects[i].GetComponent<SpriteRenderer>().sprite = null;
        }

        // Set both color values and alpha value of logo to black and transparent respectively.
        for (int i = 0; i < frozenLogoComponents.Length; i++)
        {
            frozenLogoComponents[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        // Sets alpha value of each component variable corresponding to the normal Chill logo to 0f.
        for (int i = 0; i < maskedLogoComponents.Length; i++)
        {
            maskedLogoComponents[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        // Color and alpha values for the other Chill Topic logo components.
        logoBg.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Start fully transparent.
        whiteLight.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Start fully transparent.

        topic.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Start fully transparent.
    }

    void FixedUpdate()
    {
        if (buffer > 0f) // Only perform function after buffer period has concluded.
        {
            Buffer();
        }
        else
        {
            if (borderActive)
            {
                BorderMovement();
                LogoFadeIn();
            }
            else
            {
                BaseMovement();
            }
        }

        maskedPosX = methods.DecimalsRounded(posX - 2.91f);
        maskedPosY = methods.DecimalsRounded(posY + 1.26f);

        // Assign values to respective objects and their components.
        AssignObjectValues();
    }
    #endregion

    #region Methods
    // Meant to assign objects in Awake() method.
    private void InitialObjectAssignment()
    {
        // Components for normal masked Chill logo.
        maskedLogoComponents[0] = maskedLogoObject.transform.Find("Chill_Mask").gameObject; // Normal Chill logo mask.
        maskedLogoComponents[1] = maskedLogoObject.transform.Find("Chill_Outline").gameObject; // Outline of normal Chill logo.

        // Most of the components for the frozen masked Chill logo.
        frozenLogoComponents[0] = maskedLogoObject.transform.Find("Chill_Frozen_Mask").gameObject; // Frozen Chill logo mask.
        frozenLogoComponents[1] = maskedLogoObject.transform.Find("Chill_Frozen_Mask/Logo_Frozen/Chill_Frozen").gameObject; // Frozen Chill logo.
        frozenLogoComponents[2] = maskedLogoObject.transform.Find("Chill_Frozen_Outline").gameObject; // Outline of frozen Chill logo.

        // These components won't fit in well with the for loops we'll be using for the previous components.
        logoBg = maskedLogoObject.transform.Find("Chill_Background").gameObject; // Background pattern that appears below the Chill Topic logo.
        whiteLight = maskedLogoObject.transform.Find("Chill_WhiteLight").gameObject; // White light that flashes to hide the transition from the frozen Chill logo to the normal one.

        topic = maskedLogoObject.transform.Find("Logo_Topic").gameObject; // The Topic part of the Chill Topic logo.
    }
    
    // Responsible for moving the main banner into place.
    private void BaseMovement()
    {
        posX += speed; // Move horizontally, relative to speed.

        // Decrement speed until speed reaches 0.
        if (speed > 0f)
        {
            speed -= speedDecrement;
        }
        else
        {
            speed = 0f;
            borderActive = true; // Give permission to run border movement method.
        }

        if (this.transform.position.x > -12f) // Revert sprite to non-blurred version.
        {
            spriteIndex = 1;
        }
    }

    // Responsible for moving the banner borders outward.
    private void BorderMovement()
    {
        // Decrement speed until speed reaches 0.
        if (borderSpeed > 0f)
        {
            borderSpeed -= borderDec / 2f; // Because of delay in fixed update method, we decrement in two parts; before and after movement.

            borderPosX += borderSpeed * Time.fixedDeltaTime; // Movement; I don't think I needed to use fixed delta time here, but it works and the script was made with it in mind, so I decided to keep it.

            borderSpeed -= borderDec / 2f;
        }
        else
        {
            borderSpeed = 0f; // Never let speed go below 0.
        } 

        // Global position is based on world space while local position is based on parent objects' position in world space (is relative).
        bannerObjects[0].transform.localPosition = new Vector3(methods.DecimalsRounded(-borderPosX), methods.DecimalsRounded(borderPosY), 0f); // Pink, top.
        bannerObjects[1].transform.localPosition = new Vector3(methods.DecimalsRounded(borderPosX), methods.DecimalsRounded(borderPosY), 0f); // Pink, bottom.

        bannerObjects[2].transform.localPosition = new Vector3(methods.DecimalsRounded(-borderPosX * 2), methods.DecimalsRounded(borderPosY), 0f); // Magenta, top.
        bannerObjects[3].transform.localPosition = new Vector3(methods.DecimalsRounded(borderPosX * 2), methods.DecimalsRounded(borderPosY), 0f); // Magenta, bottom.

        // If border sprites are allowed to be rendered, then render border sprites.
        if (borderActive)
        {
            // I manually updated them all because I felt like a for loop would be a bit much here.
            bannerObjects[0].GetComponent<SpriteRenderer>().sprite = spritesheet[2];
            bannerObjects[1].GetComponent<SpriteRenderer>().sprite = spritesheet[2];
            bannerObjects[2].GetComponent<SpriteRenderer>().sprite = spritesheet[3];
            bannerObjects[3].GetComponent<SpriteRenderer>().sprite = spritesheet[3];
        }
    }

    // Responsible for the fade effects used for the Chill Topic logo.
    private void LogoFadeIn()
    {
        // If logo isn't full opaque, make more opaque.
        if (logoColor < 1f)
        {
            logoColor += 0.05f;
        }
        else // It should never be possible for the logo color to go above 1f, but we add this in as a just-in-case failsafe.
        {
            logoColor = 1f;

            if (fadePhase == 0) // If fade phase is 0, we prepare for fade phase 1 and increment to move on to the next fade phase.
            {
                frozenLight.canMove = true;
                fadePhase++;
            }
        }
    }

    // Just a simple buffer for timing-related purposes.
    private void Buffer()
    {
        // Simple decrement and rounding.
        buffer -= Time.deltaTime;
        buffer = methods.DecimalsRounded(buffer);
    }

    // Assign values to respective objects and their components.
    private void AssignObjectValues()
    {
        // Simpler way to re-use this reverse alpha value.
        float alphaDifference = methods.DecimalsRounded(1f - logoColor);
        
        // Update banner position.
        this.transform.position = new Vector3(methods.DecimalsRounded(posX), methods.DecimalsRounded(posY), 0f);

        // Update banner sprite (used for motion blur).
        bannerMain.GetComponent<SpriteRenderer>().sprite = spritesheet[spriteIndex];

        // Update normal logo position.
        maskedLogoObject.transform.position = new Vector3(maskedPosX, maskedPosY, 0f);

        // Assigns values based on current fade phase.
        FadePhaseAssignment(alphaDifference); // Carry alpha difference over.
    }

    // Assigns values to objects and their components based on current fade phase.
    private void FadePhaseAssignment(float alphaDifference) // To carry the alpha difference over.
    {
        switch (fadePhase)
        {
            case 0: // Frozen Chill logo fade in, alongside Topic logo and logo background.
                // Updates the alpha for the logo sprite and logo background pattern.
                for (int i = 1; i < frozenLogoComponents.Length; i++) // Skip mask and make it full opaque when the rest is as to avoid a weird white-fade look (it looks like the masking works best when fully opaque).
                {
                    frozenLogoComponents[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, logoColor);
                }

                logoBg.GetComponent<Image>().color = new Color(1f, 1f, 1f, logoColor); // Logo background alpha update.
                topic.GetComponent<Image>().color = new Color(1f, 1f, 1f, logoColor); // Topic logo alpha udpate.
                break;
            case 1: // Hard set frozen Chill logo mask's alpha value to 1f and increment fade phase once frozen light is done moving.
                frozenLogoComponents[0].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // Mask is now fully opaque.

                if (!frozenLight.canMove)
                {
                    logoColor = 0f; // Reset logo color.
                    fadePhase++; // Increment fade phase.
                }
                break;
            case 2: // Fade out frozen Chill while fading in normal Chill logo, which is covered up by the white light fading in on top of that.
                // Updates the alpha for the frozen Chill logo objects (fade out).
                for (int i = 0; i < frozenLogoComponents.Length; i++)
                {
                    frozenLogoComponents[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, alphaDifference);
                }

                // Updates the alpha for the normal Chill logo objects (fade in).
                for (int i = 0; i < maskedLogoComponents.Length; i++)
                {
                    maskedLogoComponents[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, logoColor);
                }

                // Updates the alpha for the white light object (fade in).
                whiteLight.GetComponent<Image>().color = new Color(1f, 1f, 1f, logoColor);

                // Now that we've reached the highest possible value, we prepare for the next fade phase and increment.
                if (logoColor == 1f)
                {
                    logoColor = 0f; // Reset logo color.
                    fadePhase++; // Increment fade phase.
                }
                break;
            case 3: // Fade white light out.
                // Updates the alpha for the white light object (fade out).
                whiteLight.GetComponent<Image>().color = new Color(1f, 1f, 1f, alphaDifference);

                // Once the white light has fully faded out, increment the fade phase.
                if (alphaDifference == 0f)
                {
                    fadePhase++;
                }
                break;
            case 4: // Destroy objects that are know longer needed for optimal performance.
                // Destroy all objects in array.
                for (int i = 0; i < frozenLogoComponents.Length; i++)
                {
                    Destroy(frozenLogoComponents[i].gameObject);
                }

                Destroy(whiteLight.gameObject);

                fadePhase++; // Increment so nothing else is on loop.
                break;
            default:
                break;
        }
    }
    #endregion
}
