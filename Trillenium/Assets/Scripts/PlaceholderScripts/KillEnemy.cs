using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//

public class KillEnemy : MonoBehaviour
{

    //Destroys the UI button preventing players from selecting enemies which aren't there after the enemy is destroyed
    public GameObject menuItem;

    void OnDestroy()
    {
        Destroy(this.menuItem);
    }
  
}
