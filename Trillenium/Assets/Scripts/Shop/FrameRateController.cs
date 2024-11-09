using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    void Start()
    {
        // Set the target frame rate to 60 FPS.
        Application.targetFrameRate = 60;
    }
}
