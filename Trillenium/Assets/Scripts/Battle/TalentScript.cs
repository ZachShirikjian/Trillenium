using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TalentScript : MonoBehaviour
{
    //REFERENCES//
    private EnemyAttack enemyScript;
    private BattleManager bm;
    private BattleUI bUI;
    public bool canPerformTalent = true; 
    public bool enemySelected = false; 
    public InputActionAsset controls;

    public InputActionReference talentAttack;
    public InputActionReference talentAttack2;
    public InputActionReference talentAttack3;

    // Start is called before the first frame update
    void Start()
    {
        //controls.UI.TalentAttack.performed += ctx => ButtonMash();

        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        bUI = GameObject.Find("Canvas").GetComponent<BattleUI>();
        enemyScript = bm.enemies[bm.currentEnemy].GetComponent<EnemyAttack>();
        canPerformTalent = true;
        //talentAttack2.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TEMP METHOD//

    //ENABLE K AND L KEYS FOR SYLVIA'S TALENT DURING TALENT ACTIVATION
    private void OnEnable()
    {
        talentAttack.action.performed += SlashAttack;
        talentAttack3.action.performed += SlashAttack;
        talentAttack2.action.performed += SlashAttack;
        talentAttack2.action.Enable();
        talentAttack3.action.Enable();
    }

    private void OnDisable()
    {
        Debug.Log("DISABLE INPUT");
        talentAttack2.action.performed -= SlashAttack;
        talentAttack2.action.Disable();
        talentAttack3.action.performed -= SlashAttack;
        talentAttack3.action.Disable();
    }

    //SYLVIA'S TALENT --> PERFORM LARGE SLASH ATTACKS IF J K AND L ARE PRESSED @ SAME TIME
    //TEMOPORARY, WILL BE CHANGED LATER
    public void SlashAttack(InputAction.CallbackContext context)
    {
        if(canPerformTalent == true && enemySelected == true)
        {
            if(talentAttack.action.triggered && talentAttack2.action.triggered && talentAttack3.action.triggered)
            {
                    Debug.Log("Talent Performed!!!");
                    enemyScript.TakeDamage(50);
                    bm.talentPerformed = true;
                    bUI.StartNextTurn();
                    canPerformTalent = false;
            }
        }

    }

    //SYLVIA'S TALENT//
    public void CommandInputs()
    {
        //If correct series of buttons are pressed
        //Deal 50 damage to the enemy 
        enemyScript = bm.enemies[bm.currentEnemy].GetComponent<EnemyAttack>();
    }

    //VAHAN'S TALENT//
    public void ButtonMash()
    {
        if(talentAttack.action.triggered && enemySelected == true)
        {
            enemyScript = bm.enemies[bm.currentEnemy].GetComponent<EnemyAttack>();
            enemyScript.TakeDamage(5);
            bm.talentPerformed = true;
        }

    }

    //IF CURTURN = 0 (SYLVIA)
    //PERFORM HER TALENT 
    //ELSE IF CURTURN = 1 (VAHAN) 
    //PERFORM HIS TALENT AS NORMAL (BUTTON MASH UNTIL ENEMY IS DEAD)
    public void PerformTalent(InputAction.CallbackContext context)
    {
        if(canPerformTalent == true && bm.talentActivated == true && bm.enemies[bm.currentEnemy] != null && enemySelected == true)
        {
            if(bm.curTurn == 0)
            {
                CommandInputs();
                OnEnable();
            }
            else if(bm.curTurn == 1)
            {
                ButtonMash();
                OnDisable(); //Disable Sylvia's Talent 2 and 3 attack buttons so only J works for Vahan's talent
            }
        }
    }
}
