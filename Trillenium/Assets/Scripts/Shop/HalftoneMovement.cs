using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalftoneMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private PublicMethods methods; // Public methods script.

    private float posX;
    private float speedX;

    private float localPosY;
    private float posY;

    private float halfWidth = 1440f; // Half of the width is 1440f.

    private float thetaY = 0f; // Used for Sin function.
    #endregion

    void Awake()
    {
        localPosY = this.transform.localPosition.y; // Keep track of starting local Y position.

        if (this.transform.localPosition.y > 0f) // If top pattern, then set corresponding values.
        {
            speedX = 30f;
            posY = localPosY; // Y position remains the same if top pattern.
        }
        else // If bottom pattern, set corresponding values.
        {
            speedX = 50f; // Speed is faster.
        }
    }
    
    void Update()
    {
        posX += Time.deltaTime * speedX; // Increment X position by speed.

        posX %= halfWidth; // Modulo wrap to keep X position from going passed half the width (results in perfect image loop).

        if (speedX == 50f) // If speed is expected bottom pattern speed, allow for bobbing moving.
        {
            thetaY += Time.deltaTime * 2f; // Increment theta.
            thetaY %= 360f; // Modulo wrap to keep theta from going passed 360f.

            posY = localPosY + (Mathf.Sin(thetaY) * 7f); // Bobbing motion is based on Sin function that takes theta as input.
        }
    }

    void LateUpdate()
    {
        // Update position.
        this.transform.localPosition = new Vector3(methods.DecimalsRounded(posX), methods.DecimalsRounded(posY), 0f);
    }
}
