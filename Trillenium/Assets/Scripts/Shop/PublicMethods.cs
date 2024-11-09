using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicMethods : MonoBehaviour
{
    // In computing, floating numbers aren't always calculated with complete accuracy, so we need to create a method that will ensure that accuracy.
    // This method will keep to two decimal places so that we don't end up with a messy number like 1.37840364.
    public float DecimalsRounded(float num)
    {
        // Number is multiplied by 100 to shift over by two decimal places (the two decimal numbers we want are safe in whole numbers place).
        // Number is rounded to nearest integer, and is divided by 100 to shift the two right-most digits back to the decimal side of the number.
        float roundedNum = Mathf.Round(num * 100f) / 100f;

        // Return properly rounded number.
        return roundedNum;
    }

    // I added this in after the animation was finished, but I made an updated version of the previous method that allows the user to input how many decimal places they want to round their number to.
    public float RoundTo(float num, int exponent) // Even though there's already a normal rounding method, if you wanted to round to 0 decimal places, enter 0 for the exponent as anything to the power of 0 is 1.
    {
        float roundedNum = Mathf.Round(num * Mathf.Pow(exponent, 10f)) / Mathf.Pow(exponent, 10f);

        // Return properly rounded number.
        return roundedNum;
    }
}
