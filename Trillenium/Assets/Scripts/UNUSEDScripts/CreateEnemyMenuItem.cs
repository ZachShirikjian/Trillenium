using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//

//Creates a UI button for every enemy which spawns in the Battle scene
//Calculates menu position based on the # of enemies in the scene
//Instantiates button as a child of the Menu, and sets local position/scale
//Sets OnClick callback to selectEnemyTalent() and sets menu item as 1 for one which KillEnemy script will be used
public class CreateEnemyMenuItem : MonoBehaviour
{

    [SerializeField]
    private GameObject targetEnemyPrefab; //selected Enemy which players can fight against 

    [SerializeField]
    private Sprite menuItemSprite; //sprite of enemy which players can select

    [SerializeField]
    private Vector2 initialPosition, itemDimensions; //inital position of the enemy 

    [SerializeField]
    private KillEnemy killEnemyScript; //reference to script which destroys UI allowing enemy to be selected after enemy is dead
    // Start is called before the first frame update
    void Awake()
    {
        GameObject enemyUnitsMenu = GameObject.Find("EnemyUnitsMenu");

        GameObject[] existingItems = GameObject.FindGameObjectsWithTag("TargetEnemy");
        Vector2 nextPosition = new Vector2(this.initialPosition.x + (existingItems.Length * this.itemDimensions.x), this.initialPosition.y);

        //as GameObject casts the UI button Prefab as a GameObject
        GameObject targetEnemyUnit = Instantiate(this.targetEnemyPrefab, enemyUnitsMenu.transform) as GameObject;
        targetEnemyUnit.name = "Target" + this.gameObject.name;

        targetEnemyUnit.transform.localPosition = nextPosition;
        targetEnemyUnit.transform.localScale = new Vector2(0.7f,0.7f);

        targetEnemyUnit.GetComponent<Button>().onClick.AddListener(() => selectEnemyTarget());
        targetEnemyUnit.GetComponent<Image>().sprite = this.menuItemSprite;
    
        killEnemyScript.menuItem = targetEnemyUnit;
    }

    //Find PlayerParty and call its attackEnemyTarget() method
    public void selectEnemyTarget()
    {
        GameObject partyData = GameObject.Find("PlayerParty");
        partyData.GetComponent<SelectUnit>().attackEnemyTarget(this.gameObject);
    }
}
