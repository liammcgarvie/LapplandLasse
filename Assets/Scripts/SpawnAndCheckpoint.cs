using UnityEngine;
using UnityEngine.Events;

public class SpawnAndCheckpoint : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 respawnPoint;
    
    public UnityEvent onRespawn;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
    }
    
    public void CheckpointEntered(Vector3 checkpointPosition) // Can be called with events
    {
        respawnPoint = checkpointPosition;
    }

    public void Respawn() // Can be called with events
    {
        rb.velocity = Vector2.zero;
        transform.position = respawnPoint;
        onRespawn.Invoke();
    }
}
