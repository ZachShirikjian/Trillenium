using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataExample : MonoBehaviour
{
    #region Variables
    public int[][] statsData = new int[3][]
    {
        new int[4],
        new int[4],
        new int[4]
    };

    #region References
    //Change this to reference the DontDestroyOnLoad object 
    private GameObject BattleSylviaRef;
    private GameObject BattleVahanRef; 

    #endregion

    public int moneyData;
    #endregion

    void OnEnable()
    {
        // Sylvia stats.
        statsData[0][0] = 200; //HP
        statsData[0][1] = 100; //AP
        statsData[0][2] = 5; //ATK
        statsData[0][3] = 10; //DEF

        // Vahan stats.
        statsData[1][0] = 250; //HP
        statsData[1][1] = 100; //AP
        statsData[1][2] = 10; //ATK
        statsData[1][3] = 5; //DEF

        // Assign all 0s.
        for (int i = 0; i < 4; i++)
        {
            statsData[2][i] = 0;
        }
        
        /*
        for (int i = 0; i < statsData.Length; i++)
        {
            for (int j = 0; j < statsData[0].Length; j++)
            {
                statsData[i][j] = Random.Range(0, 1000);
            }
        }
        */

        moneyData = 3000; // Temp money amount for trailer.
        //moneyData = Random.Range(0, 10000); // 0-9999.
    }
}
