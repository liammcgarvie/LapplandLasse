using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("This is the force that is put upon the player when you jump")]
    [SerializeField] private float jumpForce = 10f;
    [Tooltip("This is the force that is put on the player in the x-axis when moving")]
    [SerializeField] private float speedForce = 10f;
    [Tooltip("This is the max speed of the player")]
    [SerializeField] private float maxSpeed = 10f;
    
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isMoving;
    
    private Rigidbody2D rb;
    private CircleCollider2D groundCheckCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
    }
    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        
        float currentVelocity = rb.velocity.x;
        
        if (isMoving)
        {
            rb.AddForce(moveInput * speedForce, ForceMode2D.Impulse);
        }

        if (Mathf.Abs(rb.velocity.x) >= maxSpeed)
        {
            rb.velocity = new Vector2(currentVelocity, rb.velocity.y);
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
}
