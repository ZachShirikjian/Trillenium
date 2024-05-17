using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is used from Referencing Assets through Code
//https://www.youtube.com/watch?v=iD1_JczQcFY&t=75s&ab_channel=CodeMonkey

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets instance {
        get{
            if(_i == null) 
            {
                _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            }
                return _i;
        }
    }

    public Transform dTextPrefab;
}
