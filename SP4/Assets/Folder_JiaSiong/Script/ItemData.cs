using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon; // The sprite representing the item
    public GameObject itemPrefab;
    public string description;
    // Add more fields as needed for your specific item data

    [Range(1, 999)]
    public int maxStack = 1;
}
