using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHookCollision : MonoBehaviour
{
    [Tooltip("This is the layer where a player will be able to jump")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private BoxCollider2D groundCheckCollider;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float offsetX = 1.0f;
    
    [SerializeField] private AudioSource chompSound1;
    [SerializeField] private AudioSource chompSound2;
    [SerializeField] private AudioSource chompSound3;
    [SerializeField] private AudioSource chompSound4;
    
    public UnityEvent playPlayerDeathSound;
    
    private bool pulling;
    private bool isGrounded;

    private Vector3 playerPos;
    
    private Transform pullTarget;
    
    public UnityEvent OnHit;

    public UnityEvent respawn;
    
    private void Awake()
    {
        pulling = false;
    }

    private void Update()
    {
        isGrounded = IsGrounded();

        if (Input.GetMouseButtonUp(0))
        {
            pulling = false;
        }
        
        if (pulling && isGrounded)
        {
            GroundedEnemyPull();
        }

        if (pullTarget)
        {
            if (Vector3.Distance(player.transform.position, pullTarget.position) <= offsetX + 0.5f)
            {
                RandomChompSound();
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

    public void CheckForRespawn()
    {
        if (pulling == false)
        {
            playPlayerDeathSound.Invoke();
            respawn.Invoke();
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
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && isGrounded)
        {
            OnHit.Invoke();
            playerPos = player.transform.position;
            pulling = true;
            pullTarget = other.transform;
        }
    }

    private void RandomChompSound()
    {
        int random = Random.Range(0, 4);

        if (random == 0)
        {
            chompSound1.Play();
        }
        else if (random == 1)
        {
            chompSound2.Play();
        }
        else if (random == 2)
        {
            chompSound3.Play();
        }
        else if (random == 3)
        {
            chompSound4.Play();
        }
    }
}
