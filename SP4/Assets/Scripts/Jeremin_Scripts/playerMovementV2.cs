using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovementV2 : MonoBehaviour
{
    CharacterController controller;
    Animator anim;

    //movement
    Vector2 movement;
    public float moveSpeed;
    public float sprintSpeed;
    bool sprinting;
    public float trueSpeed;
    public float rotationSpeed = 3f;

    //jumping
    public float jumpHeight;
    public float gravity;
    bool isGrounded;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        trueSpeed = moveSpeed;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        RotateCameraWithMouse();
        Jump();
    }

    void MovePlayer()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            trueSpeed = sprintSpeed;
            sprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            trueSpeed = moveSpeed;
            sprinting = false;
        }

        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * movement.y + right * movement.x;
        direction.Normalize();

        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * trueSpeed * Time.deltaTime);

            // Set forward or strafe animation based on movement direction
            if (movement.y > 0) // Moving forward
            {
                anim.SetFloat("speed", sprinting ? 2 : 1);


            }
            else if (movement.y < 0) // Moving backward
            {
                anim.SetFloat("speed", sprinting ? -2 : -1);
            }
            else if (movement.x < 0) // Strafing left (A key)
            {
                anim.SetFloat("speedL", 1);
                anim.SetFloat("speedR", 0);
                anim.SetFloat("speedL", sprinting ? 2 : 1);
            }
            else if (movement.x > 0) // Strafing right (D key)
            {
                anim.SetFloat("speedL", 0);
                anim.SetFloat("speedR", 1);
                anim.SetFloat("speedR", sprinting ? 2 : 1);
            }
            else
            {
                anim.SetFloat("speed", 0);
                anim.SetFloat("speedL", 0);
                anim.SetFloat("speedR", 0);
            }
        }
        else
        {
            anim.SetFloat("speed", 0);
            anim.SetFloat("speedL", 0);
            anim.SetFloat("speedR", 0);
        }

    }


    void Jump()
    {
        isGrounded = Physics.CheckSphere(transform.position, .1f, 1);
        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt((jumpHeight * 10) * -2f * gravity);
        }

        if (velocity.y > -20f)
        {
            velocity.y += (gravity * 10) * Time.deltaTime;
        }



        controller.Move(velocity * Time.deltaTime);


    }

    void RotateCameraWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(Vector3.up * mouseX);
    }


}
