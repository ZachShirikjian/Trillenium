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

    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        anim = GetComponent<Animator>();
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
        Invoke("StartNextTurn", 1f);

        randomIndex = Random.Range(0,2);
        bm.partyMembers[randomIndex].GetComponent<PlayerAction>().TakeDamage(20);
    }

    public void StartNextTurn()
    {
        Debug.Log("NEXT ENEMY TURN");
        bm.curTurn++;
        bm.currentEnemy++;
        bm.NextTurn();
    }
}
