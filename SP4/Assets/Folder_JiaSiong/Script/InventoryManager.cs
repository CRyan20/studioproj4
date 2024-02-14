using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] inventorySlots; // Assuming these are the empty slots in your inventory
    public GameObject selectionIndicator; // The GameObject representing the selection indicator
    private int selectedSlotIndex = 0;
    private float oriYpos;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSelectionIndicator();
        oriYpos = selectionIndicator.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for key input to change the selected slot
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlotIndex = 0;
            UpdateSelectionIndicator();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotIndex = 1;
            UpdateSelectionIndicator();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotIndex = 2;
            UpdateSelectionIndicator();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlotIndex = 3;
            UpdateSelectionIndicator();
        }
    }

    void UpdateSelectionIndicator()
    {
        // Change the parent of the selection indicator to the selected slot
        selectionIndicator.transform.SetParent(inventorySlots[selectedSlotIndex].transform);

        // Set the local position of the selection indicator to (0, 0, 0)
        selectionIndicator.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Debug.Log("Selected slot: " + (selectedSlotIndex + 1));
    }
}
