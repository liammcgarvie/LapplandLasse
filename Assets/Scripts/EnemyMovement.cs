using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float leftDistance;
    [SerializeField] private float rightDistance;
    
    [SerializeField] private float speed;
    
    private Vector3 startPosition;

    private bool movingRight;
    private bool movingLeft;
    
    private float distance;
    
    private void Start()
    {
        startPosition = transform.position;

        movingRight = true;
        movingLeft = false;
    }

    private void Update()
    {
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
        
        MovementController();
    }

    private void MovementController()
    {
        if (movingRight && !movingLeft)
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }

        if (movingLeft && !movingRight)
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
    }
}
