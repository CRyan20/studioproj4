using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject selectionIndicator;
    public GameObject invItemPrefab; // Prefab of the inventory item game object
    public GameObject[] inventorySlots;
    private int selectedSlotIndex = 0;
    [SerializeField]
    private bool pickupbool = false;
    private bool isNearItem = false; // Flag to track if the player is near an item
    public TextMeshProUGUI pickupText; // Reference to TextMeshProUGUI
    private Dictionary<int, PickableItem> inventoryItems = new Dictionary<int, PickableItem>();
    private GameObject currentInvItemPrefab; // Reference to the currently instantiated inventory item prefab
    private bool isPickingUp = false; // Flag to prevent multiple pickups
    void Update()
    {
        HandleSlotSelectionInput();

        // Check for key input to pick up and drop items
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isNearItem) // Check if the player is near an item before picking up
            {
                pickupbool = true;
                TryPickupItem();
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            TryDropItem();
        }
        pickupbool = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            isNearItem = true;
            PickableItem pickableItem = other.GetComponent<PickableItem>();
            if (pickableItem != null)
            {
                ShowPickupText("Press F to pick up " + pickableItem.itemData.itemName);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            isNearItem = false;
            pickupText.gameObject.SetActive(false);
        }
    }

    void HandleSlotSelectionInput()
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

    public void TryPickupItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Item") && pickupbool == true)
            {
                PickableItem pickableItem = collider.GetComponent<PickableItem>();
                if (pickableItem != null)
                {
                    // Check if the selected slot is already occupied
                    if (!inventoryItems.ContainsKey(selectedSlotIndex))
                    {
                        // Show pickup text
                        ShowPickupText(pickableItem.itemData.itemName);

                        // Pick up the item
                        PickUpItem(pickableItem);
                        pickupbool = false;
                    }
                    else
                    {
                        // Show a message indicating the slot is already occupied
                        ShowPickupText("Selected slot is already occupied. Drop the current item first.");
                    }
                    break;
                }
            }
        }
    }


    public void TryDropItem()
    {
        if (inventoryItems.ContainsKey(selectedSlotIndex))
        {
            PickableItem itemToDrop = inventoryItems[selectedSlotIndex];

            // Set the position and activate the item in the scene
            itemToDrop.transform.position = transform.position + transform.forward * 1f; // Drop the item 2 units in front of the player
            itemToDrop.gameObject.SetActive(true);

            // Remove the item from the inventory
            inventoryItems.Remove(selectedSlotIndex);

            // Destroy the instantiated inventory item prefab
            Destroy(currentInvItemPrefab);
        }
    }

    void PickUpItem(PickableItem item)
    {
        // Add the item to the inventory
        inventoryItems[selectedSlotIndex] = item;

        // Instantiate the inventory item game object
        currentInvItemPrefab = Instantiate(invItemPrefab, Vector3.zero, Quaternion.identity);
        currentInvItemPrefab.SetActive(true);

        // Set the parent of the instantiated object to the selected slot
        currentInvItemPrefab.transform.SetParent(inventorySlots[selectedSlotIndex].transform);

        // Set the position of the instantiated object to (0, 0, 0)
        currentInvItemPrefab.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Set the scale of the instantiated object to (0.75, 0.75, 0.75)
        currentInvItemPrefab.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        // Set the sprite of the inventory item game object based on the ScriptableObject
        Image invItemImage = currentInvItemPrefab.GetComponent<Image>();
        invItemImage.sprite = item.itemData.itemIcon;

        // Deactivate the item in the scene
        item.gameObject.SetActive(false);

        // Show pickup text and hide it after 2 seconds (adjust the time as needed)
        ShowPickupText("Picked up item: " + item.itemData.itemName);
    }

    void UpdateSelectionIndicator()
    {
        selectionIndicator.transform.SetParent(inventorySlots[selectedSlotIndex].transform);
        selectionIndicator.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Debug.Log("Selected slot: " + (selectedSlotIndex + 1));
    }
    void ShowPickupText(string text, float displayTime = 2f)
    {
        // Display the pickup text with the provided message
        pickupText.text = text;
        pickupText.gameObject.SetActive(true);

        // Hide the pickup text after a delay
        StartCoroutine(HidePickupTextDelayed(displayTime));
    }

    IEnumerator HidePickupTextDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hide the pickup text
        pickupText.gameObject.SetActive(false);
    }

}
