using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject bouncePad;
    
    [SerializeField] private AudioSource deathSound;
    
    //TODO: Ljudet funkar inte
    
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
    }
}
