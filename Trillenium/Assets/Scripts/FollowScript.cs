using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    #region Variables
    public PlayerMovement player; // Connect to player so this script can access the player's script.
    public GameObject self; // Attach to self.

    public float rate = 2f; // Multiplier for interpolation time. You can play around with the values, but this might work best with 1.
    public float distance = 1f; // Distance from player.
    Vector3 targetPosition; // Position for self to move to.
    #endregion

    // On start, set targetPosition distance behind player (assumes player is facing down) and set self position to targetPosition.
    void Start()
    {
        targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + distance);
        self.transform.position = targetPosition;
    }

    void Update()
    {
        // Calculate target position so that NPC is never directly where the player is and is instead a certain distance behind the player.
        DirectionalCalc();

        // Self always interpolates toward the target position except for when lastKey is less than 0 (-1), which is at the very start when no input is given.
        if(player.lastKey == -1)
        {
             self.transform.position = targetPosition;
        }
        else if (player.lastKey >= 0f)
        {
            self.transform.position = Vector3.Lerp(self.transform.position, targetPosition, rate * Time.deltaTime);
        }
    }

    // Calculate target position so that NPC is never directly where the player is and is instead behind the player.
    void DirectionalCalc()
    {
        switch (player.lastKey)
        {
            case 0:
                // Player is facing up.
                targetPosition = new Vector3(player.transform.position.x, player.transform.position.y - distance);

                //PLAY FACING UP ANIMATION
                break;
            case 1:
                // Player is facing right.
                targetPosition = new Vector3(player.transform.position.x - distance, player.transform.position.y);

                //PLAY FACING RIGHT ANIM
                break;
            case 2:
                // Player is facing down.
                targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + distance);

                //PLAY FACING DOWN ANIM
                break;
            case 3:
                // Player is facing left.
                targetPosition = new Vector3(player.transform.position.x + distance, player.transform.position.y);

                //PLAY FACING LEFT ANIM
                break;
            case -1:
                //PLAY IDLE ANIMATION
                self.transform.position =  targetPosition;
                break;

            default: 
                self.transform.position =  targetPosition;
                break;
        }
    }
}
