using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f; // Speed when sprinting
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float crouchHeight = 0.5f; // Adjust as needed
    public KeyCode crouchKey = KeyCode.LeftControl;

    public KeyCode sprintKey = KeyCode.LeftShift; // Define sprint key

    Vector3 velocity;
    bool isGrounded;
    bool isCrouching = false;
    bool isSprinting = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Adjust speed based on whether crouching or sprinting
        float currentSpeed = isCrouching ? walkSpeed / 2f : (isSprinting ? sprintSpeed : walkSpeed);
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Toggle crouch
        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                characterController.height = crouchHeight;
            }
            else
            {
                characterController.height = 2f; // Default height
            }
        }

        // Toggle sprint
        if (Input.GetKeyDown(sprintKey) && !isCrouching)
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(sprintKey) || isCrouching)
        {
            isSprinting = false;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}
