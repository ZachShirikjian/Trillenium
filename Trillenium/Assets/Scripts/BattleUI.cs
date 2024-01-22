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
    public int sHP;
    public int vHP;
    public int sMaxHP;
    public int vMaxHP;

    //REFERENCES//
    public BattleDialogue battleDialogue;
    public GameObject tutorialDialoguePanel; 

    public BattleManager bm;
    public GameObject attackButton;
    public GameObject talentButton;
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

    //ENEMY SLIDERS// (REPLACE WITH SLIDERS ATTACHED TO EACH SPAWNED ENEMY LATER)
    public Slider enemyHPSlider;
    private int enemyHP;
    public GameObject targetEnemy; 

    //REFERENCE TO VAHAN TALENT UI PROMPT//
    public GameObject vahanTalentUIPrompt;

    // Start is called before the first frame update
    void Start()
    {
        vahanTalentUIPrompt.SetActive(false);
        tutorialDialoguePanel.SetActive(false);
        battleDialogue.enabled = false;
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        EventSystem.current.SetSelectedGameObject(attackButton);

        //CHANGE IT SO THE HP AND TP IS BASED ON UNITSTATS OF EACH PARTY MEMBER
        // sHP = bm.partyMembers[0].GetComponent<TheUnitStats>().health;
        // vHP = bm.partyMembers[1].GetComponent<TheUnitStats>().health;
        sMaxHP = bm.partyMembers[0].GetComponent<TheUnitStats>().maxHealth;
        vMaxHP = bm.partyMembers[1].GetComponent<TheUnitStats>().maxHealth;

        sylviaHP.maxValue = sMaxHP;
        // sylviaHP.value = sHP;
//        vahanHP.maxValue = vMaxHP;
        vahanHP.maxValue = vMaxHP;
        // vahanTP.value = 0;
        // sylviaTP.value = 0;
        // sylviaTPText.text = sylviaTP.value.ToString();
        // vahanTPText.text = vahanTP.value.ToString();
        // sylviaHPText.text = sylviaHP.value.ToString();
        // vahanHPText.text = vahanHP.value.ToString();

        enemyHP = targetEnemy.GetComponent<TheUnitStats>().health;
        enemyHPSlider.maxValue = targetEnemy.GetComponent<TheUnitStats>().maxHealth;
        enemyHPSlider.value = enemyHP;
    }

    // Update is called once per frame
    void Update()
    {
        sylviaTPText.text = bm.partyMembers[0].GetComponent<TheUnitStats>().talent.ToString() + "%";
        sylviaTP.value = bm.partyMembers[0].GetComponent<TheUnitStats>().talent;
        vahanTPText.text = bm.partyMembers[1].GetComponent<TheUnitStats>().talent.ToString().ToString() + "%";
        vahanTP.value = bm.partyMembers[1].GetComponent<TheUnitStats>().talent;

        sHP = bm.partyMembers[0].GetComponent<TheUnitStats>().health;
        vHP = bm.partyMembers[1].GetComponent<TheUnitStats>().health;

        sylviaHPText.text = sHP.ToString() + "/" + sMaxHP.ToString();
        sylviaHP.value =  sHP;
        vahanHPText.text = vHP.ToString() + "/" + vMaxHP.ToString();
        vahanHP.value = vHP;

        if(targetEnemy != null)
        {
            enemyHP = targetEnemy.GetComponent<TheUnitStats>().health;
            enemyHPSlider.maxValue = targetEnemy.GetComponent<TheUnitStats>().maxHealth;
            enemyHPSlider.value = enemyHP;
        }

    }


    //Reference to Attack button in the scene
    //Switches to the Enemy Buttons so players can choose which enemies they want to attack
    //Then after that the player plays the attack animation, and after 3 seconds, it switches to the next person's turn
    public void Attack()
    {
            Debug.Log("ATTACK ENEMY");
            //Add in option for selecting enemy to target

            //CHANGE LATER so it's set to the first enemySelection UI button in the scene from the enemySpawner//
            EventSystem.current.SetSelectedGameObject(enemySelection);
    }

    //FOR PERFORMING TALENT ATTACK
    //CHECK IF CURRENT PLAYER'S TP IS >=100
    //IF YES, CALL CONFIRM TALENT
    //OTHERWISE, TALENT CAN'T BE PERFORMED

    //IF BM.SPECIALBATTLE == TRUE
    //Open DialogueBox and pause other actions to allow dialogue to play
    //Otherwise just allow for talent to happen 
    public void Talent()
    {
        if(bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent >= 100)
        {
            Debug.Log("SELECT ENEMY FOR TALENT");
            //CHANGE LATER so it's set to the first enemySelection UI button in the scene from the enemySpawner//
            bm.talentActivated = true;
            EventSystem.current.SetSelectedGameObject(enemySelection);
            //else if true, perform talent based on current character in party 
        }
        else
        {
            Debug.Log("Not Enough Talent");
        }
    }

    //Makes talent button interactable after Vahan's dialogue finishes (for now, will change later so it's on when a party member's TP is 100%)
    public void ActivateTalent()
    {
        EventSystem.current.SetSelectedGameObject(talentButton);
        talentButton.GetComponent<Button>().interactable = true;
        attackButton.GetComponent<Button>().interactable = false; //temporarily disables attack button
        tutorialDialoguePanel.SetActive(false);
    }

    //Performs an attack on the enemy selected
    //Calls the PlayerAction script on the player selected to perform the attack and deals damage to the enemy 
    public void ConfirmAttack()
    {
        //Plays attacking animation after the attack button is selected
        playerAnim = bm.partyMembers[bm.curTurn].GetComponent<Animator>();
        EventSystem.current.SetSelectedGameObject(null);
        if(bm.talentActivated == true)
        {
            //ACTIVATES VAHAN'S TALENT IF IT'S HIS TURN, CALL BM CHANGE MUSIC METHOD TO CHANGE BATTLE MUSIC THEME

            //ENABLE SCRIPT TO ALLOW FOR VAHAN TO DO HIS BUTTON MASHING//
            if(bm.curTurn == 1)
            {
                vahanTalentUIPrompt.SetActive(true);
                bm.ChangeMusic();
                bm.vahanTalentScript.enabled = true;
            }
            //playerAnim.SetTrigger("Talent");
            bm.partyMembers[bm.curTurn].GetComponent<PlayerAction>().TalentAttack(bm.enemies[bm.currentEnemy].GetComponent<EnemyAttack>());
            Invoke("StartNextTurn", 10f); //16s is duration of Vahan's placeholder Talent BGM
        }
        else if(bm.talentActivated == false)
        {
            playerAnim.SetTrigger("Attacking");
            bm.partyMembers[bm.curTurn].GetComponent<PlayerAction>().Attack(bm.enemies[bm.currentEnemy].GetComponent<EnemyAttack>());
            Invoke("StartNextTurn", 1f);
        }
    }

    //If backspace pressed while selecting an enemy
    //Return to having the Attack button be the one that's selected
    public void CancelAttack()
    {
        EventSystem.current.SetSelectedGameObject(attackButton);
    }


//Delay in between turns so enemies don't attack right away
//Increases talent by 25% at the end of each player turn
    public void StartNextTurn()
    {
        if(bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent < 100 && bm.talentActivated == false)
        {
            bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent += 25;
        }
        if(bm.talentActivated == true)
        {
            bm.talentActivated = false;
            vahanTalentUIPrompt.SetActive(false);
            bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent = 0;
            bm.ChangeMusic();

            //TEMPORARY FOR FIRST BATTLE ONLY!!!//
            //For now, after Vahan players must use Sylvia's talent, but future battles will allow attacks and talents to be performed
            EventSystem.current.SetSelectedGameObject(talentButton);
        }
        bm.curTurn++;
        bm.NextTurn();
            //If tutorial hasn't been done yet AND talent is >=100% && cur turn is less than party members length (2 for now)
            if(bm.curTurn < 2)
            {
                if(bm.tutorial == false && bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent == 100)
                {
                    Debug.Log("VAHAN TUTORIAL BEGINS");
                    battleDialogue.enabled = true;
                    tutorialDialoguePanel.SetActive(true);
                    bm.tutorial = true;
                }

                else
                {
                    EventSystem.current.SetSelectedGameObject(attackButton);
                }
            }


    }
}
