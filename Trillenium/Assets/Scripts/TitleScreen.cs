using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    //TEMP METHOD//

    //Loads the TestCutscene of the game.
    //REPLACE THIS WITH LOADING SAVE FILES LATER!!!
    //Put on the New Game button in the Title Screen
    public void NewGame()
    {
        SceneManager.LoadScene("TestCutscene");
    }


    //Quits out of the game in the Title Screen.
    //Can be done both in the Editor (Play Mode) and in Builds.
    public void QuitGame()
    {
        //Exits Play Mode if QUIT is clicked during Play Mode (Editor).
        # if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;


        //Closes the Game if QUIT is clicked in a Build of the game.
        #endif
        Application.Quit();
    }
}
