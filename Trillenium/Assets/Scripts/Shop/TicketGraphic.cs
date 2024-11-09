using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketGraphic : MonoBehaviour
{
    // This script just handles the base graphics for the tickets (not text).
    
    #region Variables
    public Sprite[] sprites = new Sprite[5];
    public int spriteIndex; // Sprites index.
    #endregion

    #region Body
    void Awake()
    {
        spriteIndex = 0;
    }

    void LateUpdate()
    {
        this.GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
    }
    #endregion
}
