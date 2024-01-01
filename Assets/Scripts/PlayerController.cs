using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    Vector3[] LEVELS_COORDS = { new(-66, 22, -315), new(-50, 12, -20), new(0, 0, 0), new(-100, 72, 740), new(0, 0, 0), new(0, 0, 0) };
    Vector3[] LEVELS_ROTATIONS = { new(0, 90, 0), new(0, 90, 0), new(0, 90, 0), new(0, 90, 0), new(0, 90, 0), new(0, 90, 0) };

    public Camera playerCamera;
    public float walkSpeed = 6.0f;
    public float jumpPower = 7.0f;
    public float gravity = 10.0f;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;
    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!canMove)
        { 
            return;
        }
        
        // Moving
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float currentSpeedX = walkSpeed * Input.GetAxis("Vertical");
        float currentSpeedY = walkSpeed * Input.GetAxis("Horizontal");
        float moveDirectionY = moveDirection.y;

        moveDirection = forward * currentSpeedX + right * currentSpeedY;

        // Jumping / Falling
        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = moveDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Rotation
        characterController.Move(moveDirection * Time.deltaTime);
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        // Teleport player to the certain level if key is pressed
        for (int i = 1; i <= LEVELS_COORDS.Length; ++i)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                TeleportPlayerToLevel(i - 1);
            }
        }
    }

    void TeleportPlayerToLevel(int level)
    {
        var coords = LEVELS_COORDS[level];
        var rotation = LEVELS_ROTATIONS[level];
        transform.SetPositionAndRotation(coords, Quaternion.Euler(rotation));
    }
}
