using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    #region Variables
    private string sceneToLoad; //name of the scene we have to load 

    // We connect this script to the script where we store our public (reusable) methods so that we can use them here. We also store the scene value here!
    [SerializeField] private PublicMethods methods;

    // Controls enabled.
    [SerializeField] private GameObject toBeEnabled; // Object group that is parent to all objects that require small delay before starting their movement/animations.

    private const float width = 242f;
    private const float height = 369f;

    private float scale; // Represents scale value.
    private const float scaleBuffer = 2f; // We start off at 2f to give extra time before 0f, allowing us to have a fully black screen for just a little longer.
    private const float scaleMax = 15f; // Essentially our scale limit/bounds (maximum scale amount).
    private float scaleFactor; // How much we scale by each update.

    private bool enter = true; // Are we entering the scene? If not, then we assume we are exiting the scene.
    private bool active = true; // Is the screen transition animation active?
    #endregion

    void Awake()
    {
        toBeEnabled.SetActive(false); // By default, disable the object that we enabled later.

        scale = -scaleBuffer; // Assign buffer to scale.
        scaleFactor = methods.DecimalsRounded((scaleMax + scale) / ((scaleMax + scale) * 2.5f)); // Intialize scale factor relative to scale.

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f); // Set default scale offset to 0f.
    }

    void FixedUpdate()
    {
        if (enter) // Entering shop.
        {
            if (scale < scaleMax && active)
            {
                scale += scaleFactor;
            }
            else if (active)
            {
                scale = scaleMax;
                enter = false;
                active = false;
            }
        }
        else // Leaving shop.
        {
            if (scale > 0f && active)
            {
                scale -= scaleFactor;
            }
            else if (active)
            {
                scale = 0f;
                enter = true;
                active = false;

                // Animation is over, so now we can move on to the over world scene.
                SceneManager.LoadScene(sceneToLoad);
            }
        }

        // Enable our disabled object group to begin movement of ToBeEnabled's children objects after scale has gotten big enough.
        if (scale > methods.DecimalsRounded((scaleMax + scale) / 4f))
        {
            toBeEnabled.SetActive(true);
        }


        // When scale goes below 0f, lock scale offset (different from total scaling) to 0f.
        if (scale < 0)
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
        }
        else
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(methods.DecimalsRounded(width * scale), methods.DecimalsRounded(height * scale));
        }
    }

    #region Methods
    public void ExitShop(string sceneName)
    {
        if (!enter && !active)
        {
            active = true;
            sceneToLoad = sceneName;
        }
    }
    #endregion
}
