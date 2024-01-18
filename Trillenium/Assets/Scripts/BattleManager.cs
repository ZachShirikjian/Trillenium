using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleManager : MonoBehaviour
{

    //VARIABLES//
    public int numEnemiesLeft; //the number of enemies that're left in a battle based on that in an enemy encounter prefab
    public int curTurn; //current turn, 0-2 are for party and anything more are for the enemies

    //REFERENCES//
    public GameObject battleUI; //reference to player battle UI panel; 
    public GameObject attackButton;
    // Start is called before the first frame update
    void Start()
    {
        //numEnemiesLeft = enemyEncounter.numEnemies;
        numEnemiesLeft = 2; //REPLACE THIS SO IT'S BASED ON NUMBER OF ENEMIES SPAWNED IN A SPAWNER LATER//
        curTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(numEnemiesLeft == 0)
        {
            Invoke("LoadNextCutscene", 3f);
            battleUI.SetActive(false);
        }
    }

    //If # of enemies left = 0, 
    //Load the OwlsNestCutscene after 3 seconds
    public void LoadNextCutscene()
    {
            SceneManager.LoadScene("OwlsNest");
    }

     public void NextTurn()
     {

        if(curTurn >= 2)
        {
            Debug.Log("ENEMIES' TURN");
            battleUI.SetActive(false);
        }
        else
        {
            Debug.Log("Still player's turn");
        }
     }

    //After all enemies' turns are done,
    //Reset to Player's Turn (starting with Sylvia) and re-enable the battle uI
     public void ResetTurns()
     {
        curTurn = 0;
        battleUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(attackButton);
     }
}
