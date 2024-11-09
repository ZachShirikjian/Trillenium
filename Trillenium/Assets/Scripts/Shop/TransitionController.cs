using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    #region Variables
    // We connect this script to the script where we store our public (reusable) methods so that we can use them here. We also store the scene value here!
    [SerializeField] private PublicMethods methods;

    // Controls enabled.
    [SerializeField] private GameObject sceneMain;

    private float width = 242f;
    private float height = 369f;

    private float scale;
    private float scaleBuffer = 2f; // We start off at 2f to give extra time before 0f, allowing us to have a fully black screen for just a little longer.
    private float scaleMax = 15f;
    private float scaleFactor;

    private bool enter = true;
    private bool active = true;
    #endregion

    void Awake()
    {
        sceneMain.SetActive(false);

        scale = -scaleBuffer;
        scaleFactor = methods.DecimalsRounded((scaleMax + scale) / ((scaleMax + scale) * 2.5f));

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
    }

    void FixedUpdate()
    {
        if (enter)
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
        else
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
            }
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
            //USE THIS CODE FOR WHEN THE SCENE IS BEING EXITED OUT//
            //if (!active)
            //{
            //    active = true;
            //}
        //}

        if (scale > methods.DecimalsRounded((scaleMax + scale) / 4f))
        {
            sceneMain.SetActive(true);
        }

        
        // ---------------------------------------------------------------------------------------------------------------------------------------------
        if (scale < 0)
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
        }
        else
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(methods.DecimalsRounded(width * scale), methods.DecimalsRounded(height * scale));
        }
    }
}
