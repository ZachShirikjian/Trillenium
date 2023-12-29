using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    [TextArea]

    public string speakerText;
    public string personSpeaking;
    public bool newPersonSpeaking;
    public Sprite speakerPortait;
    public AudioClip audioClip;
}
