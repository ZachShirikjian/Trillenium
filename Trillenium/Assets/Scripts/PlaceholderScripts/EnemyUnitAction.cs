using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//


public class EnemyUnitAction : MonoBehaviour
{
    [SerializeField]
    private GameObject attack;

    [SerializeField]
    private string targetsTag;

    void Awake()
    {
        this.attack = Instantiate(this.attack);
        this.attack.GetComponent<AttackTarget>().owner = this.gameObject;

    }

    //Find and attack random target in the party
    GameObject findRandomTarget()
    {
        GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag(targetsTag);
        
        if(possibleTargets.Length > 0)
        {
            int targetIndex = Random.Range(0, possibleTargets.Length);
            GameObject target = possibleTargets[targetIndex];

            return target;
        }

        return null;
    }

    //Attacks the Enemy for their action
    public void Act()
    {
        GameObject target = findRandomTarget();
        this.attack.GetComponent<AttackTarget>().Hit(target);
    }
}
