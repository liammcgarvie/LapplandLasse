using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject bouncePad;
    
    [SerializeField] private AudioSource deathSound;
    
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
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
    }

    public void Respawn() // Can be used with events
    {
        transform.position = startPos;
    }
}
