using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//THIS SCRIPT IS FROM THE TOP DOWN 2D RPG TUTORIAL FROM https://gamedevacademy.org/how-to-create-an-rpg-game-in-unity-comprehensive-guide/#Background_and_HUD_Canvases
//IT MAY BE CHANGED BUT THIS IS TO GET IT WORKING IN THE PROTOTYPE FOR NOW//

public class SpawnEnemy : MonoBehaviour
{

    //Reference to enemy encounter prefab which player enters in
    [SerializeField]
    private GameObject enemyEncounterPrefab;

    private bool spawning = false; //checks if the enemy spawner is starting a battle or not


    // Start is called before the first frame update
    //Ensure that there's only 1 EnemySpawner script which spawns at a time
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //If player enters the TestBattle scene
        //Spawn a copy of the enemy encounter, including all the enemies inside that prefab 
        if(scene.name == "TestBattle")
        {
            if(this.spawning)
            {
                Instantiate(enemyEncounterPrefab);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
    }

    //If player enters enemy encounter trigger,
    //Load the TestBattle scene
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            this.spawning = true;
            SceneManager.LoadScene("TestBattle");
        }
    }
}
