using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Current move speed, will switch between walkSpeed and sprintSpeed at runtime")]
    public float moveSpeed;
    
    [Tooltip("Drag applied when on the ground")]
    public float groundDrag;
    
    [Tooltip("Force applied when jumping")]
    public float jumpForce;
    
    [Tooltip("Time between jumps")]
    public float jumpCooldown;
    
    [Tooltip("Multiplier for movement in air")]
    public float airMultiplier;
    
    private bool readyToJump = true;

    [Header("Movement Speeds")]
    [Tooltip("Speed while walking")]
    public float walkSpeed = 6f;
    
    [Tooltip("Speed while sprinting")]
    public float sprintSpeed = 12f;

    [Header("Keybinds")]
    [Tooltip("Key used to jump")]
    public KeyCode jumpKey = KeyCode.Space;
    
    [Tooltip("Key used to sprint")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    [Tooltip("Height of the player capsule")]
    public float playerHeight = 2f;
    
    [Tooltip("Which layers are considered ground")]
    public LayerMask whatIsGround;
    
    private bool grounded;

    [Tooltip("Transform for orientation (usually the player camera or empty object)")]
    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        // Ensure we start at walking speed
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(
            transform.position, 
            Vector3.down, 
            playerHeight * 0.5f + 0.3f, 
            whatIsGround
        );

        MyInput();
        HandleSprinting();
        SpeedControl();

        // Handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // When to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    /// <summary>
    /// Checks whether the sprint key is held down and adjusts moveSpeed accordingly.
    /// </summary>
    private void HandleSprinting()
    {
        if (Input.GetKey(sprintKey) && grounded)
        {
            // Set to sprint speed
            moveSpeed = sprintSpeed;
        }
        else
        {
            // Set to walk speed
            moveSpeed = walkSpeed;
        }
    }

    /// <summary>
    /// Applies forces to the Rigidbody to move the player.
    /// </summary>
    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On ground
        if (grounded)
        {
            rb.AddForce(
                moveDirection.normalized * moveSpeed * 10f, 
                ForceMode.Force
            );
        }
        else // In air
        {
            rb.AddForce(
                moveDirection.normalized * moveSpeed * 10f * airMultiplier,
                ForceMode.Force
            );
        }
    }

    /// <summary>
    /// Ensures the player does not exceed the current moveSpeed (walk or sprint).
    /// </summary>
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

    /// <summary>
    /// Applies an upward force to perform a jump.
    /// </summary>
    private void Jump()
    {
        // Reset Y velocity for a consistent jump
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
