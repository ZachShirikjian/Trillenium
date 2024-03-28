using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovement : MonoBehaviour
{
    #region Variables
    public GameObject player; // Attach to player.

    public int lastKey; // Keeps track of the way the player is facing. Directions are numbered 0-3 and are based on clockwise rotation (up, right, down, left) for easier readability.
    #endregion

    void Start()
    {
        lastKey = -1; // Clean slate for lastKey so that it doesn't complicate anything with start-up.
    }

    void Update()
    {
        // Basic horizontal movement.
        if (Input.GetKey(KeyCode.D)) // Move right.
        {
            player.transform.position = new Vector2(player.transform.position.x + 1 * Time.deltaTime, player.transform.position.y);
            lastKey = 1; // Player is facing to the right.
        }
        else if (Input.GetKey(KeyCode.A)) // Move left.
        {
            player.transform.position = new Vector2(player.transform.position.x - 1 * Time.deltaTime, player.transform.position.y);
            lastKey = 3; // Player is facing to the left.
        }

        // Basic vertical movement.
        if (Input.GetKey(KeyCode.W)) // Move up.
        {
            player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 1 * Time.deltaTime);
            lastKey = 0; // Player is facing upward.
        }
        else if (Input.GetKey(KeyCode.S)) // Move down.
        {
            player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - 1 * Time.deltaTime);
            lastKey = 2;  // Player is facing downward.
        }
    }
}
