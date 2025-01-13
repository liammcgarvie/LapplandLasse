using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject bouncePad;
    
    [SerializeField] private AudioSource deathSound;
    
    private Vector3 startPos;
    private Vector3 bouncePadStartPos;

    private void Start()
    {
        startPos = transform.position;
        bouncePadStartPos = bouncePad.transform.position;
        
        GameManager.gameManager.RegisterEnemy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hook"))
        {
            bouncePad.SetActive(false);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hook"))
        {
            bouncePad.SetActive(true);
        }
    }
    public void Death() // Can be called with events
    {
        deathSound.Play();
        gameObject.SetActive(false);
        bouncePad.SetActive(false);
    }

    public void Respawn() // Can be used with events
    {
        gameObject.SetActive(true);
        bouncePad.SetActive(true);
        transform.position = startPos;
        bouncePad.transform.position = bouncePadStartPos;
    }
}
