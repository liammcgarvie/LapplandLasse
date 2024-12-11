using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    
    private int currentHealth;

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
