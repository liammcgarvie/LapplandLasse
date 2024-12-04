using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Vector3Event : UnityEvent<Vector3> { }
public class OnTrigger : MonoBehaviour
{
    [SerializeField] string activationTag = "Player";

    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;
    
    public Vector3Event onTriggerEnterWithPosition;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(activationTag))
        {
            triggerEnter.Invoke();
            onTriggerEnterWithPosition.Invoke(transform.position);
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
