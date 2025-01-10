using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MyMovement : MonoBehaviour
{
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("This is the player sprite renderer")]
    [SerializeField] private SpriteRenderer playerSprite;
    [Tooltip("This is the player grappling hook arm sprite renderer")]
    [SerializeField] private SpriteRenderer armSprite;
    [Tooltip("This is the player grappling hook arm sprite renderer while grappling")]
    [SerializeField] private SpriteRenderer grappleArmSprite;
    [Tooltip("This is the player animator")]
    [SerializeField] private Animator playerAnimator;
    [Tooltip("This is the speed of the player")]
    [SerializeField] private float maxSpeed = 10f;
    [Tooltip("This is the force that is put upon the player when you jump")]
    [SerializeField] private float jumpForce = 10f;
    [Tooltip("This is the force that is put upon the player when you jump")]
    [SerializeField] private float bounceForce = 10f;
    [Tooltip("This is the force that is put upon the player when you are moving while using the grappling hook")]
    [SerializeField] private float swingForce = 10f;
    [Tooltip("This is the speed of which the player accelerates when starting to move")]
    [SerializeField] private float acceleration = 1f;
    [Tooltip("This is the speed of which the player decelerates when stopping to move")]
    [SerializeField] private float deceleration = 1f;
    [Tooltip("Time in seconds for the jump buffer window")]
    [SerializeField] private float jumpBufferTime = 0.2f;

    private Vector2 moveInput;
    private Vector2 startPosition;
    private bool isGrounded;
    private bool isMoving;
    private bool isGrappling;
    private bool justGrappled;
    private bool accelerate;
    private float jumpBufferCounter; // Tracks the jump buffer window time

    private Rigidbody2D rb;
    private BoxCollider2D groundCheckCollider;

    public UnityEvent OnGrounded;
    public UnityEvent OffGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        groundCheckCollider = GetComponent<BoxCollider2D>();
        groundCheckCollider.isTrigger = true;

        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isMoving && !isGrappling && !justGrappled)
        {
            float targetSpeed = moveInput.x * maxSpeed;
            float newVelocityX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
        }

        if (isMoving && (isGrappling || justGrappled))
        {
            rb.AddForce(moveInput * swingForce, ForceMode2D.Impulse);
        }

        if (!isMoving && !isGrappling && !justGrappled)
        {
            float newVelocityX = Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
        }
    }

    private void Update()
    {
        isGrounded = IsGrounded();

        // Handle ground transitions and jump buffer
        if (isGrounded)
        {
            OnGrounded.Invoke();
            if (jumpBufferCounter > 0) // If jump was buffered, execute the jump
            {
                Jump();
                jumpBufferCounter = 0f; // Clear the buffer
            }
        }
        else
        {
            OffGrounded.Invoke();
        }

        // Decrease jump buffer time
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Flip sprite and handle animations
        AnimationController();

        // Grappling and movement fixes
        if (isGrappling)
        {
            justGrappled = true;
        }

        if (isGrounded && !isGrappling)
        {
            justGrappled = false;
        }

        if (isMoving && !isGrappling && !justGrappled)
        {
            if ((rb.velocity.x > 0 && moveInput.x < 0) || (rb.velocity.x < 0 && moveInput.x > 0))
            {
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.deltaTime), rb.velocity.y);
            }
        }
    }

    private bool IsGrounded()
    {
        return groundCheckCollider.IsTouchingLayers(groundLayer);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity before jump
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
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
            jumpBufferCounter = jumpBufferTime; // Start the jump buffer countdown
            if (isGrounded) // Jump immediately if grounded
            {
                Jump();
            }
        }
    }

    public void PositionReset()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPosition;
    }

    public void StartGrapple()
    {
        isGrappling = true;
    }

    public void EndGrapple()
    {
        isGrappling = false;
    }

    public void EnemyBounce()
    {
        if (!isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, bounceForce), ForceMode2D.Impulse);
        }
    }

    private void AnimationBooleansFalse()
    {
        playerAnimator.SetBool("isWalking", false);
        playerAnimator.SetBool("isRunning", false);
        playerAnimator.SetBool("isJumping", false);
        playerAnimator.SetBool("isPulling", false);
        playerAnimator.SetBool("isSwinging", false);
    }

    private void AnimationController()
    {
        if (rb.velocity.x > 0.01f)
        {
            armSprite.flipX = false;
            grappleArmSprite.flipX = false;
            playerSprite.flipX = false;
        }

        if (rb.velocity.x < -0.01f)
        {
            armSprite.flipX = true;
            grappleArmSprite.flipX = true;
            playerSprite.flipX = true;
        }

        if (Mathf.Abs(rb.velocity.x) == 0 && isGrounded)
        {
            AnimationBooleansFalse();
        }

        if (Mathf.Abs(rb.velocity.x) > 0.01f && isGrounded)
        {
            AnimationBooleansFalse();
            playerAnimator.SetBool("isWalking", true);
        }

        if (Mathf.Abs(rb.velocity.x) >= maxSpeed - 1 && isGrounded)
        {
            AnimationBooleansFalse();
            playerAnimator.SetBool("isRunning", true);
        }

        if ((isGrappling || justGrappled) && !isGrounded)
        {
            AnimationBooleansFalse();
            playerAnimator.SetBool("isSwinging", true);
        }

        if (!isGrounded && !isGrappling && !justGrappled)
        {
            AnimationBooleansFalse();
            playerAnimator.SetBool("isJumping", true);
        }
    }
}


