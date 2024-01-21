using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System;
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
    public int currentEnemy; //current enemy attacking, goes up by 1 every time they damage a player
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject[] partyMembers = new GameObject[2];
    public bool tutorial = false; //CHANGE LATER -> if tutorial hasn't been activated yet, set this to true when talent is at 100, then false afterwards
    //REFERENCES//
    public GameObject battleUI; //reference to player battle UI panel; 
    public GameObject attackButton;

    //AUDIO REFERENCES//
    public AudioManager audioManager;
    public AudioSource sfxSource;
    // Start is called before the first frame update
    void Start()
    {
        //numEnemiesLeft = enemyEncounter.numEnemies;
        numEnemiesLeft = 2; //REPLACE THIS SO IT'S BASED ON NUMBER OF ENEMIES SPAWNED IN A SPAWNER LATER//
        curTurn = 0;
        currentEnemy = 0;
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            //Add Enemy
            enemies.Add(enemy);
        }

        //Enables selection circle for Sylvia at start of battle 
        partyMembers[0].transform.GetChild(0).gameObject.SetActive(true);
        partyMembers[1].transform.GetChild(0).gameObject.SetActive(false);
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

    //LATER CHANGE TO RETURN TO THE OVERWORLD IF CURRENT BATTLE ISN'T A CUTSCENE
    public void EndBattle()
    {
        if(numEnemiesLeft == 0)
        {
            battleUI.SetActive(false);
            sfxSource.PlayOneShot(audioManager.battleWon);
            Invoke("LoadNextCutscene", 3f);
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

        if(curTurn < 2)
        {
                 //If player's HP is 0 or less when it's their turn
                //Skip their turn and move onto the next turn
                if(partyMembers[curTurn].GetComponent<TheUnitStats>().health <= 0)
                {
                    partyMembers[curTurn].GetComponent<TheUnitStats>().dead = true;
                    curTurn++;
                }

                        //If it's Vahan's turn, display his selection circle to indicate it's his turn 
                if(curTurn == 1)
                {
                    partyMembers[0].transform.GetChild(0).gameObject.SetActive(false);
                    partyMembers[1].transform.GetChild(0).gameObject.SetActive(true);
                }
                else if(curTurn == 0)
                {
                    partyMembers[0].transform.GetChild(0).gameObject.SetActive(true);
                    partyMembers[1].transform.GetChild(0).gameObject.SetActive(false);
                }
        }
        //For Enemy's Turn, call the Attack method for each enemy to attack the player 
        if(curTurn >= 2 && curTurn < (enemies.Count + partyMembers.Length))
        {
            Debug.Log("ENEMIES' TURN");
            partyMembers[1].transform.GetChild(0).gameObject.SetActive(false);
            battleUI.SetActive(false);
            enemies[currentEnemy].GetComponent<EnemyAttack>().Attack();
        }
        else if(curTurn == (enemies.Count + partyMembers.Length)) //Length of PartyMembers array (goes up to 3 when Petros joins) PLUS # of enemies currently in the scene
        {
            Invoke("ResetTurns", 1f);
        }
     }

    //After all enemies' turns are done,
    //Reset to Player's Turn (starting with Sylvia) and re-enable the battle uI
     public void ResetTurns()
     {
        Debug.Log("ResetTurns");
        curTurn = 0;
        currentEnemy = 0;
        battleUI.SetActive(true);
       //Enables selection circle for Sylvia to indicate it's her turn
        partyMembers[0].transform.GetChild(0).gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(attackButton);
     }
}
