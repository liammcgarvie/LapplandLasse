using UnityEngine;
using UnityEngine.Events;

public class EnemyBouncePad : MonoBehaviour
{
    private int i;
    
    public UnityEvent onJumped;

    private void Start()
    {
        i = 0;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (i == 0)
            {
                i++;
                onJumped.Invoke();
            }
        }
    }
}
