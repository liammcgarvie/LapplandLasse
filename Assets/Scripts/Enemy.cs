using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject bouncePad;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hook"))
        {
            bouncePad.SetActive(false);
        }
    }
    public void Death() // Can be called with events
    {
        Destroy(gameObject);
    }
}
