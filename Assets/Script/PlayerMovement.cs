using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;

    [Header("Audio")]
    public AudioSource footstepSource;
    public float stepInterval = 0.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private Camera playerCamera;
    private float stepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Vertical rotation (camera only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (whole player)
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // Check if player is on ground
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Create move direction relative to where player is facing
        Vector3 moveDirection = transform.right * x + transform.forward * z;

        // Sprint with Left Shift
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Apply movement
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // Handle footsteps
        HandleFootsteps(moveDirection);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFootsteps(Vector3 moveDirection)
    {
        // Check if player is moving and on ground
        if (moveDirection.magnitude > 0.1f && isGrounded)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                // Play footstep sound
                if (footstepSource != null)
                {
                    footstepSource.pitch = Random.Range(0.9f, 1.1f); // Slight pitch variation
                    footstepSource.Play();

                    // Alternative: Use AudioManager if you have one
                    // if (AudioManager.Instance != null)
                    //     AudioManager.Instance.PlayRandomFootstep();
                }

                stepTimer = 0f;
            }
        }
        else
        {
            // Reset timer when not moving
            stepTimer = stepInterval;
        }
    }
}