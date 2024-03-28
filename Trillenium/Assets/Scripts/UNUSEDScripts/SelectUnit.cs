using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//

public class SelectUnit : MonoBehaviour
{

    private GameObject currentUnit;

    private GameObject actionsMenu, enemyUnitsMenu;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "TestBattle")
        {
            this.actionsMenu = GameObject.Find("PlayerActionsMenu");
            this.enemyUnitsMenu = GameObject.Find("EnemyUnitsMenu");
        }
    }

    //Sets unit as current one, enable actions menu to allow players to choose action
    public void selectCurrentUnit(GameObject unit)
    {
        this.currentUnit = unit;
        this.actionsMenu.SetActive(true);
        //this.currentUnit.GetComponent<PlayerUnitAction>().updateHUD();
    }

    //Call SelectAttack() for currentUnit, change current menu, disables action menu & enables enemies menu
    public void selectAttack(bool physical)
    {
        this.currentUnit.GetComponent<PlayerUnitAction>().selectAttack(physical);

        this.actionsMenu.SetActive(false);
        this.enemyUnitsMenu.SetActive(true);
    }

    //Disables both menus and calls act method for current unit with selected enemy as target
    public void attackEnemyTarget(GameObject target)
    {
        this.actionsMenu.SetActive(false);
        this.enemyUnitsMenu.SetActive(false);

        this.currentUnit.GetComponent<PlayerUnitAction>().Act(target);
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
