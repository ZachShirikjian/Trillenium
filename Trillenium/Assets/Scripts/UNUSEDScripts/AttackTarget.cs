using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//


public class AttackTarget : MonoBehaviour
{

    public GameObject owner;

    [SerializeField] private string attackAnimation;

    public bool magicAttack;

    public float manaCost;

    public float minAttackMultiplier;

    public float maxAttackMultiplier;

    public float minDefenseMultiplier;

    public float maxDefenseMultiplier;

    //Check if owner has enough mana to do attack
    //If yes, pick random attack/defense multipliers based on min and max values
    //Calculates damage based on multipliers and attack/defense of the units
    //If attack is magic attack, uses magic stat of unit, otherwise use attack stat
    public void Hit(GameObject target)
    {
        UnitStats ownerStats = this.owner.GetComponent<UnitStats>();
        UnitStats targetStats = target.GetComponent<UnitStats>();

        if(ownerStats.mana >= this.manaCost)
        {
            float attackMultiplier = (Random.value * (this.maxAttackMultiplier - this.minAttackMultiplier)) + this.minAttackMultiplier;
            float damage = (this.magicAttack) ? attackMultiplier * ownerStats.magic : attackMultiplier * ownerStats.attack;

            float defenseMultiplier = (Random.value * (this.maxDefenseMultiplier - this.minDefenseMultiplier)) + this.minDefenseMultiplier;
            damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));

            this.owner.GetComponent<Animator>().Play(this.attackAnimation);

            targetStats.receiveDamage(damage);
            ownerStats.mana -= this.manaCost;  
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
