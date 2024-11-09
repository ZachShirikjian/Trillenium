using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserJiggle : MonoBehaviour
{
    #region Variables
    #region Reference-Related
    private DispenserMovement[] dispensers = new DispenserMovement[3];
    private TicketMovement[] tickets = new TicketMovement[3];
    #endregion

    #region Rotation-related
    private float theta = Mathf.Deg2Rad * 180f; // We calculate in radians, so it's easier to convert from known degree values into radians.
    private const float rotSpeed = 30f; // Speed at which theta increments at (does not need to be converted into radians).

    private float[] localRotZ = new float[3];

    private int dispenserIndex = 0; // Index for which dispenser in the array's rotation is being modified.
    private int rotIndex = 0; // Index representing how many rotations the current dispenser has made.
    private const int rotLimit = 1; // Limit for rotation index.
    #endregion

    private bool activeCheck = false; // Check for whether or not this script can run its main code.
    private bool jiggleActive = false; // Can the jiggle animation play?

    private float timerIndex = 0f; // Index for timer before jiggle animation can play.
    private float timerLimit; // Randomly generated time limit for timer before jiggle animation can play.

    private float[] timerBounds = { 6.00f, 10.00f };
    #endregion

    void Start()
    {
        // Assign dispensers and tickets in their respective arrays.
        for (int i = 0; i < dispensers.Length; i++)
        {
            dispensers[i] = GameObject.Find("Dispenser" + i + "/Dispenser").GetComponent<DispenserMovement>();
            tickets[i] = GameObject.Find("Dispenser" + i + "/Tickets").GetComponent<TicketMovement>();
        }
    }

    void FixedUpdate()
    {
        if (activeCheck) // If active, then serve main function.
        {
            if (jiggleActive) // If jiggle is active, do jiggle animation.
            {
                JiggleAnimation();
            }
            else // If not, do not do jiggle animation and handle semi-random timer before jiggle animation can be active again.
            {
                TimerHandler();
            }
        }
        else // If inavtive, then check dispensers and their timers.
        {
            CheckDispensers();
        }
    }

    #region Methods
    // Checks dispensers and their repsective tickets until all dispensers and their tickets have stopped moving.
    // Allows script to serve main function when this is the case.
    private void CheckDispensers()
    {
        // Cycle through all dispensers and their tickets.
        for (int i = 0; i < dispensers.Length; i++)
        {
            if (dispensers[i].mainMovDone && !tickets[i].ticketsMoving) // Have all dispensers and their tickets have stopped moving?
            {
                localRotZ[i] = dispensers[i].transform.localRotation.z; // Save last local rotation for later use.

                if (i == dispensers.Length - 1) // We've made it through all check? Grant script persmission to serve main function.
                {
                    activeCheck = true;
                }
            }
            else // If not, then leave for loop until next check.
            {
                return;
            }
        }
    }
    
    // Handles all timer-related procedures.
    private void TimerHandler()
    {
        // If index has not been used or has been reset, randomize timer limit.
        if (timerIndex == 0f)
        {
            timerLimit = Random.Range(timerBounds[0], timerBounds[1] + Mathf.Epsilon); // Randomly pick between X seconds (inclusive) and Y seconds (inclusive because of epsilon).
        }

        if (timerIndex < timerLimit) // If our timer index has not reached its limit yet, then continue to increment.
        {
            timerIndex += Time.deltaTime;
        }
        else// Otherwise, reset timer index and allow jiggle animation to play.
        {
            timerIndex = 0f;
            jiggleActive = true;
        }
    }
    
    // Jiggle animation for each dispenser (iterates, then turns animation off until timer conditions have been met again).
    private void JiggleAnimation()
    {
        float cosOffset = Mathf.Cos(180f) - 0.5f; // We offset the value of Cos(theta) for even rotational movement.
        
        theta += Time.deltaTime * rotSpeed; // Increment by rotation speed.

        // Every time theta wraps around, increment rotation index.
        if (theta >= Mathf.Deg2Rad * 360f)
        {
            theta %= Mathf.Deg2Rad * 360f; // Modulo wrap to 360f.
            rotIndex++; // Increment rotation index.
        }
        
        // Update rotation of current dispenser by taking our last saved local rotation for it, add Cos(theta), offset it by our Cos offset,
        // then subtract by 1f to account for Cos(0) being equal to 1.
        dispensers[dispenserIndex].transform.localRotation = Quaternion.Euler(0f, 0f, localRotZ[dispenserIndex] + ((Mathf.Cos(theta) - cosOffset - 1f) * 2f));

        if (rotIndex > rotLimit) // Have we rotated enough times?
        {
            // Good, now continue rotation until we get within bounds of -0.5f and 0.5f.
            if (Mathf.Abs(dispensers[dispenserIndex].transform.localRotation.z) <= 0.5f)
            {
                // Once within bounds close enough to 0f, we set local rotation of current dispenser to 0f for a seemless reset.
                dispensers[dispenserIndex].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                // Now reset rotation index and theta, then increment dispender index.
                rotIndex = 0;
                theta = Mathf.Deg2Rad * 180f;
                dispenserIndex++;

                // Reset dispenser index and disable jiggle animation once we've jiggled all dispensers.
                if (dispenserIndex >= dispensers.Length)
                {
                    dispenserIndex = 0;
                    jiggleActive = false;
                }
            }
        }
    }
    #endregion
}
