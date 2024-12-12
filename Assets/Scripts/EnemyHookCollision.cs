using UnityEngine;
using UnityEngine.Events;

public class EnemyHookCollision : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private float offsetX = 1.0f;
    
    private bool pulling;

    private Vector3 playerPos;
    
    private Transform pullTarget;
    
    public UnityEvent OnHit;
    
    private void Awake()
    {
        pulling = false;
    }

    private void Update()
    {
        if (pulling)
        {
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

        if (pullTarget)
        {
            if (Vector3.Distance(player.transform.position, pullTarget.position) <= offsetX + 0.5f)
            {
                pullTarget.gameObject.SetActive(false);
                pullTarget = null;
                pulling = false;
            }
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
