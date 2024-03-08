using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DungeonPartyUI : MonoBehaviour
{
    //VARIABLES//
    public int sHP;
    public int vHP;
    public int sMaxHP;
    public int vMaxHP;

    public List<GameObject> partyMembers = new List<GameObject>();
    private GameObject sylvia;
    private GameObject vahan; 

    //REFERENCES//

    //HP SLIDERS//
    public Slider sylviaHP;
    public Slider vahanHP;

    //TALENT SLIDERS//
    public Slider sylviaTP;
    public Slider vahanTP;
    public TextMeshProUGUI sylviaHPText;
    public TextMeshProUGUI sylviaTPText;
    public TextMeshProUGUI vahanHPText;
    public TextMeshProUGUI vahanTPText;

    //ENEMY SLIDERS// (REPLACE WITH SLIDERS ATTACHED TO EACH SPAWNED ENEMY LATER)
    // public Slider enemyHPSlider;
    // private int enemyHP;
    // public GameObject targetEnemy; 

    //REFERENCE TO VAHAN TALENT UI PROMPT//
    // public GameObject vahanTalentUIPrompt;
    // public GameObject sylviaTalentUIPrompt;
    // Start is called before the first frame update
    void Start()
    {

        //INITALIZE HP AND TP VALUES TO BE BASED ON ONES IN UNIT STATS//

        //ADD SYLVIA AND VAHAN TO PARTY SO THEY CAN SPAWN IN OUTSIDE OF THIS SCENE
        sylvia = GameObject.Find("Sylvia");
        vahan = GameObject.Find("Vahan");
        partyMembers.Add(sylvia);
        partyMembers.Add(vahan);


         //CHANGE IT SO THE HP AND TP IS BASED ON UNITSTATS OF EACH PARTY MEMBER
        
        sMaxHP = partyMembers[0].GetComponent<TheUnitStats>().maxHealth;
        vMaxHP = partyMembers[1].GetComponent<TheUnitStats>().maxHealth;
        sylviaHP.maxValue = sMaxHP;
        vahanHP.maxValue = vMaxHP;

        
        sHP = partyMembers[0].GetComponent<TheUnitStats>().health;
        vHP = partyMembers[1].GetComponent<TheUnitStats>().health;
        sylviaHP.value = sHP;
        vahanHP.value = vHP;

        sylviaTPText.text = partyMembers[0].GetComponent<TheUnitStats>().talent.ToString() + "%";
        sylviaTP.value = partyMembers[0].GetComponent<TheUnitStats>().talent;
        vahanTPText.text = partyMembers[1].GetComponent<TheUnitStats>().talent.ToString().ToString() + "%";
        vahanTP.value = partyMembers[1].GetComponent<TheUnitStats>().talent;

    }

    //CONSTANTLY UPDATES HP/TP VALUES OF PARTY MEMBERS UI 
    void Update()
    {
        sylviaTPText.text = partyMembers[0].GetComponent<TheUnitStats>().talent.ToString() + "%";
        sylviaTP.value = partyMembers[0].GetComponent<TheUnitStats>().talent;
        vahanTPText.text = partyMembers[1].GetComponent<TheUnitStats>().talent.ToString().ToString() + "%";
        vahanTP.value = partyMembers[1].GetComponent<TheUnitStats>().talent;

        sHP = partyMembers[0].GetComponent<TheUnitStats>().health;
        vHP = partyMembers[1].GetComponent<TheUnitStats>().health;
        sylviaHP.value = sHP;
        vahanHP.value = vHP;
        sylviaHPText.text = sHP.ToString();
        vahanHPText.text = vHP.ToString();
    }

}
