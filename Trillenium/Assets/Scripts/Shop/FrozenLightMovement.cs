using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenLightMovement : MonoBehaviour
{
    #region Variables
    private float localPosX = -500f;
    private const float speedX = 2500f; // Speed at which the light moves at.
    private const float limitX = 2500f; // When to stop moving.

    public bool canMove = false; // Can I move?
    #endregion

    void Update()
    {
        // Once movement is permitted, move by amount until movement limit is reached, then disable movement permission.
        if (canMove)
        {
            if (localPosX < limitX)
            {
                localPosX += Time.deltaTime * speedX;
            }
            else
            {
                canMove = false;
            }
        }
    }

    void LateUpdate()
    {
        // Update local position.
        this.transform.localPosition = new Vector3(localPosX, 0f, 0f);
    }
}
