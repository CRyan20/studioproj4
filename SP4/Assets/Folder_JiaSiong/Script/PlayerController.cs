using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    void Update()
    {
        // Handle player movement
        HandleMovement();

        // Check for pickup when player presses the "F" key
        if (Input.GetKeyDown(KeyCode.F))
        {
            inventoryManager.TryPickupItem();
        }

        // Check for dropping when player presses the "G" key
        if (Input.GetKeyDown(KeyCode.G))
        {
            inventoryManager.TryDropItem();
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);

        float rotateAmount = horizontal * rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotateAmount);
    }
}
