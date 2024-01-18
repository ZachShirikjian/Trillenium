using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//

public class UnitStats : MonoBehaviour, IComparable
{
    public float health;
    public float mana;
    public float attack;
    public float magic;
    public float defense;
    public float speed;
    public int nextActTurn;
    private bool dead = false;

    public GameObject damageTextPrefab;
    public Vector3 damageTextPosition;

    public void calculateNextActTurn (int currentTurn)
    {
        this.nextActTurn = currentTurn + (int)Math.Ceiling(100.0f / this.speed);
    }
    public int CompareTo (object otherStats)
    {
        return nextActTurn.CompareTo(((UnitStats)otherStats).nextActTurn);
    }
    public bool isDead ()
    {
        return this.dead;
    }

    //Display text over Unit's head when taking damage//
    //Reduces unit's heatlh and creates damage text, 
    //Damage text is child of Canvas b/c it's UI element
    //Properly set localPosition and localScale
    //If health < 0, set unit as dead, change tag & destroy unit
    public void receiveDamage(float damage)
    {
        this.health -= damage;
        //animator.Play("Hit");

        GameObject HUDCanvas = GameObject.Find("Canvas");
        GameObject damageText = Instantiate(this.damageTextPrefab, HUDCanvas. transform) as GameObject;
        damageText.GetComponent<Text>().text = "" + damage;
        damageText.transform.localPosition = this.damageTextPosition;
        damageText.transform.localScale = new Vector2(1.0f, 1.0f);

        if(this.health <= 0)
        {
            this.dead = true;
            this.gameObject.tag = "Dead";
            Destroy(this.gameObject);
        }
    }
}