using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Objective : ScriptableObject
{
    [TextArea]
    public string objectiveText;
}
