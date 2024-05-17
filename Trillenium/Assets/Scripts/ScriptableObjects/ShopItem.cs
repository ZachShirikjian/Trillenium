using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ShopItem : MonoBehaviour
{
    public string itemName;
    [TextArea]
    public string itemDescription;
    public int itemCost;
    public GameObject itemSprite;
}
