using UnityEngine;
using UnityEngine.Events;

public class EnemyHookCollision : MonoBehaviour
{
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private CircleCollider2D groundCheckCollider;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float offsetX = 1.0f;
    
    private bool pulling;
    private bool isGrounded;

    private Vector3 playerPos;
    
    private Transform pullTarget;
    
    public UnityEvent OnHit;
    
    private void Awake()
    {
        pulling = false;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        
        if (pulling && isGrounded)
        {
            GroundedEnemyPull();
        }

        if (pulling && isGrounded == false)
        {
            ElevatedEnemyPull();
        }

        if (pullTarget)
        {
            if (Vector3.Distance(player.transform.position, pullTarget.position) <= offsetX + 0.5f)
            {
                pullTarget.gameObject.SetActive(false);
                pullTarget = null;
                pulling = false;
            }
        }
        
        if (pulling == false)
        {
            playerAnimator.SetBool("isPulling", false);
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

    private void GroundedEnemyPull()
    {
        playerAnimator.SetBool("isPulling", true);
            
        player.transform.position = playerPos;
        
        if (pullTarget.transform.position.x > player.transform.position.x)
        {
            pullTarget.position = rope.GetPosition(0);
            pullTarget.position += new Vector3(offsetX, 0, 0);
        }
            
        if (pullTarget.transform.position.x < player.transform.position.x)
        {
            pullTarget.position = rope.GetPosition(0);
            pullTarget.position -= new Vector3(offsetX, 0, 0);
        }
    }

    private void ElevatedEnemyPull()
    {
        playerAnimator.SetBool("isPulling", true);
        
        if (pullTarget.transform.position.x > player.transform.position.x)
        {
            pullTarget.position = rope.GetPosition(0);
            pullTarget.position += new Vector3(offsetX, 0, 0);
        }
            
        if (pullTarget.transform.position.x < player.transform.position.x)
        {
            pullTarget.position = rope.GetPosition(0);
            pullTarget.position -= new Vector3(offsetX, 0, 0);
        }
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnHit.Invoke();
            playerPos = player.transform.position;
            pulling = true;
            pullTarget = other.transform;
        }
    }
}
