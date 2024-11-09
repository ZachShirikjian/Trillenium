using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingEffect : MonoBehaviour
{
    // This script creates a slight lighting effect in the background.

    #region Variables
    [SerializeField] private GameObject lightBg; // Lighting in the background.
    [SerializeField] private GameObject floorShadow; // Shadow from the floor.
    [SerializeField] private GameObject shelfShadow; // Shadow cast from the shelf.

    private float lightAlpha = 0f;
    private float alphaDifference = 1f; // Inverse of light alpha.
    private bool fadeIn = true; // If true, increment light alpha; if false, decrement light alpha.
    #endregion

    void Update()
    {
        if (fadeIn)
        {
            lightAlpha += 0.125f * Time.deltaTime; // Increment/become more opaque.
        }
        else
        {
            lightAlpha -= 0.125f * Time.deltaTime; // Decrement/become more transparent.
        }
    }

    void LateUpdate()
    {
        if (lightAlpha >= 1f) // Don't go passed 1f.
        {
            lightAlpha = 1f;
            fadeIn = false;
        }
        else if (lightAlpha <= 0f) // Don't go below 0f.
        {
            lightAlpha = 0f;
            fadeIn = true;
        }

        alphaDifference = 1f - lightAlpha;

        // Update alpha.
        lightBg.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, lightAlpha * 0.65f);
        floorShadow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alphaDifference * 0.15f + 0.6f);
        shelfShadow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, lightAlpha * 0.5f + 0.2f);
    }
}
