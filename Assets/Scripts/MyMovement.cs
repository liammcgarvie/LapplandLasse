using UnityEngine;
using UnityEngine.InputSystem;

public class MyMovement : MonoBehaviour
{
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("This is the speed of the player")]
    [SerializeField] private float speed = 10f;
    [Tooltip("This is the force that is put upon the player when you jump")]
    [SerializeField] private float jumpForce = 10f;
    [Tooltip("This is the force that is put upon the player when you are moving while using the grappling hook")]
    [SerializeField] private float swingForce = 10f;

    private Vector2 moveInput;
    private Vector2 startPosition;
    private bool isGrounded;
    private bool isMoving;
    private bool isGrappling;
    private bool justGrappled;

    private Rigidbody2D rb;
    private CircleCollider2D groundCheckCollider;

    //TODO: Fixa acceleration och momentum i movementet (helt enkelt gör om nästan allt)
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
        
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 currentVelocity = rb.velocity;

        // Using velocity based movement while not using the grappling hook
        if (isMoving && isGrappling == false && justGrappled == false)
        {
            rb.velocity = new Vector2(TranslateInputToVelocityX(moveInput), currentVelocity.y);
        }

        // Makes the movement more realistic while grappling
        if (isMoving && (isGrappling || justGrappled))
        {
            rb.AddForce(moveInput * swingForce, ForceMode2D.Impulse);
        }

        // Stops the player if it is not supposed to move
        if (isMoving == false && isGrappling == false && justGrappled == false)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
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
        return input.x * speed;
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
    
    public void PositionReset() // Can be called with events
    {
        rb.velocity = Vector2.zero;
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

