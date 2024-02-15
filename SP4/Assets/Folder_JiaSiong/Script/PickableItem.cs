using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PickableItem : MonoBehaviour
{
    public ItemData itemData; // Reference to the ScriptableObject containing item data

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventoryManager = other.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.TryPickupItem();
            }
        }
    }
}
