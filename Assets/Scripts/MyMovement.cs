using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyMovement : MonoBehaviour
{
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private SpriteRenderer playerSprite;
    [Tooltip("This is the speed of the player")]
    [SerializeField] private float maxSpeed = 10f;
    [Tooltip("This is the force that is put upon the player when you jump")]
    [SerializeField] private float jumpForce = 10f;
    [Tooltip("This is the force that is put upon the player when you are moving while using the grappling hook")]
    [SerializeField] private float swingForce = 10f;
    [Tooltip("This is the speed of which the player accelerates when starting to move")]
    [SerializeField] private float acceleration = 1f;
    [Tooltip("This is the speed of which the player decelerates when stopping to move")]
    [SerializeField] private float deceleration = 1f;

    private Vector2 moveInput;
    private Vector2 startPosition;
    private bool isGrounded;
    private bool isMoving;
    private bool isGrappling;
    private bool justGrappled;

    private float speed;

    private Rigidbody2D rb;
    private CircleCollider2D groundCheckCollider;

    //TODO: Fixa acceleration och momentum i movementet (helt enkelt gör om nästan allt)
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
        
        startPosition = transform.position;
        
        speed = 0;
    }

    private void FixedUpdate()
    {
        if (isMoving == false)
        {
            speed = 0;
        }
        
        // Using velocity based movement while not using the grappling hook
        if (isMoving && isGrappling == false && justGrappled == false)
        {
            speed += acceleration * Time.deltaTime;
            
            Vector2 currentVelocity = rb.velocity;
            
            //rb.velocity = new Vector2(TranslateInputToVelocityX(moveInput) * speed, currentVelocity.y);
            rb.AddForce(new Vector2(TranslateInputToVelocityX(moveInput) * speed, 0), ForceMode2D.Impulse);

            if (Mathf.Abs(currentVelocity.x) >= maxSpeed)
            {
                rb.velocity = new Vector2(currentVelocity.x, rb.velocity.y);
            }
        }

        // Makes the movement more realistic while grappling
        if (isMoving && (isGrappling || justGrappled))
        {
            rb.AddForce(moveInput * swingForce, ForceMode2D.Impulse);
        }

        // Stops the player if it is not supposed to move
        if (isMoving == false && isGrappling == false && justGrappled == false)
        {
            if (rb.velocity.x > 0.5f)
            {
                rb.velocity -= new Vector2(deceleration * Time.deltaTime, 0f);
            }
            else if (rb.velocity.x < -0.5f)
            {
                rb.velocity += new Vector2(deceleration * Time.deltaTime, 0f);
            }
            else
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
        
        isGrounded = IsGrounded();
    }

    private void Update()
    {
        // Flips the sprite
        if (rb.velocity.x > 0.01f)
        {
            playerSprite.flipX = false;
        }
        
        // Flips the sprite
        if (rb.velocity.x < -0.01f)
        {
            playerSprite.flipX = true;
        }
        
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
        
        //Bugfix
        if (isMoving && isGrappling == false && justGrappled == false)
        {
            if (rb.velocity.x > 0 && moveInput.x < 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        
            if (rb.velocity.x < 0 && moveInput.x > 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
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
        return input.x;
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

