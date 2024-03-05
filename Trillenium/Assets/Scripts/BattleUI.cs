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
    public GameObject sylviaTalentUIPrompt;

    // Start is called before the first frame update
    void Start()
    {
        sylviaTalentUIPrompt.SetActive(false);
        vahanTalentUIPrompt.SetActive(false);
        tutorialDialoguePanel.SetActive(false);
        battleDialogue.enabled = false;
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        EventSystem.current.SetSelectedGameObject(attackButton);

        //CHANGE IT SO THE HP AND TP IS BASED ON UNITSTATS OF EACH PARTY MEMBER
        sMaxHP = bm.partyMembers[0].GetComponent<TheUnitStats>().maxHealth;
        vMaxHP = bm.partyMembers[1].GetComponent<TheUnitStats>().maxHealth;

        sylviaHP.maxValue = sMaxHP;
        vahanHP.maxValue = vMaxHP;

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

        //sylviaHPText.text = sHP.ToString() + "/" + sMaxHP.ToString();
        sylviaHPText.text = sHP.ToString();
        sylviaHP.value =  sHP;
        vahanHPText.text = vHP.ToString();
        //vahanHPText.text = vHP.ToString() + "/" + vMaxHP.ToString();
        vahanHP.value = vHP;

        if(targetEnemy != null)
        {
            enemyHP = targetEnemy.GetComponent<TheUnitStats>().health;
            enemyHPSlider.maxValue = targetEnemy.GetComponent<TheUnitStats>().maxHealth;
            enemyHPSlider.value = enemyHP;
        }

    }


//UI BUTTON METHODS//
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

//TEMP METHOD FOR TALENT TUTORIAL ONLY!!!//
    //Makes talent button interactable after Vahan's dialogue finishes (for now, will change later so it's on when a party member's TP is 100%)
    public void ActivateTalent()
    {
        EventSystem.current.SetSelectedGameObject(talentButton);
        talentButton.GetComponent<Button>().interactable = true;
        attackButton.GetComponent<Button>().interactable = false; //temporarily disables attack button
        tutorialDialoguePanel.SetActive(false);
    }

    //AFTER A TALENT IS PERFORMED AND BOTH CHARACTERS HAVE 0 TP, MAKE ATTACKS SELECTABLE AGAIN
    public void ResetAttacks()
    {
        Debug.Log("ATTACKS CAN BE PERFORMED AGAIN");
        EventSystem.current.SetSelectedGameObject(attackButton);
        talentButton.GetComponent<Button>().interactable = false;
        attackButton.GetComponent<Button>().interactable = true; //temporarily disables attack button
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
            
            if(bm.curTurn == 0)
            {
                sylviaTalentUIPrompt.SetActive(true);
                bm.ChangeMusic();
                bm.sylviatalentAttackScript.enabled = true;
                bm.vahantalentAttackScript.enabled = false;
                Invoke("StartNextTurn", 3f); //16s is duration of Vahan's placeholder Talent BGM
            }
            else if(bm.curTurn == 1)
            {
                //bm.sylviaTalentScript.enabled = true;
                vahanTalentUIPrompt.SetActive(true);
                bm.ChangeMusic();
                bm.sylviatalentAttackScript.enabled = false;
                bm.vahantalentAttackScript.enabled = true;
                Invoke("StartNextTurn", 10f); //16s is duration of Vahan's placeholder Talent BGM
            }
            //playerAnim.SetTrigger("Talent");
            bm.partyMembers[bm.curTurn].GetComponent<PlayerAction>().TalentAttack(bm.enemies[bm.currentEnemy].GetComponent<EnemyAttack>());

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
    //Make sure to deactivate talent so talentActivated is false
    public void CancelAttack()
    {
        Debug.Log("ATTACK CANCELLED");  
        EventSystem.current.SetSelectedGameObject(attackButton);
        bm.talentActivated = false;
    }


//UPDATE THE UI BEFORE STARTING THE NEXT PLAYER OR ENEMY TURN (BEFORE IT RESETS TO SYLVIA)
    public void StartNextTurn()
    {
        if(bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent < 100 && bm.talentActivated == false)
        {
            bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent += 25;
            EventSystem.current.SetSelectedGameObject(attackButton);
            bm.sylviatalentAttackScript.enabled = false;
            bm.vahantalentAttackScript.enabled = false;
        }

        else if(bm.talentActivated == true)
        {
            Debug.Log("Talent Reset");
            //TEMPORARY FOR FIRST BATTLE ONLY!!!//
            //For now, after Vahan players must use Sylvia's talent, but future battles will allow attacks and talents to be performed
            bm.talentActivated = false;
            sylviaTalentUIPrompt.SetActive(false);
            vahanTalentUIPrompt.SetActive(false);
            bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent = 0;
            bm.ChangeMusic();
            bm.sylviatalentAttackScript.enabled = false;
            bm.vahantalentAttackScript.enabled = false;
        }

        //CALL NEXTTURN() TO START THE NEXT PLAYER OR ENEMY TURN
        bm.curTurn++;
        bm.NextTurn();

        if(bm.curTurn < 2)
        {
            if(bm.partyMembers[bm.curTurn].GetComponent<TheUnitStats>().talent >= 100)
            {
                ActivateTalent();
            }
            else 
            {
                ResetAttacks();
            }
        }
    }

    //ONLY FOR FIRST BATTLE
    public void TalentTutorial()
    {
        Debug.Log("VAHAN TUTORIAL BEGINS");
        battleDialogue.enabled = true;
        tutorialDialoguePanel.SetActive(true);
        bm.tutorial = true;
    }
}