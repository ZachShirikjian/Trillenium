using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAction : MonoBehaviour
{
    //VARIABLES//

    //REFERENCES//
    private TheUnitStats unitStats;
    void Start()
    {
        unitStats = GetComponent<TheUnitStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Code used from the Turn Based RPG Tutorial from CodeMonkey
    //https://www.youtube.com/watch?v=0QU0yV0CYT4&t=189s&ab_channel=CodeMonkey 
    //Gets the position of the target enemy 
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    //Moves to the selected enemy's position and does an attack on them 
    public void Attack(EnemyAttack targetChar)
    {
        Vector3 attackDir = (targetChar.GetPosition() - GetPosition()).normalized;
        Debug.Log("ATTACKING ENEMY");
    }

    public void TalentAttack()
    {
        Debug.Log("PERFORMING A TALENT!");
    }

    //Take Damage from Enemy (called from EnemyAttack or BossAttack scripts)
    public void TakeDamage(int damageAmount)
    {
        Debug.Log(this.gameObject.name + " IS TAKING DAMAGE");
        unitStats.health -= damageAmount;
    }


}
