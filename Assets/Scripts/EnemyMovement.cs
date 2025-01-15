using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float leftDistance;
    [SerializeField] private float rightDistance;
    [SerializeField] private float upDistance;
    [SerializeField] private float downDistance;
    
    [SerializeField] private float speed;
    
    [SerializeField] private SpriteRenderer sprite;
    
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

        movingRight = true;
        movingUp = true;
        movingDown = false;
        movingLeft = false;
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(0,0);
        
        distance = Vector3.Distance(transform.position, startPosition);

        if (transform.position.x < startPosition.x && distance >= leftDistance)
        {
            movingRight = true;
            movingLeft = false;
        }
        
        if (transform.position.x > startPosition.x && distance >= rightDistance)
        {
            movingRight = false;
            movingLeft = true;
        }

        if (transform.position.y < startPosition.y && distance >= downDistance)
        {
            movingDown = false;
            movingUp = true;
        }
        
        if (transform.position.y > startPosition.y && distance >= upDistance)
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
            sprite.flipX = true;
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }

        if (movingLeft && !movingRight)
        {
            sprite.flipX = false;
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }

        if (movingUp && !movingDown)
        {
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }

        if (movingDown && !movingUp)
        {
            transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
        }
    }
}
