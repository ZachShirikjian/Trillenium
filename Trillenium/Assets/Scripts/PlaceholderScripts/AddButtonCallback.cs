using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//


//PlayerParty isn't from the same scene as the UI buttons in the battle scene
//Adds callback to Start() method
//Calls selectAttack() from SelectUnit() script 
public class AddButtonCallback : MonoBehaviour
{
    [SerializeField] private bool physical;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => addCallback());
    }

    private void addCallback()
    {
        GameObject playerParty = GameObject.Find("PlayerParty");
        playerParty.GetComponent<SelectUnit>().selectAttack(this.physical);
    }
}