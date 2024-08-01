using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{

    //VARIABLES//

    //Name of the NPC you can talk to//
    public string npcName;

    //List of all Dialogue an NPC has//
    public Dialogue[] dialogue;

   // public bool alreadySpokenTo = false;
    public bool specialNPC = false; //If this is a special NPC, it has a portrait (eg Vahan talking w/ Brigala memorial)

    //public bool objectInteractable = false; //If this is an Interactable, NOT an NPC, change the InteractPrompt to say CHECK instead of TALK

    //REFERENCES//
    public GameObject helpIcon; //Icon to indicate you can talk to this NPC

    // Start is called before the first frame update
    void Start()
    {
        helpIcon = this.transform.GetChild(0).gameObject; //ALWAYS is 1st child of NPC if it's a speakable NPC 
        helpIcon.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
