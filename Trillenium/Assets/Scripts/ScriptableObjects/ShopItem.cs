using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ShopItem: MonoBehaviour
{
    public string itemName;
    [TextArea]
    public string itemDescription;
    public int itemCost;
    public float cursorOffset; // Amount to offset cursor by to the left of the item when highlighted; added by Cerulean.
}
