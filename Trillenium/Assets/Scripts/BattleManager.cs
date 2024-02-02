using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleManager : MonoBehaviour
{

    //VARIABLES//

    //DELETE THIS VAR LATER? IT'S UNUSED//
    public int numEnemiesLeft; //the number of enemies that're left in a battle based on that in an enemy encounter prefab
    
    public int curTurn; //current turn, 0-2 are for party and anything more are for the enemies
    public int currentEnemy; //current enemy attacking, goes up by 1 every time they damage a player
    public List<GameObject> enemies = new List<GameObject>();
    public GameObject[] partyMembers = new GameObject[2];
    public bool tutorial = false; //CHANGE LATER -> if tutorial hasn't been activated yet, set this to true when talent is at 100, then false afterwards
    public bool talentActivated = false;
    
    //REFERENCES//
    public BattleUI battleUIScript;
    public GameObject battleUI; //reference to player battle UI panel; 
    public GameObject attackButton;
    public GameObject talentButton;
    
    //CHARACTER PORTRAITS
    public Image sylviaPortrait;
    public Image vahanPortrait;
    public Sprite sylviaDefault;
    public Sprite sylviaAction;
    public Sprite vahanDefault;
    public Sprite vahanAction;

    public TalentScript sylviatalentAttackScript;
    public TalentScript vahantalentAttackScript;
    public TextMeshProUGUI battleStatusText;

    public GameObject transitionScreen;

    //AUDIO REFERENCES//
    public AudioManager audioManager;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    // Start is called before the first frame update
    void Start()
    {
        battleStatusText.text = "";
       // numEnemiesLeft = 2; //REPLACE THIS SO IT'S BASED ON NUMBER OF ENEMIES SPAWNED IN A SPAWNER LATER//
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

        sylviaPortrait.sprite = sylviaAction;
        vahanPortrait.sprite = vahanDefault;

        talentActivated = false;
        transitionScreen.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    //CHANGES MUSIC TO BE TALENT THEME WHEN PERFORMING A TALENT
    //MIGHT REMOVE
    public void ChangeMusic()
    {
        if(talentActivated == true)
        {
            musicSource.Stop();
            if(curTurn == 0)
            {
                musicSource.PlayOneShot(audioManager.sylviaTalentAttack);
            }
            else if(curTurn == 1) 
            {
                 musicSource.PlayOneShot(audioManager.talentAttack);
            }
        }
        else if(talentActivated == false)
        {
            musicSource.Stop();
            musicSource.PlayOneShot(audioManager.battleTheme);
        }

    }

    //LATER CHANGE TO RETURN TO THE OVERWORLD IF CURRENT BATTLE ISN'T A CUTSCENE
    public void EndBattle()
    {
            Debug.Log("BATTLE WON!");
            musicSource.Stop();
            battleUI.SetActive(false);
            battleStatusText.text = "VICTORY!";
            sfxSource.PlayOneShot(audioManager.battleWon);
            transitionScreen.SetActive(true);
            transitionScreen.GetComponent<Animator>().Play("Loading");
            Invoke("LoadNextCutscene", 3f);
    }

    //If # of enemies left = 0, 
    //Load the OwlsNestCutscene after 3 seconds
    public void LoadNextCutscene()
    {
        SceneManager.LoadScene("OwlsNest");
    }

     public void NextTurn()
     {
        //If all enemies have been defeated, End the Battle! 
        //If there's still at least 1 enemy left, continue battle turns as normal
        if(enemies.Count == 0)
        {
            EndBattle();
        }
        else if(enemies.Count > 0)
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

                        
                    //FOR SELECTION CIRCLE ON PARTY MEMBERS//
                        //If curTurn = 1 (Vahan), activate his selection circle
                        if(curTurn == 1)
                        {
                            partyMembers[0].transform.GetChild(0).gameObject.SetActive(false);
                            partyMembers[1].transform.GetChild(0).gameObject.SetActive(true);

                            //Display front-facing portrait when it's a character's turn 
                            sylviaPortrait.sprite = sylviaDefault;
                            vahanPortrait.sprite = vahanAction;
                            
                        }

                        //if curTurn = 0 (Sylvia), activate her selection circle
                        else if(curTurn == 0)
                        {
                            partyMembers[0].transform.GetChild(0).gameObject.SetActive(true);
                            partyMembers[1].transform.GetChild(0).gameObject.SetActive(false);

                            sylviaPortrait.sprite = sylviaAction;
                            vahanPortrait.sprite = vahanDefault;
                        }
                }

                //ENEMY ATTACKS PLAYER
                //For Enemy's Turn, call the Attack method for each enemy to attack the player 
                if(curTurn >= 2 && curTurn < (enemies.Count + partyMembers.Length))
                {
                    Debug.Log("ENEMIES' TURN");
                    sylviaPortrait.sprite = sylviaDefault;
                    vahanPortrait.sprite = vahanDefault;
                  //  partyMembers[0].transform.GetChild(0).gameObject.SetActive(false);
                    partyMembers[1].transform.GetChild(0).gameObject.SetActive(false);
                    battleUI.SetActive(false);
                    enemies[currentEnemy].GetComponent<EnemyAttack>().Attack();
                }

                else if(curTurn >= (enemies.Count + partyMembers.Length)) //Length of PartyMembers array (goes up to 3 when Petros joins) PLUS # of enemies currently in the scene
                {
                    Invoke("ResetTurns", 1f);
                }
        }

     }

    //RESETS BACK TO PLAYER TURN (Sylvia first)
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
        sylviaPortrait.sprite = sylviaAction;
        vahanPortrait.sprite = vahanDefault;

        //FOR FIRST BATTLE, ACTIVATE TALENT TUTORIAL AND CALL THE TALENT TUTORIAL() METHOD IN THE BATTLE UI SCRIPT
        if(partyMembers[curTurn].GetComponent<TheUnitStats>().talent >= 100)
        {
            Debug.Log("CAN PERFORM TALENT");
            if(tutorial == false)
            {
                battleUIScript.TalentTutorial();
            }
            else if(tutorial == true)
            {
                battleUIScript.ActivateTalent();
            }
        }

        else if(partyMembers[curTurn].GetComponent<TheUnitStats>().talent < 100)
        {
            battleUIScript.ResetAttacks();
        }

     }
}
