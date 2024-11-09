using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChillGraphicsHandler : MonoBehaviour
{
    #region Variables
    private GameObject chillBg; // Chill logo background.
    [SerializeField] private Sprite[] bgSprites = new Sprite[6]; // Array sprites for Chill logo background.
    
    private int bgSpriteIndex = 0; // Index for said sprites.
    private float timer = 0f;
    private const float timerLimit = 0.12f;
    #endregion

    void Start()
    {
        chillBg = this.transform.Find("Chill_Background").gameObject; // Assign object.
    }
    
    void FixedUpdate()
    {
        timer += Time.deltaTime; // Increment timer.

        if (timer >= timerLimit) // When timer conditions have been met, increment sprite index, then reset timer.
        {
            bgSpriteIndex++;
            bgSpriteIndex %= bgSprites.Length; // Modulo wrap.
            
            timer = 0f;
        }

        chillBg.GetComponent<Image>().sprite = bgSprites[bgSpriteIndex]; // Update sprite.
    }
}
