using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{

    //Name of the building you can enter (in the InteractPrompt script) 
    public string buildingName;

    //Name of Scene to Load when entering the Door 
    public string SceneName; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


//TEMP METHOD FOR LOADING NEXT SCENE WHEN PLAYER INTERACTS WITH A DOOR
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
