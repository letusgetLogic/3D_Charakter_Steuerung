using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerGravity gravity;

    [Header("Horizontal Movement")]
    [SerializeField] private float speed = 150f;
    [SerializeField] private float moonSpeed = 1f;
    [SerializeField] private float airAcceleration = 0.7f;

    [Tooltip("How much to smooth out the movement")]
    [Range(0, .3f)]
    [SerializeField]
    private float movementSmoothing = .05f;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravityScale = 2f;
    [Header("- Jump Buffer")]
    [SerializeField] private bool canBufferJump; // Buffer combined with static jumps better.
    [SerializeField] private float jumpBuffer = 0.2f;
    private float jumpBufferCountDown;

    [Header("Camera")]
    [SerializeField] private Transform cameraPivotPointTransform;
    [SerializeField] private float lookSensitivityHorizontal = 10f; // Sensitivity of the camera movement horizontally.
    [SerializeField] private float lookSensitivityVertikal = 10f; // Sensitivity of the camera movement vertically.
    [SerializeField] private float lookDeepEndAngle = -50f; // Low end position of the inclination angle.
    [SerializeField] private float lookHighEndAngle = 80f; // High end position of the inclination angle.

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;

    [Header("Bowl to prevent stucking and climb up on the corner of obstacles")]
    [SerializeField] private Transform bowlTransform;
    [SerializeField] private float bowlRadius = 0.5f;
    [SerializeField] private float climbGoal = 0.1f;
    [SerializeField] private float climbSpeed = 1f;

    public bool IsOnSpaceship { get; set; } = false;
    public bool IsOnTheMoon
    {
        get
        {
            if (gravity && gravity.Moon)
                return gravity.Moon.IsInGravitySphere(col, myLayer);

            return false;
        }
    }

    private Rigidbody rb;
    private Collider col;
    private LayerMask myLayer;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 moveDirection;
    private Vector3 velocity;
    private Vector3 rotation; // Direction of horizontal movement of the camera.
    private Vector3 rotation2; // Direction of vertical movement of the camera.

    private bool contextStartedJump;
    private bool alreadyTriggered; // contextStartedJump
    private bool contextCanceledJump;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        myLayer = gameObject.layer;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        LookAround();
        Jump();
        ClimbUpOnEdge();
        Fall();
        MoveWithGravity();
    }

    private void MoveWithGravity()
    {
        // Apply moon gravity when player is leaving spaceship.
        if (IsEffectedByMoonGravity())
        {
            gravity.SetMoonGravity();
            gravity.Moon.Attract(transform);
            MoveOnTheMoon();
        }
        else
        {
            gravity.SetEarthGravity();
            Move3D();
        }
    }

    public void OnMove(InputAction.CallbackContext _context)
    {
        moveInput = _context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (_context.started)
            contextStartedJump = true;

        contextCanceledJump = _context.canceled;

    }

    public void OnLook(InputAction.CallbackContext _context)
    {
        lookInput = _context.ReadValue<Vector2>();
    }

    private void Jump()
    {
        if (IsEffectedByMoonGravity() == false)
        {
            // Buffer
            if (canBufferJump && rb.velocity.y < 0)
            {
                if (contextStartedJump) jumpBufferCountDown = jumpBuffer;

                jumpBufferCountDown -= Time.fixedDeltaTime;
            }

            if (alreadyTriggered == false &&
                /* Ground Jump */ (IsGrounded() && contextStartedJump) ||
                /* Buffer */      (canBufferJump && jumpBufferCountDown > 0f && IsGrounded()))

            {
                rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);

                jumpBufferCountDown = 0f;
                alreadyTriggered = true;
            }
            else alreadyTriggered = false;

            contextStartedJump = false;
        }
    }

    /// <summary>
    /// Moves the character in the direction of the input. If the character is on the ground, 
    /// it moves with full speed, otherwise it moves with reduced speed (airAcceleration).
    /// </summary>
    private void Move3D()
    {
        float acceleration = IsGrounded() ? 1f : airAcceleration;
        float moveSpeed = speed * acceleration * Time.fixedDeltaTime;

        moveDirection = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.y * moveSpeed);
        moveDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * moveDirection; // move direction to look direction

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
    }

    private void MoveOnTheMoon()
    {
        float acceleration = IsGrounded() ? 1f : airAcceleration;
        float moveSpeed = moonSpeed * acceleration * Time.fixedDeltaTime;

        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        rb.MovePosition(rb.position + transform.TransformDirection(moveDirection.normalized) * moveSpeed);
    }

    /// <summary>
    /// Rotates yaw of the player and pitch of camera pivot point.
    /// </summary>
    private void LookAround()
    {
        // If player is not effected by moon gravity, player can look around,
        // because it has the issue with rotate & move direction on the moon.
        if (IsEffectedByMoonGravity() == false)
        {
            rotation.y += lookInput.x * lookSensitivityHorizontal * Time.fixedDeltaTime; // yaw of the player
            rotation2.y = rotation.y;
        }

        rotation2.x += lookInput.y * lookSensitivityVertikal * Time.fixedDeltaTime; // pitch of camera pivot point
        rotation2.x = Mathf.Clamp(rotation2.x, lookDeepEndAngle, lookHighEndAngle); // pitch in between low and high end angle

        transform.eulerAngles = rotation; // yaw of the player

        cameraPivotPointTransform.eulerAngles = rotation2; // pitch of camera pivot point
    }

    private void Fall()
    {
        //if (rb.velocity.y < 0)
        //{
        //    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * gravityScale, rb.velocity.z);
        //}
    }

    private void ClimbUpOnEdge()
    {
        if (IsBowlToClimbGrounded())
        {
            // Move the player up to the bowl position
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + climbGoal, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * speed);
        }
    }

    /// <summary>
    /// Checks overlap of collider with groundChecks position on ground's collider.
    /// </summary>
    /// <returns></returns>
    bool IsGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        return colliders.Length > 0;
    }

    /// <summary>
    /// Checks overlap of collider with bowl position on ground's collider.
    /// </summary>
    /// <returns></returns>
    bool IsBowlToClimbGrounded()
    {
        if (IsGrounded())
            return false;

        Collider[] colliders = Physics.OverlapSphere(bowlTransform.position, bowlRadius, groundLayer);
        return colliders.Length > 0;
    }

    private bool IsEffectedByMoonGravity()
    {
        return IsOnTheMoon && IsOnSpaceship == false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = IsBowlToClimbGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(bowlTransform.position, bowlRadius);
    }

}