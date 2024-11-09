using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizzFadeEffect : MonoBehaviour
{
    #region Variables
    // We connect this script to the script we use for housing public methods.
    [SerializeField] private PublicMethods methods;
    
    // Alpha value.
    private float alpha = 0f;
    #endregion

    void Start()
    {
        // Initialize as fully transparent.
        this.GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
    }

    void FixedUpdate()
    {
        // As long as alpha hasn't reached 1, keep incrementing.
        if (alpha < 1f)
        {
            alpha += 0.02f;
        }
        else
        {
            alpha = 1f; // Ensure alpha never goes higher than 1.
        }

        // Update alpha value.
        this.GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, alpha);
    }
}
