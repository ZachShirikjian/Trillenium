using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class EnemyAttack : MonoBehaviour
{
    //VARIABLES//
    private int randomIndex;
    //REFERENCES//
    private BattleManager bm; //ref to battle manager for getting current turn 
    private Animator anim;
    private TheUnitStats enemyUnitStats;
    private SpriteRenderer spriteR;

    //AUDIO REFERENCES//
    public AudioManager audioManager;
    public AudioSource sfxSource;

    //REFERENCE TO ITS HEALTH BAR ABOVE ENEMY (FIX LATER?)
    public GameObject enemyHP;

    //ALLOWS ENEMY TO BE SELECTED (FIX LATER?)
    public GameObject enemySelection;

    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        anim = GetComponent<Animator>();
        enemyUnitStats = GetComponent<TheUnitStats>();
        spriteR = GetComponent<SpriteRenderer>();

        sfxSource = GameObject.Find("SFXSource").GetComponent<AudioSource>();
        audioManager = sfxSource.GetComponent<AudioManager>();

        enemyHP.SetActive(true);
        enemySelection.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Gets the position of the target enemy 
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    //If currentTurn is > 2
    //Trigger enemy attack animation
    //Deal damage to a random player 
    //currentTurn++
    public void Attack()
    {
        Debug.Log("PERFORM ENEMY ATTACK");
        anim.SetTrigger("Attacking");

        randomIndex = Random.Range(0,2);

        //TEMP METHOD: DAMAGE PLAYER AFTER 0.5 SECONDS (AFTER ANIMATION IS DONE PLAYING)
        Invoke("DamagePlayer", 1f);
        Invoke("StartNextTurn", 1.5f);
    }

    //TEMP METHOD TO DAMAGE PLAYER AFTER ANIMATION FINISHES
    public void DamagePlayer()
    {
        sfxSource.PlayOneShot(audioManager.enemyAttack);

            //If player's HP is <= 0, they're dead, so enemy attacks other character
        if(bm.partyMembers[randomIndex].GetComponent<TheUnitStats>().dead == true)
        {
            Debug.Log(bm.partyMembers[randomIndex].ToString() + " is already dead!");
        }

        //If chosen unit's HP is above 0 
        //Attack them
        else if (bm.partyMembers[randomIndex].GetComponent<TheUnitStats>().dead == false)
        {
            bm.partyMembers[randomIndex].GetComponent<PlayerAction>().TakeDamage(20);
            DamagePopup.Create(bm.partyMembers[randomIndex].transform.position, 20);  
        }

    }
    public void TakeDamage(int damageAmount)
    {
        Debug.Log("ENEMY COMBATANT");
        enemyUnitStats.health -= damageAmount;


        //If enemy's health is <= 0
        //Remove enemy from enemies list
        //Destroy this gameObject
        //Lower # of enemies to destroy by 1
        if(enemyUnitStats.health <= 0)
        {
            Debug.Log("ENEMY DEFEATED");
            bm.numEnemiesLeft--;
            bm.enemies.Remove(this.gameObject);
            enemyHP.SetActive(false);
            enemySelection.SetActive(false);
            Destroy(this.gameObject);
        }

        //If enemy is still alive, show that it's taking damage by making it flash red
        else
        {
            spriteR.color = Color.red;
            Invoke("ResetColor", 0.5f);
        }

    }

    //Reset color after player has taken damage 
    public void ResetColor()
    {
        spriteR.color = Color.white;
    }


    public void StartNextTurn()
    {
        Debug.Log("NEXT ENEMY TURN");
        bm.curTurn++;
        bm.currentEnemy++;
        bm.NextTurn();
    }
}
