using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//

//Has list of all UnitStats scripts of all Units (Player and Enemy)
//Pops first element of list, Unit acts, then puts it back into the list 
//Keeps list sorted based on unit attacking turns 
public class TurnSystem : MonoBehaviour 
{
    private List<UnitStats> unitsStats;
    [SerializeField]  private GameObject actionsMenu, enemyUnitsMenu;

    private GameObject playerParty; 
    void Start ()
    {
        playerParty = GameObject.Find("PlayerParty");
        unitsStats = new List<UnitStats>();
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("Player");
        
        foreach(GameObject playerUnit in playerUnits) 
        {
            UnitStats currentUnitStats = playerUnit.GetComponent<UnitStats>();
            currentUnitStats.calculateNextActTurn(0);
            unitsStats.Add (currentUnitStats);
        }
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemyUnit in enemyUnits) 
        {
            UnitStats currentUnitStats = enemyUnit.GetComponent<UnitStats>();
            currentUnitStats.calculateNextActTurn(0);
            unitsStats.Add(currentUnitStats);
        }
        unitsStats.Sort();
        this.actionsMenu.SetActive(false);
        this.enemyUnitsMenu.SetActive(false);
        this.nextTurn();
    }
    public void nextTurn () 
    {
        UnitStats currentUnitStats = unitsStats [0];
        unitsStats.Remove(currentUnitStats);
        if(!currentUnitStats.isDead()) 
        {
            GameObject currentUnit = currentUnitStats.gameObject;
            currentUnitStats.calculateNextActTurn(currentUnitStats.nextActTurn);
            unitsStats.Add(currentUnitStats);
            unitsStats.Sort();
            
            if(currentUnit.tag == "Player")
            {
                Debug.Log("Player unit acting");
                this.playerParty.GetComponent<SelectUnit>().selectCurrentUnit(currentUnit.gameObject);
            } 
            else 
            {
                Debug.Log("Enemy unit acting");
                currentUnit.GetComponent<EnemyUnitAction>().Act();
            }
        } 
        else 
        {
            this.nextTurn();
        }
    }
}
