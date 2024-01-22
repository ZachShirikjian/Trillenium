using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VahanTalent : MonoBehaviour
{
    //REFERENCES//
    private EnemyAttack enemyScript;
    private BattleManager bm;
    // Start is called before the first frame update
    void Start()
    {
        //controls.UI.TalentAttack.performed += ctx => ButtonMash();

        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        enemyScript = bm.enemies[bm.currentEnemy].GetComponent<EnemyAttack>();
    }

    //private void OnEnable() => controls.UI.Enable();
    //private void OnDisable() => controls.UI.Disable();

    // Update is called once per frame
    void Update()
    {
        
    }

    //Performs button mash on the enemy while a talent is activated 
    public void ButtonMash(InputAction.CallbackContext context)
    {
        if(bm.talentActivated == true)
        {
            Debug.Log("Dealing Damage");
            enemyScript.TakeDamage(1);
        }
        // enemyScript.TakeDamage(1);
    }
}
