using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    
    private int currentHealth;
    
    public UnityEvent checkForRespawn;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Death();
        }
        
        //Debug.Log("Health: " + currentHealth);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HitEnemy();
        }
    }

    private void HitEnemy() // When you hit an enemy and take damage
    {
        LoseHealth(1);
        checkForRespawn.Invoke();
    }

    private void Death() // Is called when you lose all your health
    {
        //Vad ska hända när man dör??
    }

    public void LoseHealth(int damage) // Can be called with events
    {
        currentHealth -= damage;
    }

    public void GainHealth(int gainedHealth) // Can be called with events
    {
        currentHealth += gainedHealth;
    }
}
