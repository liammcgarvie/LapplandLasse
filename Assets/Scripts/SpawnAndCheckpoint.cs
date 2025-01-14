using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SpawnAndCheckpoint : MonoBehaviour
{
    [SerializeField] private AudioSource deathSound1;
    [SerializeField] private AudioSource deathSound2;
    [SerializeField] private AudioSource deathSound3;
    [SerializeField] private AudioSource deathSound4;
    [SerializeField] private AudioSource deathSound5;
    
    private Rigidbody2D rb;
    private Vector3 respawnPoint;
    
    public UnityEvent onRespawn;

    private bool canPlaySound;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
        
        canPlaySound = true;
    }
    
    public void CheckpointEntered(Vector3 checkpointPosition) // Can be called with events
    {
        respawnPoint = checkpointPosition;
    }

    public void Respawn() // Can be called with events
    {
        DeathSound();
        
        rb.velocity = Vector2.zero;
        transform.position = respawnPoint;
        onRespawn.Invoke();

        StartCoroutine(SoundDelay());
    }

    private void DeathSound()
    {
        int random = Random.Range(0, 5);
        
        if (canPlaySound)
        {
            if (random == 0)
            {
                deathSound1.Play();
            }
            else if (random == 1)
            {
                deathSound2.Play();
            }
            else if (random == 2)
            {
                deathSound3.Play();
            }
            else if (random == 3)
            {
                deathSound4.Play();
            }
            else if (random == 4)
            { 
                deathSound5.Play();
            }
            canPlaySound = false;
        }
    }

    private IEnumerator SoundDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canPlaySound = true;
    }
}
