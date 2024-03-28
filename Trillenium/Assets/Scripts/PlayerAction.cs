using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAction : MonoBehaviour
{
    //VARIABLES//

    //REFERENCES//
    private TheUnitStats unitStats;
    private SpriteRenderer spriteR;
    private TalentScript talentScript;

    //AUDIO REFERENCES//
    public AudioManager audioManager;
    public AudioSource sfxSource;
    
    //USE THIS FOR ADDING DAMAGED PORTRAITS
    public Image battlePortrait;
    public Sprite damagedSprite;
    public Sprite neutralSprite; 

    void Start()
    {
        unitStats = GetComponent<TheUnitStats>();
        spriteR = GetComponent<SpriteRenderer>(); //for now, when player takes damage, make their sprite red for a second
        talentScript = GetComponent<TalentScript>(); //reference to talent script associated with this player's GameObject, perform different methods depending on character used
    
        sfxSource = GameObject.Find("SFXSource").GetComponent<AudioSource>();
        audioManager = sfxSource.GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Code used from the Turn Based RPG Tutorial from CodeMonkey
    //https://www.youtube.com/watch?v=0QU0yV0CYT4&t=189s&ab_channel=CodeMonkey 
    //Gets the position of the target enemy 
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    //Moves to the selected enemy's position and does an attack on them 
    public void Attack(EnemyAttack targetChar)
    {
        Vector3 attackDir = (targetChar.GetPosition() - GetPosition()).normalized;

        sfxSource.PlayOneShot(audioManager.playerAttack);
        Debug.Log("ATTACKING ENEMY");
        
        StopAllCoroutines();
        StartCoroutine(DamageEnemy(targetChar));
    }

        //TODO: Adjust based on length of player char's animation (damage enemy after animation ends)
    public IEnumerator DamageEnemy(EnemyAttack enemyToDamage)
    {
        yield return new WaitForSeconds(0.5f);
        enemyToDamage.TakeDamage(unitStats.attack);
        DamagePopup.Create(enemyToDamage.GetPosition(), unitStats.attack);  
    }


    //Performs a Talent Attack minigame (different for each character)
    public void TalentAttack(EnemyAttack targetChar)
    {
        Debug.Log("PERFORMING A TALENT!");
        talentScript.canPerformTalent = true;
        //ENABLE TALENT ATTACK ASSOCIATED WITH THIS SCRIPT?//
    }

    //Take Damage from Enemy (called from EnemyAttack or BossAttack scripts)
    public void TakeDamage(int damageAmount)
    {
        Debug.Log(this.gameObject.name + " IS TAKING DAMAGE");
        battlePortrait.sprite = damagedSprite;
        unitStats.health -= damageAmount;
        spriteR.color = Color.red;
        Invoke("ResetColor", 0.5f);
    }

    //Reset color after player has taken damage 
    public void ResetColor()
    {
        spriteR.color = Color.white;
        battlePortrait.sprite = neutralSprite;
    }


}
