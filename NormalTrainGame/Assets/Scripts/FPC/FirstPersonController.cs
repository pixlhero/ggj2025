using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Movement Speeds")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 12f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    [Tooltip("Key used to pick up and drop objects")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Orientation")]
    [Tooltip("Usually an empty gameobject or the camera pivot that indicates forward direction.")]
    public Transform orientation;

    [Header("Pickup System")]
    [Tooltip("Camera used for raycasting; drag your main camera here.")]
    public Camera playerCam;
    [Tooltip("Point where held objects will be placed (child of camera or player).")]
    public Transform holdPoint;
    [Tooltip("How far we can reach to pick up objects.")]
    public float pickupRange = 3f;

    private Pickupable currentlyHeldObject;

    [Header("Weapon Bobbing")]
    [Tooltip("Speed of the bobbing effect.")]
    public float bobbingSpeed = 5f;
    [Tooltip("Vertical amplitude of the bobbing.")]
    public float bobbingAmount = 0.1f;
    [Tooltip("How quickly the weapon returns to its original position when not moving.")]
    public float bobReturnSpeed = 5f;

    private float bobTimer = 0f;
    private Vector3 holdPointDefaultLocalPos;

    // Footstep Settings
    [Header("Footstep Settings")]
    [Tooltip("How often footstep sounds are played (seconds between each step).")]
    public float footstepInterval = 0.5f;

    private float footstepTimer = 0f;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    Rigidbody rb;

    // --- NEW: Subway Camera Shake Fields ---
    [Header("Subway Camera Shake")]
    public bool enableCameraShake = true;    // Toggle shake on/off in Inspector
    public float shakeIntensity = 0.02f;     // How strong the shake is
    public float shakeSpeed = 1.5f;          // How fast the shake oscillates
    private Vector3 camDefaultLocalPos;      // To store the camera’s original local position

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Start at walking speed by default
        moveSpeed = walkSpeed;

        // Save the default local position of the hold point for bobbing reference
        if (holdPoint != null)
        {
            holdPointDefaultLocalPos = holdPoint.localPosition;
        }

        // Cache the camera's default local position
        if (playerCam != null)
        {
            camDefaultLocalPos = playerCam.transform.localPosition;
        }
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down,
                                   playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        HandleSprinting();
        SpeedControl();

        // Handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        // Check for pickup/drop input
        if (Input.GetKeyDown(interactKey))
        {
            TryPickupOrDrop();
            TryOpenDoor();
        }

        // Check if showing the pickup hint
        if (currentlyHeldObject != null)
        {
            UIManager.Instance.pickupHint.SetActive(false);
        }
        else
        {
            // Check for pickupable objects in range
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            {
                if (hit.transform.GetComponent<Pickupable>() != null || hit.transform.GetComponent<DoorInteractible>() != null)
                {
                    UIManager.Instance.pickupHint.SetActive(true);
                }
                else
                {
                    UIManager.Instance.pickupHint.SetActive(false);
                }
            }
            else
            {
                UIManager.Instance.pickupHint.SetActive(false);
            }
        }

        // Update the weapon bobbing
        WeaponBobbing();

        // Subway Camera Shake
        SubwayCameraShake();

        // Update footstep logic
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void HandleSprinting()
    {
        // Switch between walk speed and sprint speed
        if (Input.GetKey(sprintKey) && grounded)
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else // In air
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // Reset y velocity so jump is consistent
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void TryPickupOrDrop()
    {
        // If we're holding an object, drop it
        if (currentlyHeldObject != null)
        {
            currentlyHeldObject.OnDrop();
            currentlyHeldObject = null;
        }
        else
        {
            // Attempt to pick up
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            {
                Pickupable pickupable = hit.transform.GetComponent<Pickupable>();
                if (pickupable != null)
                {
                    currentlyHeldObject = pickupable;
                    pickupable.OnPickUp(holdPoint);
                }
            }
        }
    }

    private void TryOpenDoor() {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            DoorInteractible door = hit.transform.GetComponent<DoorInteractible>();
            if (door != null)
            {
                door.ToggleDoor();
            }
        }
    }

    private void WeaponBobbing()
    {
        if (holdPoint == null) return; // Safety check

        // 1) Determine how fast the player is moving on the XZ plane.
        float horizontalSpeed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;

        // 2) If the player is barely moving or is not grounded, reset and return holdPoint to default.
        if (horizontalSpeed < 0.1f || !grounded)
        {
            bobTimer = 0f;
            // Smoothly lerp back to the original local position
            holdPoint.localPosition = Vector3.Lerp(
                holdPoint.localPosition,
                holdPointDefaultLocalPos,
                Time.deltaTime * bobReturnSpeed
            );
            return;
        }

        // 3) Increase bobbing timer
        bobTimer += Time.deltaTime * bobbingSpeed;

        // 4) Use Sin wave for vertical offset
        float waveSlice = Mathf.Sin(bobTimer);
        float verticalOffset = waveSlice * bobbingAmount;

        // 5) Apply the offset to holdPoint
        Vector3 newLocalPos = new Vector3(
            holdPointDefaultLocalPos.x,
            holdPointDefaultLocalPos.y + verticalOffset,
            holdPointDefaultLocalPos.z
        );

        holdPoint.localPosition = newLocalPos;
    }

    private void HandleFootsteps()
    {
        // If not on the ground, reset timer and do nothing
        if (!grounded)
        {
            footstepTimer = 0f;
            return;
        }

        // Get horizontal speed
        float horizontalSpeed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;

        // If not moving fast enough, reset timer and do nothing
        if (horizontalSpeed < 0.1f)
        {
            footstepTimer = 0f;
            return;
        }

        // Accumulate time
        footstepTimer += Time.deltaTime;

        // If we exceed the footstep interval, play a sound and reset the timer
        if (footstepTimer >= footstepInterval)
        {
            footstepTimer = 0f;

            // Randomly choose one of the two footsteps
            if (Random.value < 0.5f)
            {
                AudioManager.Instance.Play("FootStep1");
            }
            else
            {
                AudioManager.Instance.Play("FootStep2");
            }
        }
    }

    // Subway Camera Shake Method ---
    private void SubwayCameraShake()
    {
        if (!enableCameraShake || playerCam == null)
            return;

        // Generate Perlin Noise offsets (smooth, natural rumble)
        float xOffset = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeIntensity;
        float yOffset = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

        // Apply them to the camera's default position
        Vector3 newLocalPos = camDefaultLocalPos + new Vector3(xOffset, yOffset, 0f);
        playerCam.transform.localPosition = newLocalPos;
    }
}
