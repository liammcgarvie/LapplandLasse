using UnityEngine;
using UnityEngine.InputSystem;

public class MyMovement : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    
    public bool controlEnabled { get; set; }

    private Vector2 moveInput;
    private Vector2 startPosition;
    private bool isGrounded;
    private bool isMoving;
    private bool isGrappling;
    private bool justGrappled;

    private Rigidbody2D rb;
    private CircleCollider2D groundCheckCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
        
        controlEnabled = true;
        
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 currentVelocity = rb.velocity;

        if (isMoving || (isGrappling == false && justGrappled == false))
        {
            rb.velocity = new Vector2(TranslateInputToVelocityX(moveInput), currentVelocity.y);
        }
        
        isGrounded = IsGrounded();
    }

    private void Update()
    {
        // If you are grappling justGrappled is set to true
        if (isGrappling)
        {
            justGrappled = true;
        }

        // Sets justGrappled to false when you have landed on the ground after grappling
        if (isGrounded && isGrappling == false)
        {
            justGrappled = false;
        }
    }
    
    private bool IsGrounded() // Checks if the player is on the ground
    {
        if (groundCheckCollider.IsTouchingLayers(groundLayer))
        {
            return true;
        }
        
        return false;
    }

    float TranslateInputToVelocityX(Vector2 input)
    {
        return input.x * maxSpeed;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
            isMoving = true;
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
            isMoving = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
        }
    }
    
    public void PositionReset()
    {
        transform.position = startPosition;
    }
    
    public void StartGrapple() // Can be called with events
    {
        isGrappling = true;
    }

    public void EndGrapple() // Can be called with events
    {
        isGrappling = false;
    }
}

