using IngameDebugConsole;
using UnityEngine;

/// <summary>
/// Handles Player Movement, Camera Rotation, and Jumping
/// </summary>
/// <remarks>
/// This script is based on the Unity FPS Microgame PlayerMovement script <br/>
/// All Public parameters are public so that they can be modified in game
/// </remarks>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    
    public Camera playerCamera;
    
    [Header("Parameters")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true; // Used to freeze player movement when necessary

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //Debug commands
        DebugLogConsole.AddCommandInstance("DoubleSpeed", "Doubles the players Speed", "DebugDoubleSpeed", this);
        DebugLogConsole.AddCommandInstance("ResetSpeed", "Resets the players Speed", "DebugResetSpeed", this);
        
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runningSpeed : walkingSpeed;
        float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
        
        

        // Player and Camera rotation
        if (!canMove) return;
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    public void DebugDoubleSpeed()
    {
        walkingSpeed *= 2;
    }
    public void DebugResetSpeed()
    {
        walkingSpeed = 7.5f;
    }
    
}
