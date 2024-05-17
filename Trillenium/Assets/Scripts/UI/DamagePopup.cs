using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//This script is used from POLISH Your Game with Damage Popups UnityTutorial by CodeMonkey:
//https://www.youtube.com/watch?v=iD1_JczQcFY&t=75s&ab_channel=CodeMonkey

public class DamagePopup : MonoBehaviour
{

    //Creates an instance of this damage popup prefab 
    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        Transform damagePopupTransform = Instantiate(GameAssets.instance.dTextPrefab, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
    }
    private TextMeshPro damageTextMesh; 
    private float disappearTimer; //destroys instance of damagePopup text after certain time 
    private Color textColor; //color of text value 

    //Reference TextMesh on the damagePopup prefab object//
    private void Awake()
    {
        damageTextMesh = transform.GetComponent<TextMeshPro>();
    }

    //Sets text of this damage prefab to be damage amount dealt by enemies or players
    //Gets color of the textMesh for the text
    public void Setup(int damageAmount)
    {
        damageTextMesh.SetText(damageAmount.ToString());
        textColor = damageTextMesh.color;
        disappearTimer = 1f;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    //Moves DamagePopup upward at constant speed
    //Makes DamagePopup prefab instance disappear after certain amount of time 
    void Update()
    {
        float moveYSpeed = 5f;
        transform.position += new Vector3(0,moveYSpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;

        //Slowly disappear damageText after 1 second
        if(disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime; //Reduce alpha of damageText color by disappearSpeed * Time.deltaTime 
            damageTextMesh.color = textColor;//sets color of textMesh to be the textColor

            //Destroy the damagePopup instance if the alpha is < 0 
            if(damageTextMesh.color.a < 0) 
            {
                Destroy(this.gameObject);
            }
        }
    }
}
