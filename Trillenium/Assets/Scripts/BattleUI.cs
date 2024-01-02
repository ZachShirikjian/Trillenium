using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleUI : MonoBehaviour
{

    //REFERENCES//
    public GameObject attackButton;
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(attackButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Reference to Attack button in the scene
    //Attacks an Enemy
    public void Attack()
    {
        Debug.Log("ATTACK ENEMY");
    }
}
