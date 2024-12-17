using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public void Death() // Can be called with events
    {
        Destroy(gameObject);
    }
}
