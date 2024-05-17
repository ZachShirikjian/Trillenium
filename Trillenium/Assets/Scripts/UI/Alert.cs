using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert: MonoBehaviour
{
    public string signalMessage;

    public void AlertObserver(string message)
    {
        signalMessage = message;
    }
}
