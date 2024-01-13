using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //VARIABLES//

    //REFERENCES//
    public GameObject npcDialogue;
    // Start is called before the first frame update
    void Start()
    {   
        npcDialogue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
