using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float speed = 150f;
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

    [Header("Bowl to climb up")]
    [SerializeField] private Transform bowlTransform;
    [SerializeField] private float bowlRadius = 0.5f;
    [SerializeField] private float climbGoal = 0.1f;
    [SerializeField] private float climbSpeed = 1f;

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 velocity;
    private Vector3 rotation; // Direction of horizontal movement of the camera.
    private Vector3 rotation2; // Direction of vertical movement of the camera.

    private bool contextStartedJump;
    private bool alreadyTriggered; // contextStartedJump
    private bool contextCanceledJump;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Jump();
    }

    private void Update()
    {
        LookAround();
        ClimbUpOnEdge();
        Move3D();
        Fall();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            contextStartedJump = true;

        contextCanceledJump = context.canceled;

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Jump()
    {
        // Buffer
        if (canBufferJump && rb.velocity.y < 0)
        {
            if (contextStartedJump) jumpBufferCountDown = jumpBuffer;

            jumpBufferCountDown -= Time.deltaTime;
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

    /// <summary>
    /// Moves the character in the direction of the input. If the character is on the ground, 
    /// it moves with full speed, otherwise it moves with reduced speed (airAcceleration).
    /// </summary>
    private void Move3D()
    {
        float acceleration = IsGrounded() ? 1f : airAcceleration;
        float moveSpeed = speed * acceleration;

        Vector3 moveDirection = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.y * moveSpeed);
        moveDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * moveDirection; // move direction to look direction

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
    }

    /// <summary>
    /// Rotates yaw of the player and pitch of camera pivot point.
    /// </summary>
    private void LookAround()
    {
        rotation.y += lookInput.x * lookSensitivityHorizontal * Time.deltaTime; // yaw of the player

        rotation2.x += lookInput.y * lookSensitivityVertikal * Time.deltaTime; // pitch of camera pivot point

        rotation2.x = Mathf.Clamp(rotation2.x, lookDeepEndAngle, lookHighEndAngle); // pitch in between low and high end angle
        rotation2.y = rotation.y;

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
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
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

    void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = IsBowlToClimbGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(bowlTransform.position, bowlRadius);
    }

    public void EnableRigid(bool _enable)
    {
        rb.isKinematic = !_enable;
        rb.detectCollisions = _enable;
    }
}