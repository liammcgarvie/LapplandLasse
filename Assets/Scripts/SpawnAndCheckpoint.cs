using UnityEngine;

public class SpawnAndCheckpoint : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 respawnPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
    }
    
    public void CheckpointEntered() // Can be called with events
    {
        respawnPoint = transform.position;
    }

    public void Respawn() // Can be called with events
    {
        rb.velocity = Vector2.zero;
        transform.position = respawnPoint;
    }
}
