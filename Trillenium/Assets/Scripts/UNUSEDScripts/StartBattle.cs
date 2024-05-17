using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL ON https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases

//IT MAY BE REPLACED LATER BUT THIS IS TO GET THE COMBAT SYSTEM WORKING FOR NOW

public class StartBattle : MonoBehaviour
{
    //Use DontDestroyOnLoad to prevent player from being destroyed when switching from movement to battle scene
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        this.gameObject.SetActive(false);
    }

    //
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "TitleScreen")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(scene.name == "TestBattle");
        }
    }
}
