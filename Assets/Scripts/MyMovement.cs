using Unity.VisualScripting;
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

    private Vector2 moveInput;
    private Vector2 startPosition;
    private bool isGrounded;
    private bool isMoving;
    private bool isGrappling;
    private bool justGrappled;
    private bool accelerate;

    private Rigidbody2D rb;
    private CircleCollider2D groundCheckCollider;
    
    public UnityEvent OnGrounded;
    public UnityEvent OffGrounded;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
        
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Using velocity based movement while not using the grappling hook
        if (isMoving && isGrappling == false && justGrappled == false)
        {
            float targetSpeed = moveInput.x * maxSpeed;
            
            if (rb.velocity.x == 0 || accelerate)
            {
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, targetSpeed, acceleration * Time.deltaTime), rb.velocity.y);
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
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.deltaTime), rb.velocity.y);
        }
    }

    private void Update()
    {
        if (moveInput.x > 0)
        {
            if (rb.velocity.x > 0)
            {
                accelerate = true;
            }
            else
            {
                accelerate = false;
            }
        }

        if (moveInput.x < 0)
        {
            if (rb.velocity.x < 0)
            {
                accelerate = true;
            }
            else
            {
                accelerate = false;
            }
        }
        
        isGrounded = IsGrounded();
        
        // This is used to switch the isGrounded variable in other scripts as well
        switch (isGrounded)
        {
            case true:
                OnGrounded.Invoke();
                break;
            case false:
                OffGrounded.Invoke();
                break;
        }


        AnimationController();
        
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
            if ((rb.velocity.x > 0 && moveInput.x < 0) || (rb.velocity.x < 0 && moveInput.x > 0))
            {
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.deltaTime), rb.velocity.y);
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
    
    public void EnemyBounce() // Can be used with events
    {
        if (isGrounded == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0f, bounceForce), ForceMode2D.Impulse);
        }
    }

    // Sets all animator booleans to false
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
        // Flips the sprite
        if (rb.velocity.x > 0.01f)
        {
            armSprite.flipX = false;
            grappleArmSprite.flipX = false;
            playerSprite.flipX = false;
        }
        
        // Flips the sprite
        if (rb.velocity.x < -0.01f)
        {
            armSprite.flipX = true;
            grappleArmSprite.flipX = true;
            playerSprite.flipX = true;
        }
        
        // Sets animation to idle
        if (Mathf.Abs(rb.velocity.x) == 0 && isGrounded)
        {
            AnimationBooleansFalse();
        }

        // Sets animation to walking
        if (Mathf.Abs(rb.velocity.x) > 0.01f && isGrounded)
        {
            AnimationBooleansFalse();
            
            playerAnimator.SetBool("isWalking", true);
        }
        
        // Sets animation to running
        if (Mathf.Abs(rb.velocity.x) >= maxSpeed - 1 && isGrounded)
        {
            AnimationBooleansFalse();
            
            playerAnimator.SetBool("isRunning", true);
        }

        // Sets animation to swinging
        if ((isGrappling || justGrappled) && isGrounded == false)
        {
            AnimationBooleansFalse();
            
            playerAnimator.SetBool("isSwinging", true);
        }

        // Sets animation to jumping
        if (isGrounded == false && isGrappling == false && justGrappled == false)
        {
            AnimationBooleansFalse();
            
            playerAnimator.SetBool("isJumping", true);
        }
    }
}

