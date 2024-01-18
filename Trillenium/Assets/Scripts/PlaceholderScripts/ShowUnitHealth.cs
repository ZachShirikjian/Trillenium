using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherits ShowUnitStat Abstract Parent Class
public class ShowUnitHealth : ShowUnitStats
{
    override protected float newStatValue()
    {
        return unit.GetComponent<UnitStats>().health;
    }
}
