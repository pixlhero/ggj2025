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

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Start at walking speed by default
        moveSpeed = walkSpeed;
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
        }
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

    /// Attempts to pick up an object if none is held,
    /// or drops the currently held object if we're already holding something.
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
}
