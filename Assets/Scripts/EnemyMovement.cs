using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float leftDistance;
    [SerializeField] private float rightDistance;
    [SerializeField] private float upDistance;
    [SerializeField] private float downDistance;
    
    [SerializeField] private float speed;
    
    [SerializeField] private SpriteRenderer sprite;
    
    [SerializeField] private Animator animator;
    
    private Vector3 startPosition;
    
    private Rigidbody2D rb;

    private bool movingRight;
    private bool movingLeft;
    private bool movingUp;
    private bool movingDown;
    
    private float distance;
    
    private void Start()
    {
        startPosition = transform.position;

        if (rightDistance > 0 || leftDistance > 0)
        {
            movingLeft = false;
            movingRight = true;
        }
        else
        {
            movingLeft = false;
            movingRight = false;
        }
        
        movingUp = true;
        movingDown = false;
        
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(0,0);
        
        distance = Vector3.Distance(transform.position, startPosition);

        if (rightDistance > 0 || leftDistance > 0)
        {
            animator.SetBool("IsWalking", true);
        }

        if (transform.position.x < startPosition.x && distance > leftDistance && leftDistance > 0)
        {
            movingRight = true;
            movingLeft = false;
            
            sprite.flipX = true;
        } 
        
        if (transform.position.x > startPosition.x && distance > rightDistance && rightDistance > 0)
        {
            movingRight = false;
            movingLeft = true;
            
            sprite.flipX = false;
        }
        
        if (transform.position.y < startPosition.y && distance > downDistance)
        {
            movingDown = false;
            movingUp = true;
        }
        
        if (transform.position.y > startPosition.y && distance > upDistance)
        {
            movingDown = true;
            movingUp = false;
        }
        
        MovementController();
    }

    private void MovementController()
    {
        if (movingRight && !movingLeft)
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        else if (movingLeft && !movingRight)
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
        
        if (movingUp && !movingDown)
        {
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        else if (movingDown && !movingUp)
        {
            transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
        }
    }
}
