using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

        //VARIABLES//
    private int randomIndex;
    //REFERENCES//
   // private Animator anim;
    private TheUnitStats enemyUnitStats;
    private SpriteRenderer spriteR;

    //REFERENCE TO ITS HEALTH BAR ABOVE ENEMY (FIX LATER?)
    //public GameObject enemyHP;


    // Start is called before the first frame update
    void Start()
    {
     //   anim = GetComponent<Animator>();
        enemyUnitStats = GetComponent<TheUnitStats>();
        spriteR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        //TEMP METHOD TO DAMAGE PLAYER AFTER ANIMATION FINISHES
    public void DamagePlayer()
    {
       // bm.partyMembers[randomIndex].GetComponent<PlayerAction>().TakeDamage(30);
       // DamagePopup.Create(bm.partyMembers[randomIndex].transform.position, 30);  
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
           // bm.numEnemiesLeft--;
           // bm.enemies.Remove(this.gameObject);
           // enemyHP.SetActive(false);
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
}
