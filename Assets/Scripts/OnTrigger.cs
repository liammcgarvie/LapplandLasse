using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    [SerializeField] string activationTag = "Player";

    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(activationTag))
        {
            triggerEnter.Invoke();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(activationTag))
        {
            triggerExit.Invoke();
        }
    }
}
