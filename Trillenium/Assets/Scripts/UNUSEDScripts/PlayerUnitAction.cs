using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//


//Set current attack to Physical one by default, instantiate the attack
public class PlayerUnitAction : MonoBehaviour
{
    public GameObject physicalAttack;

    public GameObject magicAttack;
    
    private GameObject currentAttack;

    void Awake()
    {
        this.physicalAttack = Instantiate(this.physicalAttack, this.transform) as GameObject;
        this.magicAttack = Instantiate(this.magicAttack, this.transform) as GameObject;

        this.physicalAttack.GetComponent<AttackTarget>().owner = this.gameObject;
        this.magicAttack.GetComponent<AttackTarget>().owner = this.gameObject;
    
        this.currentAttack = this.physicalAttack;
    }
    
    public void Act(GameObject target)
    {
        this.currentAttack.GetComponent<AttackTarget>().Hit(target);
    }

    //Call SelectAttack() for currentUnit, change current menu, disables action menu & enables enemies menu
    //Allows players to select target after attack is selected
    public void selectAttack(bool physical)
    {
        this.currentAttack = (physical) ? this.physicalAttack : this.magicAttack;
    }
}
