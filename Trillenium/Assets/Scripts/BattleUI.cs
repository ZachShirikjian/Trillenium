using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleUI : MonoBehaviour
{

    //VARIABLES//
    //List of all the party members
    public GameObject[] partyMembers = new GameObject[2];
    public int sHP;
    public int sMaxHP;
    public int vHP;
    public int vMaxHP;

    //REFERENCES//
    private BattleManager bm;
    public GameObject attackButton;
    public GameObject player;

    private Animator playerAnim;

    public GameObject enemySelection;

    //HP SLIDERS//
    public Slider sylviaHP;
    public Slider vahanHP;

    //TALENT SLIDERS//
    public Slider sylviaTP;
    public Slider vahanTP;
    public TextMeshProUGUI sylviaTPText;
    public TextMeshProUGUI vahanTPText;
    public TextMeshProUGUI sylviaHPText;
    public TextMeshProUGUI vahanHPText;
    // Start is called before the first frame update
    void Start()
    {

        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        EventSystem.current.SetSelectedGameObject(attackButton);

        sHP = partyMembers[0].GetComponent<TheUnitStats>().health;
        vHP = partyMembers[1].GetComponent<TheUnitStats>().health;

        sMaxHP = partyMembers[0].GetComponent<TheUnitStats>().maxHeath;
        vMaxHP = partyMembers[1].GetComponent<TheUnitStats>().maxHeath;

        sylviaHP.maxValue = sHP;
        sylviaHP.value = sHP;
        vahanHP.maxValue = vHP;
        vahanHP.value = vHP;
        sylviaTP.maxValue = 0;
        vahanTP.value = 0;
        sylviaTPText.text = sylviaTP.value.ToString();
        vahanTPText.text = vahanTP.value.ToString();
        sylviaHPText.text = sylviaHP.value.ToString();
        vahanHPText.text = vahanHP.value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        sylviaTPText.text = sylviaTP.value.ToString();
        vahanTPText.text = vahanTP.value.ToString();
        sylviaHPText.text = sHP.ToString() + "/" + sMaxHP.ToString();
        vahanHPText.text = vHP.ToString() + "/" + vMaxHP.ToString();
    }


    //Reference to Attack button in the scene
    //Switches to the Enemy Buttons so players can choose which enemies they want to attack
    //Then after that the player plays the attack animation, and after 3 seconds, it switches to the next person's turn
    public void Attack()
    {
            Debug.Log("ATTACK ENEMY");
            //Add in option for selecting enemy to target

            //CHANGE LATER//
            EventSystem.current.SetSelectedGameObject(enemySelection);
    }

    //Performs an attack on the enemy selected
    public void ConfirmAttack()
    {
        Debug.Log("ATTACK");
        //Plays attacking animation after the attack button is selected
        playerAnim = partyMembers[bm.curTurn].GetComponent<Animator>();
        playerAnim.SetTrigger("Attacking");

        EventSystem.current.SetSelectedGameObject(attackButton);
        bm.curTurn++;
        bm.NextTurn();
    }

    //If backspace pressed while selecting an enemy
    //Return to having the Attack button be the one that's selected
    public void CancelAttack()
    {
            EventSystem.current.SetSelectedGameObject(attackButton);
    }
}
