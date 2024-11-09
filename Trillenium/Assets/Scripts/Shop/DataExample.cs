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

    public int moneyData;
    #endregion

    void OnEnable()
    {
        // Sylvia stats.
        statsData[0][0] = 867;
        statsData[0][1] = 195;
        statsData[0][2] = 173;
        statsData[0][3] = 156;

        // Vahan stats.
        statsData[1][0] = 743;
        statsData[1][1] = 206;
        statsData[1][2] = 224;
        statsData[1][3] = 131;

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
