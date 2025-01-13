using UnityEngine;
using UnityEngine.Events;

public class EnemyBouncePad : MonoBehaviour
{
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("This is the collider that is on the player which checks if the player is on the ground")]
    [SerializeField] private BoxCollider2D groundCheckCollider;
    
    private int i;
    
    private bool isGrounded;
    
    public UnityEvent onJumped;

    private void Start()
    {
        i = 0;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (i == 0)
            {
                if (isGrounded == false)
                {
                    i++;
                    onJumped.Invoke();
                }
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

    public void ChangeI()
    {
        i = 0;
    }
}
