using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float runSpeed = 9f;
    public float jumpHeight = 3f;
    public float gravity = 20f;

    [Header("Camera & Rotation")]
    public float rotationSpeed = 90f;

    private CharacterController controller;
    private Vector3 moveDirection;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = controller.isGrounded;

        // Handle movement
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (movement.magnitude > 0.1f)
        {
            // Rotate player to face movement direction
            transform.Rotate(0, rotationSpeed * Time.deltaTime * horizontal, 0);

            // Apply movement
            moveDirection = transform.TransformDirection(movement);
            moveDirection *= (Input.GetButton("Run") ? runSpeed : moveSpeed);
        }

        // Handle jumping
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        if (moveDirection.y < 0)
            moveDirection.y -= gravity * Time.deltaTime;

        // Move controller
        controller.Move(moveDirection * Time.deltaTime);
    }
}